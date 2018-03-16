namespace SAGA.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddActivoRequi : DbMigration
    {
        public override void Up()
        {
            AddColumn("Vtas.Requisiciones", "Activo", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("Vtas.Requisiciones", "Activo");
        }
    }
}
