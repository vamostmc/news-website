using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Configuration;
using System.Text;
using System.Text.Json.Serialization;
using Web1.Data;
using Web1.DataNew;
using Web1.Middleware;
using Web1.Models;
using Web1.Repository;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

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
    options => options.UseSqlServer(builder.Configuration.GetConnectionString("StudentDB"))
);

//Them Repository
/*builder.Services.AddScoped<IStudentRepository, StudentRepository>();*/
builder.Services.AddScoped<IAccountRepository, AccountRepository>();
builder.Services.AddScoped<ITintucRepository, TinTucRepository>();
builder.Services.AddScoped<IDanhMucRepository, DanhMucRepository>();
builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
builder.Services.AddScoped<ICommentRepository, CommentRepository>();
builder.Services.AddScoped<INotificationRepository, NotificationRepository>();

//Dang ki Identity 
builder.Services.AddIdentity<AppUser, IdentityRole>()
                .AddEntityFrameworkStores<TinTucDbContext>()
                .AddDefaultTokenProviders();


//Dang ki JWT Bearer Authentication 
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
   /* options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;*/
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,           // Kiểm tra issuer (người cấp token)
        ValidateAudience = true,         // Kiểm tra audience (đối tượng nhận token)
        ValidateLifetime = true,         // Kiểm tra thời gian hết hạn token
        ValidateIssuerSigningKey = true, // Kiểm tra khóa ký của issuer
        ValidIssuer = builder.Configuration["JWT:Issuer"],
        ValidAudience = builder.Configuration["JWT:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Key"]))
    };
});/*.AddCookie(options =>
{
    options.LoginPath = "/api/account/Unauthorized";               // Path log in 
    options.LogoutPath = "/Account/Logout";                        // Path log out
    options.AccessDeniedPath = "/Account/AccessDenied";            // Path khi user khong co quyen truy cap
}); */

//Dang ki HttpContext
builder.Services.AddHttpContextAccessor();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder =>
        {
            builder.AllowAnyOrigin()  // Cho phép bất kỳ nguồn nào
                   .AllowAnyMethod()  // Cho phép bất kỳ phương thức nào
                   .AllowAnyHeader(); // Cho phép bất kỳ tiêu đề nào
        });
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
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

app.MapControllers();

app.Run();