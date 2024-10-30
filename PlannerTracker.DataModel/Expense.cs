using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace PlannerTracker.DataModel
{
    [Table("expenses")]
    public partial class Expense
    {
        public Expense()
        {
            ExpenseTags = new HashSet<ExpenseTag>();
        }

        [Key]
        [Column("id")]
        public Guid Id { get; set; }
        [Column("budget_plan_id")]
        public Guid BudgetPlanId { get; set; }
        [Column("category_id")]
        public Guid CategoryId { get; set; }
        [Column("expense_name")]
        [StringLength(100)]
        [Unicode(false)]
        public string ExpenseName { get; set; } = null!;
        [Column("amount", TypeName = "decimal(18, 2)")]
        public decimal Amount { get; set; }
        [Column("expense_date", TypeName = "datetime")]
        public DateTime ExpenseDate { get; set; }
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

        [ForeignKey("BudgetPlanId")]
        [InverseProperty("Expenses")]
        public virtual BudgetPlan BudgetPlan { get; set; } = null!;
        [ForeignKey("CategoryId")]
        [InverseProperty("Expenses")]
        public virtual Category Category { get; set; } = null!;
        [ForeignKey("CreatedBy")]
        [InverseProperty("Expenses")]
        public virtual User? CreatedByNavigation { get; set; }
        [InverseProperty("Expense")]
        public virtual ICollection<ExpenseTag> ExpenseTags { get; set; }
    }
}
