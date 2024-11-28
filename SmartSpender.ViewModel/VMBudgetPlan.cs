namespace SmartSpender.ViewModel
{
    public class VMBudgetPlan
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string PlanName { get; set; } = null!;
        public decimal TotalBudget { get; set; }
        public decimal TotalIncome { get; set; } = 0;
        public decimal TotalExpense { get; set; } = 0;
        public int? Progress { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public Guid? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public Guid? ModifiedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
    }
}
