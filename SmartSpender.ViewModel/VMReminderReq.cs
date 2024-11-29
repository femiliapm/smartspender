namespace SmartSpender.ViewModel
{
    public class VMReminderReq
    {
        public Guid UserId { get; set; }
        public string Title { get; set; } = null!;
        public DateTime ReminderDate { get; set; }
        public Guid? ModifiedBy { get; set; }
    }
}
