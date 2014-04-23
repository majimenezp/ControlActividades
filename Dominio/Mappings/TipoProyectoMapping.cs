using Entidades;
using FluentNHibernate.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominio.Mappings
{
    public class TipoProyectoMapping:ClassMap<TipoProyecto>
    {
        public TipoProyectoMapping()
        {
            Table("TiposProyecto");
            Id(x => x.Id).GeneratedBy.Identity();
            Map(x => x.Descripcion).Length(500).Not.Nullable();

        }
    }
}
