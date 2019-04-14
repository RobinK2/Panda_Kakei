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
        private const string SYNCFUSION_LICENSE_KEY = "NjYwNzFAMzEzNjJlMzQyZTMwbTNNU2tlYndveG1Ed25DV1VLbEJYUi8ybEo3ODFWbzJTa2ZUT05VY05zND0=";
        private const string SYNCFUSION_LICENSE_KEY2 =  "NjYxMTFAMzEzNjJlMzMyZTMwYzArWFRGVERja012R0w3c0lBQ2xpOVNqRVRucFJPQjFMaDUzczY0cmFtST0=";
        public App (string dbPath)
		{
            //Register Syncfusion license
            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense(SYNCFUSION_LICENSE_KEY);
            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense(SYNCFUSION_LICENSE_KEY2);

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
            //Paths.EXCEL_STORAGE_PATH = System.IO.Path.Combine(excelPath, "Report.xlsx");
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
