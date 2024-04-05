using Microsoft.Extensions.Options;
using OKeeffeCraft.Entities;
using OKeeffeCraft.ExternalServiceProviders.Interfaces;
using OKeeffeCraft.Helpers;
using OKeeffeCraft.Models.Email;
using PostmarkDotNet;
using PostmarkDotNet.Exceptions;
using PostmarkDotNet.Model;
using System.Text.Json;

namespace OKeeffeCraft.ExternalServiceProviders.Services
{
    public class PostmarkEmailServiceProvider : IPostmarkEmailServiceProvider
    {
        private readonly AppSettings _appSettings;

        public PostmarkEmailServiceProvider(IOptions<AppSettings> appSettings)
        {
            _appSettings = appSettings.Value;
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
            var client = new PostmarkClient(_appSettings.PostmarkServerApiKey);

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
                return response;
            }

            if (deliveryEvents.RecordType == "Bounce" || deliveryEvents.RecordType == "Dropped" || deliveryEvents.RecordType == "SpamComplaint" || deliveryEvents.RecordType == "Open" || deliveryEvents.RecordType == "Click" || deliveryEvents.RecordType == "SpamComplaint")
            {
                response.Status = EmailStatuses.Failed;
                response.ExternalRef = deliveryEvents.MessageID;
                response.DeliveryMessage = $"{deliveryEvents.Description}";
                return response;
            }

            return new EmailDeliveryInfo();
        }
    }
}
