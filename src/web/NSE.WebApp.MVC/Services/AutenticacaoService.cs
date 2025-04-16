using Microsoft.Extensions.Options;
using NSE.WebApp.MVC.Extensions;
using NSE.WebApp.MVC.Models;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace NSE.WebApp.MVC.Services
{
  public class AutenticacaoService : Service, IAutenticacaoService
  {
    private readonly HttpClient _httpClient;

    public AutenticacaoService(HttpClient httpClient, IOptions<AppSettings> settings)
    {
      httpClient.BaseAddress = new Uri(settings.Value.AutenticacaoUrl);

      _httpClient = httpClient;
    }

    public async Task<UsuarioRespostaLogin> Login(UsuarioLogin usuarioLogin)
    {
      // FAZENDO O SERIALIZE
      var loginContent = ObterConteudo(usuarioLogin);

      // CHAMADA API VIA HTTPCLIENT
      var response = await _httpClient.PostAsync("/api/identidade/autenticar", loginContent);

      if (!TratarErrosResponse(response))
      {
        return new UsuarioRespostaLogin
        {
          ResponseResult = await DeserializarObjetoResponse<ResponseResult>(response)
        };
      }

      return await DeserializarObjetoResponse<UsuarioRespostaLogin>(response);
    }

    public async Task<UsuarioRespostaLogin> Registro(UsuarioRegistro usuarioRegistro)
    {
      var registroContent = ObterConteudo(usuarioRegistro);

      var response = await _httpClient.PostAsync("/api/identidade/nova-conta", registroContent);

      if (!TratarErrosResponse(response))
      {
        return new UsuarioRespostaLogin
        {
          ResponseResult = await DeserializarObjetoResponse<ResponseResult>(response)
        };
      }

      return await DeserializarObjetoResponse<UsuarioRespostaLogin>(response);
    }
  }
}
