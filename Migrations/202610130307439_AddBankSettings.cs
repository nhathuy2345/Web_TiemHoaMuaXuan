namespace Web_TiemHoaMuaXuan.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddBankSettings : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.BankSettings",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        BankName = c.String(maxLength: 100),
                        AccountNumber = c.String(maxLength: 50),
                        AccountHolder = c.String(maxLength: 200),
                        QRImageUrl = c.String(maxLength: 500),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.BankSettings");
        }
    }
}
