using Panda_Kakei.Services;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Panda_Kakei.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class SettingsPage : ContentPage
	{
        private string oldCurrencySymbol;

        public SettingsPage()
        {
            InitializeComponent();

            oldCurrencySymbol = SharedObject.currencySymbol;
        }

        protected override void OnDisappearing()
        {
            // If currency symbol is updated, update it on the database too.
            if(oldCurrencySymbol != SharedObject.currencySymbol)
            {
                SharedObject.dbManager.ModifyCurrencySettings(SharedObject.currencySymbol);
            }
        }

        private void btnEndDate_OnClicked(object sender, EventArgs e)
        {

        }

        private void btnChangeColour_OnClicked(object sender, EventArgs e)
        {

        }

        private void copyDatabase(string srcPath, string destPath)
        {
            File.Copy(srcPath, destPath, true);
        }

        private async void btnBackup_OnClickedAsync(object sender, EventArgs e)
        {
            try
            {
                PermissionStatus status = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Storage);
                if(status != PermissionStatus.Granted)
                {


                    if (await CrossPermissions.Current.ShouldShowRequestPermissionRationaleAsync(Permission.Storage))
                    {
                        await DisplayAlert(Panda_Kakei.Resources.AppResource.AlertNeedFileAccessTitleText,
                            Panda_Kakei.Resources.AppResource.AlertNeedFileAccessMessageText,
                            Panda_Kakei.Resources.AppResource.OkText);
                    }

                    var results = await CrossPermissions.Current.RequestPermissionsAsync(Permission.Storage);
                    status = results[Permission.Storage];

                }

                if(status == PermissionStatus.Granted)
                {
                    var answer = await DisplayAlert(Panda_Kakei.Resources.AppResource.ConfirmTitleText,
                        Panda_Kakei.Resources.AppResource.DatabaseExportConfirmText,
                        Panda_Kakei.Resources.AppResource.YesText,
                        Panda_Kakei.Resources.AppResource.NoText);

                    // Answer is yes
                    if(answer == true)
                    {
                        switch (Device.RuntimePlatform)
                        {
                            case Device.Android:
                                try
                                {
                                    SharedObject.dbManager.Commit();
                                    SharedObject.dbManager.Close();
                                    copyDatabase(Paths.DATABASE_PATH, Paths.DATABASE_EXTERNAL_STORAGE_PATH);
                                    SharedObject.dbManager = new DataManager(Paths.DATABASE_PATH);
                                }
                                catch (Exception)
                                {
                                    await DisplayAlert(Panda_Kakei.Resources.AppResource.FailedTitleText,
                                        Panda_Kakei.Resources.AppResource.DatabaseCopyFailedText,
                                        Panda_Kakei.Resources.AppResource.OkText);
                                    return;
                                }
                                await DisplayAlert(Panda_Kakei.Resources.AppResource.CompletedTitleText,
                                    Panda_Kakei.Resources.AppResource.DatabaseExportSuccessText + "\r\n" + Paths.DATABASE_EXTERNAL_STORAGE_PATH,
                                    Panda_Kakei.Resources.AppResource.OkText);
                                break;
                            case Device.iOS:
                                break;
                            case Device.UWP:
                                string savePath = string.Empty;
                                try
                                {
                                    SharedObject.dbManager.Commit();
                                    SharedObject.dbManager.Close();
                                    
                                    savePath = await DependencyService.Get<ISave>().CopyFilePickDest(Paths.DATABASE_PATH, "kakei.db3");
                                    SharedObject.dbManager = new DataManager(Paths.DATABASE_PATH);
                                }
                                catch(Exception)
                                {
                                    await DisplayAlert(Panda_Kakei.Resources.AppResource.FailedTitleText,
                                        Panda_Kakei.Resources.AppResource.DatabaseCopyFailedText,
                                        Panda_Kakei.Resources.AppResource.OkText);
                                    return;
                                }
                                await DisplayAlert(Panda_Kakei.Resources.AppResource.CompletedTitleText,
                                    Panda_Kakei.Resources.AppResource.DatabaseExportSuccessText + "\r\n" + savePath,
                                    Panda_Kakei.Resources.AppResource.OkText);
                                break;
                            default:
                                break;
                        }
                    }
                }
            }
            catch (Exception)
            {
                await DisplayAlert(Panda_Kakei.Resources.AppResource.FileAccessTitleText,
                    Panda_Kakei.Resources.AppResource.DialogExternalDatabaseAccessErrorText,
                    Panda_Kakei.Resources.AppResource.OkText);
            }
        }

        private async void btnReadBackup_OnClickedAsync(object sender, EventArgs e)
        {
            try
            {
                PermissionStatus status = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Storage);
                if (status != PermissionStatus.Granted)
                {                    
                    if (await CrossPermissions.Current.ShouldShowRequestPermissionRationaleAsync(Permission.Storage))
                    {
                        await DisplayAlert(Panda_Kakei.Resources.AppResource.AlertNeedFileAccessTitleText,
                            Panda_Kakei.Resources.AppResource.AlertNeedFileAccessMessageText,
                            Panda_Kakei.Resources.AppResource.OkText);
                    }

                    var results = await CrossPermissions.Current.RequestPermissionsAsync(Permission.Storage);
                    status = results[Permission.Storage];

                }

                if (status == PermissionStatus.Granted)
                {
                    var answer = await DisplayAlert(Panda_Kakei.Resources.AppResource.ReadBackupSettingText,
                        Panda_Kakei.Resources.AppResource.DialogReadBackupWarningText, Panda_Kakei.Resources.AppResource.YesText,
                        Panda_Kakei.Resources.AppResource.NoText);

                    // Answer is yes, overwrite local database with external database
                    if (answer == true)
                    {
                        switch (Device.RuntimePlatform)
                        {
                            case Device.Android:
                                try
                                {
                                    SharedObject.dbManager.Close();
                                    copyDatabase(Paths.DATABASE_EXTERNAL_STORAGE_PATH, Paths.DATABASE_PATH);
                                    // Need to make new sqlite connection.
                                    SharedObject.dbManager = new DataManager(Paths.DATABASE_PATH);
                                    // Refresh calculations on MainPage
                                    MessagingCenter.Send<SettingsPage>(this, Constants.REFRESH_MAIN_PAGE_BALANCE_MESSAGE);
                                }
                                catch (Exception)
                                {
                                    await DisplayAlert(Panda_Kakei.Resources.AppResource.FailedTitleText,
                                        Panda_Kakei.Resources.AppResource.DatabaseCopyFailedText,
                                        Panda_Kakei.Resources.AppResource.OkText);
                                    return;
                                }
                                await DisplayAlert(Panda_Kakei.Resources.AppResource.CompletedTitleText,
                                    Panda_Kakei.Resources.AppResource.DatabaseImportSuccessText,
                                    Panda_Kakei.Resources.AppResource.OkText);
                                break;
                            case Device.iOS:
                                break;
                            case Device.UWP:
                                try
                                {
                                    SharedObject.dbManager.Close();
                                    await Xamarin.Forms.DependencyService.Get<ISave>().CopyFilePickSrc("kakei.db3", Paths.DATABASE_PATH);
                                    // Need to make new sqlite connection.
                                    SharedObject.dbManager = new DataManager(Paths.DATABASE_PATH);
                                    // Refresh calculations on MainPage
                                    MessagingCenter.Send<SettingsPage>(this, Constants.REFRESH_MAIN_PAGE_BALANCE_MESSAGE);
                                }
                                catch (Exception)
                                {
                                    await DisplayAlert(Panda_Kakei.Resources.AppResource.FailedTitleText,
                                        Panda_Kakei.Resources.AppResource.DatabaseCopyFailedText,
                                        Panda_Kakei.Resources.AppResource.OkText);
                                    return;
                                }
                                await DisplayAlert(Panda_Kakei.Resources.AppResource.CompletedTitleText,
                                    Panda_Kakei.Resources.AppResource.DatabaseImportSuccessText,
                                    Panda_Kakei.Resources.AppResource.OkText);
                                break;
                            default:
                                break;
                        }
                    }
                }
            }
            catch (Exception)
            {
                await DisplayAlert(Panda_Kakei.Resources.AppResource.FileAccessTitleText,
                    Panda_Kakei.Resources.AppResource.DialogExternalDatabaseAccessErrorText,
                    Panda_Kakei.Resources.AppResource.OkText);
            }
        }

        private void btnShareFb_OnClicked(object sender, EventArgs e)
        {

        }

        private void btnPay_OnClicked(object sender, EventArgs e)
        {

        }

        private async void btnResetDatabase_ClickedAsync(object sender, EventArgs e)
        {
            var answer = await DisplayAlert(Panda_Kakei.Resources.AppResource.ConfirmTitleText,
                Panda_Kakei.Resources.AppResource.DatabaseResetWarningText,
                Panda_Kakei.Resources.AppResource.YesText,
                Panda_Kakei.Resources.AppResource.NoText);

            // If answer is yes
            if(answer == true)
            {
                try
                {
                    // Remove all data in the database
                    SharedObject.dbManager.DeleteAllDataItems();
                    // Refresh calculations on MainPage
                    MessagingCenter.Send<SettingsPage>(this, Constants.REFRESH_MAIN_PAGE_BALANCE_MESSAGE);
                }
                catch(Exception)
                {
                    await DisplayAlert(Panda_Kakei.Resources.AppResource.FailedTitleText,
                        Panda_Kakei.Resources.AppResource.DatabaseResetFailedText,
                        Panda_Kakei.Resources.AppResource.OkText);

                    return;
                }

                await DisplayAlert(Panda_Kakei.Resources.AppResource.CompletedTitleText,
                    Panda_Kakei.Resources.AppResource.DatabaseResetCompleteText,
                    Panda_Kakei.Resources.AppResource.OkText);
            }
        }

        private void entryCurrency_TextChanged(object sender, TextChangedEventArgs e)
        {
            SharedObject.currencySymbol = entryCurrency.Text;
        }

        private void btnRegularIncomeExpense_OnClicked(object sender, EventArgs e)
        {
            Page newPage = new ViewRegularDataItemPage(Panda_Kakei.Resources.AppResource.RegularIncomeExpenseSettingsText,
                Panda_Kakei.Resources.AppResource.RegularIncomeExpenseSubtitleText);

            string message = Constants.ADD_EDIT_REGULAR_DATA_ITEM_MESSAGE;
            MessagingCenter.Unsubscribe<ViewRegularDataItemPage>(this, message);
            MessagingCenter.Subscribe<ViewRegularDataItemPage>(this, message, (RegularDataItemPage) =>
            {
                // Refresh calculations on MainPage
                MessagingCenter.Send<SettingsPage>(this, Constants.REFRESH_MAIN_PAGE_BALANCE_MESSAGE);

                MessagingCenter.Unsubscribe<ViewRegularDataItemPage>(this, message);
            });

            Navigation.PushAsync(newPage);
        }
    }
}