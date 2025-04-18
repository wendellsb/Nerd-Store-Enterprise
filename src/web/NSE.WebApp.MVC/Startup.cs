using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NSE.WebApp.MVC.Configuration;

namespace NSE.WebApp.MVC
{
  public class Startup
  {    
    public IConfiguration Configuration { get; }

    public Startup(IHostEnvironment hostEnvironment)
    {
      var builder = new ConfigurationBuilder()
        .SetBasePath(hostEnvironment.ContentRootPath)
        .AddJsonFile("appsettings.json", true, true)
        .AddJsonFile($"appsettings.{hostEnvironment.EnvironmentName}.json", true, true)
        .AddEnvironmentVariables();

      if (hostEnvironment.IsDevelopment())
      {
        builder.AddUserSecrets<Startup>();
      }

      Configuration = builder.Build();
    }

    public void ConfigureServices(IServiceCollection services)
    {
      // ADICIONANDO CHAMADA A CLASSE IdentityConfig
      services.AddIdentityConfiguration();

      // ADICIONANDO CHAMADA A CLASSE WebAppConfig
      services.AddMvcConfiguration(Configuration);

      services.RegisterServices();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
      // USANDO CHAMADA A CLASSE WebAppConfig
      app.UseMvcConfiguration(env);
    }
  }
}
