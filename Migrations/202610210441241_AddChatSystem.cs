namespace Web_TiemHoaMuaXuan.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddChatSystem : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ChatMessages",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserEmail = c.String(nullable: false),
                        Message = c.String(nullable: false),
                        SentAt = c.DateTime(nullable: false),
                        IsAdmin = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.ChatMessages");
        }
    }
}
