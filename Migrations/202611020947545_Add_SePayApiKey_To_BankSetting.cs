namespace Web_TiemHoaMuaXuan.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Add_SePayApiKey_To_BankSetting : DbMigration
    {
        public override void Up()
        {
            /*CreateTable(
                "dbo.AppUsers",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Email = c.String(nullable: false),
                        Password = c.String(nullable: false),
                        FullName = c.String(nullable: false),
                        Phone = c.String(),
                        Address = c.String(),
                        IsAdmin = c.Boolean(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);*/
            
            AddColumn("dbo.BankSettings", "SePayApiKey", c => c.String());
            AlterColumn("dbo.BankSettings", "BankName", c => c.String(nullable: false));
            AlterColumn("dbo.BankSettings", "AccountNumber", c => c.String(nullable: false));
            AlterColumn("dbo.BankSettings", "AccountHolder", c => c.String());
            AlterColumn("dbo.BankSettings", "QRImageUrl", c => c.String());
            AlterColumn("dbo.Products", "Name", c => c.String(nullable: false, maxLength: 100));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Products", "Name", c => c.String(nullable: false, maxLength: 200));
            AlterColumn("dbo.BankSettings", "QRImageUrl", c => c.String(maxLength: 500));
            AlterColumn("dbo.BankSettings", "AccountHolder", c => c.String(maxLength: 200));
            AlterColumn("dbo.BankSettings", "AccountNumber", c => c.String(maxLength: 50));
            AlterColumn("dbo.BankSettings", "BankName", c => c.String(maxLength: 100));
            DropColumn("dbo.BankSettings", "SePayApiKey");
            DropTable("dbo.AppUsers");
        }
    }
}
