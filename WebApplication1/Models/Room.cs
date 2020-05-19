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

        [Column("Rent_User_Id")]
        public int Rent_User_Id { get; set; }

        public virtual ICollection<ItemInRoom> Items { get; set; }

        public virtual ICollection<Media> Medias { get; set; }

    }
}