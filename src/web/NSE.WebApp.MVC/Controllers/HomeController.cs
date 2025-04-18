﻿using Microsoft.AspNetCore.Mvc;
using NSE.WebApp.MVC.Models;

namespace NSE.WebApp.MVC.Controllers
{
  public class HomeController : Controller
  {
    public IActionResult Index()
    {
      return View();
    }

    public IActionResult Privacy()
    {
      return View();
    }

    [Route("erro/{id:length(3,3)}")]
    public IActionResult Error(int id)
    {
      var modelErro = new ErrorViewModel();

      if (id == 500)
      {
        modelErro.Mensagem = "Ocorreu um erro! Tente novamente mais tarde ou contate nosso suporte.";
        modelErro.Titulo = "Ocorreu um erro!";
        modelErro.ErroCode = id;
      }
      else if (id == 404)
      {
        modelErro.Mensagem = "A pagina que está procurando nao existe! <br />Em caso de duvida contate nosso suporte.";
        modelErro.Titulo = "Pagina não encontrada!";
        modelErro.ErroCode = id;
      }
      else if (id == 403)
      {
        modelErro.Mensagem = "Você não tem permissão para fazer isso!";
        modelErro.Titulo = "Acesso negado!";
        modelErro.ErroCode = id;
      }
      else
      {
        return StatusCode(404);
      }

      return View("Error", modelErro);
    }
  }
}
