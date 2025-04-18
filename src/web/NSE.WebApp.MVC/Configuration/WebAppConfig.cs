﻿using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NSE.WebApp.MVC.Extensions;

namespace NSE.WebApp.MVC.Configuration
{
  public static class WebAppConfig
  {
    public static void AddMvcConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
      services.AddControllersWithViews();

      services.Configure<AppSettings>(configuration);
    }

    public static void UseMvcConfiguration(this IApplicationBuilder app, IWebHostEnvironment env)
    {
      //if (env.IsDevelopment())
      //{
      //  app.UseDeveloperExceptionPage();
      //}
      //else
      //{
      //  app.UseExceptionHandler("/erro/500");
      //  app.UseStatusCodePagesWithRedirects("/erro/{0}");
      //  app.UseHsts();
      //}

      app.UseExceptionHandler("/erro/500");
      app.UseStatusCodePagesWithRedirects("/erro/{0}");
      app.UseHsts();

      app.UseHttpsRedirection();
      app.UseStaticFiles();

      app.UseRouting();

      // USANDO CHAMADA A CLASSE IdentityConfig
      app.UseIdentityConfiguration();

      // USANDO MIDDLEWARE CONFIGURADO
      app.UseMiddleware<ExceptionMiddleware>();

      app.UseEndpoints(endpoints =>
      {
        endpoints.MapControllerRoute(
                  name: "default",
                  pattern: "{controller=Home}/{action=Index}/{id?}");
      });
    }
  }
}
