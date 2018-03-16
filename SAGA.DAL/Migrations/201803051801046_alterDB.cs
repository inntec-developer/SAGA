namespace SAGA.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class alterDB : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("Recl.DAMFO_290", "ContratoFinalId", "Recl.TiposContratos");
            DropIndex("Recl.DAMFO_290", new[] { "ContratoFinalId" });
            CreateTable(
                "Recl.JornadasLaborales",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Jornada = c.String(nullable: false, maxLength: 50),
                        Orden = c.Int(nullable: false),
                        VariosHorarios = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "Recl.TiemposContratos",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Tiempo = c.String(nullable: false, maxLength: 50),
                        Orden = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "Recl.TiposModalidades",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Modalidad = c.String(nullable: false, maxLength: 50),
                        Orden = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("Recl.DAMFO_290", "TiempoContratoId", c => c.Int());
            AddColumn("Recl.DAMFO_290", "FlexbilidadHorario", c => c.Boolean(nullable: false));
            AddColumn("Recl.DAMFO_290", "JornadaLaboralId", c => c.Int());
            AddColumn("Recl.DAMFO_290", "TipoModalidadId", c => c.Int());
            AlterColumn("Recl.DAMFO_290", "fch_Modificacion", c => c.DateTime());
            CreateIndex("Recl.DAMFO_290", "TiempoContratoId");
            CreateIndex("Recl.DAMFO_290", "JornadaLaboralId");
            CreateIndex("Recl.DAMFO_290", "TipoModalidadId");
            AddForeignKey("Recl.DAMFO_290", "JornadaLaboralId", "Recl.JornadasLaborales", "Id");
            AddForeignKey("Recl.DAMFO_290", "TiempoContratoId", "Recl.TiemposContratos", "Id");
            AddForeignKey("Recl.DAMFO_290", "TipoModalidadId", "Recl.TiposModalidades", "Id");
            DropColumn("Recl.DAMFO_290", "ContratoFinalId");
        }
        
        public override void Down()
        {
            AddColumn("Recl.DAMFO_290", "ContratoFinalId", c => c.Int(nullable: false));
            DropForeignKey("Recl.DAMFO_290", "TipoModalidadId", "Recl.TiposModalidades");
            DropForeignKey("Recl.DAMFO_290", "TiempoContratoId", "Recl.TiemposContratos");
            DropForeignKey("Recl.DAMFO_290", "JornadaLaboralId", "Recl.JornadasLaborales");
            DropIndex("Recl.DAMFO_290", new[] { "TipoModalidadId" });
            DropIndex("Recl.DAMFO_290", new[] { "JornadaLaboralId" });
            DropIndex("Recl.DAMFO_290", new[] { "TiempoContratoId" });
            AlterColumn("Recl.DAMFO_290", "fch_Modificacion", c => c.DateTime(nullable: false));
            DropColumn("Recl.DAMFO_290", "TipoModalidadId");
            DropColumn("Recl.DAMFO_290", "JornadaLaboralId");
            DropColumn("Recl.DAMFO_290", "FlexbilidadHorario");
            DropColumn("Recl.DAMFO_290", "TiempoContratoId");
            DropTable("Recl.TiposModalidades");
            DropTable("Recl.TiemposContratos");
            DropTable("Recl.JornadasLaborales");
            CreateIndex("Recl.DAMFO_290", "ContratoFinalId");
            AddForeignKey("Recl.DAMFO_290", "ContratoFinalId", "Recl.TiposContratos", "Id", cascadeDelete: true);
        }
    }
}
