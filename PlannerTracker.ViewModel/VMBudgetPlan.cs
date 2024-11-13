using PlannerTracker.DataModel;

namespace PlannerTracker.ViewModel
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
        public Guid? DeletedBy { get; set; }
        public DateTime? DeletedOn { get; set; }
        public bool? IsDelete { get; set; }

        public VMBudgetPlan() { }

        public VMBudgetPlan(BudgetPlan db)
        {
            Id = db.Id;
            UserId = db.UserId;
            PlanName = db.PlanName;
            TotalBudget = db.TotalBudget;
            StartDate = db.StartDate;
            EndDate = db.EndDate;
            CreatedBy = db.CreatedBy;
            CreatedOn = db.CreatedOn;
            ModifiedBy = db.ModifiedBy;
            ModifiedOn = db.ModifiedOn;
            DeletedBy = db.DeletedBy;
            DeletedOn = db.DeletedOn;
            IsDelete = db.IsDelete;
        }
    }
}
