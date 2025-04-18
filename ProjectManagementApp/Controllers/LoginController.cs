using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ProjectManagementApp.Helpers.Context;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ProjectManagementApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly ProjectDbContext _dbcontext;
        private readonly ILogger<ProjectController> _logger;

        public LoginController(ProjectDbContext dbcontext, ILogger<ProjectController> logger)
        {
            _dbcontext = dbcontext;
            _logger = logger;
        }

        [HttpPost]

        public async Task<JsonResult> Login(string userEmail, string userPass)
        {
            try
            {
                if (string.IsNullOrEmpty(userEmail) || string.IsNullOrEmpty(userPass))
                {
                    return new JsonResult(new { success = false, message = "Invalid login data." });
                }

                var user = await _dbcontext.mst_User
                    .FirstOrDefaultAsync(u => u.Email == userEmail && u.Password == userPass);

                if (user == null)
                {
                    return new JsonResult(new { success = false, message = "Invalid username or password." });
                }

                string? userRole = await _dbcontext.mst_Role
                    .Where(a => a.RoleID == user.RoleID)
                    .Select(a => a.RoleName)
                    .FirstOrDefaultAsync();

                var claims = new List<Claim>
                    {
                        new Claim("role", userRole ?? ""),
                        new Claim("roleID", user.RoleID.ToString()),
                        new Claim("userName", user.UserName)
                    };

                var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("YourVeryLongAndSecureSecretKey12345!"));
                var signinCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);

                var tokenOptions = new JwtSecurityToken(
                    issuer: "https://localhost:5001",
                    audience: "https://localhost:5001",
                    claims: claims,
                    expires: DateTime.Now.AddMinutes(5),
                    signingCredentials: signinCredentials
                );

                var tokenString = new JwtSecurityTokenHandler().WriteToken(tokenOptions);

                return new JsonResult(new { success = true, message = "Login successful.", token = tokenString });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during login.");
                return new JsonResult(new { success = false, message = "An error occurred during login." });
            }
        }
    }

}
