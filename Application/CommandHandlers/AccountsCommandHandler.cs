using Application.Commands;
using Application.Common.Identities;
using Application.Common.interfaces;
using Domain.Entities;
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
        private readonly IApplicationDbContext _applicationDbContext;
        private readonly IConfiguration _configuration;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly string key;

        public AccountsCommandHandler(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IApplicationDbContext applicationDbContext, IConfiguration configuration)
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



            if(IsValidUser()){
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
                var empDetail = _applicationDbContext.Employees.FirstOrDefault(a => a.Email == identityUser.Email);
                employeeDetail = empDetail != null ? empDetail : new Employees();
                employeeDetail.Token = tokenResult.token;
            }
            }

            return employeeDetail;
        }
        
        private bool IsValidUser(UserManager identityUser){
            if(identityUser != null){
                return identityUser.IsActive;
            }else{
                return false;
            }
        }

        private JwtSecurityToken GenerateJwtToken(List<Claim> userClaims)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Tokens:JwtKey"]));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                                             issuer: _configuration["Tokens:JwtIssuer"],
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
