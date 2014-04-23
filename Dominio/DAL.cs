using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NHibernate;
using FluentNHibernate.Cfg;
using NHibernate.Cfg;
using NHibernate.Tool.hbm2ddl;
using NHibernate.Linq;
using NHibernate.Criterion;
using FluentNHibernate.Cfg.Db;
using Entidades;
namespace Dominio
{
    public sealed partial class DAL
    {
        private static readonly Lazy<DAL> instance = new Lazy<DAL>(() => new DAL());

        ISessionFactory currentSession;
        private DAL()
        {
            this.currentSession = Fluently.Configure().Database(MsSqlConfiguration.MsSql2008.ConnectionString(C => C.FromConnectionStringWithKey("Conexion")))
                .Mappings(M => M.FluentMappings.AddFromAssemblyOf<DAL>())
                .ExposeConfiguration(cfg =>
                {
                    BuildSchema(cfg);
                })
                .BuildSessionFactory();
        }

        private void BuildSchema(Configuration cfg)
        {
            new SchemaUpdate(cfg).Execute(false, true);
        }

        public static DAL Instance { get { return instance.Value; } }



        public Proyecto[] GetProjectsByUser(Usuario user)
        {
            Proyecto[] result = new Proyecto[] { };
             using (var sesion = this.currentSession.OpenSession())
             {
                 using (var trans = sesion.BeginTransaction())
                 {
                     result = sesion.Query<Proyecto>().Where(x => x.Usuario.UserId == user.UserId).ToArray();
                 }
             }
             foreach (var proy in result)
             {
                 proy.IdEstado = proy.Estado==null?1:proy.Estado.Id;
                 proy.IdResponsable = proy.Responsable == null ? 1 : proy.Responsable.InternalId;
                 proy.IdTipoProyecto = proy.Tipo == null ? 1 : proy.Tipo.Id;
             }
             return result;
        }

        public Proyecto CreateProyect(Proyecto project,Usuario user)
        {
            Proyecto result = new Proyecto();
            using (var sesion = this.currentSession.OpenSession())
            {
                using (var trans = sesion.BeginTransaction())
                {
                    try
                    {
                        Usuario currentUser = sesion.Query<Usuario>().Where(x => x.UserId == user.UserId).FirstOrDefault();
                        if (currentUser==null)
                        {
                            currentUser = new Usuario(user);
                            sesion.Save(currentUser);
                            project.Usuario = user;
                        }
                        project.Estado = sesion.Get<Estado>(project.IdEstado);
                        project.Tipo = sesion.Get<TipoProyecto>(project.IdTipoProyecto);
                        project.Responsable = sesion.Get<Usuario>(project.IdResponsable);
                        project.Usuario = currentUser;
                        sesion.Save(project);
                        trans.Commit();
                        result = project;
                    }
                    catch(Exception ex)
                    {
                        trans.Rollback();
                    }
                }
            }

            return result;
        }

        public Actividad[] GetActividadesByUser(DateTime startDate, DateTime endDate, Usuario user)
        {
            Actividad[] result = new Actividad[] { };
            using (var sesion = this.currentSession.OpenSession())
            {
                using (var trans = sesion.BeginTransaction())
                {
                    result = sesion.Query<Actividad>().Where(x => x.Usuario.UserId == user.UserId && x.Fecha>=startDate && x.Fecha<=endDate).ToArray();
                }
            }
            return result;

        }

        public Actividad CreateActividad(int IdProject, DateTime date, Usuario user)
        {
            Actividad result = new Actividad();
            using (var sesion = this.currentSession.OpenSession())
            {
                using (var trans = sesion.BeginTransaction())
                {
                    try
                    {
                        Usuario currentUser = sesion.Query<Usuario>().Where(x => x.UserId == user.UserId).FirstOrDefault();
                        Proyecto currentProy = sesion.Query<Proyecto>().Where(x => x.Id == IdProject).FirstOrDefault();
                        if (currentUser != null && currentProy!=null)
                        {
                            result.Fecha = date;
                            result.Proyecto = currentProy;
                            result.Usuario = currentUser;
                            sesion.Save(result);
                        }
                        trans.Commit();
                    }
                    catch (Exception ex)
                    {
                        trans.Rollback();
                    }
                }
            }

            return result;
        }

