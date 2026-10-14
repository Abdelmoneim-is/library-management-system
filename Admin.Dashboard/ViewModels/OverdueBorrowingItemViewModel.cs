namespace Admin.Dashboard.ViewModels
{
    public class OverdueBorrowingItemViewModel
    {
        public int BorrowingId { get; set; }
        public string BookTitle { get; set; } = default!;
        public string UserName { get; set; } = default!;
        public DateTime DueDate { get; set; }
        public int OverdueDays { get; set; }
        public decimal FineAmount { get; set; }
    }
}
