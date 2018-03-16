namespace SAGA.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class alterDamfoFlexible : DbMigration
    {
        public override void Up()
        {
            AddColumn("Recl.DAMFO_290", "FlexibilidadHorario", c => c.Boolean(nullable: false));
            DropColumn("Recl.DAMFO_290", "FlexbilidadHorario");
        }
        
        public override void Down()
        {
            AddColumn("Recl.DAMFO_290", "FlexbilidadHorario", c => c.Boolean(nullable: false));
            DropColumn("Recl.DAMFO_290", "FlexibilidadHorario");
        }
    }
}
