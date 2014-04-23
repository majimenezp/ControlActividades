using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entidades
{
    public class Usuario
    {
        public Usuario()
        {
            this.InternalId = 0;
            this.UserId = Guid.Empty;
            this.FullName = string.Empty;
            this.UsrName = string.Empty;
        }
        public Usuario(Usuario user)
        {
            this.InternalId = user.InternalId;
            this.UserId = user.UserId;
            this.FullName = user.FullName;
            this.UsrName = user.UsrName;
        }
        public virtual int InternalId { get; set; }
        public virtual Guid UserId { get; set; }
        public virtual string FullName { get; set; }
        public virtual string UsrName { get; set; }
        
    }
}
