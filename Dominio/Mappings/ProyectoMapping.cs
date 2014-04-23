using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entidades;
using FluentNHibernate.Mapping;

namespace Dominio.Mappings
{
    public class ProyectoMapping : ClassMap<Proyecto>
    {
        public ProyectoMapping()
        {
            Table("Proyectos");
            Id(x => x.Id).GeneratedBy.Identity();
            Map(x => x.Nombre).Not.Nullable().Length(2000);
            References(x => x.Tipo).Column("IdTipoProyecto").Fetch.Join().Not.LazyLoad();
            References(x => x.Usuario).Column("IdUsuario").Fetch.Join().Not.LazyLoad();
            References(x => x.Estado).Column("Idestado").Fetch.Join().Not.LazyLoad();
            References(x => x.Responsable).Column("IdResponsable").Fetch.Join().Not.LazyLoad();
        }
    }
}
