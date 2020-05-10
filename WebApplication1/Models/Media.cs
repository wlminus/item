using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace WebApplication1.Models
{
    [Table("Media")]
    public class Media
    {
        [Column("Id")]
        public Guid Id { get; set; }

        [Column("Media_Name")]
        [StringLength(1000)]
        public string Media_Name { get; set; }

        [Column("Media_Extension")]
        [StringLength(1000)]
        public string Media_Extension { get; set; }

        public virtual ICollection<House> Houses { get; set; }

        public virtual ICollection<Room> Rooms { get; set; }

        public virtual ICollection<ItemInHouse> ItemInHouses { get; set; }

        public virtual ICollection<ItemInRoom> ItemInRooms { get; set; }
    }
}