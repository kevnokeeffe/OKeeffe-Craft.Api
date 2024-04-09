using Microsoft.AspNetCore.Mvc;
using OKeeffeCraft.Authorization;
using OKeeffeCraft.Core.Interfaces;
using OKeeffeCraft.Entities;
using Swashbuckle.AspNetCore.Annotations;

namespace OKeeffeCraft.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class ContactMessageController : BaseController
    {
        private readonly IContactMessageService _contactMessageService;

        public ContactMessageController(IContactMessageService contactMessageService)
        {
            _contactMessageService = contactMessageService;
        }

        [AllowAnonymous]
        [HttpPost]
        [SwaggerOperation(Summary = "Create a new contact message")]
        [SwaggerResponse(200, "Contact message created successfully")]
        [SwaggerResponse(400, "Error creating contact message")]
        public async Task<IActionResult> Create([FromBody] ContactMessage contactMessage)
        {
            var response = await _contactMessageService.Create(contactMessage);
            return Ok(response);
        }

        [HttpGet]
        [SwaggerOperation(Summary = "Retrieve all contact messages")]
        [SwaggerResponse(200, "Contact messages retrieved successfully")]
        [SwaggerResponse(400, "Error retrieving contact messages")]
        public async Task<IActionResult> Get()
        {
            var response = await _contactMessageService.Get();
            return Ok(response);
        }

        [HttpGet("{id}")]
        [SwaggerOperation(Summary = "Retrieve a contact message by ID")]
        [SwaggerResponse(200, "Contact message retrieved successfully")]
        [SwaggerResponse(400, "Error retrieving contact message")]
        public async Task<IActionResult> GetById(string id)
        {
            var response = await _contactMessageService.GetById(id);
            return Ok(response);
        }

        [HttpPut("{id}")]
        [SwaggerOperation(Summary = "Update a contact message by ID")]
        [SwaggerResponse(200, "Contact message updated successfully")]
        [SwaggerResponse(400, "Error updating contact message")]
        public async Task<IActionResult> Update(string id, [FromBody] ContactMessage contactMessage)
        {
            var response = await _contactMessageService.Update(id, contactMessage);
            return Ok(response);
        }

        [HttpDelete("{id}")]
        [SwaggerOperation(Summary = "Delete a contact message by ID")]
        [SwaggerResponse(200, "Contact message deleted successfully")]
        [SwaggerResponse(400, "Error deleting contact message")]
        public async Task<IActionResult> Delete(string id)
        {
            var response = await _contactMessageService.Delete(id);
            return Ok(response);
        }
    }
}
