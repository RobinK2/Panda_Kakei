using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Panda_Kakei.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class ExcelPrintPage : ContentPage
	{
        private int startMonth;
        private int endMonth;
        private int startYear;
        private int endYear;
        string excelPath;

		public ExcelPrintPage ()
		{
			InitializeComponent ();

            this.startMonth = DateTime.Now.Date.Month;
            this.endMonth = DateTime.Now.Date.Month;
            this.startYear = DateTime.Now.Date.Year;
            this.endYear = DateTime.Now.Date.Year;
            this.btnStartDate.Text = DateTime.Now.Date.Year.ToString() +
                "\t\t" + CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(DateTime.Now.Date.Month);
            this.btnEndDate.Text = DateTime.Now.Date.Year.ToString() +
                "\t\t" + CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(DateTime.Now.Date.Month);

            //entryExcelPath.Text = Paths.EXCEL_STORAGE_PATH;
        }

        private void btnStartDate_Clicked(object sender, EventArgs e)
        {
            startDatePicker.IsOpen = !startDatePicker.IsOpen;
        }

        private void btnEndDate_Clicked(object sender, EventArgs e)
        {
            endDatePicker.IsOpen = !endDatePicker.IsOpen;
        }

        private void startDatePicker_OkButtonClicked(object sender, Syncfusion.SfPicker.XForms.SelectionChangedEventArgs e)
        {
            this.startYear = DateTime.ParseExact((e.NewValue as IList)[0].ToString(), "yyyy", CultureInfo.CurrentCulture).Year;
            this.startMonth = DateTime.ParseExact((e.NewValue as IList)[1].ToString(), "MMMM", CultureInfo.CurrentCulture).Month;
            this.btnStartDate.Text = (e.NewValue as IList)[0].ToString() + "\t\t" + (e.NewValue as IList)[1].ToString();
        }

        private void startDatePicker_CancelButtonClicked(object sender, Syncfusion.SfPicker.XForms.SelectionChangedEventArgs e)
        {

        }

        private void endDatePicker_OkButtonClicked(object sender, Syncfusion.SfPicker.XForms.SelectionChangedEventArgs e)
        {
            this.endYear = DateTime.ParseExact((e.NewValue as IList)[0].ToString(), "yyyy", CultureInfo.CurrentCulture).Year;
            this.endMonth = DateTime.ParseExact((e.NewValue as IList)[1].ToString(), "MMMM", CultureInfo.CurrentCulture).Month;
            this.btnEndDate.Text = (e.NewValue as IList)[0].ToString() + "\t\t" + (e.NewValue as IList)[1].ToString();
        }

        private void endDatePicker_CancelButtonClicked(object sender, Syncfusion.SfPicker.XForms.SelectionChangedEventArgs e)
        {

        }

        private async void btnGenerate_ClickedAsync(object sender, EventArgs e)
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
                    Services.ExcelPrinter printer = new Services.ExcelPrinter(startMonth.ToString(), startYear.ToString(),
                        endMonth.ToString(), endYear.ToString());

                    await printer.PrintExcel(Paths.EXCEL_STORAGE_PATH);

                    await DisplayAlert(Panda_Kakei.Resources.AppResource.ExcelReportCompleteTitle,
                        Panda_Kakei.Resources.AppResource.ExcelReportCompleteText + "\r\n" + Paths.EXCEL_STORAGE_PATH,
                        Panda_Kakei.Resources.AppResource.OkText);
                }
            }
            catch (Exception exc)
            {
                await DisplayAlert(Panda_Kakei.Resources.AppResource.ExcelPrintErrorTitle,
                    Panda_Kakei.Resources.AppResource.ExcelPrintErrorMessage + "\r\n" + exc.Message,
                    Panda_Kakei.Resources.AppResource.OkText);
            }
        }

        //private void entryExcelPath_TextChanged(object sender, TextChangedEventArgs e)
        //{
        //    Paths.EXCEL_STORAGE_PATH = entryExcelPath.Text;
        //}
    }
}