namespace Web_TiemHoaMuaXuan.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Categories",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 100),
                        Description = c.String(maxLength: 500),
                        IsActive = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Products",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 200),
                        Description = c.String(maxLength: 2000),
                        Price = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OldPrice = c.Decimal(precision: 18, scale: 2),
                        DiscountPercent = c.Int(nullable: false),
                        ImageUrl = c.String(maxLength: 500),
                        Stock = c.Int(nullable: false),
                        CategoryId = c.Int(nullable: false),
                        CreatedAt = c.DateTime(nullable: false),
                        IsFeatured = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Categories", t => t.CategoryId, cascadeDelete: true)
                .Index(t => t.CategoryId);
            
            CreateTable(
                "dbo.ContactMessages",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 100),
                        Email = c.String(maxLength: 200),
                        Phone = c.String(maxLength: 20),
                        Message = c.String(nullable: false, maxLength: 2000),
                        SentAt = c.DateTime(nullable: false),
                        IsRead = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.OrderItems",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        OrderId = c.Int(nullable: false),
                        ProductId = c.Int(nullable: false),
                        ProductName = c.String(maxLength: 200),
                        Image = c.String(maxLength: 500),
                        Price = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Quantity = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Orders", t => t.OrderId, cascadeDelete: true)
                .Index(t => t.OrderId);
            
            CreateTable(
                "dbo.Orders",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        OrderCode = c.String(maxLength: 50),
                        UserEmail = c.String(maxLength: 200),
                        FullName = c.String(nullable: false, maxLength: 100),
                        Phone = c.String(nullable: false, maxLength: 20),
                        Address = c.String(nullable: false, maxLength: 500),
                        PaymentMethod = c.String(maxLength: 50),
                        Status = c.String(maxLength: 50),
                        OrderDate = c.DateTime(nullable: false),
                        TotalAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.OrderItems", "OrderId", "dbo.Orders");
            DropForeignKey("dbo.Products", "CategoryId", "dbo.Categories");
            DropIndex("dbo.OrderItems", new[] { "OrderId" });
            DropIndex("dbo.Products", new[] { "CategoryId" });
            DropTable("dbo.Orders");
            DropTable("dbo.OrderItems");
            DropTable("dbo.ContactMessages");
            DropTable("dbo.Products");
            DropTable("dbo.Categories");
        }
    }
}
