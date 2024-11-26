using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SmartSpender.DataModel
{
    [Table("tags")]
    public partial class Tag
    {
        public Tag()
        {
            ExpenseTags = new HashSet<ExpenseTag>();
            IncomeTags = new HashSet<IncomeTag>();
        }

        [Key]
        [Column("id")]
        public Guid Id { get; set; }
        [Column("tag_name")]
        [StringLength(50)]
        [Unicode(false)]
        public string TagName { get; set; } = null!;
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
        [InverseProperty("Tags")]
        public virtual User? CreatedByNavigation { get; set; }
        [InverseProperty("Tag")]
        public virtual ICollection<ExpenseTag> ExpenseTags { get; set; }
        [InverseProperty("Tag")]
        public virtual ICollection<IncomeTag> IncomeTags { get; set; }
    }
}
