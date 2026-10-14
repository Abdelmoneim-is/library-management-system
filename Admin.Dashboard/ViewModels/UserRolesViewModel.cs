namespace Admin.Dashboard.ViewModels
{
    public class UserRolesViewModel
    {
        public string UserId { get; set; } = default!;
        public string UserName { get; set; } = default!;
        public List<RoleManageViewModel> Roles { get; set; } = default!;
    }
}
