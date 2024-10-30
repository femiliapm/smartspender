using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace PlannerTracker.DataModel
{
    [Table("budget_plans")]
    public partial class BudgetPlan
    {
        public BudgetPlan()
        {
            Expenses = new HashSet<Expense>();
            Incomes = new HashSet<Income>();
        }

        [Key]
        [Column("id")]
        public Guid Id { get; set; }
        [Column("user_id")]
        public Guid UserId { get; set; }
        [Column("plan_name")]
        [StringLength(100)]
        [Unicode(false)]
        public string PlanName { get; set; } = null!;
        [Column("total_budget", TypeName = "decimal(18, 2)")]
        public decimal TotalBudget { get; set; }
        [Column("start_date", TypeName = "datetime")]
        public DateTime? StartDate { get; set; }
        [Column("end_date", TypeName = "datetime")]
        public DateTime? EndDate { get; set; }
        [Column("created_by")]
        public Guid? CreatedBy { get; set; }
        [Column("created_on", TypeName = "datetime")]
        public DateTime? CreatedOn { get; set; }
        [Column("modified_by")]
        public Guid? ModifiedBy { get; set; }
        [Column("modified_on", TypeName = "datetime")]
        public DateTime? ModifiedOn { get; set; }
        [Column("deleted_by")]
        public Guid? DeletedBy { get; set; }
        [Column("deleted_on", TypeName = "datetime")]
        public DateTime? DeletedOn { get; set; }
        [Column("is_delete")]
        public bool? IsDelete { get; set; }

        [ForeignKey("CreatedBy")]
        [InverseProperty("BudgetPlanCreatedByNavigations")]
        public virtual User? CreatedByNavigation { get; set; }
        [ForeignKey("UserId")]
        [InverseProperty("BudgetPlanUsers")]
        public virtual User User { get; set; } = null!;
        [InverseProperty("BudgetPlan")]
        public virtual ICollection<Expense> Expenses { get; set; }
        [InverseProperty("BudgetPlan")]
        public virtual ICollection<Income> Incomes { get; set; }
    }
}
