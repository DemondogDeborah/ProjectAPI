using LoginMongoAPI.Interfaces;
using LoginMongoAPI.Models;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
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


    }
}
