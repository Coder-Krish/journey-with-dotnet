using Application.Commands;
using Domain.Entities;
using Infrastructure.DataAccess;
using Infrastructure.Identity;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Application.CommandHandlers
{
    public class AccountsCommandHandler : IRequestHandler<AccountsCommand, Employees>
    {
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly IConfiguration _configuration;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public AccountsCommandHandler(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, ApplicationDbContext applicationDbContext, IConfiguration configuration)
        {
            _applicationDbContext = applicationDbContext;
            _configuration = configuration;
            _userManager = userManager;
            _signInManager = signInManager;
        }
        public async Task<Employees> Handle(AccountsCommand request, CancellationToken cancellationToken)
        {
            Employees employeeDetail = new Employees();
            var loginCredentials = request.login;
            var identityUser = await _userManager.FindByNameAsync(loginCredentials.UserName);

            if (identityUser == null || identityUser.IsActive == false)
                return null;


            var result = await _signInManager.CheckPasswordSignInAsync(identityUser, loginCredentials.Password, lockoutOnFailure: false);
            if (result.Succeeded)
            {
                List<Claim> userClaims = await ConstructUserClaimsAsync(identityUser);

                JwtSecurityToken token = GenerateJwtToken(userClaims);

                var tokenResult = new
                {
                    token = new JwtSecurityTokenHandler().WriteToken(token),
                    expiration = token.ValidTo
                };
                employeeDetail = _applicationDbContext.Employees.FirstOrDefault(a => a.Email == identityUser.Email);

                employeeDetail.Token = tokenResult.token;
            }


            return employeeDetail;
        }


        private JwtSecurityToken GenerateJwtToken(List<Claim> userClaims)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Tokens:JwtKey"]));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(issuer: _configuration["Tokens:JwtIssuer"],
                                             audience: _configuration["Tokens:JwtAudience"],
                                             claims: userClaims,
                                             expires: DateTime.UtcNow.AddMinutes(Convert.ToDouble(_configuration["Tokens:JwtValidMinutes"])),
                                             signingCredentials: creds);

            return token;
        }

        private async Task<List<Claim>> ConstructUserClaimsAsync(ApplicationUser identityUser)
        {
            var roles = await _userManager.GetRolesAsync(identityUser);
            List<Claim> roleClaims = roles.Select(role => new Claim("roles", role)).ToList();

            // other claims of User if present
            List<Claim> userClaims = (await _userManager.GetClaimsAsync(identityUser)).ToList();

            userClaims = userClaims.Union(roleClaims).ToList();

            userClaims = new List<Claim>(userClaims)
            {
                new Claim(JwtRegisteredClaimNames.Sub, identityUser.Id),
                new Claim(JwtRegisteredClaimNames.UniqueName, identityUser.UserName),
                new Claim(JwtRegisteredClaimNames.Email, identityUser.Email)
            };

            return userClaims;
        }
    }
}
