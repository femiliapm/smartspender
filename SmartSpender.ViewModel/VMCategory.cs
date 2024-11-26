namespace SmartSpender.ViewModel
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
        public bool? IsDelete { get; set; }
    }
}
