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

        [Column("Manage_Id")]
        public int Manage_Id { get; set; }

        [Column("Rent_User_Id")]
        public int Rent_User_Id { get; set; }

        public virtual ICollection<Media> House_Media { get; set; }

        public virtual ICollection<Room> Rooms { get; set; }

        public virtual ICollection<Item> Items { get; set; }

        public int LocationCategoryId { get; set; }
        public virtual LocationCategory LocationCategory { get; set; }
    }
}