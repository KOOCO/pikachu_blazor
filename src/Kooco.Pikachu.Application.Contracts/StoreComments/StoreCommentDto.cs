using System;
using Volo.Abp.Identity;

namespace Kooco.Pikachu.StoreComments
{
    public class StoreCommentDto
    {
        public Guid Id { get; set; }
        public string Comment { get; set; }
        public DateTime CreationTime { get; set; }
        public Guid? CreatorId { get; set; }
        public IdentityUserDto User { get; set; }
    }
}
