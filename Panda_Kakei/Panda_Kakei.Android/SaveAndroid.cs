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
            Android.Net.Uri fileUri;
            if (Android.OS.Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.N)
            {
                fileUri = FileProvider.GetUriForFile(Android.App.Application.Context, "com.companyname.Panda_Kakei.fileprovider", file);
            }
            else
            {
                fileUri = Android.Net.Uri.FromFile(file);
            }
            string extension = Android.Webkit.MimeTypeMap.GetFileExtensionFromUrl(fileUri.ToString());
            string mimeType = Android.Webkit.MimeTypeMap.Singleton.GetMimeTypeFromExtension(extension);
            Intent actionIntent = new Intent(Intent.ActionView);
            actionIntent.SetDataAndType(fileUri, mimeType);
            actionIntent.AddFlags(ActivityFlags.GrantReadUriPermission | ActivityFlags.GrantWriteUriPermission
                            | ActivityFlags.NewTask | ActivityFlags.MultipleTask);

            Intent chooserIntent = Intent.CreateChooser(actionIntent, "Choose App");
            chooserIntent.AddFlags(ActivityFlags.NewTask | ActivityFlags.MultipleTask);

            Android.App.Application.Context.StartActivity(chooserIntent);
        }
    }
}
