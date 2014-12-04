using System;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using Chupachupa_DesktopApp_Test.PrivateService;
using Chupachupa_DesktopApp_Test.PublicService;
using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace Chupachupa_DesktopApp_Test
{
    [TestClass]
    class PublicServiceTest
    {
        private ServiceContractClient _serveur = new ServiceContractClient();
        private PublicServiceContractClient _pubServeur = new PublicServiceContractClient();

        [TestMethod]
        public void CreateAccount_Success()
        {
            var rd = new Random();

            var randomValue = rd.Next(0, 470);
            string username = "UnitTest_user_" + randomValue;
            string password = "UnitTest_pass_" + randomValue;
            _pubServeur.createAccount(username, password);

            _serveur.ClientCredentials.UserName.UserName = username;
            _serveur.ClientCredentials.UserName.Password = password;
            _serveur.authenticate(username, password);

            Assert.IsNotNull(_serveur.ClientCredentials);
        }

    }
}
