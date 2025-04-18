﻿using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using System;

namespace NSE.Identidade.API.Configuration
{
  public static class SwaggerConfig
  {
    public static IServiceCollection AddSwaggerConfiguration(this IServiceCollection services)
    {
      services.AddSwaggerGen(c =>
      {
        c.SwaggerDoc(name: "v1", new OpenApiInfo
        {
          Title = "NerdStore Enterprise Identity API",
          Description = "Esta API faz parte do curso ASP.NET Core Enterprise Applications.",
          Contact = new OpenApiContact() { Name = "Wendell Santos", Email = "dev.wendell@gmail.com" },
          License = new OpenApiLicense() { Name = "MIT", Url = new Uri("https://opensource.org/licenses/MIT") }
        });
      });

      return services;
    }

    public static IApplicationBuilder UseSwaggerConfiguration(this IApplicationBuilder app)
    {
      app.UseSwagger();
      app.UseSwaggerUI(c =>
      {
        c.SwaggerEndpoint(url: "/swagger/v1/swagger.json", name: "v1");
      });

      return app;
    }
  }
}
