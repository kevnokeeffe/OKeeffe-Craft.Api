using Microsoft.AspNetCore.Mvc;
using OKeeffeCraft.Authorization;
using OKeeffeCraft.Core.Interfaces;
using OKeeffeCraft.Models;
using OKeeffeCraft.Models.Email;
using Swashbuckle.AspNetCore.Annotations;
using System.Text;

namespace OKeeffeCraft.Api.Controllers
{

    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class EmailsController : BaseController
    {
        private readonly IEmailService _emailService;

        public EmailsController(IEmailService emailService)
        {
            _emailService = emailService;
        }

        [AllowAnonymous]
        [HttpPost("delivery-event/{token}")]
        [SwaggerOperation(Summary = "URL to be called by Email Provider to notify of email delivery")]
        [SwaggerResponse(200, "Notification received and processed")]
        [SwaggerResponse(400, "Error message (generally a validation error)")]
        public async Task<IActionResult> EmailDelivery(string token)
        {
            string body;
            //parse request body for email delivery message
            using (StreamReader reader = new StreamReader(Request.Body, Encoding.UTF8))
            {
                body = await reader.ReadToEndAsync();
            }

            if (!string.IsNullOrEmpty(body))
                await _emailService.ProcessCallback(body, token);

            return Ok();
        }

        [HttpGet("emails")]
        [SwaggerOperation(Summary = "Get all emails")]
        [SwaggerResponse(200, "List if all emails", typeof(ServiceResponse<IEnumerable<EmailModel>>))]
        [SwaggerResponse(400, "Error message (generally a validation error", typeof(ServiceResponse<string>))]
        public async Task<IActionResult> ListEmails()
        {
            var response = await _emailService.getEmails();
            return Ok(response);
        }

        [HttpGet("email{id}")]
        [SwaggerOperation(Summary = "Get email by id")]
        [SwaggerResponse(200, "Email by id", typeof(ServiceResponse<EmailModel>))]
        [SwaggerResponse(400, "Error message (generally a validation error", typeof(ServiceResponse<string>))]
        public async Task<IActionResult> RetrieveEmail(string id)
        {
            var response = await _emailService.getEmailById(id);
            return Ok(response);
        }

    }
}
