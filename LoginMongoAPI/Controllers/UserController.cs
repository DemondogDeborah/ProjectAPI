using ErrorOr;
using LoginMongoAPI.helpers;
using LoginMongoAPI.Interfaces;
using LoginMongoAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TodoApiMongo.StaticClasses;

namespace LoginMongoAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userservice;
        private readonly IConfiguration _configuration;

        public UserController(IUserService service, IConfiguration configuration)
        {
            _userservice = service;
            _configuration = configuration;
        }


        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] LoginUser login)
        {
            try
            {
                // Buscar el usuario por nombre de usuario en la base de datos
                User user = DBCollections.userCollection.Find(u => u.Username == login.UserName).FirstOrDefault();

                // Verificar si el usuario existe
                if (user == null)
                {
                    return BadRequest(new { message = "Credenciales incorrectas" });
                }

                // Verificar la contraseña del usuario utilizando la función VerifyPassword
                if (!AuthenticationHelper.VerifyPassword(login.Password, user.Password))
                {
                    return BadRequest(new { message = "Credenciales incorrectas" });
                }

                // Generar token JWT para el usuario
                var tokenHandler = new JwtSecurityTokenHandler();
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new Claim[]
                    {
                new Claim("id", user.Id),
                new Claim("rol", user.Rol),
                new Claim("username", user.Username),
                    }),
                    Expires = DateTime.UtcNow.AddHours(3), // Duración del token
                    SigningCredentials = new SigningCredentials(
                        new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_configuration["Jwt:Key"])),
                        SecurityAlgorithms.HmacSha256Signature)
                };
                var token = tokenHandler.CreateToken(tokenDescriptor);
                var tokenString = tokenHandler.WriteToken(token);

                // Devolver el token JWT en la respuesta
                return Ok(new { token = tokenString });
            }
            catch (Exception ex)
            {
                // Manejar la excepción de manera adecuada
                Console.WriteLine($"Error al iniciar sesión: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, "Ocurrió un error interno al procesar la solicitud.");
            }
        }



        // GET: api/getUsers 
        [HttpGet("getUsers")]
        [Authorize]
        public async Task<IActionResult> GetAll()
        {
            List<User> userList = await _userservice.GetAll();

            if (userList == null || userList.Count == 0)
            {
                return NotFound();
            }

            return Ok(userList);
        }


        // GET: api/getUserById/:id
        [HttpGet("getUserById/{id}")]
        public async Task<IActionResult> GetUser(string id)
        {
            ErrorOr<User> getUserResult = await _userservice.GetUser(id);

            if (getUserResult.Value == null)
            {
                return BadRequest();
            }

            return Ok(getUserResult.Value);
        }


        // PUT: api/updateUser
        [HttpPut("updateUser")]
        public async Task<IActionResult> UpdateUser(User user)
        {

            try
            {
                ErrorOr<User> updateUserResult = await _userservice.UpdateUser(user.Id,
                                                                           user.Name,
                                                                           user.Email,
                                                                           user.Username,
                                                                           user.Password,
                                                                           user.Rol);
                return Ok(updateUserResult.Value);
            }
            catch (Exception ex)
            {

                return StatusCode(500, "Error to update user: " + ex.Message);
            }
        }


        // POST: api/postUser
        [HttpPost("postUser")]
        public async Task<IActionResult> User(User user)
        {
            ErrorOr<User> createUserResult = await _userservice.CreateUser(user.Name,
                                                                       user.Email,
                                                                       user.Username,
                                                                       user.Password,
                                                                       user.Rol);

            return Ok(createUserResult.Value);
        }


        // DELETE: api/deleteUser/:id
        [HttpDelete("deleteUser/{id}")]
        public async Task<IActionResult> DeleteUser(string id)
        {

            try
            {
                ErrorOr<string> deleteUserResult = await _userservice.DeleteUser(id);
                return Ok(deleteUserResult.Value);
            }
            catch (Exception ex)
            {

                return StatusCode(500, "Error to delete user: " + ex.Message);
            }
        }
    }
}
