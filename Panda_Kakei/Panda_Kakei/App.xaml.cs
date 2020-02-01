using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Panda_Kakei.Views;
using Plugin.Multilingual;
using System.Globalization;

[assembly: XamlCompilation (XamlCompilationOptions.Compile)]
namespace Panda_Kakei
{
	public partial class App : Application
	{
        private const string SYNCFUSION_LICENSE_KEY = "MjA0ODYwQDMxMzcyZTM0MmUzME5IQzJJY3RnZmJGMnhDNENKZlpMRDhLMnF1cnpjTTljWmZ2Tlk1bU1NeDA9";
        public App (string dbPath)
		{
            //Register Syncfusion license
            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense(SYNCFUSION_LICENSE_KEY);

            InitializeComponent();

            Paths.DATABASE_PATH = dbPath;

            Panda_Kakei.Resources.AppResource.Culture = CrossMultilingual.Current.DeviceCultureInfo;

            MainPage = new NavigationPage(new MainPage());
		}

        public App (string dbPath, string extStoragePath, string excelPath) : this(dbPath)
        {
            Paths.DATABASE_EXTERNAL_STORAGE_PATH = extStoragePath;
            Paths.DEFAULT_DATABASE_EXTERNAL_STORAGE_PATH = extStoragePath;
            Paths.EXCEL_STORAGE_PATH = System.IO.Path.Combine(excelPath,
                Panda_Kakei.Resources.AppResource.HouseholdAccountText + ".xlsx");
        }

        public App(string dbPath, string extStoragePath) : this(dbPath)
        {
            Paths.DATABASE_EXTERNAL_STORAGE_PATH = extStoragePath;
            Paths.DEFAULT_DATABASE_EXTERNAL_STORAGE_PATH = extStoragePath;
            Paths.EXCEL_STORAGE_PATH = Panda_Kakei.Resources.AppResource.HouseholdAccountText + ".xlsx";
        }

        protected override void OnStart ()
		{
			// Handle when your app starts
		}

		protected override void OnSleep ()
		{
			// Handle when your app sleeps
		}

		protected override void OnResume ()
		{
			// Handle when your app resumes
		}
	}
}
