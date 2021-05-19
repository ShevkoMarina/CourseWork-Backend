using CourseBack.Models;
using CourseBack.Repository;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;


namespace CourseBack.Services
{
    public class SecurityService : ISecurityService
    {

        private IUsersRepository _usersRepository;

        public SecurityService(IUsersRepository usersRepository)
        {
            _usersRepository = usersRepository;
        }
        
        public (string, AuthorizeUserResponse) AuthorizeUser(UserRequest request)
        {
            try
            {
                (string error, Guid userId) = _usersRepository.AuthorizeUser(request);

      
                var userIdentity = new ClaimsIdentity();
                userIdentity.AddClaim(new Claim(ClaimsIdentity.DefaultNameClaimType, userId.ToString()));
                userIdentity.AddClaim(new Claim(ClaimsIdentity.DefaultRoleClaimType, "user"));

                // создаем JWT-токен
                var jwt = new JwtSecurityToken(
                        issuer: AuthOptions.ISSUER,
                        audience: AuthOptions.AUDIENCE,
                        notBefore: DateTime.UtcNow,
                        claims: userIdentity.Claims,
                        expires: DateTime.UtcNow.Add(TimeSpan.FromMinutes(AuthOptions.LIFETIME)),
                        signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));
                var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

                AuthorizeUserResponse response = new AuthorizeUserResponse()
                {
                    AccessToken = encodedJwt,
                    UserId = userIdentity.Name
                };

                return (error, response);
            } 
            catch (Exception ex)
            {
                return (ex.Message, null);
            }
        }
    }
}
