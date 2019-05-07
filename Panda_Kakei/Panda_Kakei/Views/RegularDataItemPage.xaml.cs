using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Panda_Kakei.Models;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Panda_Kakei.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RegularDataItemPage : ContentPage
    {
        private class CategoryTypeMap
        {
            public CategoryType CategoryTypeValue { get; set; }
            public string CategoryTypeString { get; set; }

            public override string ToString()
            {
                return CategoryTypeString;
            }
        }

        private string categoryType = string.Empty;
        private CategoryModel categoryModel;
        private RegularData regularDataItem = null;
        private bool amountEntered = false;
        private bool daySelected = false;
        private bool modifyItem = false;
        private bool categorySelected = false;

        public RegularDataItemPage(string title)
        {
            InitializeComponent();

            this.categoryModel = new CategoryModel();
            this.BindingContext = this.categoryModel;
            this.Title = title;

            populatePickerCategoryType();
            populatePickerDay();
            pickerCategoryType.SelectedIndex = 0;
        }

        public RegularDataItemPage(string title, bool modifyItem, RegularData editItem)
            : this(title)
        {
            this.modifyItem = modifyItem;
            this.regularDataItem = editItem;
            btnAdd.Text = Panda_Kakei.Resources.AppResource.SaveText;

            if(editItem.CategoryType == Constants.EXPENSE_STRING)
            {
                pickerCategoryType.SelectedIndex = 0;
            }
            else if (editItem.CategoryType == Constants.INCOME_STRING)
            {
                pickerCategoryType.SelectedIndex = 1;
            }

            loadFromExistingItem(editItem);
        }

        private void populatePickerCategoryType()
        {
            var categoryList = new List<CategoryTypeMap>();
            categoryList.Add(new CategoryTypeMap()
            {
                CategoryTypeValue = CategoryType.EXPENSE_CATEGORY,
                CategoryTypeString = Panda_Kakei.Resources.AppResource.ExpenseText
            });
            categoryList.Add(new CategoryTypeMap()
            {
                CategoryTypeValue = CategoryType.INCOME_CATEGORY,
                CategoryTypeString = Panda_Kakei.Resources.AppResource.IncomeText
            });
            pickerCategoryType.ItemsSource = categoryList;
        }

        private void populatePickerDay()
        {
            var dayList = new List<int>();
            for(int i = 1; i <= 28; i++)
            {
                dayList.Add(i);
            }
            pickerDay.ItemsSource = dayList;
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

        /// <summary>
        /// Load page with an existing item.
        /// </summary>
        /// <param name="regularDataItem"></param>
        private void loadFromExistingItem(RegularData regularDataItem)
        {            
            entryAmount.Text = regularDataItem.Amount.ToString();
            entryMemo.Text = regularDataItem.Comment;

            foreach (var item in pickerDay.Items)
            {
                int day = int.Parse(item);
                if (day == regularDataItem.Day)
                {
                    pickerDay.SelectedItem = day;
                    break;
                }
            }

            foreach (DataSettings category in this.categoryModel.Categories)
            {
                if (regularDataItem.Category == category.Name)
                {
                    pickerCategory.SelectedItem = category;
                    break;
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

            if (amountEntered && categorySelected && daySelected)
            {
                btnAdd.IsEnabled = true;
            }
            else
            {
                btnAdd.IsEnabled = false;
            }
        }

        private void pickerCategoryType_SelectedIndexChanged(object sender, EventArgs e)
        {
            CategoryTypeMap selectedItem = (CategoryTypeMap) pickerCategoryType.SelectedItem;
            if(selectedItem.CategoryTypeValue == CategoryType.EXPENSE_CATEGORY)
            {
                this.categoryType = Constants.EXPENSE_STRING;
            }
            else if(selectedItem.CategoryTypeValue == CategoryType.INCOME_CATEGORY)
            {
                this.categoryType = Constants.INCOME_STRING;
            }

            if(this.categoryType != string.Empty)
            {
                loadCategoriesFromDB();
            }
        }

        private void pickerDay_SelectedIndexChanged(object sender, EventArgs e)
        {
            daySelected = true;

            if (amountEntered && categorySelected && daySelected)
            {
                btnAdd.IsEnabled = true;
            }
            else
            {
                btnAdd.IsEnabled = false;
            }
        }

        private void btnAddCategory_Clicked(object sender, EventArgs e)
        {
            CategoryType categoryTypeEnum;
            string message = string.Empty;

            if (this.categoryType == Constants.INCOME_STRING)
            {
                categoryTypeEnum = CategoryType.INCOME_CATEGORY;
                message = Constants.INCOME_CATEGORY_NAME_MESSAGE;
            }
            else if (this.categoryType == Constants.EXPENSE_STRING)
            {
                categoryTypeEnum = CategoryType.EXPENSE_CATEGORY;
                message = Constants.EXPENSE_CATEGORY_NAME_MESSAGE;
            }
            else
            {
                return;
            }

            Page newPage = new NewCategoryPage(categoryTypeEnum);
            MessagingCenter.Unsubscribe<NewCategoryPage, string>(this, message);
            MessagingCenter.Subscribe<NewCategoryPage, string>(this, message, (messageSender, arg) =>
            {
                string name = arg;
                SharedObject.dbManager.AddDataSettingsItem(this.categoryType, name);
                loadCategoriesFromDB();

                MessagingCenter.Unsubscribe<NewCategoryPage, string>(this, message);
            });

            Navigation.PushAsync(newPage);
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
                    DataSettings deleteCategory = (DataSettings)pickerCategory.SelectedItem;
                    SharedObject.dbManager.DeleteSettingsItem(deleteCategory);
                    loadCategoriesFromDB();
                }
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

            if (amountEntered && categorySelected && daySelected)
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

            RegularData regularDataItem;
            if(this.modifyItem)
            {
                regularDataItem = this.regularDataItem;
            }
            else
            {
                regularDataItem = new RegularData();
            }

            regularDataItem.Amount = amount;
            regularDataItem.Comment = entryMemo.Text;
            regularDataItem.Category = pickerCategory.SelectedItem.ToString();
            regularDataItem.CategoryType = this.categoryType;
            regularDataItem.Day = int.Parse(pickerDay.SelectedItem.ToString());

            if (this.modifyItem)
            {
                SharedObject.dbManager.ModifyRegularDataItem(regularDataItem);
                MessagingCenter.Send<RegularDataItemPage>(this, Constants.EDITED_REGULAR_DATA_ITEM_MESSAGE);
            }
            else
            {
                SharedObject.dbManager.AddRegularDataItem(regularDataItem);
                MessagingCenter.Send<RegularDataItemPage>(this, Constants.ADDED_REGULAR_DATA_ITEM_MESSAGE);
            }

            this.Navigation.PopAsync();
        }
    }
}