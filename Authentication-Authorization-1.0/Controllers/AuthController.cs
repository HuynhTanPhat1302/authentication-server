using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Authentication_Authorization_1._0.Models;
using Authentication_Authorization_1._0.Helpers;

[Route("api/auth")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly DataContext _context;

    public AuthController(IConfiguration configuration, DataContext context)
    {
        _configuration = configuration;
        _context = context;
    }

    [HttpGet("secretkey")]
    public IActionResult GetSecretKey()
    {
        var secretKey = SecretKey.GenerateRandomSecretKey(32);
        return Ok(secretKey);
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public IActionResult Login(LoginRequest model)
    {
        // Validate the user's credentials here (e.g., against the database)
        bool isValidUser = ValidateUser(model.Email, model.Password);

        if (!isValidUser)
        {
            return Unauthorized(); // Invalid credentials, return 401 Unauthorized
        }

        // Retrieve the user's role from the database
        string userRole = GetUserRole(model.Email);

        // Generate the JWT token
        var token = GenerateJwtToken(model.Email, userRole);

        // Return the token as a response
        return Ok(new { token });
    }

    private bool ValidateUser(string email, string password)
    {
        var user = _context.Users.FirstOrDefault(u => u.Email == email && u.Password == password);

        return user != null; // Returns true if user exists with the given email and password
    }

    private string GetUserRole(string email)
    {
        var user = _context.Users.FirstOrDefault(u => u.Email == email);

        if (user != null)
        {
            var role = _context.Roles.FirstOrDefault(r => r.RoleID == user.RoleID);

            if (role != null)
            {
                return role.RoleName; // Return the role name associated with the user
            }
        }

        return "null"; // Return a default role if no role is found for the user
    }


    private string GenerateJwtToken(string email, string role)
    {
        // Read configuration values
        var secretKey = _configuration["Jwt:SecretKey"];

        

        // Create the claims for the token
        var claims = new[]
        {
        new Claim(ClaimTypes.Email, email),
        new Claim(ClaimTypes.Role, role),
        // Add any additional claims as needed
        };

        // Create the JWT token
        var token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.UtcNow.AddHours(25), // Set the token expiration time as needed
            signingCredentials: new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)), SecurityAlgorithms.HmacSha256)
        );

        // Serialize the token to a string
        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

        return tokenString;
    }

}

public class LoginRequest
{
    public string Email { get; set; }
    public string Password { get; set; }
}