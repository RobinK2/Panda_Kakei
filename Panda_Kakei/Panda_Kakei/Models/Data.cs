using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;

namespace Panda_Kakei.Models
{
    [Table(Constants.DATA_ITEMS_STRING)]
    public class Data
    {
        private string message;

        [PrimaryKey, AutoIncrement, Column(Constants.ID_STRING)]
        public int ID { get; set; }     

        [Column(Constants.DAY_STRING)]
        public int Day { get; set; }

        [Column(Constants.MONTH_STRING), NotNull]
        public string Month { get; set; }

        [Column(Constants.YEAR_STRING), NotNull]
        public string Year { get; set; }

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
