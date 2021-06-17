namespace rest_api_blueprint.Models.Email
{
    public class SmtpOptions
    {
        public string Host { get; set; }

        public int Port { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }

        public string SenderName { get; set; }

        public string SenderEmail { get; set; }
    }
}
