using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using RentEase.Service;
using RentEase.Service.AutoMapper;
using RentEase.Service.Helper;
using RentEase.Service.Service.Authenticate;
using RentEase.Service.Service.Main;
using RentEase.Service.Service.Payment;
using RentEase.Service.Service.Sub;
using System.Text;


namespace RentEase.API
{
    public static class Configure
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<IAuthenticateService, AuthenticateService>();
            services.AddScoped<IAccountVerificationService, AccountVerificationService>();
            services.AddScoped<IAccountTokenService, AccountTokenService>();
            services.AddScoped<IPayosService, PayosService>();
            services.AddHttpClient<IPayosService, PayosService>();


            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<IAptCategoryService, AptCategoryService>();
            services.AddScoped<IAptService, AptService>();
            services.AddScoped<IAptImageService, AptImageService>();
            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<IAptStatusService, AptStatusService>();
            services.AddScoped<IAptUtilityService, AptUtilityService>();
            services.AddScoped<IUtilityService, UtilityService>();
            services.AddScoped<IAptStatusService, AptStatusService>();
            services.AddScoped<IReviewService, ReviewService>();
            services.AddScoped<IRoleService, RoleService>();
            services.AddScoped<ITransactionTypeService, TransactionTypeService>();
            services.AddScoped<IOrderService, OrderService>();
            services.AddScoped<ITransactionService, TransactionService>();
            services.AddScoped<IPostService, PostService>();
            services.AddScoped<IPostRequireService, PostRequireService>();

            services.AddScoped<ServiceWrapper>();
            return services;
        }
        public static IServiceCollection AddHelperServices(this IServiceCollection services)
        {
            services.AddScoped<IEmailHelper, EmailHelper>();
            services.AddScoped<ITokenHelper, TokenHelper>();
            services.AddScoped<IPasswordHelper, PasswordHelper>();

            services.AddScoped<HelperWrapper>();
            return services;
        }
        public static IServiceCollection AddSmtpClient(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<SmtpClient>(provider =>
            {
                var smtpClient = new SmtpClient();
                smtpClient.Connect(configuration["SmtpSettings:Host"], int.Parse(configuration["SmtpSettings:Port"]), SecureSocketOptions.StartTls);
                smtpClient.Authenticate(configuration["SmtpSettings:Username"], configuration["SmtpSettings:Password"]);
                return smtpClient;
            });

            return services;
        }
        public static IServiceCollection ConfigureKestrel(this IServiceCollection services)
        {
            services.Configure<KestrelServerOptions>(options =>
            {
                options.Limits.MaxRequestHeadersTotalSize = 64 * 1024; // Ví dụ: 64KB
            });

            return services;
        }
        public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
        {

            services.AddAuthentication(e =>
            {
                e.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = configuration["JwtSettings:Issuer"],
                        ValidAudience = configuration["JwtSettings:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JwtSettings:Key"]))
                    };

                });

            services.AddSwaggerGen(options =>
            {
                //// Config
                options.DescribeAllParametersInCamelCase();
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Please enter a valid token",
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    BearerFormat = "JWT",
                    Scheme = "Bearer"
                });
                options.AddSecurityRequirement(new OpenApiSecurityRequirement
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

            return services;
        }
        public static IServiceCollection AddAutoMapperConfiguration(this IServiceCollection services)
        {
            services.AddAutoMapper(typeof(MappingProfile));
            return services;
        }

        //===============================================

        public static IServiceCollection ConfigureApiServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddApplicationServices();
            services.AddHelperServices();
            services.AddSmtpClient(configuration);
            services.AddAutoMapperConfiguration();
            services.AddJwtAuthentication(configuration);
            services.ConfigureKestrel();

            return services;
        }
    }
}
