using Panda_Kakei.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Panda_Kakei.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class DataItemPage : ContentPage
	{
        private string categoryType = string.Empty;
        private CategoryModel categoryModel;
        private Data dataItem = null;
        private bool amountEntered = false;
        private bool modifyItem = false;
        private bool categorySelected = false;

        public DataItemPage(string title, string categoryType)
        {
            InitializeComponent();

            this.Title = title;
            this.categoryType = categoryType;
            this.categoryModel = new CategoryModel();
            this.BindingContext = this.categoryModel;

            loadCategoriesFromDB();
        }

        public DataItemPage(string title, string categoryType, 
            bool modifyItem, Data dataItem) : this(title, categoryType)
        {
            this.modifyItem = modifyItem;
            this.dataItem = dataItem;
            btnAdd.Text = Panda_Kakei.Resources.AppResource.SaveText;
            loadFromExistingItem(dataItem);
        }

        /// <summary>
        /// Load page with an existing item.
        /// </summary>
        /// <param name="dataItem"></param>
        private void loadFromExistingItem(Data dataItem)
        {
            DateTime date = new DateTime(int.Parse(dataItem.Year), int.Parse(dataItem.Month),
                dataItem.Day);
            datePicker.Date = date;

            entryAmount.Text = dataItem.Amount.ToString();
            entryMemo.Text = dataItem.Comment;

            foreach (DataSettings category in this.categoryModel.Categories)
            {
                if (category.Name == dataItem.Category)
                {
                    pickerCategory.SelectedItem = category;
                    break;
                }
            }
        }

        /// <summary>
        /// Load pickerCategories with items from database.
        /// </summary>
        private void loadCategoriesFromDB()
        {
            this.categoryModel.Categories.Clear();
            List<DataSettings> categoryList = SharedObject.dbManager.GetAllSettingsItems(this.categoryType);
            foreach (DataSettings category in categoryList)
            {
                this.categoryModel.Categories.Add(category);
            }
        }

        private async void btnAddCategory_Clicked(object sender, EventArgs e)
        {
            CategoryType categoryTypeEnum;
            string message = string.Empty;

            if(this.categoryType == Constants.INCOME_STRING)
            {
                categoryTypeEnum = CategoryType.INCOME_CATEGORY;
                message = Constants.INCOME_CATEGORY_NAME_MESSAGE;
            }
            else if(this.categoryType == Constants.EXPENSE_STRING)
            {
                categoryTypeEnum = CategoryType.EXPENSE_CATEGORY;
                message = Constants.EXPENSE_CATEGORY_NAME_MESSAGE;
            }
            else
            {
                return;
            }

            btnAddCategory.IsEnabled = false;

            Page newPage = new NewCategoryPage(categoryTypeEnum);
            MessagingCenter.Unsubscribe<NewCategoryPage, string>(this, message);
            MessagingCenter.Subscribe<NewCategoryPage, string>(this, message, (messageSender, arg) =>
            {
                string name = arg;
                SharedObject.dbManager.AddDataSettingsItem(this.categoryType, name);
                loadCategoriesFromDB();

                MessagingCenter.Unsubscribe<NewCategoryPage, string>(this, message);
            });

            await Navigation.PushAsync(newPage);

            btnAddCategory.IsEnabled = true;
        }

        private async void btnRemoveCategory_ClickedAsync(object sender, EventArgs e)
        {
            if (pickerCategory.SelectedIndex != -1)
            {
                var answer = await DisplayAlert(Panda_Kakei.Resources.AppResource.RemoveText,
                    Panda_Kakei.Resources.AppResource.DialogRemoveText, Panda_Kakei.Resources.AppResource.YesText,
                    Panda_Kakei.Resources.AppResource.NoText);

                // Answer is yes, delete
                if (answer == true)
                {
                    DataSettings deleteCategory = (DataSettings) pickerCategory.SelectedItem;
                    SharedObject.dbManager.DeleteSettingsItem(deleteCategory);
                    loadCategoriesFromDB();
                }
            }
        }

        private void pickerCategory_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (pickerCategory.SelectedIndex != -1)
            {
                this.btnRemoveCategory.IsEnabled = true;
                categorySelected = true;
            }
            else
            {
                this.btnRemoveCategory.IsEnabled = false;
            }

            if (amountEntered && categorySelected)
            {
                btnAdd.IsEnabled = true;
            }
            else
            {
                btnAdd.IsEnabled = false;
            }
        }

        private void entryAmount_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (entryAmount.Text != "")
            {
                amountEntered = true;
            }
            else
            {
                amountEntered = false;
            }

            if (amountEntered && categorySelected)
            {
                btnAdd.IsEnabled = true;
            }
            else
            {
                btnAdd.IsEnabled = false;
            }
        }

        private void btnAdd_Clicked(object sender, EventArgs e)
        {
            int amount = 0;
            try
            {
                amount = int.Parse(entryAmount.Text);
            }
            catch (FormatException)
            {
                DisplayAlert(Panda_Kakei.Resources.AppResource.InputErrorText,
                    Panda_Kakei.Resources.AppResource.ErrorAmountText, Panda_Kakei.Resources.AppResource.OkText);

                return;
            }

            Data dataItem;
            if(this.modifyItem == true)
            {
                dataItem = this.dataItem;
            }
            else
            {
                dataItem = new Data();
            }
            dataItem.Amount = amount;
            dataItem.Comment = entryMemo.Text;
            dataItem.Category = pickerCategory.SelectedItem.ToString();
            dataItem.CategoryType = this.categoryType;
            dataItem.Day = datePicker.Date.Day;
            dataItem.Month = datePicker.Date.Month.ToString();
            dataItem.Year = datePicker.Date.Year.ToString();

            if(this.modifyItem == true)
            {
                SharedObject.dbManager.ModifyDataItem(dataItem);
                MessagingCenter.Send<DataItemPage>(this, Constants.EDIT_DATA_ITEM_MESSAGE);
            }
            else
            {
                SharedObject.dbManager.AddDataItem(dataItem);
                MessagingCenter.Send<DataItemPage>(this, Constants.REFRESH_MAIN_PAGE_BALANCE_MESSAGE);
            }

            this.Navigation.PopAsync();
        }
    }
}