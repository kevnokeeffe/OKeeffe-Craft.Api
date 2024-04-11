using AutoMapper;
using OKeeffeCraft.Entities;
using OKeeffeCraft.Models;
using OKeeffeCraft.Models.Accounts;
using OKeeffeCraft.Models.Email;
using OKeeffeCraft.Models.Logs;

namespace OKeeffeCraft.Helpers
{
    public class AutoMapperProfile : Profile
    {
        // mappings between model and entity objects
        public AutoMapperProfile()
        {
            CreateMap<Account, AccountResponse>();

            CreateMap<Account, AuthenticateResponse>();

            CreateMap<Account, AccountModel>();

            CreateMap<Email, EmailModel>();

            CreateMap<ErrorLog, ErrorLogModel>();

            CreateMap<ActivityLog, ActivityLogModel>();

            CreateMap<ActivityLogModel, ActivityLog>();

            CreateMap<ErrorLogModel, ErrorLog>();

            CreateMap<AccountModel, Account>();

            CreateMap<EmailModel, Email>();

            CreateMap<SnakeHighScore, SnakeHighScoreModel>();

            CreateMap<SnakeHighScoreModel, SnakeHighScore>();

            CreateMap<CreateSnakeHighScoreModel, SnakeHighScore>();

            CreateMap<UpdateSnakeHighScoreModel, SnakeHighScore>();



            CreateMap<RegisterRequest, Account>();

            CreateMap<CreateRequest, Account>();

            CreateMap<UpdateRequest, Account>()
                .ForAllMembers(x => x.Condition(
                    (src, dest, prop) =>
                    {
                        // ignore null & empty string properties
                        if (prop == null) return false;
                        if (prop.GetType() == typeof(string) && string.IsNullOrEmpty((string)prop)) return false;

                        // ignore null role
                        if (x.DestinationMember.Name == "Role" && src.Role == null) return false;

                        return true;
                    }
                ));
        }
    }
}
