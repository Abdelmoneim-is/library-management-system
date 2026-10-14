namespace Admin.Dashboard.ViewModels
{
    public class RecentReviewItemViewModel
    {
        public string BookTitle { get; set; } = default!;
        public string UserName { get; set; } = default!;
        public int Rating { get; set; }
        public string? Comment { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
