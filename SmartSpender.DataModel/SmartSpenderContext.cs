using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace SmartSpender.DataModel
{
    public partial class SmartSpenderContext : DbContext
    {
        public SmartSpenderContext()
        {
        }

        public SmartSpenderContext(DbContextOptions<SmartSpenderContext> options)
            : base(options)
        {
        }

        public virtual DbSet<BudgetPlan> BudgetPlans { get; set; } = null!;
        public virtual DbSet<Category> Categories { get; set; } = null!;
        public virtual DbSet<Expense> Expenses { get; set; } = null!;
        public virtual DbSet<ExpenseTag> ExpenseTags { get; set; } = null!;
        public virtual DbSet<Income> Incomes { get; set; } = null!;
        public virtual DbSet<IncomeTag> IncomeTags { get; set; } = null!;
        public virtual DbSet<Reminder> Reminders { get; set; } = null!;
        public virtual DbSet<Role> Roles { get; set; } = null!;
        public virtual DbSet<Tag> Tags { get; set; } = null!;
        public virtual DbSet<User> Users { get; set; } = null!;
        public virtual DbSet<UserRole> UserRoles { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Server=localhost;Initial Catalog=SmartSpender;user id=sa;Password=P@ssw0rd");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<BudgetPlan>(entity =>
            {
                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsDelete).HasDefaultValueSql("((0))");

                entity.HasOne(d => d.CreatedByNavigation)
                    .WithMany(p => p.BudgetPlanCreatedByNavigations)
                    .HasForeignKey(d => d.CreatedBy)
                    .HasConstraintName("fk_budget_plans_created_by");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.BudgetPlanUsers)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("fk_budget_plans_user");
            });

            modelBuilder.Entity<Category>(entity =>
            {
                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsDelete).HasDefaultValueSql("((0))");

                entity.HasOne(d => d.CreatedByNavigation)
                    .WithMany(p => p.Categories)
                    .HasForeignKey(d => d.CreatedBy)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("fk_categories_created_by");
            });

            modelBuilder.Entity<Expense>(entity =>
            {
                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsDelete).HasDefaultValueSql("((0))");

                entity.HasOne(d => d.BudgetPlan)
                    .WithMany(p => p.Expenses)
                    .HasForeignKey(d => d.BudgetPlanId)
                    .HasConstraintName("fk_expenses_budget_plan");

                entity.HasOne(d => d.Category)
                    .WithMany(p => p.Expenses)
                    .HasForeignKey(d => d.CategoryId)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("fk_expenses_category");

                entity.HasOne(d => d.CreatedByNavigation)
                    .WithMany(p => p.Expenses)
                    .HasForeignKey(d => d.CreatedBy)
                    .HasConstraintName("fk_expenses_created_by");
            });

            modelBuilder.Entity<ExpenseTag>(entity =>
            {
                entity.HasKey(e => new { e.ExpenseId, e.TagId })
                    .HasName("PK__expense___346200404C62648A");

                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsDelete).HasDefaultValueSql("((0))");

                entity.HasOne(d => d.CreatedByNavigation)
                    .WithMany(p => p.ExpenseTags)
                    .HasForeignKey(d => d.CreatedBy)
                    .HasConstraintName("fk_expense_tags_created_by");

                entity.HasOne(d => d.Expense)
                    .WithMany(p => p.ExpenseTags)
                    .HasForeignKey(d => d.ExpenseId)
                    .HasConstraintName("fk_expense_tags_expense");

                entity.HasOne(d => d.Tag)
                    .WithMany(p => p.ExpenseTags)
                    .HasForeignKey(d => d.TagId)
                    .HasConstraintName("fk_expense_tags_tag");
            });

            modelBuilder.Entity<Income>(entity =>
            {
                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsDelete).HasDefaultValueSql("((0))");

                entity.HasOne(d => d.BudgetPlan)
                    .WithMany(p => p.Incomes)
                    .HasForeignKey(d => d.BudgetPlanId)
                    .HasConstraintName("fk_incomes_budget_plan");

                entity.HasOne(d => d.Category)
                    .WithMany(p => p.Incomes)
                    .HasForeignKey(d => d.CategoryId)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("fk_incomes_category");

                entity.HasOne(d => d.CreatedByNavigation)
                    .WithMany(p => p.Incomes)
                    .HasForeignKey(d => d.CreatedBy)
                    .HasConstraintName("fk_incomes_created_by");
            });

            modelBuilder.Entity<IncomeTag>(entity =>
            {
                entity.HasKey(e => new { e.IncomeId, e.TagId })
                    .HasName("PK__income_t__F9EE1D8DF73D1BE2");

                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsDelete).HasDefaultValueSql("((0))");

                entity.HasOne(d => d.CreatedByNavigation)
                    .WithMany(p => p.IncomeTags)
                    .HasForeignKey(d => d.CreatedBy)
                    .HasConstraintName("fk_income_tags_created_by");

                entity.HasOne(d => d.Income)
                    .WithMany(p => p.IncomeTags)
                    .HasForeignKey(d => d.IncomeId)
                    .HasConstraintName("fk_income_tags_income");

                entity.HasOne(d => d.Tag)
                    .WithMany(p => p.IncomeTags)
                    .HasForeignKey(d => d.TagId)
                    .HasConstraintName("fk_income_tags_tag");
            });

            modelBuilder.Entity<Reminder>(entity =>
            {
                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsCompleted).HasDefaultValueSql("((0))");

                entity.Property(e => e.IsDelete).HasDefaultValueSql("((0))");

                entity.HasOne(d => d.CreatedByNavigation)
                    .WithMany(p => p.ReminderCreatedByNavigations)
                    .HasForeignKey(d => d.CreatedBy)
                    .HasConstraintName("fk_reminders_created_by");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.ReminderUsers)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("fk_reminders_user");
            });

            modelBuilder.Entity<Role>(entity =>
            {
                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsDelete).HasDefaultValueSql("((0))");

                entity.HasOne(d => d.CreatedByNavigation)
                    .WithMany(p => p.Roles)
                    .HasForeignKey(d => d.CreatedBy)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("FK__roles__created_b__2645B050");
            });

            modelBuilder.Entity<Tag>(entity =>
            {
                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsDelete).HasDefaultValueSql("((0))");

                entity.HasOne(d => d.CreatedByNavigation)
                    .WithMany(p => p.Tags)
                    .HasForeignKey(d => d.CreatedBy)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("fk_tags_created_by");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsDelete).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<UserRole>(entity =>
            {
                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.HasOne(d => d.CreatedByNavigation)
                    .WithMany(p => p.UserRoleCreatedByNavigations)
                    .HasForeignKey(d => d.CreatedBy)
                    .HasConstraintName("FK__user_role__creat__339FAB6E");

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.UserRoles)
                    .HasForeignKey(d => d.RoleId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK__user_role__role___32AB8735");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.UserRoleUsers)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK__user_role__user___31B762FC");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
