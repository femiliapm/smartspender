using PlannerTracker.DataModel;

namespace PlannerTracker.ViewModel
{
    public class VMTag
    {
        public Guid Id { get; set; }
        public string TagName { get; set; } = null!;
        public Guid? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public Guid? ModifiedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public Guid? DeletedBy { get; set; }
        public DateTime? DeletedOn { get; set; }
        public bool? IsDelete { get; set; }

        public VMTag() { }

        public VMTag(Tag db)
        {
            Id = db.Id;
            TagName = db.TagName;
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
