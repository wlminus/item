using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication1.Models
{
    public class HouseViewModel : BaseEntity
    {
        public string Name { get; set; }
        public string Location { get; set; }
        public string District { get; set; }
        public string Province { get; set; }
        public string Description { get; set; }
        public long? Room_Count { get; set; }
        public string Status { get; set; }
        public int? Rent_User_Id { get; set; }

        public virtual ICollection<Media> Medias { get; set; }

        public virtual ICollection<Room> Rooms { get; set; }

        public virtual ICollection<ItemInHouse> Items { get; set; }

        public string RoomName { get; set; }
        public string RoomType { get; set; }

        public HouseViewModel(House house)
        {
            this.Name = house.Name;
            this.Location = house.Location;
            this.District = house.District;
            this.Province = house.Province;
            this.Description = house.Description;
            this.Room_Count = house.Room_Count;
            this.Status = house.Status;
            this.Rent_User_Id = house.Rent_User_Id;
            this.Medias = house.Medias;
            this.Rooms = house.Rooms;
            this.Items = house.Items;
        }

        public HouseViewModel()
        {

        }
    }
}