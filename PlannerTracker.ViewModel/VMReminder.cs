using PlannerTracker.DataModel;

namespace PlannerTracker.ViewModel
{
    public class VMReminder
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string Title { get; set; } = null!;
        public DateTime ReminderDate { get; set; }
        public bool? IsCompleted { get; set; }
        public Guid? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public Guid? ModifiedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public Guid? DeletedBy { get; set; }
        public DateTime? DeletedOn { get; set; }
        public bool? IsDelete { get; set; }

        public VMReminder() { }

        public VMReminder(Reminder db)
        {
            Id = db.Id;
            UserId = db.UserId;
            Title = db.Title;
            ReminderDate = db.ReminderDate;
            IsCompleted = db.IsCompleted;
            CreatedBy = db.CreatedBy;
            CreatedOn = db.CreatedOn;
            ModifiedBy = db.ModifiedBy;
            ModifiedOn = db.ModifiedOn;
            DeletedBy = db.DeletedBy;
            DeletedOn = db.DeletedOn;
            IsDelete = db.IsDelete;
        }
    }
}
