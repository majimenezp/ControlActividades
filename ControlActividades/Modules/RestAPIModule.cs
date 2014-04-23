using Nancy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nancy.Security;
using ControlActividades.Security;
using Dominio;
using Entidades;
using Nancy.ModelBinding;
using ControlActividades.Entities;
using System.Threading.Tasks;

namespace ControlActividades.Modules
{
    public class RestAPIModule : NancyModule
    {
        public RestAPIModule()
            : base("API")
        {
            this.RequiresAuthentication();

            Get["/proyectos/{Id}", runAsync: true] = async (x, token) =>
            {
                Proyecto[] proyectos = await GetProyectosByUser();
                return Response.AsJson(proyectos);
            };

            Get["/proyectos", runAsync: true] = async (x, token) =>
            {
                Proyecto[] proyectos = await GetProyectosByUser();
                return Response.AsJson(proyectos);
            };
            Post["/proyectos", runAsync: true] = async (x, token) =>
            {
                var proyecto = await CreateProject();
                //this.Request.Body
                //Proyecto[] proyectos = DAL.Instance.GetProjectsByUser((Entidades.Usuario)user);
                return Response.AsJson(proyecto);
            };

            Put["/proyectos/{Id}", runAsync: true] = async (x, token) =>
            {
                Proyecto proyecto = await UpdateProyect(x);
                return Response.AsJson(proyecto);
            };
            Delete["/proyectos/{Id}", runAsync: true] = async (x, token) =>
            {
                bool deleted = await DeleteProject(x);
                return Response.AsJson(deleted);
            };

            Get["/actividad/{id}"] = x =>
            {
                int IdActividad = (int)x.id;
                return Response.AsJson(new Actividad());
            };

            Delete["/actividad/{id}"] = x =>
            {
                int IdActividad = (int)x.id;
                DAL.Instance.DeleteActividad(IdActividad);
                return Response.AsJson(true);
            };
            Post["/actividad", runAsync: true] = async (x, token) =>
            {
                var actividad = await CreateActivity();
                return Response.AsJson(actividad);
            };
            Get["/actividades", runAsync: true] = async (x, token) =>
            {
                FullcalendarEventObject[] events = await GetActividadesByUser();
                return Response.AsJson(events);
            };
            Post["/comentario", runAsync: true] = async (x, token) =>
            {
                var comment = this.Bind<ComentarioActividad>();
                await CreateUpdateComment(comment);
                return Response.AsJson(true);
            };
        }

        private Task CreateUpdateComment(ComentarioActividad comment)
        {
            return Task.Run(() => {
                DAL.Instance.UpdateComment(comment.id, comment.comment);
            });
        }

        private Task<ActividadCalendario> CreateActivity()
        {
            return Task.Run(() =>
            {
                SystemUser user = (SystemUser)this.Context.CurrentUser;
                var actividad = this.Bind<ActividadCalendario>();
                Actividad tmpAct = DAL.Instance.CreateActividad(actividad.IdProyecto, actividad.Fecha.ToUniversalTime().Date, (Entidades.Usuario)user);
                actividad.nombre = tmpAct.Proyecto.Nombre;
                actividad.id = tmpAct.Id;
                return actividad;
            });
        }

        private Task<bool> DeleteProject(dynamic x)
        {
            return Task.Run(() =>
            {
                SystemUser user = (SystemUser)this.Context.CurrentUser;
                int IdProyecto = (int)x.id;
                bool deleted = DAL.Instance.DeleteProject(IdProyecto, (Entidades.Usuario)user);
                return deleted;
            });
        }

        private Task<Proyecto> UpdateProyect(dynamic x)
        {
            return Task.Run(() =>
            {
                SystemUser user = (SystemUser)this.Context.CurrentUser;
                int IdProyecto = (int)x.id;
                var proyecto = this.Bind<Proyecto>();
                proyecto.Id = IdProyecto;
                proyecto = DAL.Instance.UpdateProject(proyecto, (Entidades.Usuario)user);
                return proyecto;
            });
        }

        private Task<Proyecto> CreateProject()
        {
            return Task.Run(() =>
            {
                SystemUser user = (SystemUser)this.Context.CurrentUser;
                var proyecto = this.Bind<Proyecto>();
                proyecto = DAL.Instance.CreateProyect(proyecto, user);
                return proyecto;
            });
        }

        private Task<FullcalendarEventObject[]> GetActividadesByUser()
        {
            return Task.Run(() =>
            {
                DateTime startDate = this.Request.Query.start;
                DateTime endDate = this.Request.Query.end;
                SystemUser user = (SystemUser)this.Context.CurrentUser;
                Actividad[] actividades = DAL.Instance.GetActividadesByUser(startDate, endDate, (Entidades.Usuario)user);
                FullcalendarEventObject[] events = FullcalendarEventObject.ConvertActividades(actividades);
                return events;
            });
        }

        private Task<Proyecto[]> GetProyectosByUser()
        {
            return Task.Run(() =>
            {
                SystemUser user = (SystemUser)this.Context.CurrentUser;
                Proyecto[] proyectos = DAL.Instance.GetProjectsByUser((Entidades.Usuario)user);
                return proyectos;
            });
        }
    }
}