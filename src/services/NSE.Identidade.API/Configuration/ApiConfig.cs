using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace NSE.Identidade.API.Configuration
{
  public static class ApiConfig
  {
    public static IServiceCollection AddApiConfiguration(this IServiceCollection services)
    {
      services.AddControllers();

      return services;
    }

    public static IApplicationBuilder UseApiConfiguration(this IApplicationBuilder app, IWebHostEnvironment env)
    {
      // // CONFIGURAÇÃO DE AMBIENTE
      if (env.IsDevelopment())
      {
        app.UseDeveloperExceptionPage();
      }

      app.UseHttpsRedirection();

      app.UseRouting();

      // OBS: ELES SEMPRE PRECISAM ESTAR ENTRE O "UseRouting" E "UseEndpoints"
      // ADICIONANDO AUTORIZAÇÃO E AUTENTICAÇÃO      
      app.UserIdentityConfiguration();

      // CONFIGURAÇÃO DE ENDPOINTS
      app.UseEndpoints(endpoints =>
      {
        endpoints.MapControllers();
      });
      return app;
    }
  }
}
