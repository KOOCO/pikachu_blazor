using Kooco.Pikachu.Items;
using System;
using Volo.Abp.Domain.Entities;

namespace Kooco.Pikachu.Images
{
    public class Image : Entity<Guid>
    {
        /// <summary>
        /// Contain Image's URL on Azure blob
        /// </summary>
        public string ImagePath { get; set; }

        /// <summary>
        /// Shows this image is using at which part of page
        /// </summary>
        public ImageType ImageType { get; set; }

        /// <summary>
        /// Indicated that which componment's ID own this image
        /// </summary>
        public Guid? TargetID { get; set; }

        /// <summary>
        /// Indicated the poition of this image in the componment
        /// </summary>
        public int SortNO { get; set; }
    }
}
