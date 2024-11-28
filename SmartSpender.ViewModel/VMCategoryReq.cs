namespace SmartSpender.ViewModel
{
    public class VMCategoryReq
    {
        public string CategoryName { get; set; } = null!;
        public string Type { get; set; } = null!;
        public string? Description { get; set; }
        public Guid? ModifiedBy { get; set; }
    }
}
