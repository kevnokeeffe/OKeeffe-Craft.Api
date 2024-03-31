namespace OKeeffeCraft.Helpers
{
    public class AppSettings
    {
        public required string Secret { get; set; }
        public required string ClientUrl { get; set; }
        public required string OpenAIKey { get; set; }
        public required string AMLAssistantId { get; set; }
        public required string MongoDBName { get; set; }


        // refresh token time to live (in days), inactive tokens are
        // automatically deleted from the database after this time
        public int RefreshTokenTTL { get; set; }
        public required string EmailFrom { get; set; }
        public required string SmtpHost { get; set; }
        public int SmtpPort { get; set; }
        public required string SmtpUser { get; set; }
        public required string SmtpPass { get; set; }
        public required MailSettings MailSettings { get; set; }
    }
}
