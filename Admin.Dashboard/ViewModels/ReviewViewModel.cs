namespace Admin.Dashboard.ViewModels
{
    public class ReviewViewModel
    {
        public int Id { get; set; }
        public int BookId { get; set; }
        public string BookTitle { get; set; } = default!;
        public string UserId { get; set; } = default!;
        public int Rating { get; set; }
        public string? Comment { get; set; } = default!;
        public DateTime CreatedAt { get; set; }
    }
}
