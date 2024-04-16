using ErrorOr;
using LoginMongoAPI.helpers;
using LoginMongoAPI.Interfaces;
using LoginMongoAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
namespace LoginMongoAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientController : ControllerBase
    {
        private readonly IClientService _clientservice;

        public ClientController(IClientService service)
        {
            _clientservice = service;
        }


        // GET: api/getClients 
        [HttpGet("getClients")]
        public async Task<IActionResult> GetAll()
        {
            List<Client> clientList = await _clientservice.GetAll();

            if (clientList == null || clientList.Count == 0)
            {
                return NotFound();
            }

            return Ok(clientList);
        }


        // GET: api/getClientById/:id
        [HttpGet("getClientById/{id}")]
        public async Task<IActionResult> GetClient(string id)
        {
            ErrorOr<Client> getClientResult = await _clientservice.GetClient(id);

            if (getClientResult.Value == null)
            {
                return BadRequest();
            }

            return Ok(getClientResult.Value);
        }


        // PUT: api/updateClient
        [HttpPut("updateClient")]
        public async Task<IActionResult> UpdateClient(Client client)
        {

            try
            {
                ErrorOr<Client> updateClientResult = await _clientservice.UpdateClient(client.Id,
                                                                           client.ClientName,
                                                                           client.Email);
                return Ok(updateClientResult.Value);
            }
            catch (Exception ex)
            {

                return StatusCode(500, "Error to update client: " + ex.Message);
            }
        }


        // POST: api/postClient
        [HttpPost("postClient")]
        public async Task<IActionResult> Client(Client client)
        {
            ErrorOr<Client> createClientResult = await _clientservice.CreateClient(client.ClientName,
                                                                       client.Email);

            return Ok(createClientResult.Value);
        }


        // DELETE: api/deleteClient/:id
        [HttpDelete("deleteClient/{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteClient(string id)
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            var rToken = await AuthenticationHelper.ValidateToken(identity);

            if (!rToken.success) return rToken;
            User user = rToken.result;
            if (user.Rol != "administrador")
            {
                return StatusCode(404, "No tienes derechos");
            }

            try
            {
                ErrorOr<string> deleteClientResult = await _clientservice.DeleteClient(id);
                return Ok(deleteClientResult.Value);
            }
            catch (Exception ex)
            {

                return StatusCode(500, "Error to delete user: " + ex.Message);
            }
        }


    }
}
