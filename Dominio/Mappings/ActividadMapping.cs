using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entidades;
using FluentNHibernate.Mapping;

namespace Dominio.Mappings
{
    public class ActividadMapping:ClassMap<Actividad>
    {
        public ActividadMapping()
        {
            Table("Actividades");
            Id(x => x.Id).GeneratedBy.Identity();
            Map(x => x.Fecha).Not.Nullable();
            Map(x => x.Comentario).Length(3000).Nullable().Default("");
            References(x => x.Proyecto).Column("IdProyecto").Fetch.Join().Not.LazyLoad();
            References(x => x.Usuario).Column("IdUsuario").Fetch.Join().LazyLoad();
        }
    }
}
