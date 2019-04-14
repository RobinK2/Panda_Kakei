#region Copyright Syncfusion Inc. 2001-2016.
// Copyright Syncfusion Inc. 2001-2016. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Windows.Storage;
using Windows.Storage.Pickers;
using Xamarin.Forms.Platform.UWP;

[assembly: Dependency(typeof(SaveWindows))]

class SaveWindows : ISave
{
    public async Task<string> CopyFilePickDest(string srcFilename, string dstFilename)
    {  
        //save the stream into the file. 
        if (Device.Idiom != TargetIdiom.Desktop)
        {
            StorageFolder local = Windows.Storage.ApplicationData.Current.LocalFolder;
            StorageFile outFile = await local.CreateFileAsync(dstFilename, CreationCollisionOption.ReplaceExisting);

            using (FileStream fileStream = File.OpenRead(srcFilename))
            using (Stream outStream = await outFile.OpenStreamForWriteAsync())
            {
                MemoryStream memStream = new MemoryStream();
                memStream.SetLength(fileStream.Length);
                fileStream.Read(memStream.GetBuffer(), 0, (int)fileStream.Length);
                outStream.Write(memStream.ToArray(), 0, (int)memStream.Length);
            }
            return outFile.Path;
        }
        else
        {
            StorageFile storageFile = null;
            FileSavePicker savePicker = new FileSavePicker();
            savePicker.SuggestedStartLocation = PickerLocationId.Desktop;
            savePicker.SuggestedFileName = dstFilename;
            savePicker.FileTypeChoices.Add(".db3 file", new List<string>() { ".db3" });

            storageFile = await savePicker.PickSaveFileAsync();

            using (FileStream fileStream = File.OpenRead(srcFilename))
            using (Stream outStream = await storageFile.OpenStreamForWriteAsync())
            {
                MemoryStream memStream = new MemoryStream();
                memStream.SetLength(fileStream.Length);
                fileStream.Read(memStream.GetBuffer(), 0, (int)fileStream.Length);
                outStream.Write(memStream.ToArray(), 0, (int)memStream.Length);
            }
            return storageFile.Path;
        }
    }

    public async Task CopyFilePickSrc(string srcFilename, string dstFilename)
    {
        //save the stream into the file. 
        if (Device.Idiom != TargetIdiom.Desktop)
        {
            StorageFolder local = Windows.Storage.ApplicationData.Current.LocalFolder;
            StorageFile outFile = await local.CreateFileAsync(dstFilename, CreationCollisionOption.ReplaceExisting);

            using (FileStream fileStream = File.OpenRead(srcFilename))
            using (Stream outStream = await outFile.OpenStreamForWriteAsync())
            {
                MemoryStream memStream = new MemoryStream();
                memStream.SetLength(fileStream.Length);
                fileStream.Read(memStream.GetBuffer(), 0, (int)fileStream.Length);
                outStream.Write(memStream.ToArray(), 0, (int)memStream.Length);
            }
        }
        else
        {
            StorageFile storageSrcFile = null;
            FileOpenPicker filePicker = new FileOpenPicker();
            filePicker.SuggestedStartLocation = PickerLocationId.Desktop;
            filePicker.FileTypeFilter.Clear();
            filePicker.FileTypeFilter.Add(".db3");
            //filePicker.SuggestedFileName = srcFilename;
            //filePicker.FileTypeChoices.Add(".db3 file", new List<string>() { ".db3" });

            storageSrcFile = await filePicker.PickSingleFileAsync();

            //File.Copy(storageSrcFile.Path, dstFilename);

            using (Stream inStream = await storageSrcFile.OpenStreamForReadAsync())
            //using (FileStream srcFileStream = File.OpenRead(storageSrcFile.Path))
            using (FileStream dstFileStream = File.OpenWrite(dstFilename))
            {
                MemoryStream memStream = new MemoryStream();
                memStream.SetLength(inStream.Length);
                inStream.Read(memStream.GetBuffer(), 0, (int)inStream.Length);
                dstFileStream.Write(memStream.ToArray(), 0, (int)memStream.Length);
            }
        }
    }

    public async Task SaveAndView(string filename, string contentType, MemoryStream stream)
    {
        //save the stream into the file. 
        if (Device.Idiom != TargetIdiom.Desktop)
        {
            StorageFolder local = Windows.Storage.ApplicationData.Current.LocalFolder;
            StorageFile outFile = await local.CreateFileAsync(filename, CreationCollisionOption.ReplaceExisting);
            using (Stream outStream = await outFile.OpenStreamForWriteAsync())
            {
                outStream.SetLength(0);
                outStream.Write(stream.ToArray(), 0, (int)stream.Length);
            }
            if (contentType != "application/html")
                await Windows.System.Launcher.LaunchFileAsync(outFile);
        }
        else
        {
            StorageFile storageFile = null;
            FileSavePicker savePicker = new FileSavePicker();
            savePicker.SuggestedStartLocation = PickerLocationId.Desktop;
            savePicker.SuggestedFileName = filename;
            switch (contentType)
            {
                case "application/vnd.openxmlformats-officedocument.presentationml.presentation":
                    savePicker.FileTypeChoices.Add("PowerPoint Presentation", new List<string>() { ".pptx", });
                    break;

                case "application/msexcel":
                    savePicker.FileTypeChoices.Add("Excel Files", new List<string>() { ".xlsx", });
                    break;

                case "application/msword":
                    savePicker.FileTypeChoices.Add("Word Document", new List<string>() { ".docx" });
                    break;

                case "application/pdf":
                    savePicker.FileTypeChoices.Add("Adobe PDF Document", new List<string>() { ".pdf" });
                    break;
                case "application/html":
                    savePicker.FileTypeChoices.Add("HTML Files", new List<string>() { ".html" });
                    break;
            }
            storageFile = await savePicker.PickSaveFileAsync();

            using (Stream outStream = await storageFile.OpenStreamForWriteAsync())
            {
                outStream.SetLength(0);
                outStream.Write(stream.ToArray(), 0, (int)stream.Length);
            }

            //Invoke the saved file for Viewing.
            await Windows.System.Launcher.LaunchFileAsync(storageFile);
        }
    }
}
