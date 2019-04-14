using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace Panda_Kakei.Models
{
    [Table(Constants.SETTINGS_STRING)]
    class AppSettings
    {
        [PrimaryKey, AutoIncrement, Column(Constants.ID_STRING)]
        public int ID { get; set; }

        [Column(Constants.COLOR_STRING)]
        public string Color { get; set; }

        [Column(Constants.CURRENCY_STRING)]
        public string Currency { get; set; }
    }
}
