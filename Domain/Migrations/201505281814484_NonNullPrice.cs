namespace Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class NonNullPrice : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Tours", "Payment", c => c.Decimal(nullable: false, storeType: "money"));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Tours", "Payment", c => c.Decimal(storeType: "money"));
        }
    }
}
