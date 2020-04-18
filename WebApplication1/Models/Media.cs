using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace WebApplication1.Models
{
    [Table("Media")]
    public class Media : BaseEntity
    {
        [Column("Media_Name")]
        [StringLength(255)]
        public string Media_Name { get; set; }

        [Column("Media_Path")]
        [StringLength(500)]
        public string Media_Path { get; set; }

        [Column("Media_size")]
        public long Media_size { get; set; }

        public int HouseId { get; set; }
        public virtual House House { get; set; }

        public int RoomId { get; set; }
        public virtual Room Room { get; set; }

        public int ItemId { get; set; }
        public virtual Item Item { get; set; }
    }
}