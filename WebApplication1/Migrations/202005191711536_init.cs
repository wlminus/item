namespace WebApplication1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class init : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.House",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Name = c.String(maxLength: 500),
                        Location = c.String(maxLength: 500),
                        District = c.String(maxLength: 500),
                        Province = c.String(maxLength: 500),
                        Description = c.String(maxLength: 1000),
                        Room_Count = c.Long(),
                        Status = c.String(maxLength: 100),
                        Rent_User_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ItemInHouse",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Name = c.String(maxLength: 255),
                        Description = c.String(maxLength: 1000),
                        HouseId = c.Long(nullable: false),
                        StatusId = c.Long(nullable: false),
                        CategoryId = c.Long(nullable: false),
                        AddedDate = c.Long(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.House", t => t.HouseId, cascadeDelete: true)
                .ForeignKey("dbo.ItemCategory", t => t.CategoryId, cascadeDelete: true)
                .ForeignKey("dbo.ItemStatus", t => t.StatusId, cascadeDelete: true)
                .Index(t => t.HouseId)
                .Index(t => t.StatusId)
                .Index(t => t.CategoryId);
            
            CreateTable(
                "dbo.ItemCategory",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Name = c.String(maxLength: 500),
                        Description = c.String(maxLength: 1000),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Media",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Media_Name = c.String(maxLength: 1000),
                        Media_Extension = c.String(maxLength: 1000),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ItemInRoom",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Name = c.String(maxLength: 255),
                        Description = c.String(maxLength: 1000),
                        RoomId = c.Int(nullable: false),
                        StatusId = c.Int(nullable: false),
                        CategoryId = c.Int(nullable: false),
                        AddedDate = c.Long(nullable: false),
                        ItemCategory_Id = c.Long(),
                        Room_Id = c.Long(),
                        Status_Id = c.Long(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ItemCategory", t => t.ItemCategory_Id)
                .ForeignKey("dbo.Room", t => t.Room_Id)
                .ForeignKey("dbo.ItemStatus", t => t.Status_Id)
                .Index(t => t.ItemCategory_Id)
                .Index(t => t.Room_Id)
                .Index(t => t.Status_Id);
            
            CreateTable(
                "dbo.Room",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Name = c.String(maxLength: 500),
                        Type = c.String(maxLength: 500),
                        Rent_User_Id = c.Int(nullable: false),
                        House_Id = c.Long(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.House", t => t.House_Id)
                .Index(t => t.House_Id);
            
            CreateTable(
                "dbo.ItemStatus",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Status = c.String(maxLength: 255),
                        Decription = c.String(maxLength: 1000),
                        Code = c.String(maxLength: 20),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.AspNetRoles",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true, name: "RoleNameIndex");
            
            CreateTable(
                "dbo.AspNetUserRoles",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        RoleId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.UserId, t.RoleId })
                .ForeignKey("dbo.AspNetRoles", t => t.RoleId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.RoleId);
            
            CreateTable(
                "dbo.Transaction",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Date = c.Long(nullable: false),
                        FromHouse = c.String(maxLength: 500),
                        FromRoom = c.String(maxLength: 500),
                        ToHouse = c.String(maxLength: 500),
                        ToRoom = c.String(maxLength: 500),
                        FromStatus = c.String(maxLength: 500),
                        ToStatus = c.String(maxLength: 500),
                        IsVerified = c.Boolean(nullable: false),
                        Type = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.AspNetUsers",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        FirstName = c.String(),
                        LastName = c.String(),
                        Email = c.String(maxLength: 256),
                        EmailConfirmed = c.Boolean(nullable: false),
                        PasswordHash = c.String(),
                        SecurityStamp = c.String(),
                        PhoneNumber = c.String(),
                        PhoneNumberConfirmed = c.Boolean(nullable: false),
                        TwoFactorEnabled = c.Boolean(nullable: false),
                        LockoutEndDateUtc = c.DateTime(),
                        LockoutEnabled = c.Boolean(nullable: false),
                        AccessFailedCount = c.Int(nullable: false),
                        UserName = c.String(nullable: false, maxLength: 256),
                        Discriminator = c.String(nullable: false, maxLength: 128),
                        Avatar_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Media", t => t.Avatar_Id)
                .Index(t => t.UserName, unique: true, name: "UserNameIndex")
                .Index(t => t.Avatar_Id);
            
            CreateTable(
                "dbo.AspNetUserClaims",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(nullable: false, maxLength: 128),
                        ClaimType = c.String(),
                        ClaimValue = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUserLogins",
                c => new
                    {
                        LoginProvider = c.String(nullable: false, maxLength: 128),
                        ProviderKey = c.String(nullable: false, maxLength: 128),
                        UserId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.LoginProvider, t.ProviderKey, t.UserId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.MediaHouses",
                c => new
                    {
                        Media_Id = c.Guid(nullable: false),
                        House_Id = c.Long(nullable: false),
                    })
                .PrimaryKey(t => new { t.Media_Id, t.House_Id })
                .ForeignKey("dbo.Media", t => t.Media_Id, cascadeDelete: true)
                .ForeignKey("dbo.House", t => t.House_Id, cascadeDelete: true)
                .Index(t => t.Media_Id)
                .Index(t => t.House_Id);
            
            CreateTable(
                "dbo.MediaItemInHouses",
                c => new
                    {
                        Media_Id = c.Guid(nullable: false),
                        ItemInHouse_Id = c.Long(nullable: false),
                    })
                .PrimaryKey(t => new { t.Media_Id, t.ItemInHouse_Id })
                .ForeignKey("dbo.Media", t => t.Media_Id, cascadeDelete: true)
                .ForeignKey("dbo.ItemInHouse", t => t.ItemInHouse_Id, cascadeDelete: true)
                .Index(t => t.Media_Id)
                .Index(t => t.ItemInHouse_Id);
            
            CreateTable(
                "dbo.ItemInRoomMedias",
                c => new
                    {
                        ItemInRoom_Id = c.Long(nullable: false),
                        Media_Id = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => new { t.ItemInRoom_Id, t.Media_Id })
                .ForeignKey("dbo.ItemInRoom", t => t.ItemInRoom_Id, cascadeDelete: true)
                .ForeignKey("dbo.Media", t => t.Media_Id, cascadeDelete: true)
                .Index(t => t.ItemInRoom_Id)
                .Index(t => t.Media_Id);
            
            CreateTable(
                "dbo.RoomMedias",
                c => new
                    {
                        Room_Id = c.Long(nullable: false),
                        Media_Id = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => new { t.Room_Id, t.Media_Id })
                .ForeignKey("dbo.Room", t => t.Room_Id, cascadeDelete: true)
                .ForeignKey("dbo.Media", t => t.Media_Id, cascadeDelete: true)
                .Index(t => t.Room_Id)
                .Index(t => t.Media_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserClaims", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUsers", "Avatar_Id", "dbo.Media");
            DropForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles");
            DropForeignKey("dbo.Room", "House_Id", "dbo.House");
            DropForeignKey("dbo.ItemInHouse", "StatusId", "dbo.ItemStatus");
            DropForeignKey("dbo.ItemInRoom", "Status_Id", "dbo.ItemStatus");
            DropForeignKey("dbo.RoomMedias", "Media_Id", "dbo.Media");
            DropForeignKey("dbo.RoomMedias", "Room_Id", "dbo.Room");
            DropForeignKey("dbo.ItemInRoom", "Room_Id", "dbo.Room");
            DropForeignKey("dbo.ItemInRoomMedias", "Media_Id", "dbo.Media");
            DropForeignKey("dbo.ItemInRoomMedias", "ItemInRoom_Id", "dbo.ItemInRoom");
            DropForeignKey("dbo.ItemInRoom", "ItemCategory_Id", "dbo.ItemCategory");
            DropForeignKey("dbo.MediaItemInHouses", "ItemInHouse_Id", "dbo.ItemInHouse");
            DropForeignKey("dbo.MediaItemInHouses", "Media_Id", "dbo.Media");
            DropForeignKey("dbo.MediaHouses", "House_Id", "dbo.House");
            DropForeignKey("dbo.MediaHouses", "Media_Id", "dbo.Media");
            DropForeignKey("dbo.ItemInHouse", "CategoryId", "dbo.ItemCategory");
            DropForeignKey("dbo.ItemInHouse", "HouseId", "dbo.House");
            DropIndex("dbo.RoomMedias", new[] { "Media_Id" });
            DropIndex("dbo.RoomMedias", new[] { "Room_Id" });
            DropIndex("dbo.ItemInRoomMedias", new[] { "Media_Id" });
            DropIndex("dbo.ItemInRoomMedias", new[] { "ItemInRoom_Id" });
            DropIndex("dbo.MediaItemInHouses", new[] { "ItemInHouse_Id" });
            DropIndex("dbo.MediaItemInHouses", new[] { "Media_Id" });
            DropIndex("dbo.MediaHouses", new[] { "House_Id" });
            DropIndex("dbo.MediaHouses", new[] { "Media_Id" });
            DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
            DropIndex("dbo.AspNetUserClaims", new[] { "UserId" });
            DropIndex("dbo.AspNetUsers", new[] { "Avatar_Id" });
            DropIndex("dbo.AspNetUsers", "UserNameIndex");
            DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
            DropIndex("dbo.AspNetRoles", "RoleNameIndex");
            DropIndex("dbo.Room", new[] { "House_Id" });
            DropIndex("dbo.ItemInRoom", new[] { "Status_Id" });
            DropIndex("dbo.ItemInRoom", new[] { "Room_Id" });
            DropIndex("dbo.ItemInRoom", new[] { "ItemCategory_Id" });
            DropIndex("dbo.ItemInHouse", new[] { "CategoryId" });
            DropIndex("dbo.ItemInHouse", new[] { "StatusId" });
            DropIndex("dbo.ItemInHouse", new[] { "HouseId" });
            DropTable("dbo.RoomMedias");
            DropTable("dbo.ItemInRoomMedias");
            DropTable("dbo.MediaItemInHouses");
            DropTable("dbo.MediaHouses");
            DropTable("dbo.AspNetUserLogins");
            DropTable("dbo.AspNetUserClaims");
            DropTable("dbo.AspNetUsers");
            DropTable("dbo.Transaction");
            DropTable("dbo.AspNetUserRoles");
            DropTable("dbo.AspNetRoles");
            DropTable("dbo.ItemStatus");
            DropTable("dbo.Room");
            DropTable("dbo.ItemInRoom");
            DropTable("dbo.Media");
            DropTable("dbo.ItemCategory");
            DropTable("dbo.ItemInHouse");
            DropTable("dbo.House");
        }
    }
}
