using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace WebApplication1.Models
{
    [Table("ItemInHouse")]
    public class ItemInHouse : BaseEntity
    {
        [Column("Name")]
        [StringLength(255)]
        public string Name { get; set; }

        [Column("Description")]
        [StringLength(1000)]
        public string Description { get; set; }

        [Column("HouseId")]
        public long HouseId { get; set; }
        public virtual House House { get; set; }

        [Column("StatusId")]
        public long StatusId { get; set; }
        public virtual ItemStatus Status { get; set; }

        [Column("CategoryId")]
        public long ItemCategoryId { get; set; }
        public virtual ItemCategory ItemCategory { get; set; }

        [Column("AddedDate")]
        public long AddedDate { get; set; }

        public virtual ICollection<Media> Medias { get; set; }
    }
}