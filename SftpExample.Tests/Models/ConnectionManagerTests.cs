using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SftpExample.Models;

namespace SftpExample.Tests.Models
{
    [TestClass()]
    public class ConnectionManagerTests
    {
        [TestMethod()]
        public void ConnectTest()
        {
            var cnManager = new ConnectionManager();
            bool connected = cnManager.Connect();
            Assert.IsTrue(connected);
        }

        [TestMethod()]
        public void GetFilesTest()
        {
            var cnManager = new ConnectionManager();
            bool connected = cnManager.Connect();
            Assert.IsTrue(connected);

            var files = cnManager.GetFiles();
            Assert.IsNotNull(files);
        }

        [TestMethod()]
        public async Task DownloadFileAsyncTest()
        {
            var cnManager = new ConnectionManager();
            bool connected = cnManager.Connect();
            Assert.IsTrue(connected);

            var files = cnManager.GetFiles();
            Assert.IsNotNull(files);

            var tempFolder = Path.GetTempPath();
            var file = files.First(i => !i.IsDirectory);
            file.Selected = true;
            await cnManager.DownloadAsync(files, tempFolder);

            var fileName = Path.Combine(tempFolder, file.Name);
            
            if (File.Exists(fileName))
            {
                File.Delete(fileName);
            }
            else
            {
                Assert.Fail("File hasn't downloaded");
            }
        }

        [TestMethod()]
        public async Task DownloadFolderAsyncTest()
        {
            var cnManager = new ConnectionManager();
            bool connected = cnManager.Connect();
            Assert.IsTrue(connected);

            var files = cnManager.GetFiles();
            Assert.IsNotNull(files);

            var tempFolder = Path.GetTempPath();
            var folder = files.First(i => i.IsDirectory && (!i.Name.StartsWith(".")));
            folder.Selected = true;
            await cnManager.DownloadAsync(files, tempFolder);

            var folderName = Path.Combine(tempFolder, folder.Name);

            if (Directory.Exists(folderName))
            {
                Directory.Delete(folderName, true);
            }
            else
            {
                Assert.Fail("Directory hasn't downloaded");
            }
        }

        [TestMethod()]
        public void DisposeTest()
        {
            var cnManager = new ConnectionManager();
            bool connected = cnManager.Connect();
            Assert.IsTrue(connected);

            try
            {
                cnManager.Dispose();
            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }
        }
    }
}