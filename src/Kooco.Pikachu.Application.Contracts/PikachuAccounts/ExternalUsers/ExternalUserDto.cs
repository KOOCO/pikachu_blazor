namespace Kooco.Pikachu.PikachuAccounts.ExternalUsers;

public class ExternalUserDto
{
    public virtual string? Email { get; set; }

    public virtual string? UserName { get; set; }

    public virtual string? Name { get; set; }
    
    public virtual string? ExternalId { get; set; }
}
