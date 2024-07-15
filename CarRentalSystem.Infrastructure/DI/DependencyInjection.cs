using System.Text;
using CarRentalSystem.Application.DTos;
using CarRentalSystem.Application.IService;
using CarRentalSystem.Domain.Entities;
using CarRentalSystem.Infrastructure.Persistence;
using CarRentalSystem.Infrastructure.Provider;
using CarRentalSystem.Infrastructure.Service;
using CarRentalSystem.Infrastructure.Service.Base;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace CarRentalSystem.Infrastructure.DI;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var jwtSettings = configuration.GetSection("JwtSettings");
        var key = Encoding.ASCII.GetBytes(jwtSettings["Secret"]!);

        services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = new()
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });

        
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(
                configuration.GetConnectionString("DefaultConnection"),
                b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)), ServiceLifetime.Transient);
        
        services.AddIdentity<User, IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();
        
        // HttpClient injection
        services.AddHttpClient();

        // Singleton

        
        // Scoped
        services.AddScoped<IAuthenticationService, AuthenticationService>();
        services.AddScoped<CarService>();
        services.AddScoped<BaseService<Car, CarDto>, CarService>();
        services.AddScoped<BaseService<Config, ConfigDTO>, ConfigService>();
        services.AddScoped<BaseService<Rent, RentalDto>, RentalService>();
        services.AddScoped<BaseService<Offer, OfferDto>, OfferService>();
        services.AddScoped<BaseService<Damage, DamageDto>, DamageService>();
        services.AddScoped<BaseService<Bill, BillDto>, BillService>();
        services.AddScoped<BaseService<Payment, PaymentDto>, PaymentService>();

        
        // Transient
        services.AddTransient<IAttachmentService, AttachmentService>();
        services.AddTransient<IUserService, UserService>();
        services.AddTransient<IRentalService, RentalService>();
        services.AddTransient<ICarService, CarService>();
        services.AddTransient<IConfigService, ConfigService>();
        services.AddTransient<IDamageService, DamageService>();
        services.AddTransient<IBillService, BillService>();
        services.AddTransient<IKhaltiService, KhaltiPaymentService>();
        services.AddTransient<IEmailProvider, GmailEmailProvider>();
        services.AddTransient<IOfferService, OfferService>();
        services.AddTransient<IPaymentService, PaymentService>();
        return services;
    }

}