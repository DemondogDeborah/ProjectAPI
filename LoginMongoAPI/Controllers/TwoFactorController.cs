using LoginMongoAPI.Interfaces;
using LoginMongoAPI.Models;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TwoFactorAuthNet;
using TwoFactorAuthNet.Providers.Qr;

namespace LoginMongoAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TwoFactorController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IUserService _userService;
        private readonly string _jwtSecret;

        public TwoFactorController(IUserService userService, IConfiguration configuration)
        {
            _configuration = configuration;
            _userService = userService;
            _jwtSecret = _configuration["Jwt:Key"];
        }

        [HttpGet, Route("GetQRCode")]
        public IActionResult GetQRCode(string email)
        {
            var tfa = new TwoFactorAuth("Test", 6, 30, Algorithm.SHA256, new ImageChartsQrCodeProvider());
            var secret = tfa.CreateSecret(160);

            _userService.SetSecret(email, secret);

            string imgQR = tfa.GetQrCodeImageAsDataUri(email, secret);
            string imgHTML = $"<img src='{imgQR}'>";
            return Ok(imgHTML);
        }


        [HttpGet, Route("GetQRCodeAsImage")]
        public FileContentResult GetQRCodeAsImage(string email)
        {
            var tfa = new TwoFactorAuth("Test", 6, 30, Algorithm.SHA256, new ImageChartsQrCodeProvider());
            var secret = tfa.CreateSecret(160);

            _userService.SetSecret(email, secret);

            string imgQR = tfa.GetQrCodeImageAsDataUri(email, secret);
            imgQR = imgQR.Replace("data:image/png;base64,", "");
            byte[] picture = Convert.FromBase64String(imgQR);
            return File(picture, "image/png");
        }


        [HttpGet, Route("ValidateQRCode")]
        public async Task<bool> ValidateCode(string email, string code)
        {
            try
            {
                string secret = await _userService.GetSecret(email);

                var tfa = new TwoFactorAuth("Test", 6, 30, Algorithm.SHA256);
                return tfa.VerifyCode(secret, code);
            }
            catch (Exception ex)
            {
                return false;
            }
        }


        [HttpPost, Route("VerifyQRCode")]
        public async Task<IActionResult> VerifyQRCode([FromBody] verifyQR request)
        {
            try
            {
                // Verifica el código QR
                bool isValid = await _userService.ValidateCode(request.Email, request.Code);

                if (isValid)
                {
                    // Genera el token JWT
                    var token = GenerateJwtToken(request.Email);

                    // Devuelve el token como parte de la respuesta
                    return Ok(new { Token = token });
                }
                else
                {
                    return BadRequest("El código QR no es válido.");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Se produjo un error al verificar el código QR.");
            }
        }

        // Método para generar un token JWT
        private string GenerateJwtToken(string email)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtSecret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Email, email)
                }),
                Expires = DateTime.UtcNow.AddDays(1), // Caducidad del token (1 día)
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }


}


