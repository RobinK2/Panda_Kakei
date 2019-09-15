using System;
using System.Windows.Input;
using Xamarin.Forms;
using Panda_Kakei.Services;
using System.Collections.Generic;
using Panda_Kakei.Models;

namespace Panda_Kakei.Views
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
            
            SharedObject.dbManager = new DataManager(Paths.DATABASE_PATH);
            SharedObject.currencySymbol = Panda_Kakei.Resources.AppResource.CurrencyText;
            if(0 == SharedObject.dbManager.GetAppSettingsCount())
            {
                SharedObject.dbManager.AddAppSettings(SharedObject.currencySymbol);
            }
            else
            {
                SharedObject.currencySymbol = SharedObject.dbManager.GetCurrencySettings();
            }
            checkUpdateRegularData();
            refreshBalance();
        }

        private void checkUpdateRegularData()
        {
            string thisMonth = DateTime.Today.ToString("y");

            List<RegularData> regularDataItems = SharedObject.dbManager.GetAllRegularDataItems(Constants.INCOME_STRING);
            regularDataItems.AddRange(SharedObject.dbManager.GetAllRegularDataItems(Constants.EXPENSE_STRING));

            foreach(RegularData regularData in regularDataItems)
            {
                if(regularData.LastAddedMonth != thisMonth)
                {
                    //Add regular data to data in DB
                    Data data = new Data();
                    data.Amount = regularData.Amount;
                    data.Category = regularData.Category;
                    data.CategoryType = regularData.CategoryType;
                    data.Comment = regularData.Comment;
                    data.Currency = regularData.Currency;
                    data.Day = regularData.Day;
                    data.Month = DateTime.Today.Month.ToString();
                    data.Year = DateTime.Today.Year.ToString();

                    regularData.LastAddedMonth = thisMonth;

                    SharedObject.dbManager.AddDataItem(data);
                    SharedObject.dbManager.ModifyRegularDataItem(regularData);
                    SharedObject.dbManager.Commit();
                }
            }
        }

        private void refreshBalance()
        {
            int totalIncome = 0;
            int totalExpense = 0;
            int balance = 0;
            string month = DateTime.Today.Month.ToString();
            string year = DateTime.Today.Year.ToString();
            List<Data> dataItems = SharedObject.dbManager.GetDataItemsOfPeriod(month, year);

            foreach (Data item in dataItems)
            {
                if (Constants.INCOME_STRING == item.CategoryType)
                {
                    totalIncome += item.Amount;
                }
                else if (Constants.EXPENSE_STRING == item.CategoryType)
                {
                    totalExpense += item.Amount;
                }
            }

            balance = (totalIncome - totalExpense);

            entryIncome.Text = totalIncome.ToString();
            entryExpense.Text = totalExpense.ToString();
            entryBalance.Text = balance.ToString();
        }

        private async void menuItemSettings_OnClicked(object sender, EventArgs e)
        {
            MenuItemSettings.IsEnabled = false;

            Page newPage = new SettingsPage();
            string message = Constants.REFRESH_MAIN_PAGE_BALANCE_MESSAGE;
            MessagingCenter.Unsubscribe<SettingsPage>(this, message);
            MessagingCenter.Subscribe<SettingsPage>(this, message, (SettingsPage) =>
            {
                checkUpdateRegularData();
                refreshBalance();
            });
            await Navigation.PushAsync(newPage);

            MenuItemSettings.IsEnabled = true;
        }

        private async void btnAddExpense_OnClicked(object sender, EventArgs e)
        {
            btnAddExpense.IsEnabled = false;

            Page newPage = new DataItemPage(Panda_Kakei.Resources.AppResource.AddExpenseText, Constants.EXPENSE_STRING);
            string message = Constants.REFRESH_MAIN_PAGE_BALANCE_MESSAGE;
            MessagingCenter.Unsubscribe<DataItemPage>(this, message);
            MessagingCenter.Subscribe<DataItemPage>(this, message, (DataItemPage) =>
            {
                refreshBalance();

                MessagingCenter.Unsubscribe<DataItemPage>(this, message);
            });

            await Navigation.PushAsync(newPage);

            btnAddExpense.IsEnabled = true;
        }

        private async void btnAddIncome_OnClicked(object sender, EventArgs e)
        {
            btnAddIncome.IsEnabled = false;

            Page newPage = new DataItemPage(Panda_Kakei.Resources.AppResource.AddIncomeText, Constants.INCOME_STRING);
            string message = Constants.REFRESH_MAIN_PAGE_BALANCE_MESSAGE;
            MessagingCenter.Unsubscribe<DataItemPage>(this, message);
            MessagingCenter.Subscribe<DataItemPage>(this, message, (DataItemPage) =>
            {
                refreshBalance();

                MessagingCenter.Unsubscribe<DataItemPage>(this, message);
            });

            await Navigation.PushAsync(newPage);

            btnAddIncome.IsEnabled = true;
        }

        private async void btnViewData_OnClicked(object sender, EventArgs e)
        {
            btnViewData.IsEnabled = false;

            Page newPage = new ViewDataPage();
            string message = Constants.REFRESH_MAIN_PAGE_BALANCE_MESSAGE;
            MessagingCenter.Unsubscribe<ViewDataPage>(this, message);
            MessagingCenter.Subscribe<ViewDataPage>(this, message, (DataItemPage) =>
            {
                refreshBalance();

                MessagingCenter.Unsubscribe<ViewDataPage>(this, message);
            });
            await Navigation.PushAsync(newPage);

            btnViewData.IsEnabled = true;
        }

        private async void btnExcelReport_OnClicked(object sender, EventArgs e)
        {
            btnExcelReport.IsEnabled = false;

            Page newPage = new ExcelPrintPage();
            await Navigation.PushAsync(newPage);

            btnExcelReport.IsEnabled = true;
        }
    }
}
