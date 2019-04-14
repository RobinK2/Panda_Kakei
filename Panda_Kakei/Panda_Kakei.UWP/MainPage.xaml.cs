using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace Panda_Kakei.UWP
{
    public sealed partial class MainPage
    {
        public MainPage()
        {
            this.InitializeComponent();

            string dbPath = FileAccessHelper.GetLocalFilePath("kakei.db3");
            //LoadApplication(new Panda_Kakei.App(dbPath));
            if(Xamarin.Forms.Device.Idiom != Xamarin.Forms.TargetIdiom.Desktop)
            {
                LoadApplication(new Panda_Kakei.App(dbPath, dbPath, global::Windows.Storage.ApplicationData.Current.LocalFolder.Path));
            }
            else
            {
                LoadApplication(new Panda_Kakei.App(dbPath, dbPath));
            }
        }
    }
}
