﻿using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using NSE.Identidade.API.Extensions;
using NSE.Identidade.API.Models;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace NSE.Identidade.API.Controllers
{
  [Route("api/identidade")]
  public class AuthController : MainController
  {
    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly AppSettings _appSettings;

    public AuthController(SignInManager<IdentityUser> signInManager, UserManager<IdentityUser> userManager, IOptions<AppSettings> appSettings)
    {
      _signInManager = signInManager;
      _userManager = userManager;
      _appSettings = appSettings.Value;
    }

    [HttpPost("nova-conta")]
    public async Task<ActionResult> Registrar(UsuarioRegistro usuarioRegistro)
    {
      return new StatusCodeResult(403);

      if (!ModelState.IsValid) return CustomResponse(ModelState);

      var user = new IdentityUser
      {
        UserName = usuarioRegistro.Email,
        Email = usuarioRegistro.Email,
        EmailConfirmed = true
      };

      var result = await _userManager.CreateAsync(user, usuarioRegistro.Senha);

      if (result.Succeeded)
      {
        return CustomResponse(await GerarJwt(usuarioRegistro.Email));
      }

      foreach (var error in result.Errors)
      {
        AdicionarErroProcessamento(error.Description);
      }

      return CustomResponse();
    }

    [HttpPost("autenticar")]
    public async Task<ActionResult> Login(UsuarioLogin usuarioLogin)
    {
      if (!ModelState.IsValid) return CustomResponse(ModelState);

      var result = await _signInManager.PasswordSignInAsync(
                                                      userName: usuarioLogin.Email,
                                                      password: usuarioLogin.Senha,
                                                      isPersistent: false,
                                                      lockoutOnFailure: true
                                                      );

      if (result.Succeeded)
      {
        return CustomResponse(await GerarJwt(usuarioLogin.Email));
      }

      if (result.IsLockedOut)
      {
        AdicionarErroProcessamento("Usuário temporariamente bloqueado por tentativas inválidas.");
        return CustomResponse();
      }

      AdicionarErroProcessamento("Usuário ou senha incorretos.");
      return CustomResponse();
    }

    private static long ToUnixEpochDate(DateTime date)
      => (long)Math.Round((date.ToUniversalTime() - new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero)).TotalSeconds);

    private async Task<UsuarioRespostaLogin> GerarJwt(string email)
    {
      var user = await _userManager.FindByEmailAsync(email);
      var claims = await _userManager.GetClaimsAsync(user);


      var identityClaims = await ObterClaimsUsuario(claims, user);
      var encodedToken = CodificarToken(identityClaims);
      return ObterRespostaToken(encodedToken, user, claims);
    }

    private async Task<ClaimsIdentity> ObterClaimsUsuario(ICollection<Claim> claims, IdentityUser user)
    {
      var userRoles = await _userManager.GetRolesAsync(user);

      claims.Add(new Claim(JwtRegisteredClaimNames.Sub, user.Id));
      claims.Add(new Claim(JwtRegisteredClaimNames.Email, user.Email));
      claims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));
      claims.Add(new Claim(JwtRegisteredClaimNames.Nbf, ToUnixEpochDate(DateTime.UtcNow).ToString())); // QUANDO O TOKEN VAI EXPIRAR
      claims.Add(new Claim(JwtRegisteredClaimNames.Iat, ToUnixEpochDate(DateTime.UtcNow).ToString(), ClaimValueTypes.Integer64)); // QUANDO O TOKEN FOI EMITIDO

      // Definição: role é um papel e uma claim é um dado aberto
      foreach (var userRole in userRoles)
      {
        claims.Add(new Claim("role", userRole));
      }

      var identityClaims = new ClaimsIdentity();
      identityClaims.AddClaims(claims);

      return identityClaims;
    }

    private string CodificarToken(ClaimsIdentity identityClaims)
    {
      var tokenHandler = new JwtSecurityTokenHandler();
      var key = Encoding.ASCII.GetBytes(_appSettings.Secret);

      var token = tokenHandler.CreateToken(new SecurityTokenDescriptor
      {
        Issuer = _appSettings.Emissor,
        Audience = _appSettings.ValidoEm,
        Subject = identityClaims,
        Expires = DateTime.UtcNow.AddHours(_appSettings.ExpiracaoHoras),
        SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
      });

      var encodedToken = tokenHandler.WriteToken(token);
      return encodedToken;
    }

    private UsuarioRespostaLogin ObterRespostaToken(string encodedToken, IdentityUser user, ICollection<Claim> claims)
    {
      return new UsuarioRespostaLogin
      {
        AccessToken = encodedToken,
        ExpiresIn = TimeSpan.FromHours(_appSettings.ExpiracaoHoras).TotalSeconds,
        UsuarioToken = new UsuarioToken
        {
          Id = user.Id,
          Email = user.Email,
          Claims = claims.Select(c => new UsuarioClaim { Type = c.Type, Value = c.Value })
        }
      };
    }
  }
}
