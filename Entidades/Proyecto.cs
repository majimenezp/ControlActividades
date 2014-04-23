using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entidades
{
    public class Proyecto
    {
        public virtual int Id { get; set; }
        public virtual string Nombre { get; set; }
        public virtual Usuario Usuario { get; set; }

        public virtual TipoProyecto Tipo { get; set; }
        public virtual Estado Estado { get; set; }
        public virtual Usuario Responsable { get; set; }

        public virtual int IdTipoProyecto { get; set; }
        public virtual int IdEstado { get; set; }
        public virtual int IdResponsable { get; set; }
            

    }
}
