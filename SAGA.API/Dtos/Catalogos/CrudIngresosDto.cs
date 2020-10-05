using SAGA.BOL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SAGA.API.Dtos.Catalogos
{
    public class CrudIngresosDto
    {
        public int Id { get; set; }
        public Guid IdG { get; set; }
        public string Clave { get; set; }
        public int ClaveInt { get; set; }
        public string Descripcion { get; set; }
        public string Comentario { get; set; }
        public bool Activo { get; set; }
        public Guid Usuario { get; set; }
        public DateTime Fecha { get; set; }
        public int Tipo { get; set; }
        public Int16 crud { get; set; } /* C-1 R-2 U-3 D-4 */
        public string razonSocial { get; set; }
        public int AreasId { get; set; }
        public int PuestoId { get; set; }
        public int CoordId { get; set; }
        public Guid ClienteId { get; set; }
        public int SucursalId { get; set; }
        public string catalogo { get; set; }
        public List<DiasHorasIngresos> DiasHoras { get; set; }
        public List<DiasHorasIngresos> HorarioComida { get; set; }
        public byte HorasTotales { get; set; }
        public byte HorasComida { get; set; }
        public byte HorasDescanso { get; set; }
        public byte TurnosHorariosId { get; set; }
        public decimal MontoTope { get; set; }
        public bool Servicio { get; set; }
        public string Hoja { get; set; }
        public Guid DepartamentoId { get; set; }
        public List<Guid> Empleados { get; set; }
        public decimal Porcentaje { get; set; }
        public List<int> Puestos { get; set; }
        public List<int> Sucursales { get; set; }
        public string Concepto { get; set; }
        public RegistroPatronal RegistroPatronal { get; set; }
    }
 
}