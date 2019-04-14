using Android.App;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Panda_Kakei.Droid
{
    public class FileAccessHelper
    {
        public static string GetLocalFilePath(string filename)
        {
            string folder = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);

            //string folder = "/storage/emulated/0/Android/data/com.companyname.Panda_Kakei/files";

            string dbPath = System.IO.Path.Combine(folder, filename);

            //copyDatabaseIfNotExists(dbPath, filename);

            return dbPath;
        }

        private static void copyDatabaseIfNotExists(string destPath, string filename)
        {
            if (!File.Exists(destPath))
            {
                using (var br = new BinaryReader(Application.Context.Assets.Open(filename)))
                {
                    using (var bw = new BinaryWriter(new FileStream(destPath, FileMode.Create)))
                    {
                        byte[] buffer = new byte[2048];
                        int length = 0;
                        while ((length = br.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            bw.Write(buffer, 0, length);
                        }
                    }
                }
            }
        }
    }
}