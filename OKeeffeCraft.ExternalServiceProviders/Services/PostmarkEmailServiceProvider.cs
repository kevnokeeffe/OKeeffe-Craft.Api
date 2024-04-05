using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using OKeeffeCraft.Core.Interfaces;
using OKeeffeCraft.Entities;
using OKeeffeCraft.ExternalServiceProviders.Interfaces;
using OKeeffeCraft.Helpers;
using OKeeffeCraft.Models.Email;
using PostmarkDotNet;
using PostmarkDotNet.Exceptions;
using System.Text.Json;

namespace OKeeffeCraft.ExternalServiceProviders.Services
{
    public class PostmarkEmailServiceProvider : IPostmarkEmailServiceProvider
    {
        private readonly AppSettings _appSettings;
        private readonly IConfiguration _configuration;
        private readonly ILogService _logService;

        public PostmarkEmailServiceProvider(IOptions<AppSettings> appSettings, IConfiguration configuration, ILogService logService )
        {
            _appSettings = appSettings.Value;
            _configuration = configuration;
            _logService = logService;
        }

        public async Task<string> SendMail(NewEmailModel message)
        {
            var resultStd = await SendStandardMail(message);
            return resultStd;
        }

        private async Task<string> SendStandardMail(NewEmailModel message)
        {
            var email = new PostmarkMessage()
            {
                To = message.ToEmail,
                From = _appSettings.SupportEmailAddress,
                TrackOpens = false,
                Subject = message.Subject,
                TextBody = message.Body,
                MessageStream = "outbound",
                Tag = message.ToName,

            };
            var client = new PostmarkClient(_configuration["PostmarkServerApiKey"]);

            try
            {
                PostmarkResponse sendResult = await client.SendMessageAsync(email);
                if (sendResult.Status != PostmarkStatus.Success)
                    throw new Exception("POSTMARK SEND MAIL ERROR: " + sendResult.Message);

                return sendResult.MessageID.ToString();
            }
            catch (PostmarkValidationException ex)
            {
                throw new Exception("POSTMARK SEND MAIL ERROR: " + ex.Message);
            }
        }

        public EmailDeliveryInfo ProcessCallback(string body)
        {
            var response = new EmailDeliveryInfo();

            var deliveryEvents = JsonSerializer.Deserialize<DeliveryEvent>(body);

            if (deliveryEvents!.RecordType == "Delivery")
            {
                response.Status = EmailStatuses.Delivered;
                response.ExternalRef = deliveryEvents.MessageID;
                _logService.ActivityLog(deliveryEvents.MessageID, response.Status, response.ExternalRef);
                return response;
            }

            if(deliveryEvents.RecordType == "Open")
            {
                response.Status = EmailStatuses.Opened;
                response.ExternalRef = deliveryEvents.MessageID;
                _logService.ActivityLog(deliveryEvents.MessageID, response.Status, response.ExternalRef);
                return response;
            }

            if(deliveryEvents.RecordType == "Click")
            {
                response.Status = EmailStatuses.Clicked;
                response.ExternalRef = deliveryEvents.MessageID;
                _logService.ActivityLog(deliveryEvents.MessageID, response.Status, response.ExternalRef);
                return response;
            }

            if (deliveryEvents.RecordType == "Bounce" || deliveryEvents.RecordType == "Dropped" || deliveryEvents.RecordType == "SpamComplaint"  || deliveryEvents.RecordType == "SpamComplaint")
            {
                response.Status = EmailStatuses.Failed;
                response.ExternalRef = deliveryEvents.MessageID;
                response.DeliveryMessage = $"{deliveryEvents.Description}";
                _logService.ErrorLog(deliveryEvents.MessageID, response.Status, response.ExternalRef, response.DeliveryMessage);
                return response;
            }

            return new EmailDeliveryInfo();
        }
    }
}
