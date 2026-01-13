using Amazon;
using Amazon.S3;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using StackExchange.Redis;
using System.Text;
using System.Text.Json.Serialization;
using Web1.AutoMap;
using Web1.Data;
using Web1.Middleware;
using Web1.Models;
using Web1.Repository;
using Web1.Service.Account;
using Web1.Service.AWS;
using Web1.Service.ChatBox;
using Web1.Service.Cookie;
using Web1.Service.Mail;
using Web1.Service.RabbitMq.Connection;
using Web1.Service.RabbitMq.Consumer;
using Web1.Service.RabbitMq.Producer;
using Web1.Service.Redis;
using Web1.Service.Session;
using Web1.Service.SignalR;
using Web1.Service.SignalR.SignalRNotification;


var builder = WebApplication.CreateBuilder(args);

// Cấu hình đọc từ appsettings.json và biến môi trường
builder.Configuration
       .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)  // fallback dev
       .AddEnvironmentVariables();  // đọc ENV trong container

// Đọc cấu hình từ appsettings.json
builder.Services.Configure<MailSetting>(builder.Configuration.GetSection("MailSetting"));

//Đăng kí service
builder.Services.AddTransient<ISendMailService, SendMailService>();
builder.Services.AddTransient<IConfirmMailService, ConfirmMailService>();
builder.Services.AddTransient<IForgotPasswordService, ForgotPasswordService>();
builder.Services.AddTransient<ISessionService, SessionService>();
builder.Services.AddTransient<IOAuthService, OAuthService>();
builder.Services.AddTransient<IAccountService, AccountService>();
builder.Services.AddTransient<ICookieService, CookieService>();
builder.Services.AddScoped<IUploadAWSService, UploadAWSService>();
builder.Services.AddSingleton<IRedisService, RedisService>();
builder.Services.AddTransient<IChatService, ChatService>();

// Tạo một bộ nhớ cache
builder.Services.AddMemoryCache();

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    });


// Cấu hình khởi tạo Redis
var redisConfig = builder.Configuration["Redis:ConnectionString"];
builder.Services.AddSingleton<IConnectionMultiplexer>(
    ConnectionMultiplexer.Connect(redisConfig + ",abortConnect=false")
);

//Rabit
builder.Services.AddHostedService<ForgotPasswordConsumer>();
builder.Services.AddHostedService<ConfirmEmailConsumer>();
builder.Services.AddHostedService<UserNotificationConsumer>();
builder.Services.AddHostedService<ChatBoxConsumer>();

builder.Services.AddSingleton<IRabbitMqProducer, RabbitMqProducer>();
builder.Services.AddSingleton<RabbitMqConnection>();

//SignalR
builder.Services.AddSignalR();
builder.Services.AddSingleton<ISignalRNotificationService, SignalRNotificationService>();

builder.Services.AddEndpointsApiExplorer();


builder.Services.AddSwaggerGen(option =>
{
    option.SwaggerDoc("v1", new OpenApiInfo { Title = "Book API", Version = "v1" });
    option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter a valid token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "Bearer"
    });
    option.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type=ReferenceType.SecurityScheme,
                        Id="Bearer"
                    }
                },
                new string[]{}
            }
        });
});

//Dang ki DBContext 
builder.Services.AddDbContext<TinTucDbContext>(
    options => options.UseSqlServer(builder.Configuration.GetConnectionString("TinTucDB"),
        sqlServerOptions => sqlServerOptions.EnableRetryOnFailure()
    )
);



builder.Services.AddDistributedMemoryCache();  // Lưu session trong RAM

//Đăng kí sử dụng AWS
var awsOptions = builder.Configuration.GetSection("AWS");
builder.Services.AddSingleton<IAmazonS3>(sp =>
{
    return new AmazonS3Client(
        awsOptions["AccessKey"],
        awsOptions["SecretKey"],
        RegionEndpoint.GetBySystemName(awsOptions["Region"])
    );
});

// Auto Mapper
builder.Services.AddAutoMapper(typeof(MappingProfile));

//Session
builder.Services.AddSession(options =>
{
    options.Cookie.Name = "TMC";
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Thời gian sống của session
    options.Cookie.HttpOnly = true;                 // Bảo mật: không cho JS truy cập cookie
    options.Cookie.IsEssential = true;             // Bắt buộc trình duyệt phải lưu cookie
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always; // Chỉ hoạt động trên HTTPS
});

//Them Repository
builder.Services.AddScoped<IAccountRepository, AccountRepository>();
builder.Services.AddScoped<ITintucRepository, TinTucRepository>();
builder.Services.AddScoped<IDanhMucRepository, DanhMucRepository>();
builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
builder.Services.AddScoped<ICommentRepository, CommentRepository>();
builder.Services.AddScoped<INotificationRepository, NotificationRepository>();
builder.Services.AddScoped<IMessageRepository, MessageRepository>();


//Dang ki Identity 
builder.Services.AddIdentity<AppUser, IdentityRole>( options =>
                 {
                    options.SignIn.RequireConfirmedEmail = true; // Bật yêu cầu xác nhận email
                 })
                .AddEntityFrameworkStores<TinTucDbContext>()
                .AddDefaultTokenProviders();


//Dang ki JWT Bearer Authentication 
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,           // Kiểm tra issuer (người cấp token)
        ValidateAudience = true,         // Kiểm tra audience (đối tượng nhận token)
        ValidateLifetime = true,         // Kiểm tra thời gian hết hạn token
        ValidateIssuerSigningKey = true, // Kiểm tra khóa ký của issuer
        ClockSkew = TimeSpan.Zero,
        ValidIssuer = builder.Configuration["JWT:Issuer"],
        ValidAudience = builder.Configuration["JWT:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Key"]))
    };
    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            // SignalR gửi access_token qua query string, không phải header
            var accessToken = context.Request.Query["access_token"];

            // Đảm bảo chỉ xử lý cho request đến Hub
            var path = context.HttpContext.Request.Path;
            if (!string.IsNullOrEmpty(accessToken) &&
                path.StartsWithSegments("/notificationHub"))
            {
                context.Token = accessToken;
            }

            return Task.CompletedTask;
        }
    };
})
.AddGoogle(googleOptions =>
{
    googleOptions.ClientId = builder.Configuration["OAuth:Google:ClientId"]; 
    googleOptions.ClientSecret = builder.Configuration["OAuth:Google:ClientSecret"]; 
    var callbackPath = builder.Configuration["OAuth:Google:CallbackPath"];
});

//Dang ki HttpContext
builder.Services.AddHttpContextAccessor();
builder.Services.AddHttpClient();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.WithOrigins("https://localhost:4300") // origin frontend
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials(); // bắt buộc nếu dùng token/cookie
    });
});


var app = builder.Build();

app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders =
        ForwardedHeaders.XForwardedFor |
        ForwardedHeaders.XForwardedProto
});

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseDeveloperExceptionPage();
}


app.UseCors("AllowAll");

// Đăng kí ExceptionHandlingMiddleware
app.UseMiddleware<ExceptionHandlingMiddleware>();



app.UseHttpsRedirection();

//Dang ki xac thuc thong tin dang ky, dang nhap
app.UseAuthentication();

app.UseAuthorization();

app.MapHub<NotificationHub>("/notificationHub").RequireCors("AllowAll");

app.UseSession();

app.MapControllers();

app.MapFallbackToFile("index.html");

app.Run();