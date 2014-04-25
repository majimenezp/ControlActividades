using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entidades;
using FluentNHibernate.Mapping;

namespace Dominio.Mappings
{
    public class UsuarioMapping : ClassMap<Usuario>
    {
        public UsuarioMapping()
        {
            Table("Usuarios");
            Id(x => x.InternalId).Column("Id").GeneratedBy.Identity();
            Map(x => x.FullName).Column("Nombre").Not.Nullable().Length(2000);
            Map(x => x.UsrName).Column("Username").Not.Nullable().Length(100);
            Map(x => x.UserId).Column("IdAegis").Not.Nullable();
            Map(x => x.Area).Column("Area").Default("").Length(100);
        }
    }
}
