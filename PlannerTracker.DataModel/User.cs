using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace PlannerTracker.DataModel
{
    [Table("users")]
    public partial class User
    {
        public User()
        {
            BudgetPlanCreatedByNavigations = new HashSet<BudgetPlan>();
            BudgetPlanUsers = new HashSet<BudgetPlan>();
            Categories = new HashSet<Category>();
            ExpenseTags = new HashSet<ExpenseTag>();
            Expenses = new HashSet<Expense>();
            IncomeTags = new HashSet<IncomeTag>();
            Incomes = new HashSet<Income>();
            ReminderCreatedByNavigations = new HashSet<Reminder>();
            ReminderUsers = new HashSet<Reminder>();
            Roles = new HashSet<Role>();
            Tags = new HashSet<Tag>();
            UserRoleCreatedByNavigations = new HashSet<UserRole>();
            UserRoleUsers = new HashSet<UserRole>();
        }

        [Key]
        [Column("id")]
        public Guid Id { get; set; }
        [Column("username")]
        [StringLength(50)]
        [Unicode(false)]
        public string Username { get; set; } = null!;
        [Column("password_hash")]
        [StringLength(100)]
        [Unicode(false)]
        public string PasswordHash { get; set; } = null!;
        [Column("email")]
        [StringLength(100)]
        [Unicode(false)]
        public string Email { get; set; } = null!;
        [Column("full_name")]
        [StringLength(100)]
        [Unicode(false)]
        public string? FullName { get; set; }
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

        [InverseProperty("CreatedByNavigation")]
        public virtual ICollection<BudgetPlan> BudgetPlanCreatedByNavigations { get; set; }
        [InverseProperty("User")]
        public virtual ICollection<BudgetPlan> BudgetPlanUsers { get; set; }
        [InverseProperty("CreatedByNavigation")]
        public virtual ICollection<Category> Categories { get; set; }
        [InverseProperty("CreatedByNavigation")]
        public virtual ICollection<ExpenseTag> ExpenseTags { get; set; }
        [InverseProperty("CreatedByNavigation")]
        public virtual ICollection<Expense> Expenses { get; set; }
        [InverseProperty("CreatedByNavigation")]
        public virtual ICollection<IncomeTag> IncomeTags { get; set; }
        [InverseProperty("CreatedByNavigation")]
        public virtual ICollection<Income> Incomes { get; set; }
        [InverseProperty("CreatedByNavigation")]
        public virtual ICollection<Reminder> ReminderCreatedByNavigations { get; set; }
        [InverseProperty("User")]
        public virtual ICollection<Reminder> ReminderUsers { get; set; }
        [InverseProperty("CreatedByNavigation")]
        public virtual ICollection<Role> Roles { get; set; }
        [InverseProperty("CreatedByNavigation")]
        public virtual ICollection<Tag> Tags { get; set; }
        [InverseProperty("CreatedByNavigation")]
        public virtual ICollection<UserRole> UserRoleCreatedByNavigations { get; set; }
        [InverseProperty("User")]
        public virtual ICollection<UserRole> UserRoleUsers { get; set; }
    }
}
