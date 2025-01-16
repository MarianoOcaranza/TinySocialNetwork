using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using backend.Models.DTOs;

namespace backend.Controllers 
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly SocialNetworkContext _context;
        private readonly IConfiguration _configuration;
        private readonly SymmetricSecurityKey _securityKey;

        public AuthController(SocialNetworkContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
            //JWT Key
            var key = _configuration["Jwt:Key"];
            if(string.IsNullOrEmpty(key))
            {
                throw new Exception("Jwt key not configured");
            }
            _securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
        }

        //Register 
        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] RegisterUserDto registerDto)
        {
            //Register Validators
            var emailValidation = ValidateEmail(registerDto.Email);
            if (!emailValidation)
            {
                return BadRequest(new {message="Please enter a valid email"});
            }
            if(_context.Users.Any(u=> u.Email == registerDto.Email)) {
                return Conflict(new {message= "There is already an active account with this email"});
            }
            if (_context.Users.Any(u=> u.Username == registerDto.Username))
            {
                return Conflict(new {message = "Username already exists."});
            }
            var passwordValidation = ValidatePassword(registerDto.Password);
            if (!string.IsNullOrEmpty(passwordValidation))
            {
                return BadRequest(new {message = passwordValidation});
            }

            //Everything is OK? then continue! Create the new user object and hash password
            var user = new User 
            {
                Username = registerDto.Username,
                Email = registerDto.Email,
                PasswordHash = BCrypt.Net.BCrypt.EnhancedHashPassword(registerDto.Password),
                IsEmailConfirmed = false
            };
            //Add to the database
            _context.Users.Add(user);
            //Save the changes asynchronously to give enough time to the DB access
            await _context.SaveChangesAsync();
            return Ok(new { message = "User successfully registered"});

        }

        //Login
        [HttpPost("Login")]
        public IActionResult Login([FromBody] LoginUserDto loginDto)
        {
            var user = _context.Users.FirstOrDefault(u => u.Username == loginDto.Username); //Login with username
    
            
            if (user == null || !BCrypt.Net.BCrypt.EnhancedVerify(loginDto.Password, user.PasswordHash))
            {
                return Unauthorized(new { message = "Invalid credentials."});
            }
            //Valid credentials? then generate JWT Token
            var token = GenerateJwtToken(user);
            return Ok(new { token });
        }

        //STATIC FUNCTIONS!
        //Token generator for login
        private string GenerateJwtToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor {
                Subject = new ClaimsIdentity([
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Name, user.Username)
                ]),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(_securityKey, SecurityAlgorithms.HmacSha256Signature),
                Issuer = _configuration["Jwt:Issuer"],
                Audience = _configuration["Jwt:Audience"]
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        //Password register validator
        public static string ValidatePassword(string password)
        {
            if (string.IsNullOrEmpty(password)) return "Password cannot be empty.";
            if (password.Length < 8) return "Password must contain at least 8 characters.";
            if (!password.Any(char.IsUpper)) return "Password must contain at least one upper case.";
            if (!password.Any(char.IsLower)) return "Password must contain at least one lower case.";
            if (!password.Any(char.IsDigit)) return "Password must contain at least one digit.";

            return string.Empty; //means that password is valid!
        }
        //Email register validator
        public static bool ValidateEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }
    }   
}

