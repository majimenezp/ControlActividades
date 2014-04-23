using Entidades;
using FluentNHibernate.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominio.Mappings
{
    public class EstadoMapping:ClassMap<Estado>
    {
        public EstadoMapping()
        {
            Table("Estados");
            Id(x => x.Id).GeneratedBy.Identity();
            Map(x => x.DescEstado).Length(500).Not.Nullable();
        }
    }
}
