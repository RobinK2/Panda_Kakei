using System;

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
    [Activity(Label = "@string/app_name", Icon = "@drawable/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.Locale | ConfigChanges.ScreenSize | ConfigChanges.Orientation, ScreenOrientation = ScreenOrientation.Portrait)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(bundle);
            CrossCurrentActivity.Current.Init(this, bundle);
            global::Xamarin.Forms.Forms.Init(this, bundle);

            string root;
            if (Android.OS.Environment.IsExternalStorageEmulated)
            {
                root = Android.App.Application.Context.GetExternalFilesDir(null).AbsolutePath;
            }
            else
            {
                root = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments);
            }

            //Create directory and file 
            //Java.IO.File myDir = new Java.IO.File(root + "/" + Resources.GetText(Resource.String.app_name));
            Java.IO.File myDir = new Java.IO.File(root + "/PandaHousehold");
            myDir.Mkdir();
            ExternalDatabasePath = System.IO.Path.Combine(myDir.AbsolutePath, "kakei.db3");

            string dbPath = FileAccessHelper.GetLocalFilePath("kakei.db3");
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

