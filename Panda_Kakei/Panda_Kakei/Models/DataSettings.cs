using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;

namespace Panda_Kakei.Models
{
    [Table(Constants.CATEGORIES_STRING)]
    public class DataSettings
    {
        [PrimaryKey, AutoIncrement, Column(Constants.ID_STRING)]
        public int ID { get; set; }

        [Column(Constants.CATEGORY_TYPE_STRING), NotNull]
        public string CategoryType { get; set; }

        [Column(Constants.NAME_STRING), NotNull]
        public string Name { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}
