namespace SAGA.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AlterRequisicion : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("Vtas.Requisiciones", "DireccionId", "sist.Direcciones");
            DropForeignKey("Vtas.Requisiciones", "PrioridadId", "sist.Prioridades");
            DropIndex("Vtas.Requisiciones", new[] { "PrioridadId" });
            DropIndex("Vtas.Requisiciones", new[] { "DireccionId" });
            AlterColumn("Vtas.Requisiciones", "fch_Aprobacion", c => c.DateTime());
            AlterColumn("Vtas.Requisiciones", "fch_Cumplimiento", c => c.DateTime());
            AlterColumn("Vtas.Requisiciones", "fch_Modificacion", c => c.DateTime());
            AlterColumn("Vtas.Requisiciones", "PrioridadId", c => c.Int());
            AlterColumn("Vtas.Requisiciones", "DireccionId", c => c.Guid());
            CreateIndex("Vtas.Requisiciones", "PrioridadId");
            CreateIndex("Vtas.Requisiciones", "DireccionId");
            AddForeignKey("Vtas.Requisiciones", "DireccionId", "sist.Direcciones", "Id");
            AddForeignKey("Vtas.Requisiciones", "PrioridadId", "sist.Prioridades", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("Vtas.Requisiciones", "PrioridadId", "sist.Prioridades");
            DropForeignKey("Vtas.Requisiciones", "DireccionId", "sist.Direcciones");
            DropIndex("Vtas.Requisiciones", new[] { "DireccionId" });
            DropIndex("Vtas.Requisiciones", new[] { "PrioridadId" });
            AlterColumn("Vtas.Requisiciones", "DireccionId", c => c.Guid(nullable: false));
            AlterColumn("Vtas.Requisiciones", "PrioridadId", c => c.Int(nullable: false));
            AlterColumn("Vtas.Requisiciones", "fch_Modificacion", c => c.DateTime(nullable: false));
            AlterColumn("Vtas.Requisiciones", "fch_Cumplimiento", c => c.DateTime(nullable: false));
            AlterColumn("Vtas.Requisiciones", "fch_Aprobacion", c => c.DateTime(nullable: false));
            CreateIndex("Vtas.Requisiciones", "DireccionId");
            CreateIndex("Vtas.Requisiciones", "PrioridadId");
            AddForeignKey("Vtas.Requisiciones", "PrioridadId", "sist.Prioridades", "Id", cascadeDelete: true);
            AddForeignKey("Vtas.Requisiciones", "DireccionId", "sist.Direcciones", "Id", cascadeDelete: true);
        }
    }
}
