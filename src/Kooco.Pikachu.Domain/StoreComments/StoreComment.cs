using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.Identity;

namespace Kooco.Pikachu.StoreComments
{
    public class StoreComment : CreationAuditedAggregateRoot<Guid>
    {
        public string Comment { get; set; }

        [ForeignKey(nameof(CreatorId))]
        public IdentityUser? User { get; set; }

        public StoreComment(
            [NotNull] string comment
            )
        {
            Comment = Check.NotNullOrWhiteSpace(comment, nameof(Comment));
        }
    }
}
