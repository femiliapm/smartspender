namespace SmartSpender.ViewModel
{
    public class VMTag
    {
        public Guid Id { get; set; }
        public string TagName { get; set; } = null!;
        public Guid? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public Guid? ModifiedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
    }
}
