using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace PlannerTracker.DataModel
{
    [Table("incomes")]
    public partial class Income
    {
        public Income()
        {
            IncomeTags = new HashSet<IncomeTag>();
        }

        [Key]
        [Column("id")]
        public Guid Id { get; set; }
        [Column("budget_plan_id")]
        public Guid BudgetPlanId { get; set; }
        [Column("category_id")]
        public Guid CategoryId { get; set; }
        [Column("source")]
        [StringLength(100)]
        [Unicode(false)]
        public string Source { get; set; } = null!;
        [Column("amount", TypeName = "decimal(18, 2)")]
        public decimal Amount { get; set; }
        [Column("income_date", TypeName = "datetime")]
        public DateTime IncomeDate { get; set; }
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
        [InverseProperty("Incomes")]
        public virtual BudgetPlan BudgetPlan { get; set; } = null!;
        [ForeignKey("CategoryId")]
        [InverseProperty("Incomes")]
        public virtual Category Category { get; set; } = null!;
        [ForeignKey("CreatedBy")]
        [InverseProperty("Incomes")]
        public virtual User? CreatedByNavigation { get; set; }
        [InverseProperty("Income")]
        public virtual ICollection<IncomeTag> IncomeTags { get; set; }
    }
}
