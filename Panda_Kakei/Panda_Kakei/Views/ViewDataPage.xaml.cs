using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Panda_Kakei.Services;
using Panda_Kakei.Models;

namespace Panda_Kakei.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class ViewDataPage : ContentPage
	{
        private ItemModel itemModel;
        private int selectedMonth;
        private int selectedYear;
        private List<Data> dataItems;
        private int totalIncome = 0;
        private int totalExpense = 0;
        private int balance = 0;
        private bool isAscendingSort = true;

        public ViewDataPage()
        {
            InitializeComponent();
            
            this.selectedMonth = DateTime.Now.Date.Month;
            this.selectedYear = DateTime.Now.Date.Year;

            this.itemModel = new ItemModel();
            this.BindingContext = this.itemModel;
            updateBtnSetDateText();
            pickerSort.SelectedIndex = 0;
        }

        /// <summary>
        /// Clear listViewItem and populate it with database items from selected month and year.
        /// </summary>
        private void populateItemModel()
        {
            SortType sortType = SortType.DAY_ASCENDING;
            SortOption sortOption = (SortOption) pickerSort.SelectedItem;
            switch(sortOption.Value)
            {
                case SortOption.SortOptionSelection.Date:
                    if(this.isAscendingSort)
                    {
                        sortType = SortType.DAY_ASCENDING;
                    }
                    else
                    {
                        sortType = SortType.DAY_DESCENDING;
                    }
                    break;
                case SortOption.SortOptionSelection.Category:
                    if(this.isAscendingSort)
                    {
                        sortType = SortType.CATEGORY_ASCENING;
                    }
                    else
                    {
                        sortType = SortType.CATEGORY_DESCENDING;
                    }
                    break;
                case SortOption.SortOptionSelection.Amount:
                    if(this.isAscendingSort)
                    {
                        sortType = SortType.AMOUNT_ASCENDING;
                    }
                    else
                    {
                        sortType = SortType.AMOUNT_DESCENDING;
                    }
                    break;
                default:
                    break;
            }

            dataItems = SharedObject.dbManager.GetDataItemsOfPeriodSortedBy(this.selectedMonth.ToString(), this.selectedYear.ToString(),
                sortType);
            this.itemModel.Items.Clear();

            foreach (Data dataItem in dataItems)
            {
                Item item = new Item(dataItem);
                this.itemModel.Items.Add(item);
            }

            refreshBalance();
        }

        private void refreshBalance()
        {
            this.totalIncome = 0;
            this.totalExpense = 0;

            foreach (Data item in this.dataItems)
            {
                if (Constants.INCOME_STRING == item.CategoryType)
                {
                    this.totalIncome += item.Amount;
                }
                else if (Constants.EXPENSE_STRING == item.CategoryType)
                {
                    this.totalExpense += item.Amount;
                }
            }

            this.balance = (this.totalIncome - this.totalExpense);

            displayIncomeExpenseBalance();
        }

        /// <summary>
        /// Display total income, total expense and balance
        /// </summary>
        private void displayIncomeExpenseBalance()
        {
            lblIncome.Text = Panda_Kakei.Resources.AppResource.IncomeLabelText + " " + totalIncome.ToString();
            lblExpense.Text = Panda_Kakei.Resources.AppResource.ExpenseLabelText + " " + totalExpense.ToString();
            lblBalance.Text = Panda_Kakei.Resources.AppResource.BalanceLabelText + " " + balance.ToString();
        }

        private void btnSetDate_Clicked(object sender, EventArgs e)
        {
            datePicker.IsOpen = !datePicker.IsOpen;
        }

        private void datePicker_OkButtonClicked(object sender, Syncfusion.SfPicker.XForms.SelectionChangedEventArgs e)
        {
            this.selectedYear = DateTime.ParseExact((e.NewValue as IList)[0].ToString(), "yyyy", CultureInfo.CurrentCulture).Year;
            this.selectedMonth = DateTime.ParseExact((e.NewValue as IList)[1].ToString(), "MMMM", CultureInfo.CurrentCulture).Month;
            
            updateBtnSetDateText();
            populateItemModel();
        }

        private void datePicker_CancelButtonClicked(object sender, Syncfusion.SfPicker.XForms.SelectionChangedEventArgs e)
        {
        }

        private void btnEdit_Clicked(object sender, EventArgs e)
        {
            Item selectedItem = (Item)listViewItems.SelectedItem;
            Data editItem = selectedItem.DbDataItem;
            string title = string.Empty;

            if(editItem.CategoryType == Constants.EXPENSE_STRING)
            {
                title = Panda_Kakei.Resources.AppResource.EditExpenseText;
            }
            else if(editItem.CategoryType == Constants.INCOME_STRING)
            {
                title = Panda_Kakei.Resources.AppResource.EditIncomeText;
            }

            string message = Constants.EDIT_DATA_ITEM_MESSAGE;
            Page newPage = new DataItemPage(title, editItem.CategoryType, true, editItem);
            MessagingCenter.Unsubscribe<DataItemPage>(this, message);
            MessagingCenter.Subscribe<DataItemPage>(this, message, (DataItemPage) =>
            {
                populateItemModel();
                MessagingCenter.Send<ViewDataPage>(this, Constants.REFRESH_MAIN_PAGE_BALANCE_MESSAGE);

                MessagingCenter.Unsubscribe<DataItemPage>(this, message);
            });

            Navigation.PushAsync(newPage);
        }

        private async void btnRemove_ClickedAsync(object sender, EventArgs e)
        {
            var answer = await DisplayAlert(Panda_Kakei.Resources.AppResource.RemoveText, 
                Panda_Kakei.Resources.AppResource.DialogRemoveText, Panda_Kakei.Resources.AppResource.YesText, 
                Panda_Kakei.Resources.AppResource.NoText);

            // Answer is yes, delete
            if(answer == true)
            {
                Item deleteItem = (Item) listViewItems.SelectedItem;
                SharedObject.dbManager.DeleteDataItem(deleteItem.DbDataItem);

                populateItemModel();
            }

            MessagingCenter.Send<ViewDataPage>(this, Constants.REFRESH_MAIN_PAGE_BALANCE_MESSAGE);
        }

        private void listViewItems_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            btnEdit.IsEnabled = true;
            btnRemove.IsEnabled = true;
        }

        private void btnPreviousMonth_Clicked(object sender, EventArgs e)
        {
            this.selectedMonth--;
            if (this.selectedMonth <= 0)
            {
                this.selectedMonth = 12;
                this.selectedYear--;
            }
            
            updateBtnSetDateText();
            populateItemModel();
        }

        private void btnNextMonth_Clicked(object sender, EventArgs e)
        {
            this.selectedMonth++;
            if(this.selectedMonth >= 13)
            {
                this.selectedMonth = 1;
                this.selectedYear++;
            }

            updateBtnSetDateText();
            populateItemModel();
        }

        private void updateBtnSetDateText()
        {
            DateTime date = new DateTime(this.selectedYear, this.selectedMonth, 1);

            ObservableCollection<object> selectedCollection = new ObservableCollection<object>
            {
                date.Year.ToString(),
                CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(date.Month)
            };
            datePicker.SelectedItem = selectedCollection;

            this.btnSetDate.Text = date.Year.ToString() + "\t" + CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(date.Month);
        }

        private void pickerSort_SelectedIndexChanged(object sender, EventArgs e)
        {
            populateItemModel();
        }

        private void btnSortAscDesc_Clicked(object sender, EventArgs e)
        {
            this.isAscendingSort = !this.isAscendingSort;

            if(this.isAscendingSort)
            {
                btnSortAscDesc.Text = "\xF077";
            }
            else
            {
                btnSortAscDesc.Text = "\xF078";
            }

            populateItemModel();
        }
    }
}