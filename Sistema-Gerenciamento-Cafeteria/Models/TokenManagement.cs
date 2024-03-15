using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Sistema_Gerenciamento_Cafeteria.Models
{
    public class TokenManagement
    {
        public static string Secret = "qwretqretqrwterqterqtwretqrwterqterqrwte6273846251";

        public static string GenerateToken(string email, string role)
        {
            SymmetricSecurityKey securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Secret));
            SecurityTokenDescriptor descriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] { new Claim(ClaimTypes.Email, email), new Claim(ClaimTypes.Role, role) }),
                Expires = DateTime.UtcNow.AddHours(8),
                SigningCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature)
            };
            JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
            JwtSecurityToken token = handler.CreateJwtSecurityToken(descriptor);
            return handler.WriteToken(token);
        }

        public static ClaimsPrincipal GetPrincipal(string token)
        {
            try 
            {
                JwtSecurityTokenHandler TokenHandler = new JwtSecurityTokenHandler();
                JwtSecurityToken jwtToken = (JwtSecurityToken)TokenHandler.ReadToken(token);
                if(jwtToken == null)
                {
                    return null;
                }
                TokenValidationParameters parameters = new TokenValidationParameters()
                {
                    RequireExpirationTime = true,
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Secret)),
                };

                SecurityToken securityToken;
                ClaimsPrincipal principal = TokenHandler.ValidateToken(token, parameters, out securityToken);
                return principal;

            }
            catch(Exception e)
            {
                return null;
            }
        }
        
        /*Método faz a validação do Token*/
        public static TokenClaims ValidateToken(string RawToken)
        {
            
            string[] array = RawToken.Split(' ');
            var token = array[1];
            ClaimsPrincipal principal = GetPrincipal(token);
            if(principal == null)
            {
                return null; 
            }
            ClaimsIdentity identity = null;
            try 
            {
                identity = (ClaimsIdentity)principal.Identity;
            }
            catch(Exception ex)
            {
                return null;
            }

            TokenClaims tokenClaims = new TokenClaims();
            var temp = identity.FindFirst(ClaimTypes.Email);
            tokenClaims.email = temp.Value;
            temp = identity.FindFirst(ClaimTypes.Role);
            tokenClaims.role = temp.Value;
            return tokenClaims;
        }
    }

    
}