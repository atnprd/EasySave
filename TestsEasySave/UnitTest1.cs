using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

using EasySave.Model;


namespace TestsEasySave
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
        }

        
        
        [TestMethod]
        public void TestFullSave(string target_folder, string source_folder)
        {
            //string source_folder = "C:\\Users\\smallval\\source\\repos\\EasySave\\TestsEasySave\\Tests";
            //string target_folder = "C:\\Users\\smallval\\source\\repos\\EasySave\\TestsEasySave\\Tests\\mirrortest2";


            DirectoryInfo source_subfolder = new DirectoryInfo(source_folder);
            if (!source_subfolder.Exists)
            {
                source_subfolder.Create();
            }

            DirectoryInfo target_subfolder = new DirectoryInfo(target_folder);
            if (!target_subfolder.Exists)
            {
                target_subfolder.Create();
            }


            //BackupMirror backupMirror = new BackupMirror(_name, _source_folder, _target_folder);
        }
    }
}
