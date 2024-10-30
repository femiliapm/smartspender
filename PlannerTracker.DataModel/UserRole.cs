using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace PlannerTracker.DataModel
{
    [Table("user_roles")]
    public partial class UserRole
    {
        [Key]
        [Column("id")]
        public Guid Id { get; set; }
        [Column("user_id")]
        public Guid? UserId { get; set; }
        [Column("role_id")]
        public Guid? RoleId { get; set; }
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
        [InverseProperty("UserRoleCreatedByNavigations")]
        public virtual User? CreatedByNavigation { get; set; }
        [ForeignKey("RoleId")]
        [InverseProperty("UserRoles")]
        public virtual Role? Role { get; set; }
        [ForeignKey("UserId")]
        [InverseProperty("UserRoleUsers")]
        public virtual User? User { get; set; }
    }
}
