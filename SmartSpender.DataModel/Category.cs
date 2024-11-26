using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SmartSpender.DataModel
{
    [Table("categories")]
    public partial class Category
    {
        public Category()
        {
            Expenses = new HashSet<Expense>();
            Incomes = new HashSet<Income>();
        }

        [Key]
        [Column("id")]
        public Guid Id { get; set; }
        [Column("category_name")]
        [StringLength(50)]
        [Unicode(false)]
        public string CategoryName { get; set; } = null!;
        [Column("type")]
        [StringLength(20)]
        [Unicode(false)]
        public string Type { get; set; } = null!;
        [Column("description")]
        [StringLength(255)]
        [Unicode(false)]
        public string? Description { get; set; }
        [Column("created_by")]
        public Guid? CreatedBy { get; set; }
        [Column("created_on", TypeName = "datetime")]
        public DateTime? CreatedOn { get; set; }
        [Column("modified_by")]
        public Guid? ModifiedBy { get; set; }
        [Column("modified_on", TypeName = "datetime")]
        public DateTime? ModifiedOn { get; set; }
        [Column("is_delete")]
        public bool? IsDelete { get; set; }

        [ForeignKey("CreatedBy")]
        [InverseProperty("Categories")]
        public virtual User? CreatedByNavigation { get; set; }
        [InverseProperty("Category")]
        public virtual ICollection<Expense> Expenses { get; set; }
        [InverseProperty("Category")]
        public virtual ICollection<Income> Incomes { get; set; }
    }
}
