using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NSE.Identidade.API.Configuration;

namespace NSE.Identidade.API
{
  public class Startup
  {
    public IConfiguration Configuration { get; }
    public Startup(IHostEnvironment hostEnvironment)
    {
      var builder = new ConfigurationBuilder()
        .SetBasePath(hostEnvironment.ContentRootPath)
        .AddJsonFile("appsettings.json", true, true)
        .AddJsonFile("appsettings.Development.json", true, true)
        .AddEnvironmentVariables();

      if (hostEnvironment.IsDevelopment())
      {
        builder.AddUserSecrets<Startup>();
      }

      Configuration = builder.Build();
    }

    public void ConfigureServices(IServiceCollection services)
    {
      // CONFIGURANDO A  IDENTITYCONFIG
      services.AddIdentityConfiguration(Configuration);

      // CONFIGURANDO A APICONFIG
      services.AddApiConfiguration();

      // CONFIGURANDO A SWAGGER
      services.AddSwaggerConfiguration();
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
      // USANDO A CONFIGURAÇÃO SWAGGER
      app.UseSwaggerConfiguration();

      // USANDO A CONFIGURAÇÃO SWAPICONFIGAGGER
      app.UseApiConfiguration(env);
    }
  }
}
