using Kooco.Pikachu.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace Kooco.Pikachu.InboxManagement
{
    public class EfCoreNotificationRepository : EfCoreRepository<PikachuDbContext, Notification, Guid>, INotificationRepository
    {
        public EfCoreNotificationRepository(IDbContextProvider<PikachuDbContext> dbContextProvider) : base(dbContextProvider)
        {
        }
    }
}
