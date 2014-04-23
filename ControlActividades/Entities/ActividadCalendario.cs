using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ControlActividades.Entities
{
    public class ActividadCalendario
    {
        public int IdProyecto { get; set; }
        public DateTime Fecha { get; set; }

        public string nombre { get; set; }

        public int id { get; set; }
    }
}