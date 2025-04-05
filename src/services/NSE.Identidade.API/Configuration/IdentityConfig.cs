using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using NSE.Identidade.API.Data;
using NSE.Identidade.API.Extensions;
using System.Text;

namespace NSE.Identidade.API.Configuration
{
  public static class IdentityConfig
  {
    public static IServiceCollection AddIdentityConfiguration(this IServiceCollection services,
      IConfiguration configuration)
    {
      // CONFIGURAÇÃO IDENTITY
      // CONFIGURANDO DBCONTEXT
      services.AddDbContext<ApplicationDbContext>(optionsAction: options =>
        options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

      // CONFIGURANDO IDENTITY
      services.AddDefaultIdentity<IdentityUser>()
        .AddRoles<IdentityRole>()
        .AddErrorDescriber<IdentityMensagensPortugues>()
        .AddEntityFrameworkStores<ApplicationDbContext>()
        .AddDefaultTokenProviders();

      // CONFIGURANDO APPSETTINGS
      // PEGADOS DADOS DA APPSETTINGS.DEVELOPMENT E POPULANDO ATRAVES DA CLASSE DA EXTENSIONS/APPSETTINGS
      var appSettingSection = configuration.GetSection(key: "AppSettings");
      services.Configure<AppSettings>(appSettingSection);

      var appSettings = appSettingSection.Get<AppSettings>();
      var key = Encoding.ASCII.GetBytes(appSettings.Secret);

      // CONFIGURANDO AUTHENTICATION E BEARER
      services.AddAuthentication(configureOptions: options =>
      {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
      }).AddJwtBearer(bearerOptions =>
      {
        bearerOptions.RequireHttpsMetadata = true;
        bearerOptions.SaveToken = true;
        bearerOptions.TokenValidationParameters = new TokenValidationParameters
        {
          ValidateIssuerSigningKey = true,
          IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(s: "x")),
          ValidateIssuer = true,
          ValidateAudience = true,
          ValidAudience = "x",
          ValidIssuer = "x"
        };
      });

      return services;
    }

    public static IApplicationBuilder UserIdentityConfiguration(this IApplicationBuilder app)
    {
      app.UseAuthentication();
      app.UseAuthorization();

      return app;
    }
  }
}
