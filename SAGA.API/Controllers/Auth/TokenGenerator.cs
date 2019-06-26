using System;
using System.Configuration;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using SAGA.API.Dtos;
using System.Web.Script.Serialization;

namespace SAGA.API.Controllers
{
    public class TokenGenerator
    {
        public static string GenerateTokenJwt(UsuarioDto username)
        {
            // appsetting for Token JWT
            var secretKey = ConfigurationManager.AppSettings["JWT_SECRET_KEY"];
            var audienceToken = ConfigurationManager.AppSettings["JWT_AUDIENCE_TOKEN"];
            var issuerToken = ConfigurationManager.AppSettings["JWT_ISSUER_TOKEN"];
            var expireTime = ConfigurationManager.AppSettings["JWT_EXPIRE_MINUTES"];

            var securityKey = new SymmetricSecurityKey(System.Text.Encoding.Default.GetBytes(secretKey));
            var signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);

            // create a claimsIdentity
            var jsonSerialiser = new JavaScriptSerializer();
            var privilegios = jsonSerialiser.Serialize(username.Privilegios);
            ClaimsIdentity claimsIdentity = new ClaimsIdentity(new[] {
                new Claim("IdUsuario", username.Id.ToString()),
                new Claim("Clave", username.Clave),
                new Claim("Email", username.Email),
                new Claim("TipoUsuarioId", username.TipoUsuarioId.ToString()),
                new Claim("DepartamentoId", username.DepartamentoId.ToString()),
            });

            // create token to the user
            var tokenHandler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
            var jwtSecurityToken = tokenHandler.CreateJwtSecurityToken(
                audience: audienceToken,
                issuer: issuerToken,
                subject: claimsIdentity,
                notBefore: DateTime.UtcNow,
                expires: DateTime.UtcNow.AddMinutes(Convert.ToInt32(expireTime)),
                signingCredentials: signingCredentials);

            var jwtTokenString = tokenHandler.WriteToken(jwtSecurityToken);
            return jwtTokenString;
        }

        public static string GenerateTokenUser(UsuarioDto username)
        {
            // appsetting for Token JWT
            var secretKey = ConfigurationManager.AppSettings["JWT_SECRET_KEY"];
            var securityKey = new SymmetricSecurityKey(System.Text.Encoding.Default.GetBytes(secretKey));
            var signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);

            // create a claimsIdentity
            var jsonSerialiser = new JavaScriptSerializer();
            var privilegios = jsonSerialiser.Serialize(username.Privilegios);
            ClaimsIdentity claimsIdentity = new ClaimsIdentity(new[] {
                new Claim("IdUsuario", username.Id.ToString()),
                new Claim("Clave", username.Clave),
                new Claim("Nombre", username.Nombre),
                new Claim("Sucursal", username.Sucursal),
                new Claim("Email", username.Email),
                new Claim("TipoUsuarioId", username.TipoUsuarioId.ToString()),
                new Claim("Tipo", username.Tipo),
                new Claim("Privilegios",privilegios ),
                new Claim("Usuario", username.Usuario ),
                new Claim("Lider", username.Lider ),
                new Claim("LiderId", username.LiderId.ToString()),
                new Claim("DepartamentoId", username.DepartamentoId.ToString()),
                new Claim("Departamento", username.Departamento),
                new Claim("UnidadNegocioId", username.UnidadNegocioId.ToString())
            });

            // create token to the user
            var tokenHandler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
            var jwtSecurityToken = tokenHandler.CreateJwtSecurityToken(
                subject: claimsIdentity,
                notBefore: DateTime.UtcNow,
                signingCredentials: signingCredentials);

            var jwtTokenString = tokenHandler.WriteToken(jwtSecurityToken);
            return jwtTokenString;

        }
    }   
}