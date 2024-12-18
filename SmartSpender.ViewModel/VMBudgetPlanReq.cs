﻿namespace SmartSpender.ViewModel
{
    public class VMBudgetPlanReq
    {
        public Guid UserId { get; set; }
        public string PlanName { get; set; } = null!;
        public decimal TotalBudget { get; set; }
        public string? TotalBudgetStr { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public Guid? ModifiedBy { get; set; }
    }
}
