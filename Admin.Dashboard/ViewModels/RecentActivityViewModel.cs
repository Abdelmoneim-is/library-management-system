namespace Admin.Dashboard.ViewModels
{
    public class RecentActivityViewModel
    {
        public string Title { get; set; } = default!;
        public string Description { get; set; } = default!;
        public string TimeAgo { get; set; } = default!;
        public string BadgeClass { get; set; } = "bg-primary";
    }
}
