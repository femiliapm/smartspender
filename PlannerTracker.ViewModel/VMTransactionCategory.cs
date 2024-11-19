namespace PlannerTracker.ViewModel
{
    public class VMTransactionCategory
    {
        public Guid Id { get; set; }
        public decimal Amount { get; set; }
        public decimal TotalAmount { get; set; }
        public string Category { get; set; } = null!;
        public string Type { get; set; } = null!;
    }
}
