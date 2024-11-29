namespace SmartSpender.ViewModel
{
    public class VMTransactionReq
    {
        public DateTime Date { get; set; }
        public string Type { get; set; } = null!;
        public string Name { get; set; } = null!;
        public decimal Amount { get; set; }
        public Guid BudgetPlanId { get; set; }
        public Guid CategoryId { get; set; }
        public string? Tag { get; set; }
        public Guid? ModifiedBy { get; set; }
    }
}
