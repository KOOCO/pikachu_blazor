using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp;

namespace Kooco.Pikachu.WebsiteManagement;

public class InvalidEnumValueException : BusinessException
{
    public InvalidEnumValueException(string name) : base(PikachuDomainErrorCodes.InvalidEnumValue)
    {
        WithData(nameof(name), name);
    }
}
