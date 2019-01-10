using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace TicTacToeServer.Services
{
    public interface IJwtTokenService
    {
        JwtSecurityToken GetToken(List<Claim> claims);
    }
}