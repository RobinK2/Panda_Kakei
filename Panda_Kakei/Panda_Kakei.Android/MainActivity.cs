﻿using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using System.IO;
using Plugin.Permissions;
using Plugin.CurrentActivity;

namespace Panda_Kakei.Droid
{
    [Activity(Label = "@string/app_name", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.Locale | ConfigChanges.ScreenSize | ConfigChanges.Orientation, ScreenOrientation = ScreenOrientation.Portrait)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        //static readonly int READ_WRITE_EXTERNAL_REQUEST_ID = 0;
        //static readonly int READ_EXTERNAL_REQUEST_ID = 1;

        protected override void OnCreate(Bundle bundle)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(bundle);
            CrossCurrentActivity.Current.Init(this, bundle);
            global::Xamarin.Forms.Forms.Init(this, bundle);

            string root = null;
            if (Android.OS.Environment.IsExternalStorageEmulated)
            {
                root = Android.OS.Environment.ExternalStorageDirectory.ToString();
            }
            else
            {
                root = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments);
            }

            //Create directory and file 
            //Java.IO.File myDir = new Java.IO.File(root + "/" + Resources.GetText(Resource.String.app_name));
            Java.IO.File myDir = new Java.IO.File(root + "/PandaHousehold");
            myDir.Mkdir();
            //string extStoragePath = root + "/Panda_Kakei";
            //ExternalDatabasePath = System.IO.Path.Combine(extStoragePath, "kakei.db3");
            ExternalDatabasePath = System.IO.Path.Combine(myDir.AbsolutePath, "kakei.db3");

            //string extStoragePath = GetExternalFilesDir(null).AbsolutePath;
            //ExternalDatabasePath = System.IO.Path.Combine(extStoragePath, "kakei.db3");
            string dbPath = FileAccessHelper.GetLocalFilePath("kakei.db3");
            //string excelPath = extStoragePath;
            string excelPath = myDir.AbsolutePath;
            LoadApplication(new Panda_Kakei.App(dbPath, ExternalDatabasePath, excelPath));
        }

        // Need this override to make CrossPermission library functional.
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            PermissionsImplementation.Current.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        public static string ExternalDatabasePath { get; set; }

        public static void CopyDatabase(string srcPath, string destPath)
        {
            var bytes = System.IO.File.ReadAllBytes(srcPath);
            System.IO.File.WriteAllBytes(destPath, bytes);
        }
    }
}
