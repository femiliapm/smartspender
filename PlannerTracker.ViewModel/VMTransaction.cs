namespace PlannerTracker.ViewModel
{
    public class VMTransaction
    {
        public Guid Id { get; set; }
        public DateTime Date { get; set; }
        public string Type { get; set; } = null!;
        public string Name { get; set; } = null!;
        public decimal Amount { get; set; }
        public string BudgetPlan { get; set; } = null!;
        public string Category { get; set; } = null!;
        public List<string>? Tag { get; set; }
    }
}
