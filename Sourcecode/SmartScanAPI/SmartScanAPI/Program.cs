using Entities;
using IRepository;
using Logger;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using NLog;
using Repository;
using Repository.Data;
using SmartScanAPI.Helper;
using SmartScanAPI.Middleware;
using SmartScanAPI.Services;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
string MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
LogManager.Setup().LoadConfigurationFromFile(Path.Combine(Directory.GetCurrentDirectory(), "nlog.config"));

builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));
builder.Services.Configure<EnvironmentVariables>(builder.Configuration.GetSection("Env"));
builder.Services.Configure<EBSConfig>(builder.Configuration.GetSection("EBSConfig"));
builder.Services.AddSingleton<DapperContext>();
builder.Services.AddScoped<IWrapperRepository, WrapperRepository>();
builder.Services.AddScoped<IPreprintedQRJobRepository, PreprintedQRJobRepository>();
builder.Services.AddScoped<ILoggerManager, LoggerManager>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<EBSTokenGenerator>();
builder.Services.AddHttpClient<CaptchaService>();
builder.Services.AddHostedService<PreprintedQRPushService>();

builder.Services.AddHttpClient();

//builder.Services.AddSingleton<OAuth2TokenService>();
//builder.Services.Configure<OAuth2Settings>(builder.Configuration.GetSection("OAuth2"));
//builder.Services.AddSingleton<OAuth2Validator>();


builder.Services.AddSwaggerGen(opt =>
{
    opt.SwaggerDoc("v1", new OpenApiInfo { Title = "SmartScan API", Version = "v1" });
    opt.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "JWT Authorization using Bearer",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "bearer"
    });
    opt.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme {
                Reference = new OpenApiReference {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
    // by amrit here --
    opt.MapType<IFormFile>(() => new OpenApiSchema
    {
        Type = "string",
        Format = "binary"
    });

    // Optional but recommended
    opt.EnableAnnotations();
    // by amrit to here--
});

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins, policy =>
    {
        policy.WithOrigins("https://localhost:52628", "http://localhost:5173", "https://localhost:5174", "https://localhost:3000", "https://10.85.185.40")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// Add multiple secret sources
//builder.Configuration
//    .AddUserSecrets<Program>(optional: true, reloadOnChange: true) // Windows Dev
//    .AddEnvironmentVariables();

var appSecrets = builder.Configuration.GetSection("AppSecrets").Get<AppSecrets>()!;

//var secrets = new AppSecrets
//{
//    Hex = builder.Configuration["HEX"] ?? throw new Exception("HEX missing"),
//    Tweak = builder.Configuration["TWEAK"] ?? throw new Exception("TWEAK missing"),
//};

builder.Services.AddSingleton(appSecrets);

var environments = builder.Configuration.GetSection("Env").Get<EnvironmentVariables>()!;
builder.Services.AddSingleton(environments);


var ebsConfig = builder.Configuration.GetSection("EBSConfig").Get<EBSConfig>()!;
builder.Services.AddSingleton(ebsConfig);

var jwt = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>()!;
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwt.Issuer,
            ValidAudience = jwt.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.Key))
        };
    });

builder.Services.AddAuthorizationBuilder()
    .SetDefaultPolicy(new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .RequireAssertion(context =>
        {
            var role = context.User.FindFirst(ClaimTypes.Role)?.Value;
            return role != "0"; // block system/EBS user
        })
        .Build())
    .AddPolicy("SystemOnly", policy =>
    {
        policy.RequireAssertion(context =>
            context.User.FindFirst(ClaimTypes.Role)?.Value == "0");
    })
    .AddPolicy("ITOrigin", policy =>
    {
        policy.RequireAssertion(context =>
            context.User.FindFirst(ClaimTypes.Role)?.Value == "-1");
    });

builder.Services.AddAuthorization();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

app.Use(
    async (context, next) =>
    {
        try
        {
            await next.Invoke();
        }
        catch (Exception ex)
        {
           LogManager.GetLogger("GlobalException").Error(ex, "Unhandled exception occurred.");
            throw;  // rethrow so other middleware can also handle if needed
        }
    }
);


app.UseStaticFiles();
app.UseSwagger();
app.UseSwaggerUI();

app.UseCors(MyAllowSpecificOrigins);
app.UseMiddleware<ExceptionMiddleware>();
app.UseAuthentication();
app.UseMiddleware<TokenVersionMiddleware>();
app.UseAuthorization();

app.MapControllers();
app.Run();

