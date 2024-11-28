namespace SmartSpender.ViewModel
{
    public class VMTagReq
    {
        public string TagName { get; set; } = null!;
        public Guid? ModifiedBy { get; set; }
    }
}
