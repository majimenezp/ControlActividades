using Nancy;
using Nancy.Security;
using ControlActividades.Security;
using Dominio;
using Entidades;

namespace ControlActividades
{
    
    public class IndexModule : NancyModule
    {
        public IndexModule()
        {
            this.RequiresAuthentication();
            Get["/"] = parameters =>
            {
                SystemUser user=(SystemUser)this.Context.CurrentUser;
                TipoProyecto[] projTypes=DAL.Instance.GetTiposProyectos();
                Estado[] projStates = DAL.Instance.GetEstados();
                Usuario[] scrumMaster = DAL.Instance.GetUsers();
                return View["index",
                    new {
                        FullName = user.FullName,
                    ProjectTypes = projTypes, 
                    ProjectStates = projStates ,
                    Users=scrumMaster
                    }];
            };

        }
    }
}