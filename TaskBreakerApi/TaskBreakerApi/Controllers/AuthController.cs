using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using TaskBreakerApi.Models;
using TaskBreakerApi.Services;

namespace TaskBreakerApi.Controllers {
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : Controller {
        private readonly IConfiguration _configuration;
        private readonly UserRepository _userRepository;

        public AuthController (IConfiguration configuration, UserRepository userRepository) {
            _configuration = configuration;
            _userRepository = userRepository;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Registration([FromBody] UserRegistration user) {
            if (!System.Text.RegularExpressions.Regex.IsMatch(user.Email, "^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\\.[a-zA-Z]{2,}$")) {
                return BadRequest(new {
                    field = nameof(user.Email),
                    message = "Неправильная почта"
                });
            }

            if (!System.Text.RegularExpressions.Regex.IsMatch(user.PasswordHash, "^(?=.*[a-z])(?=.*[A-Z])[a-zA-Z]{6,30}$")) {
                return BadRequest(new {
                    field = nameof(user.PasswordHash),
                    message = "Некорректный пароль: он должен содержать a-z, A-Z и быть длиной от 6 до 30 символов"
                });
            }

            if (user.PasswordHash != user.PasswordConfirm) {
                return BadRequest(new {
                    field = nameof(user.PasswordConfirm),
                    message = "Пароли не совпадают"
                });
            }

            if (await _userRepository.AnyEmail(user.Email)) {
                return Conflict(new {
                    field = nameof(user.Email),
                    message = "Пользователь с такой почтой уже существует"
                });
            }

            try {
                User newUser = await _userRepository.Register(user);
                return CreatedAtAction(nameof(Registration), new { id = newUser.Id }, newUser);
            } catch (Exception ex) {
                return BadRequest(new {
                    message = ex.Message
                });
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] User login) {
            if (!await _userRepository.AnyEmail(login.Email)) {
                return Unauthorized(new {
                    field = nameof(login.Email),
                    message = "пользователь не найден"
                });
            }

            if (!await _userRepository.CheckPassword(login.Email, login.PasswordHash)) {
                return Unauthorized(new {
                    field = nameof(login.PasswordHash),
                    message = "Неправильный пароль"
                });
            }

            // Генерация токена
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["JwtSettings:Secret"]);

            var tokenDescriptor = new SecurityTokenDescriptor {
                Subject = new ClaimsIdentity(new[] {
                    new Claim(ClaimTypes.Email, login.Email),
                    new Claim("id", (await _userRepository.GetByEmail(login.Email)).Id.ToString())
                }),
                Expires = DateTime.UtcNow.AddDays(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return Ok(new { Token = tokenHandler.WriteToken(token) });
        }

        [HttpGet("check-auth")]
        public IActionResult CheckAuth() {
            if (User.Identity.IsAuthenticated) {
                return Ok(new { Message = "Пользователь авторизован." });
            } else {
                return Unauthorized(new { Message = "Пользователь не авторизован." });
            }
        }

        [Authorize]
        [HttpPut("update")]
        public async Task<IActionResult> UpdateUser([FromBody] User user) {
            await _userRepository.Update(user);
            return Ok();
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id) { 
            var result = await _userRepository.Delete(id);

            if (result) return Ok();
            return BadRequest(new {
                message = "Пользователь с таким id не найден"
            });
        }
    }
}
