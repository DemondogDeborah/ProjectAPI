using LoginMongoAPI.Models;
using MongoDB.Driver;
using System.Security.Claims;
using TodoApiMongo.StaticClasses;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Text.RegularExpressions;


namespace LoginMongoAPI.helpers
{
    public static class AuthenticationHelper
    {
        // VALIDATE EMAIL FORMAT
        public static bool ValidateEmail(string email)
        {
            var trimmedEmail = email.Trim();
            if (trimmedEmail.EndsWith("."))
            {
                return false;
            }
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == trimmedEmail;
            }
            catch
            {
                return false;
            }
        }


        // GENERATE PASSWORD HASH
        public static string GeneratePasswordHash(string password) => BCrypt.Net.BCrypt.HashPassword(password) ?? "Error";

        // VERIFY PASSWORD AND PASSWORD HASH
        public static bool VerifyPassword(string password, string passwordHash) => BCrypt.Net.BCrypt.Verify(password, passwordHash);

        public static async Task<dynamic> ValidateToken(ClaimsIdentity identity)
        {
            try
            {
                if (identity.Claims.Count() == 0)
                {
                    return new
                    {
                        success = false,
                        message = "Verificar si es token valido",
                        result = ""
                    };
                }

                var id = identity.Claims.FirstOrDefault(u => u.Type == "id").Value;
                User user = (User)await DBCollections.userCollection.FindAsync(u => u.Id == id);
                return new
                {
                    success = true,
                    message = "Login con exito",
                    result = user
                };
            }
            catch (Exception ex)
            {
                return new
                {
                    success = false,
                    message = ex.Message,
                    result = ""
                };

            }
        }
    }
}
