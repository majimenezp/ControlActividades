using Nancy.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ControlActividades.Security
{
    public class SystemUser:Entidades.Usuario,IUserIdentity
    {
        List<string> claims;
        
        public SystemUser()
        {
            this.claims = new List<string>();
            this.UsrName = "";
        }
        public SystemUser(Aegis.Modelos.Usuario user)
        {
            this.claims = new List<string>();
            this.UsrName = user.Username;
            this.FullName = user.Nombre;
            this.UserId = user.IdUsuario;
        }

        public IEnumerable<string> Claims
        {
            get { return claims; }
        }
               
        public string UserName
        {
            get { return UsrName; }
        }
    }
}