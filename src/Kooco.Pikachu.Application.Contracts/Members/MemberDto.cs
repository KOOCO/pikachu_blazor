using System;

namespace Kooco.Pikachu.Members;

public class MemberDto
{
    public Guid Id { get; set; }
    public string UserName { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public int Orders { get; set; }
    public int Spent { get; set; }
}