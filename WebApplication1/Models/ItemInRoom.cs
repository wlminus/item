using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace WebApplication1.Models
{
    [Table("ItemInRoom")]
    public class ItemInRoom : BaseEntity
    {
        [Column("Name")]
        [StringLength(255)]
        public string Name { get; set; }

        [Column("Description")]
        [StringLength(1000)]
        public string Description { get; set; }

        [Column("RoomId")]
        public int RoomId { get; set; }
        public virtual Room Room { get; set; }

        [Column("StatusId")]
        public int StatusId { get; set; }
        public virtual ItemStatus Status { get; set; }

        [Column("CategoryId")]
        public int ItemCategoryId { get; set; }
        public virtual ItemCategory ItemCategory { get; set; }

        [Column("AddedDate")]
        public long AddedDate { get; set; }

        public virtual ICollection<Media> Medias { get; set; }
    }
}