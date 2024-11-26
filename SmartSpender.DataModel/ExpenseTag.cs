using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SmartSpender.DataModel
{
    [Table("expense_tags")]
    public partial class ExpenseTag
    {
        [Key]
        [Column("expense_id")]
        public Guid ExpenseId { get; set; }
        [Key]
        [Column("tag_id")]
        public Guid TagId { get; set; }
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
        [InverseProperty("ExpenseTags")]
        public virtual User? CreatedByNavigation { get; set; }
        [ForeignKey("ExpenseId")]
        [InverseProperty("ExpenseTags")]
        public virtual Expense Expense { get; set; } = null!;
        [ForeignKey("TagId")]
        [InverseProperty("ExpenseTags")]
        public virtual Tag Tag { get; set; } = null!;
    }
}
