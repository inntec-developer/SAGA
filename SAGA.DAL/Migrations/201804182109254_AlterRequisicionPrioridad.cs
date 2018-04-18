namespace SAGA.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AlterRequisicionPrioridad : DbMigration
    {
        public override void Up()
        {
            CreateIndex("Vtas.Requisiciones", "PrioridadId");
            AddForeignKey("Vtas.Requisiciones", "PrioridadId", "sist.Prioridades", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("Vtas.Requisiciones", "PrioridadId", "sist.Prioridades");
            DropIndex("Vtas.Requisiciones", new[] { "PrioridadId" });
        }
    }
}
