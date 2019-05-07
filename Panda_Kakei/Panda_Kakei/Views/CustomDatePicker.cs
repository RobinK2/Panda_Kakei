using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Reflection;
using System.Resources;
using Syncfusion.SfPicker.XForms;
using Xamarin.Forms;

namespace Panda_Kakei
{
    public class CustomDatePicker : SfPicker
    {
        #region Public Properties

        /// <summary>
        /// Date is the acutal DataSource for SfPicker control which will holds the collection of Day ,Month and Year
        /// </summary>
        /// <value>The date.</value>
        public ObservableCollection<object> Date { get; set; }

        //Year is the collection of Years from 1990 to 2042
        internal ObservableCollection<object> Year { get; set; }

        //Month is the collection of Month Names
        internal ObservableCollection<object> Month { get; set; }

        /// <summary>
        /// Headers api is holds the column name for every column in date picker
        /// </summary>
        /// <value>The Headers.</value>
        public ObservableCollection<string> Headers { get; set; }

        #endregion

        public CustomDatePicker()
        {
            ResourceManager rm = new ResourceManager("Panda_Kakei.Resources.AppResource", typeof(TranslateExtension).GetTypeInfo().Assembly);

            Date = new ObservableCollection<object>();
            Month = new ObservableCollection<object>();
            Year = new ObservableCollection<object>();
            Headers = new ObservableCollection<string>
            {
                rm.GetString("YearText", CultureInfo.CurrentCulture),
                rm.GetString("MonthText", CultureInfo.CurrentCulture)
            };
            populateDateCollection();
            this.ItemsSource = Date;
            this.ColumnHeaderText = Headers;
            //this.SelectionChanged += customDatePicker_SelectionChanged;
            ShowFooter = true;
            ShowHeader = false;
            ShowColumnHeader = true;
        }

        //private void customDatePicker_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
        //}
        
        private void populateDateCollection()
        {
            int startYear = 1990;
            int endYear = DateTime.Now.Date.Year + 20;

            //populate months
            for (int i = 1; i < 13; i++)
            {
                Month.Add(CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(i));
            }

            //populate year
            for (int i = startYear; i < endYear; i++)
            {
                Year.Add(i.ToString());
            }

            Date.Add(Year);
            Date.Add(Month);
        }
    }
}
