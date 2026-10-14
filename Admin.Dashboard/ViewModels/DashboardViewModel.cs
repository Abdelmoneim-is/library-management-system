namespace Admin.Dashboard.ViewModels
{
    public class DashboardViewModel
    {
        public int TotalBooks { get; set; }
        public int AvailableBooks { get; set; }
        public int TotalUsers { get; set; }
        public int TotalCategories { get; set; }
        public int TotalAuthors { get; set; }
        public int ActiveBorrowings { get; set; }
        public int OverdueBorrowings { get; set; }

        // 2. إحصائيات مالية
        public decimal TotalUnpaidFines { get; set; }
        public decimal TotalPaidFines { get; set; }

        // 3. جداول الحركات الأخيرة
        public List<OverdueBorrowingItemViewModel> RecentOverdueList { get; set; } = new();
        public List<RecentReviewItemViewModel> RecentReviewsList { get; set; } = new();

        // 4. بيانات الرسم البياني للأقسام
        public List<CategoryStatViewModel> CategoryStats { get; set; } = new();
    }
}
