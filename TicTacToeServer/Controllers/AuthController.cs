using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using TicTacToeServer.DTO;
using TicTacToeServer.Enums;
using TicTacToeServer.Models;
using TicTacToeServer.Services;

namespace TicTacToeServer.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly IGuidService _guidService;
        private readonly IJwtTokenService _tokenService;

        public AuthController(
            UserManager<AppUser> userManager, 
            IConfiguration configuration, 
            IGuidService guidService,
            IJwtTokenService tokenService)
        {
            _userManager = userManager;
            _configuration = configuration;
            _guidService = guidService;
            _tokenService = tokenService;
        }

        [Route("register")]
        [HttpPost]
        public async Task<ActionResult> Register([FromBody] RegisterDto model)
        {
            var user = new AppUser
            {
                Email = model.Email,
                UserName = model.Email,
                SecurityStamp = _guidService.NewGuid().ToString()
            };
            var result = await _userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, Roles.User.ToString());
            } 
            else
            {
                return BadRequest(result.Errors);
            }

            return Ok();
        }

        [HttpPost]
        public async Task<ActionResult> Login([FromBody] LoginDto model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);           
            if (user != null && await _userManager.CheckPasswordAsync(user, model.Password))
            {
                var claims = new List<Claim> {
                    new Claim(ClaimTypes.Role, Roles.User.ToString()),
                };
                var token = _tokenService.GetToken(claims);

                return Ok(
                    new TokenWithExpirationDto {
                        Token = new JwtSecurityTokenHandler().WriteToken(token),
                        Expiration = token.ValidTo
                    } 
                );
            }

            return Unauthorized();
        }

        [Authorize]
        [Route("refresh")]
        [HttpGet]
        public async Task<ActionResult> RefreshToken()
        {
            var claims = new List<Claim> {
                    new Claim(ClaimTypes.Role, Roles.User.ToString()),
                };
            var token = _tokenService.GetToken(claims);

            return Ok(
                new TokenWithExpirationDto
                {
                    Token = new JwtSecurityTokenHandler().WriteToken(token),
                    Expiration = token.ValidTo
                }
            );
        }
    }
}