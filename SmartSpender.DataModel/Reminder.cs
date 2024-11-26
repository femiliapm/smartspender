using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SmartSpender.DataModel
{
    [Table("reminders")]
    public partial class Reminder
    {
        [Key]
        [Column("id")]
        public Guid Id { get; set; }
        [Column("user_id")]
        public Guid UserId { get; set; }
        [Column("title")]
        [StringLength(100)]
        [Unicode(false)]
        public string Title { get; set; } = null!;
        [Column("reminder_date", TypeName = "datetime")]
        public DateTime ReminderDate { get; set; }
        [Column("is_completed")]
        public bool? IsCompleted { get; set; }
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
        [InverseProperty("ReminderCreatedByNavigations")]
        public virtual User? CreatedByNavigation { get; set; }
        [ForeignKey("UserId")]
        [InverseProperty("ReminderUsers")]
        public virtual User User { get; set; } = null!;
    }
}
