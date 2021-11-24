using Passwork;
using System;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;

namespace Sample
{

    class Program
    {
        static async Task Main(string[] args)
        {

            //Lets get the needed paramaters for the Passwork client to work.
            var settings = GetSettings();
            if (settings == null) throw new Exception("Could not find the required settings");
         
            
            //Create a Passwork.Client 
            var client = new Client(settings.BaseURL);

            //And use the client to get our context 
            var ctx = await client.LoginAsync(settings.ApiKey, settings.MasterPassword);

            //This demo will 
            //1. Create two vaults, each with 2 folders, one personal , one organization.
            //2. Create passwords in the vault as well in the two folders.
            //3. Run some test.
            //4. Removes all items (passwords, folders and the vault)

            //To prevent naming conflicts, all items will have an unique timestamp mark.
            var run = $"{DateTime.Now.Ticks}";
            var orgvaultName = $"Vault-Organization-{run}";
            var perVaultName = $"Vault-Personal-{run}";


            var orgvault = await ctx.AddVault(orgvaultName, false);

            //Test the creation of the organizational vault.
            ShouldBeTrue(orgvault != null, "Failed to create organizational vault.");
            ShouldBeTrue(orgvault.Name == orgvaultName, "Organizational vault incorrect name");

            //Check if we can found our new vault by name.
            var testOrgVault = await ctx.GetVaultByName(orgvaultName);
            ShouldBeTrue(testOrgVault != null, "Could not find org vault.");

            //Check if we can found our new vault by id.
            testOrgVault = await ctx.GetVaultByID(orgvault.Id);
            ShouldBeTrue(testOrgVault != null, "Could not find org vault by id.");


            var result = await orgvault.Delete();
            ShouldBeTrue(result, "Could not delete org vault");


            return;
        }


        private static void ShouldBeTrue(bool condition, string message)
        {
            if (!condition) throw new Exception(message);
        }

        private static PassworkSettings GetSettings()
        {
            //Let us setup a configuration reader. 
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true)
                .AddJsonFile("appsettings.development.json", optional: true);

            IConfiguration config = builder.Build();

            //Read the passwork settings from one of the json files
            return config.GetSection("PassworkSettings").Get<PassworkSettings>();
        }

    }
}
