namespace SAGA.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class changeRequiDamfo : DbMigration
    {
        public override void Up()
        {
            MoveTable(name: "Vtas.TamanosEmpresas", newSchema: "sist");
            MoveTable(name: "Vtas.TiposBases", newSchema: "sist");
            MoveTable(name: "Vtas.TiposEmpresas", newSchema: "sist");
            MoveTable(name: "Recl.EstadosEstudios", newSchema: "sist");
            MoveTable(name: "Recl.TiposBeneficios", newSchema: "sist");
            MoveTable(name: "Recl.CompetenciasAreas", newSchema: "sist");
            MoveTable(name: "Recl.CompetenciasCardinales", newSchema: "sist");
            MoveTable(name: "Recl.CompetenciasGerenciales", newSchema: "sist");
            MoveTable(name: "Recl.TiposContratos", newSchema: "sist");
            MoveTable(name: "Recl.JornadasLaborales", newSchema: "sist");
            MoveTable(name: "Recl.TiposPsicometrias", newSchema: "sist");
            MoveTable(name: "Recl.TiemposContratos", newSchema: "sist");
            MoveTable(name: "Recl.TiposModalidades", newSchema: "sist");
            MoveTable(name: "Recl.TiposNominas", newSchema: "sist");
            MoveTable(name: "Recl.TiposReclutamientos", newSchema: "sist");
            DropForeignKey("sist.Roles", "ModuloId", "sist.Modulos");
            DropForeignKey("sist.Estatus", "ModuloId", "sist.Modulos");
            DropForeignKey("Vtas.Requisiciones", "HorarioId", "Vtas.HorariosRequi");
            DropForeignKey("sist.Prioridades", "ModuloId", "sist.Modulos");
            DropForeignKey("Vtas.Requisiciones", "ContratoFinalId", "Recl.TiposContratos");
            DropForeignKey("Vtas.Requisiciones", "EstatusId", "sist.Estatus");
            DropIndex("sist.Roles", new[] { "ModuloId" });
            DropIndex("Vtas.Requisiciones", new[] { "ContratoFinalId" });
            DropIndex("Vtas.Requisiciones", new[] { "EstatusId" });
            DropIndex("Vtas.Requisiciones", new[] { "HorarioId" });
            DropIndex("sist.Estatus", new[] { "ModuloId" });
            DropIndex("sist.Prioridades", new[] { "ModuloId" });
            RenameColumn(table: "Vtas.Requisiciones", name: "ContratoFinalId", newName: "ContratoFinal_Id");
            AddColumn("Recl.DAMFO_290", "UsuarioAlta", c => c.Guid(nullable: false));
            AddColumn("Recl.DAMFO_290", "UsuarioMod", c => c.Guid());
            AddColumn("Vtas.Requisiciones", "TiempoContratoId", c => c.Int());
            AddColumn("Vtas.Requisiciones", "fch_Creacion", c => c.DateTime(nullable: false));
            AddColumn("Vtas.Requisiciones", "fch_Aprobacion", c => c.DateTime(nullable: false));
            AddColumn("Vtas.Requisiciones", "fch_Cumplimiento", c => c.DateTime(nullable: false));
            AddColumn("Vtas.Requisiciones", "fch_Modificacion", c => c.DateTime(nullable: false));
            AddColumn("Vtas.Requisiciones", "FlexibilidadHorario", c => c.Boolean(nullable: false));
            AddColumn("Vtas.Requisiciones", "JornadaLaboralId", c => c.Int());
            AddColumn("Vtas.Requisiciones", "TipoModalidadId", c => c.Int());
            AlterColumn("sist.GiroEmpresas", "giroEmpresa", c => c.String(nullable: false, maxLength: 15));
            AlterColumn("Vtas.Requisiciones", "ContratoFinal_Id", c => c.Int());
            AlterColumn("Vtas.Requisiciones", "EstatusId", c => c.Int());
            CreateIndex("Vtas.Requisiciones", "TiempoContratoId");
            CreateIndex("Vtas.Requisiciones", "EstatusId");
            CreateIndex("Vtas.Requisiciones", "JornadaLaboralId");
            CreateIndex("Vtas.Requisiciones", "TipoModalidadId");
            CreateIndex("Vtas.Requisiciones", "ContratoFinal_Id");
            AddForeignKey("Vtas.Requisiciones", "JornadaLaboralId", "sist.JornadasLaborales", "Id");
            AddForeignKey("Vtas.Requisiciones", "TiempoContratoId", "sist.TiemposContratos", "Id");
            AddForeignKey("Vtas.Requisiciones", "TipoModalidadId", "sist.TiposModalidades", "Id");
            AddForeignKey("Vtas.Requisiciones", "ContratoFinal_Id", "sist.TiposContratos", "Id");
            AddForeignKey("Vtas.Requisiciones", "EstatusId", "sist.Estatus", "Id");
            DropColumn("sist.Roles", "ModuloId");
            DropColumn("Vtas.Requisiciones", "FechaCreacion");
            DropColumn("Vtas.Requisiciones", "FechaAprobacion");
            DropColumn("Vtas.Requisiciones", "FechaCumplimiento");
            DropColumn("Vtas.Requisiciones", "FechaModificacion");
            DropColumn("Vtas.Requisiciones", "HorarioId");
            DropColumn("sist.Estatus", "ModuloId");
            DropColumn("sist.Prioridades", "ModuloId");
            DropTable("sist.Modulos");
        }
        
        public override void Down()
        {
            CreateTable(
                "sist.Modulos",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Nombre = c.String(nullable: false, maxLength: 50),
                        Accion = c.String(),
                        Icono = c.String(),
                        Orden = c.Int(),
                        IdPadre = c.Int(),
                        Activo = c.Boolean(),
                        Ambito = c.String(maxLength: 30),
                        Clave = c.String(nullable: false, maxLength: 5, fixedLength: true),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("sist.Prioridades", "ModuloId", c => c.Int(nullable: false));
            AddColumn("sist.Estatus", "ModuloId", c => c.Int(nullable: false));
            AddColumn("Vtas.Requisiciones", "HorarioId", c => c.Guid(nullable: false));
            AddColumn("Vtas.Requisiciones", "FechaModificacion", c => c.DateTime(nullable: false));
            AddColumn("Vtas.Requisiciones", "FechaCumplimiento", c => c.DateTime(nullable: false));
            AddColumn("Vtas.Requisiciones", "FechaAprobacion", c => c.DateTime(nullable: false));
            AddColumn("Vtas.Requisiciones", "FechaCreacion", c => c.DateTime(nullable: false));
            AddColumn("sist.Roles", "ModuloId", c => c.Int(nullable: false));
            DropForeignKey("Vtas.Requisiciones", "EstatusId", "sist.Estatus");
            DropForeignKey("Vtas.Requisiciones", "ContratoFinal_Id", "sist.TiposContratos");
            DropForeignKey("Vtas.Requisiciones", "TipoModalidadId", "sist.TiposModalidades");
            DropForeignKey("Vtas.Requisiciones", "TiempoContratoId", "sist.TiemposContratos");
            DropForeignKey("Vtas.Requisiciones", "JornadaLaboralId", "sist.JornadasLaborales");
            DropIndex("Vtas.Requisiciones", new[] { "ContratoFinal_Id" });
            DropIndex("Vtas.Requisiciones", new[] { "TipoModalidadId" });
            DropIndex("Vtas.Requisiciones", new[] { "JornadaLaboralId" });
            DropIndex("Vtas.Requisiciones", new[] { "EstatusId" });
            DropIndex("Vtas.Requisiciones", new[] { "TiempoContratoId" });
            AlterColumn("Vtas.Requisiciones", "EstatusId", c => c.Int(nullable: false));
            AlterColumn("Vtas.Requisiciones", "ContratoFinal_Id", c => c.Int(nullable: false));
            AlterColumn("sist.GiroEmpresas", "giroEmpresa", c => c.String());
            DropColumn("Vtas.Requisiciones", "TipoModalidadId");
            DropColumn("Vtas.Requisiciones", "JornadaLaboralId");
            DropColumn("Vtas.Requisiciones", "FlexibilidadHorario");
            DropColumn("Vtas.Requisiciones", "fch_Modificacion");
            DropColumn("Vtas.Requisiciones", "fch_Cumplimiento");
            DropColumn("Vtas.Requisiciones", "fch_Aprobacion");
            DropColumn("Vtas.Requisiciones", "fch_Creacion");
            DropColumn("Vtas.Requisiciones", "TiempoContratoId");
            DropColumn("Recl.DAMFO_290", "UsuarioMod");
            DropColumn("Recl.DAMFO_290", "UsuarioAlta");
            RenameColumn(table: "Vtas.Requisiciones", name: "ContratoFinal_Id", newName: "ContratoFinalId");
            CreateIndex("sist.Prioridades", "ModuloId");
            CreateIndex("sist.Estatus", "ModuloId");
            CreateIndex("Vtas.Requisiciones", "HorarioId");
            CreateIndex("Vtas.Requisiciones", "EstatusId");
            CreateIndex("Vtas.Requisiciones", "ContratoFinalId");
            CreateIndex("sist.Roles", "ModuloId");
            AddForeignKey("Vtas.Requisiciones", "EstatusId", "sist.Estatus", "Id", cascadeDelete: true);
            AddForeignKey("Vtas.Requisiciones", "ContratoFinalId", "Recl.TiposContratos", "Id", cascadeDelete: true);
            AddForeignKey("sist.Prioridades", "ModuloId", "sist.Modulos", "Id", cascadeDelete: true);
            AddForeignKey("Vtas.Requisiciones", "HorarioId", "Vtas.HorariosRequi", "Id", cascadeDelete: true);
            AddForeignKey("sist.Estatus", "ModuloId", "sist.Modulos", "Id", cascadeDelete: true);
            AddForeignKey("sist.Roles", "ModuloId", "sist.Modulos", "Id", cascadeDelete: true);
            MoveTable(name: "sist.TiposReclutamientos", newSchema: "Recl");
            MoveTable(name: "sist.TiposNominas", newSchema: "Recl");
            MoveTable(name: "sist.TiposModalidades", newSchema: "Recl");
            MoveTable(name: "sist.TiemposContratos", newSchema: "Recl");
            MoveTable(name: "sist.TiposPsicometrias", newSchema: "Recl");
            MoveTable(name: "sist.JornadasLaborales", newSchema: "Recl");
            MoveTable(name: "sist.TiposContratos", newSchema: "Recl");
            MoveTable(name: "sist.CompetenciasGerenciales", newSchema: "Recl");
            MoveTable(name: "sist.CompetenciasCardinales", newSchema: "Recl");
            MoveTable(name: "sist.CompetenciasAreas", newSchema: "Recl");
            MoveTable(name: "sist.TiposBeneficios", newSchema: "Recl");
            MoveTable(name: "sist.EstadosEstudios", newSchema: "Recl");
            MoveTable(name: "sist.TiposEmpresas", newSchema: "Vtas");
            MoveTable(name: "sist.TiposBases", newSchema: "Vtas");
            MoveTable(name: "sist.TamanosEmpresas", newSchema: "Vtas");
        }
    }
}
