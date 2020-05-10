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
        [Column("ItemId")]
        public int ItemId { get; set; }
        public virtual Item Item { get; set; }

        [Column("HouseId")]
        public int HouseId { get; set; }
        public virtual House House { get; set; }

        [Column("StatusId")]
        public int StatusId { get; set; }
        public virtual ItemStatus Status { get; set; }

        [Column("AddedDate")]
        public long AddedDate { get; set; }

        public virtual ICollection<Media> Medias { get; set; }
    }
}