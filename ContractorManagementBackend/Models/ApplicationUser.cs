public class ApplicationUser
{
    public Guid Id { get; set}

    public string? UserName { get; set; }
    public string? passwordHash{ get; set; }
    
}