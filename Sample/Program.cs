using Passwork;
using System;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;
using System.Linq;

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
            //1. Create an org vault with a folder
            //2. Create passwords in the folder.
            //3. Run some test.
            //4. Removes all items (passwords, folders and the vault)

            //To prevent naming conflicts, all items will have an unique timestamp mark.
            var run = $"{DateTime.Now.Ticks}";
            var orgvaultName = $"Vault-Organization-{run}";

            
            var vault = await ctx.AddVault(orgvaultName, false);

            //Test the creation of the organizational vault.
            ShouldBeTrue(vault != null, "Failed to create organizational vault.");
            ShouldBeTrue(vault.Name == orgvaultName, "Organizational vault incorrect name");

            //Check if we can found our new vault by name.
            var testVault = await ctx.GetVaultByName(orgvaultName);
            ShouldBeTrue(testVault != null, "Could not find org vault.");

            //Check if we can found our new vault by id.
            testVault = await ctx.GetVaultByID(vault.Id);
            ShouldBeTrue(testVault != null, "Could not find org vault by id.");

            var folder= await testVault.AddFolder("testfolder");

            //test to refind folder by name , getfolders and by id
            var testFolder = await vault.GetFolderByName("testfolder");
            ShouldBeTrue(testFolder != null, "Could not find just created test folder");
            ShouldBeTrue(testFolder.Id == folder.Id, "Found wrong folder");

            var folders = await vault.GetFolders();
            ShouldBeTrue(folders.Length == 1, "Expected only one folder in the vault");
            testFolder = folders[0];
            ShouldBeTrue(testFolder.Id == folder.Id, "Found wrong folder");


            //Create new password in folder
            var np = folder.CreatePassword();
            np.Name = "sample";
            np.Pass = "Test1234";
            np.Color = 1;
            np.Tags = new string[] { $"test-tag-{run}" };
            var p = await np.Save();

            ShouldBeTrue(p != null, "Failed to create password");
            
            //Change the newly created password
            p.CustomRecords.Add(new CustomField("nameoffield", "valueoffield", CustomFieldType.text));
            p = await p.Save();
            
            await p.AddAttachment("hello.txt", System.Text.Encoding.UTF8.GetBytes("Hello world!"));

            //Lets refind our password.
            var ps = await folder.GetPasswords();
            //should be exacly 1 password inside this folder
            ShouldBeTrue(ps.Length == 1, "Expected 1 password in this folder");
            var checkPass = ps[0];

            //shoudl have the correct password
            await checkPass.Unlock();
            ShouldBeTrue(checkPass.Pass == "Test1234", "Password mismatch");

            //shoudl contain 1 custom field.
            ShouldBeTrue(checkPass.CustomRecords.Count == 1, "Password should have 1 custom record");

            //custom field should be correct
            var cr = checkPass.CustomRecords[0];
            ShouldBeTrue(cr.Name == "nameoffield", "custom record has wrong name");
            ShouldBeTrue(cr.Value == "valueoffield", "custom record has wrong value.");

            //attachment should exists
            var atts = await checkPass.GetAttachments();
            ShouldBeTrue(atts.Length == 1, "Expected exaclty one attachment");
            var att = atts[0];
            ShouldBeTrue(att.Name == "hello.txt" ,"Expected the file hello.txt");
            var content = await att.GetContent();
            var contentAsString = System.Text.Encoding.UTF8.GetString(content);
            ShouldBeTrue(contentAsString == "Hello world!", "Wrong content in attachment.");


            //test query capabilities
            var q1 = await vault.Query()
                        .WhereTag($"test-tag-{run}")
                        .Get();
            ShouldBeTrue(q1.Length >= 1, "Could not find our password based on tag.");


            //delete attachment
            var result = await att.Delete();
            ShouldBeTrue(result, "Could not delete attachment");

            //make sure it is deleted
            atts = await checkPass.GetAttachments();
            ShouldBeTrue(atts.Length == 0, "Expected zero attachments");

            //delete pass
            result = await p.Delete();
            ShouldBeTrue(result, "Could not delete password");

            //make sure password is deleted from folder
            ps = await folder.GetPasswords();
            ShouldBeTrue(ps.Length==0, "Expect zero passwords in the folder");

            //delete folder
            result = await folder.Delete();
            ShouldBeTrue(result, "Could not delete folder");

            //make sure it is deleted
            folders = await vault.GetFolders();
            ShouldBeTrue(folders.Length==0, "Expect zero folders in the vault");

            result = await vault.Delete();
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
