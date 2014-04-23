namespace ControlActividades
{
    using Aegis;
    using ControlActividades.Security;
    using Nancy;
    using Nancy.Authentication.Stateless;
    using System.Threading.Tasks;
    using System.Web;

    public class Bootstrapper : DefaultNancyBootstrapper
    {
        // The bootstrapper enables you to reconfigure the composition of the framework,
        // by overriding the various methods and properties.

        // For more information https://github.com/NancyFx/Nancy/wiki/Bootstrapper

        protected override void ApplicationStartup(Nancy.TinyIoc.TinyIoCContainer container, Nancy.Bootstrapper.IPipelines pipelines)
        {
            base.ApplicationStartup(container, pipelines);
            var configuration =
             new StatelessAuthenticationConfiguration(ctx =>
             {
                 var nameParts = System.Web.HttpContext.Current.Request.LogonUserIdentity.Name.Split(new char[] {'\\'});
                 var username=nameParts[nameParts.Length - 1];
                 if (UsersVault.Instance.LoggedUser(username))
                 {
                     //already logged
                     return UsersVault.Instance.GetUserFromCache(username);
                 }
                 else
                 {
                     var User = AegisClient.AutenticarUsuarioActiveDirectory("ADMON\\" + username);
                     if (User != null && User.Autenticado)
                     {
                         var loggedUser = new SystemUser(User);
                         Task.Run(() => { 
                            UsersVault.Instance.AddUserToCache(loggedUser);
                         });
                         return loggedUser;
                     }
                     else
                     {
                         return null;
                     }
                 }
             });
            StatelessAuthentication.Enable(pipelines, configuration);
        }

    }
}