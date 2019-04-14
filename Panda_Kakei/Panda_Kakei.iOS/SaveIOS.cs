using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;


using Xamarin.Forms;
using Panda_Kakei.iOS;
using UIKit;
using QuickLook;

[assembly: Dependency(typeof(SaveIOS))]

class SaveIOS : ISave
{
    public Task CopyFilePickSrc(string srcFilename, string dstFilename)
    {
        throw new NotImplementedException();
    }

    public Task CopyFilePickDest(string srcFilename, string dstFilename)
    {
        throw new NotImplementedException();
    }


    //Method to save document as a file and view the saved document
    public async Task SaveAndView(string filename, string contentType, MemoryStream stream)
    {
        //Get the root path in iOS device.
        string path = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
        string filePath = Path.Combine(path, filename);

        //Create a file and write the stream into it.
        FileStream fileStream = File.Open(filePath, FileMode.Create);
        stream.Position = 0;
        stream.CopyTo(fileStream);
        fileStream.Flush();
        fileStream.Close();

        //Invoke the saved document for viewing
        UIViewController currentController = UIApplication.SharedApplication.KeyWindow.RootViewController;
        while (currentController.PresentedViewController != null)
            currentController = currentController.PresentedViewController;
        UIView currentView = currentController.View;

        QLPreviewController qlPreview = new QLPreviewController();
        QLPreviewItem item = new QLPreviewItemBundle(filename, filePath);
        qlPreview.DataSource = new PreviewControllerDS(item);

        currentController.PresentViewController(qlPreview, true, null);
    }
}
