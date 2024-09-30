using Kooco.Pikachu.PikachuAccounts;
using System;

namespace Kooco.Pikachu.Members;

public class CreateMemberDto : PikachuRegisterInputDto
{
    public override DateTime? Birthday { get; set; }
}
