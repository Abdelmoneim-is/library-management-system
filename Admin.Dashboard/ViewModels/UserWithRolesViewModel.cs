namespace Admin.Dashboard.ViewModels
{
    public class UserWithRolesViewModel
    {
        public string Id { get; set; } = default!;
        public string UserName { get; set; } = default!;
        public string Email { get; set; } = default!;
        public string PhoneNumber { get; set; } = default!;
        public List<string> Roles { get; set; } = default!;
    }
}
