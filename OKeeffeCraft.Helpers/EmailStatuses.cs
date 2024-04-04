using OKeeffeCraft.Entities;

namespace OKeeffeCraft.Helpers
{
    public class EmailStatuses
    {
        static readonly List<EmailStatus> _emailStatus;

        public const string Pending = "pending";
        public const string Sent = "sent";
        public const string Delivered = "delivered";
        public const string Failed = "failed";


        static EmailStatuses()
        {
            _emailStatus =
            [
                new EmailStatus { Code = Pending, Name = "Pending" },
                new EmailStatus { Code = Sent, Name = "Sent" },
                new EmailStatus { Code = Delivered, Name = "Delivered" },
                new EmailStatus { Code = Failed, Name = "Failed" },
            ];
        }

        public static IList<EmailStatus> Get()
        {
            return _emailStatus;
        }
    }
}
