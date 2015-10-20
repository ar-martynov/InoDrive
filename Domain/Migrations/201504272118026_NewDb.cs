namespace Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class NewDb : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Bids",
                c => new
                    {
                        BidId = c.Int(nullable: false, identity: true),
                        UserId = c.String(maxLength: 128),
                        TourId = c.Int(nullable: false),
                        CreationDate = c.DateTime(nullable: false),
                        IsAccepted = c.Boolean(),
                        IsWatchedByOwner = c.Boolean(nullable: false),
                        IsWatchedByUser = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.BidId)
                .ForeignKey("dbo.Tours", t => t.TourId)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .Index(t => t.UserId)
                .Index(t => t.TourId);
            
            CreateTable(
                "dbo.Tours",
                c => new
                    {
                        TourId = c.Int(nullable: false, identity: true),
                        UserId = c.String(maxLength: 128),
                        TotalSlots = c.Int(nullable: false),
                        FreeSlots = c.Int(nullable: false),
                        CreationDate = c.DateTime(nullable: false),
                        ExpirationDate = c.DateTime(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                        Payment = c.Decimal(storeType: "money"),
                    })
                .PrimaryKey(t => t.TourId)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.TourProfiles",
                c => new
                    {
                        TourId = c.Int(nullable: false),
                        IsDeviationAllowed = c.Boolean(nullable: false),
                        IsPetsAllowed = c.Boolean(nullable: false),
                        IsMusicAllowed = c.Boolean(nullable: false),
                        IsFoodAllowed = c.Boolean(nullable: false),
                        IsDrinkAllowed = c.Boolean(nullable: false),
                        IsSmokeAllowed = c.Boolean(nullable: false),
                        CarDescription = c.String(),
                        CarImage = c.String(),
                        CarImageExtension = c.String(),
                        Comfort = c.Int(nullable: false),
                        Baggage = c.Int(nullable: false),
                        AdditionalInfo = c.String(),
                        OwnerExperience = c.String(),
                    })
                .PrimaryKey(t => t.TourId)
                .ForeignKey("dbo.Tours", t => t.TourId)
                .Index(t => t.TourId);
            
            CreateTable(
                "dbo.AspNetUsers",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
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
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.UserName, unique: true, name: "UserNameIndex");
            
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
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
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
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUserRoles",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        RoleId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.UserId, t.RoleId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .ForeignKey("dbo.AspNetRoles", t => t.RoleId)
                .Index(t => t.UserId)
                .Index(t => t.RoleId);
            
            CreateTable(
                "dbo.UserProfiles",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        FirstName = c.String(),
                        LastName = c.String(),
                        PublicEmail = c.String(),
                        Phone = c.String(),
                        Age = c.Int(nullable: false),
                        AvatarPath = c.String(),
                        About = c.String(),
                    })
                .PrimaryKey(t => t.UserId)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.WayPoints",
                c => new
                    {
                        WayPointId = c.Int(nullable: false, identity: true),
                        TourId = c.Int(nullable: false),
                        CityId = c.Int(nullable: false),
                        WayPointOrder = c.Int(nullable: false),
                        IsStart = c.Boolean(nullable: false),
                        IsDestination = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.WayPointId)
                .ForeignKey("dbo.Cities", t => t.CityId)
                .ForeignKey("dbo.Tours", t => t.TourId)
                .Index(t => t.TourId)
                .Index(t => t.CityId);
            
            CreateTable(
                "dbo.Cities",
                c => new
                    {
                        CityId = c.Int(nullable: false, identity: true),
                        CityNameRu = c.String(),
                        CityNameEn = c.String(),
                        Latitude = c.Double(nullable: false),
                        Longtitude = c.Double(nullable: false),
                    })
                .PrimaryKey(t => t.CityId);
            
            CreateTable(
                "dbo.Regions",
                c => new
                    {
                        RegionId = c.Int(nullable: false, identity: true),
                        RegionNameEn = c.String(),
                        RegionNameRu = c.String(),
                    })
                .PrimaryKey(t => t.RegionId);
            
            CreateTable(
                "dbo.Clients",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Secret = c.String(nullable: false),
                        Name = c.String(nullable: false, maxLength: 100),
                        ApplicationType = c.Int(nullable: false),
                        Active = c.Boolean(nullable: false),
                        RefreshTokenLifeTime = c.Int(nullable: false),
                        AllowedOrigin = c.String(maxLength: 100),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.RefreshTokens",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Subject = c.String(nullable: false, maxLength: 50),
                        ClientId = c.String(nullable: false, maxLength: 50),
                        IssuedUtc = c.DateTime(nullable: false),
                        ExpiresUtc = c.DateTime(nullable: false),
                        ProtectedTicket = c.String(nullable: false),
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
                "dbo.RegionCities",
                c => new
                    {
                        Region_RegionId = c.Int(nullable: false),
                        City_CityId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Region_RegionId, t.City_CityId })
                .ForeignKey("dbo.Regions", t => t.Region_RegionId, cascadeDelete: true)
                .ForeignKey("dbo.Cities", t => t.City_CityId, cascadeDelete: true)
                .Index(t => t.Region_RegionId)
                .Index(t => t.City_CityId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles");
            DropForeignKey("dbo.WayPoints", "TourId", "dbo.Tours");
            DropForeignKey("dbo.WayPoints", "CityId", "dbo.Cities");
            DropForeignKey("dbo.RegionCities", "City_CityId", "dbo.Cities");
            DropForeignKey("dbo.RegionCities", "Region_RegionId", "dbo.Regions");
            DropForeignKey("dbo.UserProfiles", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Tours", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserClaims", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Bids", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.TourProfiles", "TourId", "dbo.Tours");
            DropForeignKey("dbo.Bids", "TourId", "dbo.Tours");
            DropIndex("dbo.RegionCities", new[] { "City_CityId" });
            DropIndex("dbo.RegionCities", new[] { "Region_RegionId" });
            DropIndex("dbo.AspNetRoles", "RoleNameIndex");
            DropIndex("dbo.WayPoints", new[] { "CityId" });
            DropIndex("dbo.WayPoints", new[] { "TourId" });
            DropIndex("dbo.UserProfiles", new[] { "UserId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
            DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
            DropIndex("dbo.AspNetUserClaims", new[] { "UserId" });
            DropIndex("dbo.AspNetUsers", "UserNameIndex");
            DropIndex("dbo.TourProfiles", new[] { "TourId" });
            DropIndex("dbo.Tours", new[] { "UserId" });
            DropIndex("dbo.Bids", new[] { "TourId" });
            DropIndex("dbo.Bids", new[] { "UserId" });
            DropTable("dbo.RegionCities");
            DropTable("dbo.AspNetRoles");
            DropTable("dbo.RefreshTokens");
            DropTable("dbo.Clients");
            DropTable("dbo.Regions");
            DropTable("dbo.Cities");
            DropTable("dbo.WayPoints");
            DropTable("dbo.UserProfiles");
            DropTable("dbo.AspNetUserRoles");
            DropTable("dbo.AspNetUserLogins");
            DropTable("dbo.AspNetUserClaims");
            DropTable("dbo.AspNetUsers");
            DropTable("dbo.TourProfiles");
            DropTable("dbo.Tours");
            DropTable("dbo.Bids");
        }
    }
}
