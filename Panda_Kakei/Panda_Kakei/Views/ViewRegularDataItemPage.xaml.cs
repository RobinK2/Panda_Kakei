using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Panda_Kakei.Models;

namespace Panda_Kakei.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ViewRegularDataItemPage : ContentPage
    {
        private RegularDataItemModel regularDataItemModel;

        public ViewRegularDataItemPage(string title, string subTitle)
        {
            InitializeComponent();

            this.Title = title;
            this.regularDataItemModel = new RegularDataItemModel();
            this.BindingContext = this.regularDataItemModel;
            btnAdd.IsEnabled = true;
            btnEdit.IsEnabled = false;
            btnRemove.IsEnabled = false;
            lblSubTitle.Text = subTitle;
            loadRegularDataItemsFromDB();
        }

        /// <summary>
        /// Load listview with regular data items from database.
        /// </summary>
        private void loadRegularDataItemsFromDB()
        {
            this.regularDataItemModel.RegularDataItems.Clear();
            List<RegularData> regularDataItemList = SharedObject.dbManager.GetAllRegularDataItems(Constants.INCOME_STRING);
            regularDataItemList.AddRange(SharedObject.dbManager.GetAllRegularDataItems(Constants.EXPENSE_STRING));
            foreach(RegularData regularData in regularDataItemList)
            {
                RegularDataItem item = new RegularDataItem(regularData);
                this.regularDataItemModel.RegularDataItems.Add(item);
            }
        }

        private async void btnRemove_ClickedAsync(object sender, EventArgs e)
        {
            var answer = await DisplayAlert(Panda_Kakei.Resources.AppResource.RemoveText,
                Panda_Kakei.Resources.AppResource.DialogRemoveText,
                Panda_Kakei.Resources.AppResource.YesText,
                Panda_Kakei.Resources.AppResource.NoText);

            // Answer is yes, delete
            if (answer == true)
            {
                RegularDataItem deleteItem = (RegularDataItem)listViewRegularData.SelectedItem;
                SharedObject.dbManager.DeleteRegularDataItem(deleteItem.DbRegDataItem);

                loadRegularDataItemsFromDB();
            }

            MessagingCenter.Send<ViewRegularDataItemPage>(this, Constants.ADD_EDIT_REGULAR_DATA_ITEM_MESSAGE);
        }

        private async void btnAdd_Clicked(object sender, EventArgs e)
        {
            btnAdd.IsEnabled = false;

            Page newPage = new RegularDataItemPage(Panda_Kakei.Resources.AppResource.AddRegularIncomeExpenseText);
            string message = Constants.ADDED_REGULAR_DATA_ITEM_MESSAGE;
            MessagingCenter.Unsubscribe<RegularDataItemPage>(this, message);
            MessagingCenter.Subscribe<RegularDataItemPage>(this, message, (RegularDataItemPage) =>
            {
                loadRegularDataItemsFromDB();
                // Send message back to SettingsPage that will referesh calculations on MainPage
                MessagingCenter.Send<ViewRegularDataItemPage>(this, Constants.ADD_EDIT_REGULAR_DATA_ITEM_MESSAGE);

                MessagingCenter.Unsubscribe<RegularDataItemPage>(this, message);
            });

            await Navigation.PushAsync(newPage);

            btnAdd.IsEnabled = true;
        }

        private async void btnEdit_Clicked(object sender, EventArgs e)
        {
            RegularDataItem selectedItem = (RegularDataItem) listViewRegularData.SelectedItem;
            RegularData editItem = selectedItem.DbRegDataItem;
            string title = string.Empty;

            if(editItem.CategoryType == Constants.EXPENSE_STRING)
            {
                title = Panda_Kakei.Resources.AppResource.EditRegularExpenseText;
            }
            else if(editItem.CategoryType == Constants.INCOME_STRING)
            {
                title = Panda_Kakei.Resources.AppResource.EditRegularIncomeText;
            }

            btnEdit.IsEnabled = false;

            string message = Constants.EDITED_REGULAR_DATA_ITEM_MESSAGE;
            Page newPage = new RegularDataItemPage(title, true, editItem);
            MessagingCenter.Unsubscribe<RegularDataItemPage>(this, message);
            MessagingCenter.Subscribe<RegularDataItemPage>(this, message, (RegularDataItemPage) =>
            {
                loadRegularDataItemsFromDB();
                // Send message back to SettingsPage that will referesh calculations on MainPage
                MessagingCenter.Send<ViewRegularDataItemPage>(this, Constants.ADD_EDIT_REGULAR_DATA_ITEM_MESSAGE);

                MessagingCenter.Unsubscribe<RegularDataItemPage>(this, message);
            });

            await Navigation.PushAsync(newPage);

            btnEdit.IsEnabled = true;
        }

        private void listViewRegularData_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            btnEdit.IsEnabled = true;
            btnRemove.IsEnabled = true;
        }
    }
}