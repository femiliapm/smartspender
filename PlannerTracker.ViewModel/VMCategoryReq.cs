namespace PlannerTracker.ViewModel
{
    public class VMCategoryReq
    {
        public string CategoryName { get; set; } = null!;
        public string Type { get; set; } = null!;
        public string? Description { get; set; }
        public Guid? CreatedBy { get; set; }
    }
}
