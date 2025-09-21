using APICatalogo.Domain.Entities;
using APICatalogo.DTOs;
using APICatalogo.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace APICatalogo.Controllers;
[Route("[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly ITokenService _tokenService;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IConfiguration _configuration;
    private readonly ILogger<AuthController> _logger;

    public AuthController(ITokenService tokenService, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration, ILogger<AuthController> logger)
    {
        _tokenService = tokenService;
        _userManager = userManager;
        _roleManager = roleManager;
        _configuration = configuration;
        _logger = logger;
    }

    [HttpPost]
    [Route("login")]
    public async Task<IActionResult> Login([FromBody] LoginModel loginModel)
    {
        var user = await _userManager.FindByNameAsync(loginModel.UserName!);        //Adquirindo o usuario atraves do FromBody

        if (user is not null && await _userManager.CheckPasswordAsync(user, loginModel.Password!))      //Verificando se o usuario foi encontrado e se a senha está correta
        {
            var userRoles = await _userManager.GetRolesAsync(user);     //obtendo os perfis dos usuarios

            var authClaims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName!),
                new Claim(ClaimTypes.Email, user.Email!),
                new Claim("id", user.UserName!),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                //Lista de claims usadas para serem incluidas no token, a ultima trata-se de um id exclusivo para o token, em formato GUID
            };

            foreach (var userRole in userRoles)
            {
                authClaims.Add(new Claim(ClaimTypes.Role, userRole));
                //Adicionando nova claim para cada perfil de usuário, para serem incluidos no token
            }

            var token = _tokenService.GenerateAccessToken(authClaims, _configuration);      //Gerando o token

            var refreshToken = _tokenService.GenerateRefreshToken();            //Gerando o RefreshToken

            _ = int.TryParse(_configuration["JWT:RefreshTokenValidityInMinutes"], out int refreshTokenValidityInMinutes);
            //Transformando o valor que está no appSetings pra Int e colocando na variavel refreshTokenValidityInMinutes

            user.RefreshToken = refreshToken;   //adicionando o refreshtoken ao usuario

            user.RefreshTokenExpiryTime = DateTime.Now.AddMinutes(refreshTokenValidityInMinutes);
            //adicionando a minutagem até a expiração do token ao usuario

            await _userManager.UpdateAsync(user);   //atualizando os valores de usuario no repositorio

            return Ok(new
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                RefreshToken = refreshToken,
                Expiration = token.ValidTo,
                //Retorna o Token, o Refresh e a data de expiração num objeto Json
            });
        }
        return Unauthorized();
    }

    [HttpPost]
    [Route("register")]

    public async Task<IActionResult> Register([FromBody] RegisterModel model)
    {
        var userExists = await _userManager.FindByNameAsync(model.UserName!);

        if (userExists != null)         //verifica se o usuario já existe
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new Response
            {
                Status = "Error",
                Message = "Usuário já existente",
            });
        }

        ApplicationUser user = new()        //Usando a herança de ApplicationUser com IdentityUser
        {
            Email = model.Email,
            SecurityStamp = Guid.NewGuid().ToString(),
            UserName = model.UserName,
            //O SecurityStamp é, em essência, um valor aleatório (uma string GUID) que o ASP.NET Core Identity armazena na tabela de usuários (AspNetUsers) no banco de dados.
            //Seu principal objetivo é servir como um mecanismo para invalidar remotamente todos os tokens ou cookies de login de um usuário quando uma alteração de segurança crítica acontece.
        };
        var result = await _userManager.CreateAsync(user, model.Password!);     //Criando usuário

        if (!result.Succeeded)      //Caso possua falha na criação do usuario...
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new Response
            {
                Status = "Error",
                Message = "Falha ao criar o usuário",
            });
        }
        return Ok(new Response { Status = "Success", Message = "Usuário criado com sucesso!" });
    }

    [HttpPost]
    [Route("refresh-token")]

    public async Task<IActionResult> RefreshToken(TokenModel tokenModel)
    {
        if (tokenModel == null)
        {
            return BadRequest();
        }
        //Pega o acessToken (token expirado) e refreshToken (token renovado), caso não existam lançam exceção
        string? accessToken = tokenModel.AccessToken ?? throw new ArgumentNullException(nameof(tokenModel));
        string? refreshToken = tokenModel.RefreshToken ?? throw new ArgumentNullException(nameof(tokenModel));

        var principal = _tokenService.GetPrincipalFromExpiredToken(accessToken!, _configuration);
        //Extrai as claims do token expirado

        if (principal == null)
        {
                return BadRequest("Acess token/Refresh token inválidos");
        }
        
        string username = principal.Identity.Name;

        var user = await _userManager.FindByNameAsync(username!);

        //Verificando se o usuario é nulo, se o refreshtoken está correto e se ele já foi expirado
        if (user == null || user.RefreshToken != refreshToken || user.RefreshTokenExpiryTime <= DateTime.Now)
        {
            return BadRequest("Acesso inválido");
        }
        //Gerando novo acess token e refresh com as claims
        var newAccessToken = _tokenService.GenerateAccessToken(principal.Claims.ToList(), _configuration);

        var newRefreshToken = _tokenService.GenerateRefreshToken();

        user.RefreshToken = newRefreshToken;
        await _userManager.UpdateAsync(user);

        return new ObjectResult(new
        {
            accessToken = new JwtSecurityTokenHandler().WriteToken(newAccessToken),
            refreshToken = newRefreshToken,
        });
    }

    [HttpPost]
    [Route("revoke/{username}")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "ExclusiveOnly")]

    public async Task<IActionResult> Revoke(string username)
    {
        var user = await _userManager.FindByNameAsync(username);

        if (user == null)
        {
            return BadRequest("usuário inválido");
        }

        user.RefreshToken = null;

        await _userManager.UpdateAsync(user);

        return NoContent();
    }

    [HttpPost]
    [Route("CreateRole")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "SuperAdminOnly")]

    public async Task<IActionResult> CreateRole(string roleName)
    {
        var roleExist = await _roleManager.RoleExistsAsync(roleName);       //verifica se o nome já existe 
        if (!roleExist)
        {
            var roleResult = await _roleManager.CreateAsync(new IdentityRole(roleName));

            if (roleResult.Succeeded)        //Se a role for criada... 
            {
                _logger.LogInformation(1, "RolesAdded");
                return StatusCode(StatusCodes.Status200OK, new Response
                {
                    Status = "Success",
                    Message = $"Role {roleName} added successfully"
                });
            }
            else
            {
                _logger.LogInformation(2, "Error");
                return StatusCode(StatusCodes.Status400BadRequest, new Response
                {
                    Status = "Error",
                    Message = $"Issue adding the new {roleName} role"
                });
            }
        }
        return StatusCode(StatusCodes.Status400BadRequest, new Response
        {
            Status = "Error",
            Message = "Role already exist."
        });
    }

    [HttpPost]
    [Route("AddUserToRole")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "SuperAdminOnly")]

    public async Task<IActionResult> AddUserToRole(string email, string roleName)
    {
        var user = await _userManager.FindByEmailAsync(email);

        if (user != null)
        {
            var result = await _userManager.AddToRoleAsync(user, roleName);         //adicionando a role no usuario
            if (result.Succeeded)
            {
                _logger.LogInformation(1, $"Add user {user.Email} to role {roleName}");
                return StatusCode(StatusCodes.Status200OK, new Response
                {
                    Status = "Success",
                    Message = $"Add user {user.Email} to role {roleName}"
                });
            }
            else
            {
                _logger.LogInformation(2, $"Error: Unable to add user {user.Email} to the {roleName} role");
                return StatusCode(StatusCodes.Status400BadRequest, new Response
                {
                    Status = "Error",
                    Message = $"Error: Unable to add user {user.Email} to the {roleName} role"
                });
            }
        }
        return BadRequest(new { error = "Unable to find user" });
    }
}
