using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entidades
{
    public class Actividad
    {
        public virtual int Id { get; set; }
        public virtual DateTime Fecha { get; set; }
        public virtual Proyecto Proyecto { get; set; }
        public virtual Usuario Usuario { get; set; }
        public virtual string Comentario { get; set; }
    }
}
