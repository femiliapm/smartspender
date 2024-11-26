using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SmartSpender.DataModel
{
    [Table("income_tags")]
    public partial class IncomeTag
    {
        [Key]
        [Column("income_id")]
        public Guid IncomeId { get; set; }
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
        [InverseProperty("IncomeTags")]
        public virtual User? CreatedByNavigation { get; set; }
        [ForeignKey("IncomeId")]
        [InverseProperty("IncomeTags")]
        public virtual Income Income { get; set; } = null!;
        [ForeignKey("TagId")]
        [InverseProperty("IncomeTags")]
        public virtual Tag Tag { get; set; } = null!;
    }
}
