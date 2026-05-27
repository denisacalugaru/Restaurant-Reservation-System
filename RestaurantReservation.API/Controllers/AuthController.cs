using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using RestaurantReservation.API.DTOs;
using RestaurantReservation.API.Models;

namespace RestaurantReservation.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IConfiguration _configuration;

    public AuthController(
        UserManager<User> userManager,
        SignInManager<User> signInManager,
        RoleManager<IdentityRole> roleManager,
        IConfiguration configuration)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _roleManager = roleManager;
        _configuration = configuration;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterDto dto)
    {
        var user = await _userManager.FindByEmailAsync(dto.Email);
        if (user != null)
        {
            return BadRequest(new { message = "Email-ul este deja folosit." });
        }

        var newUser = new User
        {
            UserName = dto.Email,
            Email = dto.Email,
            FullName = dto.FullName
        };

        var result = await _userManager.CreateAsync(newUser, dto.Password);

        if (result.Succeeded)
        {
            if (dto.Email.ToLower().Contains("admin"))
            {
                if (!await _roleManager.RoleExistsAsync("Admin"))
                    await _roleManager.CreateAsync(new IdentityRole("Admin"));

                await _userManager.AddToRoleAsync(newUser, "Admin");
            }
            else
            {
                if (!await _roleManager.RoleExistsAsync("User"))
                    await _roleManager.CreateAsync(new IdentityRole("User"));

                await _userManager.AddToRoleAsync(newUser, "User");
            }

            return Ok(new { message = "Cont creat cu succes!" });
        }

        var errorDetails = string.Join(" ", result.Errors.Select(e => e.Description));
        return BadRequest(new { message = errorDetails });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto dto)
    {
        var user = await _userManager.FindByEmailAsync(dto.Email);
        if (user == null)
        {
            return Unauthorized(new { message = "Email sau parolă incorectă." });
        }

        var result = await _signInManager.CheckPasswordSignInAsync(user, dto.Password, false);
        if (result.Succeeded)
        {
            var roles = await _userManager.GetRolesAsync(user);
            var token = GenerateJwtToken(user, roles);

            return Ok(new { token = token });
        }

        return Unauthorized(new { message = "Email sau parolă incorectă." }); //401
    }

    private string GenerateJwtToken(User user, IList<string> roles)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.Email, user.Email!),
            new Claim("fullName", user.FullName)
        };

        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        var keyString = _configuration["Jwt:Key"] ?? "asdmifnjkeoirnkQWEJDFKDAOWIKJSDFJKDM!";
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyString));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"] ?? "RestaurantAPI",
            audience: _configuration["Jwt:Audience"] ?? "RestaurantUI",
            claims: claims,
            expires: DateTime.UtcNow.AddDays(7),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}