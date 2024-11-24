using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Restaurant_API.Data;
using Restaurant_API.Models;
using Restaurant_API.Models.Dto;
using Restaurant_API.Utility;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Restaurant_API.Repository
{
    public class AuthRepository : IAuthRepository
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly string _secretKey;

        public AuthRepository(ApplicationDbContext db, IConfiguration configuration, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _db = db;
            _userManager = userManager;
            _roleManager = roleManager;
            _secretKey = configuration.GetValue<string>("ApiSettings:Secret");
        }

        public async Task<ApiResponse> RegisterAsync(RegisterRequestDTO model)
        {
            var response = new ApiResponse();

            if (_db.ApplicationUsers.Any(u => u.UserName.ToLower() == model.UserName.ToLower()))
            {
                response.StatusCode = HttpStatusCode.BadRequest;
                response.IsSuccess = false;
                response.ErrorMessages.Add("Username already exists");
                return response;
            }

            var newUser = new ApplicationUser
            {
                UserName = model.UserName,
                Email = model.UserName,
                NormalizedEmail = model.UserName.ToUpper(),
                Name = model.Name
            };

            var result = await _userManager.CreateAsync(newUser, model.Password);
            if (result.Succeeded)
            {
                if (!await _roleManager.RoleExistsAsync(Constants.Role_Admin))
                {
                    await _roleManager.CreateAsync(new IdentityRole(Constants.Role_Admin));
                    await _roleManager.CreateAsync(new IdentityRole(Constants.Role_Customer));
                }

                var role = model.Role.ToLower() == Constants.Role_Admin ? Constants.Role_Admin : Constants.Role_Customer;
                await _userManager.AddToRoleAsync(newUser, role);

                response.StatusCode = HttpStatusCode.OK;
                response.IsSuccess = true;
                return response;
            }

            response.StatusCode = HttpStatusCode.BadRequest;
            response.IsSuccess = false;
            response.ErrorMessages.Add("Error while registering");
            return response;
        }

        public async Task<ApiResponse> LoginAsync(LoginRequestDTO model)
        {
            var response = new ApiResponse();
            var userFromDb = _db.ApplicationUsers.FirstOrDefault(u => u.UserName.ToLower() == model.UserName.ToLower());

            if (userFromDb == null || !await _userManager.CheckPasswordAsync(userFromDb, model.Password))
            {
                response.StatusCode = HttpStatusCode.BadRequest;
                response.IsSuccess = false;
                response.ErrorMessages.Add("Username or password is incorrect");
                return response;
            }

            var roles = await _userManager.GetRolesAsync(userFromDb);
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_secretKey);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim("fullName", userFromDb.Name),
                    new Claim("id", userFromDb.Id.ToString()),
                    new Claim(ClaimTypes.Email, userFromDb.UserName),
                    new Claim(ClaimTypes.Role, roles.FirstOrDefault()),
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            response.StatusCode = HttpStatusCode.OK;
            response.IsSuccess = true;
            response.Result = new LoginResponseDTO
            {
                Email = userFromDb.Email,
                Token = tokenHandler.WriteToken(token)
            };
            return response;
        }
    }
}

