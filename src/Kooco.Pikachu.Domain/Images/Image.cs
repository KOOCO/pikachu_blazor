using Kooco.Pikachu.EnumValues;
using Kooco.Pikachu.Items;
using System;
using System.Diagnostics.CodeAnalysis;
using Volo.Abp.Domain.Entities;

namespace Kooco.Pikachu.Images
{
    public class Image : Entity<Guid>
    {
        /// <summary>
        /// The Original name of the Image
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The modified name of the image that is stored on the BLOB
        /// </summary>
        public string BlobImageName { get; set; }

        /// <summary>
        /// Contain Image's URL on Azure blob
        /// </summary>
        public string ImageUrl { get; set; }

        /// <summary>
        /// Shows this image is using at which part of page
        /// </summary>
        public ImageType ImageType { get; set; }

        /// <summary>
        /// Indicated that which componment's ID own this image
        /// </summary>
        public Guid? TargetId { get; set; }

        /// <summary>
        /// Indicated the poition of this image in the componment
        /// </summary>
        public int SortNo { get; set; }

        public string? Link { get; set; }

        public StyleForCarouselImages? CarouselStyle { get; set; }

        public int? ModuleNumber { get; set; }

        public Image()
        {
            
        }

        public Image(
            [NotNull] Guid id,
            string name,
            string blobImageName,
            string imageUrl,
            ImageType imageType,
            Guid? targetId,
            int sortNo
            ) : base(id)
        {
            Name = name;
            BlobImageName = blobImageName;
            ImageUrl = imageUrl;
            ImageType = imageType;
            TargetId = targetId;
            SortNo = sortNo;
        }
    }
}
