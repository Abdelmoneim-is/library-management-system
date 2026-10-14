namespace Admin.Dashboard.ViewModels
{
    public class BookIndexViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string ISBN { get; set; } = string.Empty;
        public string AuthorName { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
        public int AvailableCopies { get; set; }
        public int TotalCopies { get; set; }

        public int AuthorId { get; set; }
        public int CategoryId { get; set; }
    }
}
