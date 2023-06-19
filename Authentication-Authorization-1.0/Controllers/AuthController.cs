using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Authentication_Authorization_1._0.Models;
using Authentication_Authorization_1._0.Helpers;
using Authentication_Authorization_1._0.ApiModels;

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


    [HttpPost("create-account")]
    [AllowAnonymous]
    public IActionResult CreateAccount(RegisterRequestModel model)
    {
        // Validate the registration data
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        // Check if the email already exists in the database
        if (!string.IsNullOrEmpty(model.Email))
        {
            if (EmailExists(model.Email))
            {
                return Conflict("Email already exists");
            }
        }






        if (string.IsNullOrEmpty(model.Email) || string.IsNullOrEmpty(model.Password))
        {
            return BadRequest();
        }
        // Create a new user object
        var user = new User
        {
            Email = model.Email,
            Password = model.Password,
            RoleID = model.RoleID, // Set the user's role ID based on the registration data
            CreatedAt = model.CreatedAt,
        };

        // Save the new user to the database
        _context.Users.Add(user);
        _context.SaveChanges();



        // Return the token as a response
        return Ok();
    }

    [HttpPut("update-account/{email}")]
    [AllowAnonymous]
    public IActionResult UpdateAccount(string email, UpdateAccountRequestModel model)
    {
        // Validate the email and model
        if (string.IsNullOrEmpty(email))
        {
            return BadRequest(ModelState);
        }

        // Check if the email exists in the database
        var existingUser = _context.Users.FirstOrDefault(u => u.Email == email);
        if (existingUser == null)
        {
            return NotFound();
        }

        // Update the user properties based on the model, only if they are provided
        if (!string.IsNullOrEmpty(model.Password))
        {
            existingUser.Password = model.Password;
        }

        if (!string.IsNullOrEmpty(model.Email))
        {
            existingUser.Email = model.Email;
        }

        if (model.RoleID.HasValue)
        {
            existingUser.RoleID = model.RoleID.Value;
        }


        // Save the changes to the database
        _context.SaveChanges();

        return NoContent();
    }


    private bool EmailExists(string email)
    {
        return _context.Users.Any(u => u.Email == email);
    }


}

public class LoginRequest
{
    public string Email { get; set; }
    public string Password { get; set; }
}