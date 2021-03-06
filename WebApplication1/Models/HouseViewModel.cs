﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebApplication1.Models
{
    public class HouseViewModel
    {
        public long Id { get; set; }
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

        [Required]
        public string ItemName { get; set; }
        public string ItemDescription { get; set; }
        public long ItemStatusId { get; set; }
        public long ItemCategoryId { get; set; }

        public HouseViewModel(House house)
        {
            this.Id = house.Id;
            this.Name = house.Name;
            this.Location = house.Location;
            this.District = house.District;
            this.Province = house.Province;
            this.Description = house.Description;
            this.Room_Count = house.Room_Count;
            this.Status = house.Status;
            this.Medias = house.Medias;
            this.Rooms = house.Rooms;
            this.Items = house.Items;
        }

        public HouseViewModel()
        {

        }
    }
}