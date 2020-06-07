using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace WebApplication1.Models
{
    [Table("House")]
    public class House : BaseEntity
    {
        [Column("Name")]
        [StringLength(500)]
        public string Name { get; set; }

        [Column("Location")]
        [StringLength(500)]
        public string Location { get; set; }

        [Column("District")]
        [StringLength(500)]
        public string District { get; set; }

        [Column("Province")]
        [StringLength(500)]
        public string Province { get; set; }

        [Column("Description")]
        [StringLength(1000)]
        public string Description { get; set; }

        [Column("Room_Count")]
        public long? Room_Count { get; set; }

        [Column("Status")]
        [StringLength(100)]
        public string Status { get; set; }

        public virtual ICollection<Media> Medias { get; set; }

        public virtual ICollection<Room> Rooms { get; set; }

        public virtual ICollection<ItemInHouse> Items { get; set; }
    }
}