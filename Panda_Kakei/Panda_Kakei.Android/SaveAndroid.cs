using System;
using System.IO;
using Panda_Kakei.Droid;
using Android.Content;
using Java.IO;
using Xamarin.Forms;
using System.Threading.Tasks;
using Android.Support.V4.Content;
using Android.App;

[assembly: Dependency(typeof(SaveAndroid))]

class SaveAndroid : ISave
{
    public Task CopyFilePickSrc(string srcFilename, string dstFilename)
    {
        throw new NotImplementedException();
    }

    public Task<string> CopyFilePickDest(string srcFilename, string dstFilename)
    {
        throw new NotImplementedException();
    }


    //Method to save document as a file in Android and view the saved document
    public async Task SaveAndView(string fileName, String contentType, MemoryStream stream)
    {
        //string root = null;
        ////Get the root path in android device.
        //if (Android.OS.Environment.IsExternalStorageEmulated)
        //{
        //    root = Android.OS.Environment.ExternalStorageDirectory.ToString();
        //}
        //else
        //{
        //    root = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        //}

        ////Create directory and file 
        //Java.IO.File myDir = new Java.IO.File(root + "/Panda_Kakei");
        //myDir.Mkdir();

        Java.IO.File file = new Java.IO.File(fileName);

        //Remove if the file exists
        if (file.Exists()) file.Delete();

        //Write the stream into the file
        FileOutputStream outs = new FileOutputStream(file);
        outs.Write(stream.ToArray());

        outs.Flush();
        outs.Close();

        //Invoke the created file for viewing
        if (file.Exists())
        {
            Android.Net.Uri path = Android.Net.Uri.FromFile(file);
            //Android.Net.Uri path = FileProvider.GetUriForFile(((Activity)Forms.Context), "com.companyname.Panda_Kakei.fileprovider", file);
            string extension = Android.Webkit.MimeTypeMap.GetFileExtensionFromUrl(Android.Net.Uri.FromFile(file).ToString());
            string mimeType = Android.Webkit.MimeTypeMap.Singleton.GetMimeTypeFromExtension(extension);
            Intent intent = new Intent(Intent.ActionView);
            intent.SetDataAndType(path, mimeType);

            //Forms.Context.StartActivity(Intent.CreateChooser(intent, "Choose App"));
            Android.App.Application.Context.StartActivity(Intent.CreateChooser(intent, "Choose App"));
        }
    }
}
