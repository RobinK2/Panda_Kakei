using System;
using System.Collections.Generic;
using System.Text;
using SQLite;

namespace Panda_Kakei.Models
{
    [Table(Constants.REGULAR_DATA_ITEMS_STRING)]
    public class RegularData
    {
        private string message;

        [PrimaryKey, AutoIncrement, Column(Constants.ID_STRING)]
        public int ID { get; set; }

        [Column(Constants.DAY_STRING), NotNull]
        public int Day { get; set; }

        [Column(Constants.CATEGORY_TYPE_STRING), NotNull]
        public string CategoryType { get; set; }

        [Column(Constants.CATEGORY_STRING), NotNull]
        public string Category { get; set; }

        [Column(Constants.AMOUNT_STRING), NotNull]
        public int Amount { get; set; }

        [Column(Constants.CURRENCY_STRING)]
        public string Currency { get; set; }

        [Column(Constants.COMMENT_STRING)]
        public string Comment { get; set; }

        [Column(Constants.LAST_ADDED_MONTH_STRING)]
        public string LastAddedMonth { get; set; }

        public void SetMessage(string message)
        {
            this.message = message;
        }

        public override string ToString()
        {
            return this.message;
        }
    }
}
