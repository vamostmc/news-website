using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using StackExchange.Redis;
using System.Configuration;
using System.Text;
using System.Text.Json.Serialization;
using Web1.Data;
using Web1.DataNew;
using Web1.Middleware;
using Web1.Models;
using Web1.Repository;
using Web1.Service;


var builder = WebApplication.CreateBuilder(args);


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

// Tạo một bộ nhớ cache
builder.Services.AddMemoryCache();

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    });
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
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
    options => options.UseSqlServer(builder.Configuration.GetConnectionString("TinTucDB"))
);


builder.Services.AddDistributedMemoryCache();  // Lưu session trong RAM

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
builder.Services.AddScoped<IPasswordRepository, PasswordRepository>();
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
})
.AddCookie("CookieAuth", options =>
{
    options.ExpireTimeSpan = TimeSpan.FromDays(14);             // Thời gian lưu cookie
    options.SlidingExpiration = true;                           // Gia hạn thời gian nếu người dùng còn hoạt động
    options.Cookie.HttpOnly = true;                             // Cookie chỉ được truy cập bởi máy chủ
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;  // Yêu cầu HTTPS cho cookie
    options.Cookie.SameSite = SameSiteMode.None;               // Hỗ trợ cross-origin
    /*options.Cookie.SecurePolicy = CookieSecurePolicy.Always;*/    // Yêu cầu HTTPS
    //options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
    //options.Cookie.SameSite = SameSiteMode.Lax;               // Cho phép gửi cookie trong các yêu cầu cross-origin
}).AddGoogle(googleOptions =>
{
    googleOptions.ClientId = builder.Configuration["OAuth:Google:ClientId"]; 
    googleOptions.ClientSecret = builder.Configuration["OAuth:Google:ClientSecret"]; 
    var baseUrl = builder.Configuration["OAuth:BaseUrl"];
    var callbackPath = builder.Configuration["OAuth:Google:CallbackPath"];
    googleOptions.CallbackPath = $"{callbackPath}";
});

//Dang ki HttpContext
builder.Services.AddHttpContextAccessor();
builder.Services.AddHttpClient();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder =>
        {
            // builder.AllowAnyOrigin()  // Cho phép tất cả các nguồn
            //       .AllowAnyMethod()  // Cho phép tất cả các phương thức HTTP (GET, POST, PUT, DELETE, ...)
            //       .AllowAnyHeader();  // Cho phép tất cả các header

            builder.WithOrigins("https://localhost:4200", "https://accounts.google.com")
                   .AllowAnyHeader()
                   .AllowAnyMethod()
                   .AllowCredentials();  // Cho phép gửi cookie
        });
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseDeveloperExceptionPage();
}
app.UseCors("AllowAll");

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(Directory.GetCurrentDirectory(), "Hinh")),
    RequestPath = "/Hinh"
});

// Đăng kí ExceptionHandlingMiddleware
app.UseMiddleware<ExceptionHandlingMiddleware>();



app.UseHttpsRedirection();

//Dang ki xac thuc thong tin dang ky, dang nhap
app.UseAuthentication();

app.UseAuthorization();

app.UseSession();

app.MapControllers();

app.MapFallbackToFile("index.html");

app.Run();