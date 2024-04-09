using AutoMapper;
using OKeeffeCraft.Core.Interfaces;
using OKeeffeCraft.Entities;
using OKeeffeCraft.Models;

namespace OKeeffeCraft.Core.Services
{
    public class ContactMessageService : IContactMessageService
    {
        private readonly IMongoDBService _context;
        private readonly IMapper _mapper;
        private readonly ILogService _logService;

        public ContactMessageService(
            IMongoDBService context,
            IMapper mapper,
            ILogService logService)
        {
            _context = context;
            _mapper = mapper;
            _logService = logService;
        }

        public async Task<ServiceResponse<string>> Create(ContactMessage contactMessage)
        {
            try
            {
                contactMessage.CreatedDate = DateTime.UtcNow;
                await _context.CreateContactMessageAsync(contactMessage);
                return new ServiceResponse<string> { Data = null, Message = "Message sent sucessfully", Success = true };
            }
            catch (Exception ex)
            {
                await _logService.ErrorLog(ex.Message, ex.StackTrace, null, null);
                return new ServiceResponse<string> { Data = null, Message = "Error sending message", Success = false };
            }
        }

        public async Task<ServiceResponse<IEnumerable<ContactMessage>>> Get()
        {
            try
            {
                var messages = await _context.GetContactMessagesAsync();
                var mappedMessages = _mapper.Map<IEnumerable<ContactMessage>>(messages);
                return new ServiceResponse<IEnumerable<ContactMessage>> { Data = mappedMessages, Message = "Messages retrieved successfully", Success = true };
            }
            catch (Exception ex)
            {
                await _logService.ErrorLog(ex.Message, ex.StackTrace, null, null);
                return new ServiceResponse<IEnumerable<ContactMessage>> { Data = null, Message = "Error retrieving messages", Success = false };
            }

        }

        public async Task<ServiceResponse<ContactMessage>> GetById(string id)
        {
            try
            {
                var message = await _context.GetContactMessageAsync(id);
                var mappedMessage = _mapper.Map<ContactMessage>(message);
                return new ServiceResponse<ContactMessage> { Data = mappedMessage, Message = "Message retrieved successfully", Success = true };
            }
            catch (Exception ex)
            {
                await _logService.ErrorLog(ex.Message, ex.StackTrace, null, null);
                return new ServiceResponse<ContactMessage> { Data = null, Message = "Error retrieving message", Success = false };
            }
        }

        public async Task<ServiceResponse<string>> Update(string id, ContactMessage contactMessage)
        {
            try
            {
                contactMessage.UpdatedDate = DateTime.UtcNow;
                await _context.UpdateContactMessageAsync(id, contactMessage);
                return new ServiceResponse<string> { Data = null, Message = "Message updated successfully", Success = true };
            }
            catch (Exception ex)
            {
                await _logService.ErrorLog(ex.Message, ex.StackTrace, null, null);
                return new ServiceResponse<string> { Data = null, Message = "Error updating message", Success = false };
            }
        }

        public async Task<ServiceResponse<string>> Delete(string id)
        {
            try
            {
                await _context.RemoveContactMessageAsync(id);
                return new ServiceResponse<string> { Data = null, Message = "Message deleted successfully", Success = true };
            }
            catch (Exception ex)
            {
                await _logService.ErrorLog(ex.Message, ex.StackTrace, null, null);
                return new ServiceResponse<string> { Data = null, Message = "Error deleting message", Success = false };
            }
        }
    }
}
