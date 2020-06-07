using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace WebApplication1.Models
{
    [Table("Room")]
    public class Room : BaseEntity
    {
        [Column("Name")]
        [StringLength(500)]
        public string Name { get; set; }

        [Column("Type")]
        [StringLength(500)]
        public string Type { get; set; }

        [Column("RentUser")]
        public string RentUser { get; set; }

        [Column("Verified")]
        public bool Verified { get; set; }

        public long HouseId { get; set; }
        public House House { get; set; }

        public virtual ICollection<ItemInRoom> Items { get; set; }

        public virtual ICollection<Media> Medias { get; set; }

    }
}