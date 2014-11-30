using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chupachupa_DesktopApp.PrivateService;

namespace Chupachupa_DesktopApp.Tools
{
    public class ServeurSingleton
   {
        private static ServiceContractClient _serveur = null;

        private ServeurSingleton() {}

        public static ServiceContractClient GetInstance()
        {
            if (_serveur == null)
                _serveur =  new ServiceContractClient();
            return _serveur;
        }
    }
}
