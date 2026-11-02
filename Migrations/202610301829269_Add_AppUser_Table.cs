namespace Web_TiemHoaMuaXuan.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Add_AppUser_Table : DbMigration
    {
        public override void Up()
        {
            CreateTable(
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
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.AppUsers");
        }
    }
}
