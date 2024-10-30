using PlannerTracker.DataModel;

namespace PlannerTracker.ViewModel
{
    public class VMUser
    {
        public Guid Id { get; set; }
        public string Username { get; set; } = null!;
        public string PasswordHash { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string? FullName { get; set; }
        public Guid? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public Guid? ModifiedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public Guid? DeletedBy { get; set; }
        public DateTime? DeletedOn { get; set; }
        public bool? IsDelete { get; set; }

        public VMUser() { }

        public VMUser(User user)
        {
            Id = user.Id;
            Username = user.Username;
            Email = user.Email;
            FullName = user.FullName;
            CreatedBy = user.CreatedBy;
            CreatedOn = user.CreatedOn;
            ModifiedBy = user.ModifiedBy;
            ModifiedOn = user.ModifiedOn;
            DeletedBy = user.DeletedBy;
            DeletedOn = user.DeletedOn;
            IsDelete = user.IsDelete;
        }
    }
}
