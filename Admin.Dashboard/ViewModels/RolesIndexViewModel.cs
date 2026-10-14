namespace Admin.Dashboard.ViewModels
{
    public class RolesIndexViewModel
    {
        public IEnumerable<RoleViewModel> Roles { get; set; } = new List<RoleViewModel>();
        public RoleViewModel NewRole { get; set; } = new RoleViewModel(); 
    }
}
