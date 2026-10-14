namespace Admin.Dashboard.ViewModels
{
    public class LoanViewModel
    {
        public int Id { get; set; }
        public string BookTitle { get; set; } = default!;
        public string UserId { get; set; } = default!;
        public DateTime BorrowDate { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime? ReturnDate { get; set; }
        public decimal FineAmount { get; set; }
        public bool IsFinePaid { get; set; }
        public bool IsReturned { get; set; }
        public bool IsOverDue { get; set; }
    }
}
