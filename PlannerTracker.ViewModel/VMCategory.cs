using PlannerTracker.DataModel;

namespace PlannerTracker.ViewModel
{
    public class VMCategory
    {
        public Guid Id { get; set; }
        public string CategoryName { get; set; } = null!;
        public string Type { get; set; } = null!;
        public string? Description { get; set; }
        public Guid? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public Guid? ModifiedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public Guid? DeletedBy { get; set; }
        public DateTime? DeletedOn { get; set; }
        public bool? IsDelete { get; set; }

        public VMCategory() { }

        public VMCategory(Category category)
        {
            Id = category.Id;
            CategoryName = category.CategoryName;
            Type = category.Type;
            Description = category.Description;
            CreatedBy = category.CreatedBy;
            CreatedOn = category.CreatedOn;
            ModifiedBy = category.ModifiedBy;
            ModifiedOn = category.ModifiedOn;
            DeletedBy = category.DeletedBy;
            DeletedOn = category.DeletedOn;
            IsDelete = category.IsDelete;
        }
    }
}
