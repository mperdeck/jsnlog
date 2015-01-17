using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using Microsoft.VisualBasic.FileIO;
using System.Collections.ObjectModel;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Text;
using System.Diagnostics;

namespace JSNLog.Tests.IntegrationTests
{
    [TestClass]
    public class NuGetTests
    {
        // Directories below are relative to solution directory
        private readonly string DirEmptyTestSites = "EmptyTestSites";
        private readonly string DirTestSiteSlot = "TestSiteSlot";
        private readonly string DirGeneratedPackages = @"JSNLog\NuGet\GeneratedPackages";
        private readonly string ScriptImportPackageAndF5 = @"JSNLog.Tests\PowerShellScripts\ImportPackageAndF5.ps1";

        [TestInitialize()]
        public void MyTestInitialize()
        {
        }

        // Use TestCleanup to run code after each test has run
        [TestCleanup()]
        public void MyTestCleanup()
        {
        }

        [TestCategory("NuGetTest"), TestMethod]
        [Ignore]
        public void zNuGetTestCommon()
        {
            RunTest("EmptyTestSiteLog4Net");
        }

        private void RunTest(string testSite)
        {
            // Create NuGet package

            string absoluteBatchFilePath = AbsoluteDirPath(@"JSNLog\NuGet\Build\BuildNuGetPackage.bat", "");
            string absoluteBatchFileDir = Path.GetDirectoryName(absoluteBatchFilePath);

            RunBatchFile(absoluteBatchFilePath, absoluteBatchFileDir);

            // -------------------------------------
            // Copy test site

            string importPackagesAndF5Script = AbsoluteDirPath(ScriptImportPackageAndF5);

            string testProjectsDir = AbsoluteDirPath(DirEmptyTestSites);
            string scratchProjectsDir = AbsoluteDirPath(DirTestSiteSlot);
            string projectName = testSite;
            string packageName = "JSNLog";
            string generatedPackagesDir = AbsoluteDirPath(DirGeneratedPackages);

            // --------------------
            // Output handy strings to output window, in case automatic package import fails

            string scratchProjectDir = System.IO.Path.Combine(scratchProjectsDir, projectName);
            string scratchProjectFilePath = System.IO.Path.Combine(scratchProjectDir, projectName + ".csproj");

            Debug.WriteLine(scratchProjectFilePath);

            string installPackage = 
                string.Format("Install-Package {0} -Source {1}", packageName, generatedPackagesDir);

            Debug.WriteLine(installPackage);

            // -------------------------------------
            
            RunPowerShellScript(
                importPackagesAndF5Script,
                new Dictionary<string, string>
                {
                    {"testProjectsDir", testProjectsDir},
                    {"scratchProjectsDir", scratchProjectsDir},
                    {"projectName", projectName},
                    {"packageName", packageName},
                    {"generatedPackagesDir", generatedPackagesDir}
                });
        }

        private static string AbsoluteDirPath(string relativeDir1, string relativeDir2 = "", string relativeDir3 = "")
        {
            string assemblyDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            string[] paths = { assemblyDir, @"..\..", relativeDir1, relativeDir2, relativeDir3 };
            string fullPath = Path.Combine(paths);

            return fullPath;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="batchFileRelativePath">
        /// batch file name relative to solution dir
        /// </param>
        private static void RunBatchFile(string commandLine, string absoluteBatchFileDir = null)
        {
            System.Diagnostics.Process proc = new System.Diagnostics.Process(); // Declare New Process
            proc.StartInfo.FileName = commandLine;
            proc.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            proc.StartInfo.CreateNoWindow = true;
            if (absoluteBatchFileDir != null) { proc.StartInfo.WorkingDirectory = absoluteBatchFileDir; }

            proc.StartInfo.RedirectStandardError = true;
            proc.StartInfo.RedirectStandardOutput = true;
            proc.StartInfo.UseShellExecute = false;

            proc.Start();
            proc.WaitForExit();

            string errorMessage = proc.StandardError.ReadToEnd();
            proc.WaitForExit();

            string outputMessage = proc.StandardOutput.ReadToEnd();
            proc.WaitForExit();
        }

        private static string RunPowerShellScript(string command, Dictionary<string,string> parameters)
        {
            RunspaceConfiguration runspaceConfiguration = RunspaceConfiguration.Create();

            Runspace runspace = RunspaceFactory.CreateRunspace(runspaceConfiguration);
            runspace.Open();

            RunspaceInvoke scriptInvoker = new RunspaceInvoke(runspace);

            Pipeline pipeline = runspace.CreatePipeline();

            //Here's how you add a new script with arguments
            Command myCommand = new Command(command);

            foreach (KeyValuePair<string, string> parameter in parameters)
            {
                CommandParameter param = new CommandParameter(parameter.Key, parameter.Value);
                myCommand.Parameters.Add(param);
            }

            pipeline.Commands.Add(myCommand);

            // Execute PowerShell script
            var results = pipeline.Invoke();

            // close the runspace

            runspace.Close();

            // convert the script result into a single string

            StringBuilder stringBuilder = new StringBuilder();
            foreach (PSObject obj in results)
            {
                stringBuilder.AppendLine(obj.ToString());
            }

            string returnText =  stringBuilder.ToString();
            return returnText;
        }
    }
}

