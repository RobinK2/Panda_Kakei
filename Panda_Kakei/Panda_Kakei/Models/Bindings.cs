﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Text;
using Panda_Kakei.Services;

namespace Panda_Kakei.Models
{
    public class CategoryModel
    {
        public ObservableCollection<DataSettings> Categories { get; set; }

        public CategoryModel()
        {
            Categories = new ObservableCollection<DataSettings>();
        }
    }

    public class Item
    {
        public Item()
        {

        }

        public Item(Data dbDataItem)
        {
            DbDataItem = dbDataItem;

            if (dbDataItem.CategoryType == Constants.INCOME_STRING)
            {
                Type = CategoryType.INCOME_CATEGORY;
                TypeText = Panda_Kakei.Resources.AppResource.IncomeText;
            }
            else if (dbDataItem.CategoryType == Constants.EXPENSE_STRING)
            {
                Type = CategoryType.EXPENSE_CATEGORY;
                TypeText = Panda_Kakei.Resources.AppResource.ExpenseText;
            }

            CategoryText = dbDataItem.Category;
            AmountText = dbDataItem.Amount.ToString();
            MemoText = dbDataItem.Comment;

            DateTime date = new DateTime(int.Parse(dbDataItem.Year), int.Parse(dbDataItem.Month),
                dbDataItem.Day);
            if(CultureInfo.CurrentUICulture.TwoLetterISOLanguageName == "ja")
            {
                DateText = date.ToString("m");
            }
            else
            {
                DateText = date.ToString("d MMM");
            }
        }

        public Data DbDataItem { get; set; }

        public CategoryType Type { get; set; }
        public string TypeText { get; set; }
        public string CategoryText { get; set; }
        public string AmountText { get; set; }
        public string MemoText { get; set; }
        public string DateText { get; set; }
    }

    public class ItemModel
    {
        public ObservableCollection<Item> Items { get; }
        public ItemModel()
        {
            Items = new ObservableCollection<Item>();
        }
    }

    public class SortOption
    {
        public enum SortOptionSelection
        {
            Date,
            Category,
            Amount
        }

        public SortOptionSelection Value { get; set; }
        public string Name { get; set; }
    }

    public class SortOptionModel
    {
        public ObservableCollection<SortOption> SortOptions { get; }
        public SortOptionModel()
        {
            SortOptions = new ObservableCollection<SortOption>
            {
                new SortOption
                {
                    Value = SortOption.SortOptionSelection.Date,
                    Name = Panda_Kakei.Resources.AppResource.DateText
                },
                new SortOption
                {
                    Value = SortOption.SortOptionSelection.Category,
                    Name = Panda_Kakei.Resources.AppResource.CategoryText
                },
                new SortOption
                {
                    Value = SortOption.SortOptionSelection.Amount,
                    Name = Panda_Kakei.Resources.AppResource.AmountText
                }
            };

        }
    }

    public class RegularDataItem
    {
        public RegularDataItem()
        {

        }

        public RegularDataItem(RegularData regularData)
        {
            DbRegDataItem = regularData;
            if(regularData.CategoryType == Constants.INCOME_STRING)
            {
                Type = CategoryType.INCOME_CATEGORY;
                TypeText = Panda_Kakei.Resources.AppResource.IncomeText;
            }
            else if(regularData.CategoryType == Constants.EXPENSE_STRING)
            {
                Type = CategoryType.EXPENSE_CATEGORY;
                TypeText = Panda_Kakei.Resources.AppResource.ExpenseText;
            }

            CategoryText = regularData.Category;
            AmountText = regularData.Amount.ToString();
            MemoText = regularData.Comment;
            DateTime date = new DateTime(DateTime.Today.Year, DateTime.Today.Month, regularData.Day);          
            DayText = date.Day.ToString("d"); ;
        }

        public RegularData DbRegDataItem { get; set; }               
        public CategoryType Type { get; set; }
        public string TypeText { get; set; }
        public string CategoryText { get; set; }
        public string AmountText { get; set; }
        public string MemoText { get; set; }
        public string DayText { get; set; }
    }

    public class RegularDataItemModel
    {
        public ObservableCollection<RegularDataItem> RegularDataItems { get; }

        public RegularDataItemModel()
        {
            RegularDataItems = new ObservableCollection<RegularDataItem>();
        }
    }

    public class DatePickerViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<object> _startdate;

        public ObservableCollection<object> StartDate
        {
            get { return _startdate; }
            set { _startdate = value; raisePropertyChanged("StartDate"); }
        }

        private ObservableCollection<object> _enddate;

        public ObservableCollection<object> EndDate
        {
            get { return _enddate; }
            set { _enddate = value; raisePropertyChanged("EndDate"); }
        }

        public DatePickerViewModel()
        {
            ObservableCollection<object> todaycollection = new ObservableCollection<object>
            {

                //Select today dates
                DateTime.Now.Date.Year.ToString(),
                System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(DateTime.Now.Date.Month)
            };

            this.StartDate = todaycollection;
            this.EndDate = todaycollection;
        }

        private void raisePropertyChanged(string name)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
