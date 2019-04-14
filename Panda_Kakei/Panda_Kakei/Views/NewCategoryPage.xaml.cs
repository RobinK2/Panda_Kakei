using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Panda_Kakei.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class NewCategoryPage : ContentPage
    {
        private CategoryType categoryType;

        public NewCategoryPage (CategoryType categoryType)
		{
			InitializeComponent ();
            this.categoryType = categoryType;
		}

        private void btnCancel_OnClicked(object sender, EventArgs e)
        {
            Navigation.PopAsync();
        }

        private void btnAdd_OnClicked(object sender, EventArgs e)
        {
            if(!string.IsNullOrEmpty(entryCategoryName.Text))
            {
                if(this.categoryType == CategoryType.EXPENSE_CATEGORY)
                {
                    MessagingCenter.Send<NewCategoryPage, string>(this, Constants.EXPENSE_CATEGORY_NAME_MESSAGE, entryCategoryName.Text);
                }
                else
                {
                    MessagingCenter.Send<NewCategoryPage, string>(this, Constants.INCOME_CATEGORY_NAME_MESSAGE, entryCategoryName.Text);
                }

                Navigation.PopAsync();
            }
        }
    }
}