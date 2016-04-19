using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.Azure.KeyVault;

using Microsoft.Azure;

namespace CawaConfigurationManager
{
    public class GetConnectionString
    {

        #region public
        //GetSetting
        public static string GetValue(string Key)
        {

            return CloudConfigurationManager.GetSetting(Key).ToString();
           
        }

        //GetSettingFromKeyVault
        public static string GetValueFomKeyVault(string Key)
        {
            var kv = new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(GetAccessToken));
            string keyVaultURL = GetValue(Key);
            var connStr = (kv.GetSecretAsync(keyVaultURL)).GetAwaiter().GetResult().Value;

            string s = connStr.ToString();

            return s;
        }

        #endregion

        #region private
        //Get Aacess token for Keyvault client to use
        private static async Task<string> GetAccessToken(string authority, string resource, string scope)
        {

            var authenticationContext = new AuthenticationContext(authority);
            var credential = GetCredential();
            AuthenticationResult result = await authenticationContext.AcquireTokenAsync(resource, credential);

            if (result == null)
            {
                throw new InvalidOperationException("Failed to obtain the JWT token");
            }

            string token = result.AccessToken;

            return token;
        }

        //Get the Service Principle credential for getting the access token
        private static ClientCredential GetCredential()
        {
            string filePath = "c:\\Users\\vmuser\\azure\\profiles\\default.profile";
            ClientCredential creds;
            //obtain credential from default location - dev machine
            if (File.Exists(filePath))
            {
                string[] secrets = GetCredentialFromProfile(filePath);
                creds = new ClientCredential(secrets[0], secrets[1]);
            }
            else if (true) //todo: change to check the Azure environment this app is running in
            {
                //obtain credential from custom data settings - App Services, Cloud Services 
               var clientID = CloudConfigurationManager.GetSetting("ClientID");
                var clientSecret = CloudConfigurationManager.GetSetting("ClientSecret");
                creds = new ClientCredential(clientID, clientSecret);
            }
            else if (false)
            {
                //TODO: obtain credential from instance metadata -VM, VMSS
            }
            else
                creds = null;

            return creds;
        }

        private static string[] GetCredentialFromProfile(string filePath)
        {
            string line;

            string clientID = "";
            string clientSecret = "";

            System.IO.StreamReader file = new System.IO.StreamReader(filePath);

            while ((line = file.ReadLine()) != null)
            {
                if (line.Contains("clientId="))
                    clientID = line.Replace("clientId=", "");
                if (line.Contains("clientSecret="))
                    clientSecret = line.Replace("clientSecret=", "");
            }

            file.Close();

            return new string[] { clientID, clientSecret };

        }

        #endregion
    }
}
