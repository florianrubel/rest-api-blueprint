namespace rest_api_blueprint.Models.Identity.User
{
    public class AdminUserInitializerOptions
    {
        public string Email { get; set; }

        public char Gender { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string BirthdayFromIso { get; set; }
    }
}
