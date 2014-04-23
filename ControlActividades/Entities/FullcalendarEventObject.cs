using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ControlActividades.Entities
{
    public class FullcalendarEventObject
    {
        public int id { get; set; }
        public string title { get; set; }
        public bool allDay { get; set; }
        public string start { get; set; }
        public string comment { get; set; }

        public FullcalendarEventObject()
        {
            this.id = 0;
            this.title = string.Empty;
            this.allDay = true;
            this.start = DateTime.Now.ToString("yyyy-MM-dd");
            this.comment = string.Empty;
        }
        public FullcalendarEventObject(Entidades.Actividad actividad)
        {
            this.id = actividad.Id;
            this.title = actividad.Proyecto.Nombre;
            this.allDay = true;
            this.start = actividad.Fecha.ToString("yyyy-MM-dd");
            this.comment = actividad.Comentario != null ? actividad.Comentario : string.Empty;
        }
        internal static FullcalendarEventObject[] ConvertActividades(Entidades.Actividad[] actividades)
        {
            return (from act in actividades
                    select new FullcalendarEventObject(act)
                        ).ToArray();
        }
    }
}