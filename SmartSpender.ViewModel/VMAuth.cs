namespace SmartSpender.ViewModel
{
    public class VMAuth
    {
        public Guid? Id { get; set; }
        public string? Username { get; set; }
        public string Password { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string? FullName { get; set; }
        public string? Token { get; set; }
        public string? Role { get; set; }
    }
}
