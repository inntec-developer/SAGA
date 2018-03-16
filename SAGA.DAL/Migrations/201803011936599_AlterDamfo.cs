namespace SAGA.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AlterDamfo : DbMigration
    {
        public override void Up()
        {
            AddColumn("Recl.DAMFO_290", "Activo", c => c.Boolean(nullable: false));
            AddColumn("Recl.DAMFO_290", "fch_Creacion", c => c.DateTime(nullable: false));
            AddColumn("Recl.DAMFO_290", "fch_Modificacion", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("Recl.DAMFO_290", "fch_Modificacion");
            DropColumn("Recl.DAMFO_290", "fch_Creacion");
            DropColumn("Recl.DAMFO_290", "Activo");
        }
    }
}