        public void DeleteActividad(int IdActividad)
        {
            using (var sesion = this.currentSession.OpenSession())
            {
                using (var trans = sesion.BeginTransaction())
                {
                    try
                    {
                        var query=sesion.CreateQuery("delete from Actividad where Id=:id").SetParameter("id",IdActividad);
                        query.ExecuteUpdate();
                        trans.Commit();
                    }
                    catch (Exception ex)
                    {

                    }
                }
            }
        }

        public TipoProyecto[] GetTiposProyectos()
        {
            TipoProyecto[] types = new TipoProyecto[] { };
            using (var sesion = this.currentSession.OpenSession())
            {
                using (var trans = sesion.BeginTransaction())
                {
                    types=sesion.Query<TipoProyecto>().ToArray();
                }
            }
            return types;
        }

        public Estado[] GetEstados()
        {
            Estado[] states = new Estado[] { };
            using (var sesion = this.currentSession.OpenSession())
            {
                using (var trans = sesion.BeginTransaction())
                {
                    states = sesion.Query<Estado>().ToArray();
                }
            }
            return states;
        }

        public Usuario[] GetUsers()
        {
            Usuario[] users = new Usuario[] { };
            using (var sesion = this.currentSession.OpenSession())
            {
                using (var trans = sesion.BeginTransaction())
                {
                    users = sesion.Query<Usuario>().OrderBy(x=>x.FullName).ToArray();
                }
            }
            return users;
        }

        public Proyecto UpdateProject(Proyecto project, Usuario user)
        {
            Proyecto result = new Proyecto();
            using (var sesion = this.currentSession.OpenSession())
            {
                using (var trans = sesion.BeginTransaction())
                {
                    try
                    {
                        Usuario currentUser = sesion.Query<Usuario>().Where(x => x.UserId == user.UserId).FirstOrDefault();
                        if (currentUser == null)
                        {
                            currentUser = new Usuario(user);
                            sesion.Save(currentUser);
                            project.Usuario = user;
                        }
                        project.Usuario = currentUser;
                        var oldProject=sesion.Query<Proyecto>().Where(x => x.Id == project.Id).FirstOrDefault();
                        oldProject.Nombre = project.Nombre;
                        oldProject.Responsable = sesion.Query<Usuario>().Where(x => x.InternalId == project.IdResponsable).FirstOrDefault();
                        oldProject.Estado = sesion.Query<Estado>().Where(x => x.Id == project.IdEstado).FirstOrDefault();
                        oldProject.Tipo = sesion.Query<TipoProyecto>().Where(x => x.Id == project.IdTipoProyecto).FirstOrDefault();
                        sesion.Update(oldProject);
                        trans.Commit();
                        result = oldProject;
                    }
                    catch (Exception ex)
                    {
                        trans.Rollback();
                    }
                }
            }

            return result;
        }

        public bool DeleteProject(int IdProyecto, Usuario usuario)
        {
            bool result = false;
            using (var sesion = this.currentSession.OpenSession())
            {
                using (var trans = sesion.BeginTransaction())
                {
                    try
                    {
                        var query = sesion.CreateQuery("delete from Proyecto where Id=:id")
                            .SetParameter("id", IdProyecto);
                            
                        result=query.ExecuteUpdate() > 0;
                        trans.Commit();
                    }
                    catch (Exception ex)
                    {
                        result = false;
                    }
                }
            }
            return result;
            
        }

        public  void AddUser(Usuario user)
        {
            using (var sesion = this.currentSession.OpenSession())
            {
                using (var trans = sesion.BeginTransaction())
                {
                    Usuario currentUser = sesion.Query<Usuario>().Where(x => x.UserId == user.UserId).FirstOrDefault();
                    if (currentUser == null)
                    {
                        currentUser = new Usuario(user);
                        try
                        {
                            sesion.Save(currentUser);
                            trans.Commit();
                        }
                        catch (Exception ex)
                        {

                        }
                    }
                }
            }
        }

        public void UpdateComment(int activityId, string comment)
        {
            using (var sesion = this.currentSession.OpenSession())
            {
                using (var trans = sesion.BeginTransaction())
                {
                    var query=sesion.CreateQuery("update from Actividad set Comentario=:comment where Id=:id")
                        .SetParameter("id", activityId)
                        .SetParameter("comment", comment);
                    try
                    {
                        query.ExecuteUpdate();
                        trans.Commit();
                    }
                    catch (Exception ex1)
                    {

                    }
                }
            }
        }
    }
}
