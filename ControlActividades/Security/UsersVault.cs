using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Collections.Concurrent;
using Dominio;
namespace ControlActividades.Security
{
    public sealed class UsersVault
    {
        ConcurrentDictionary<string, SystemUser> cachedUsers;
        private static readonly Lazy<UsersVault> instance = new Lazy<UsersVault>(() => new UsersVault());
        private UsersVault()
        {
            cachedUsers = new ConcurrentDictionary<string, SystemUser>();
        }

        public static UsersVault Instance { get { return instance.Value; } }

        public bool LoggedUser(string username)
        {
            return cachedUsers.ContainsKey(username.ToLower());
        }
        public SystemUser GetUserFromCache(string username)
        {
            SystemUser result=null;
            bool exist=cachedUsers.TryGetValue(username.ToLower(), out result);

            return exist ? result : null;
        }

        public void AddUserToCache(SystemUser user)
        {
            cachedUsers.AddOrUpdate(user.UserName.ToLower(), user, 
                (oldUsername,oldUser) => {
                return new SystemUser();
            });
            DAL.Instance.AddUser((Entidades.Usuario)user);
        }
    }
}