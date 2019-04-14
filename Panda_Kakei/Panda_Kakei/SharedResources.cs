using System;
using System.Collections.Generic;
using System.Text;

namespace Panda_Kakei
{
    public enum CategoryType
    {
        EXPENSE_CATEGORY,
        INCOME_CATEGORY
    }

    public static class Constants
    {
        public const string EXPENSE_CATEGORY_NAME_MESSAGE = "EXPENSE_CATEGORY_NAME";
        public const string INCOME_CATEGORY_NAME_MESSAGE = "INCOME_CATEGORY_NAME";
        public const string EDIT_DATA_ITEM_MESSAGE = "EDIT_DATA_ITEM";
        public const string REFRESH_MAIN_PAGE_BALANCE_MESSAGE = "REFRESH_MAIN_PAGE_BALANCE";

        public const string EXPENSE_STRING = "expense";
        public const string INCOME_STRING = "income";
        public const string ID_STRING = "id";
        public const string CATEGORIES_STRING = "categories";
        public const string CATEGORY_STRING = "category";
        public const string NAME_STRING = "name";
        public const string DATA_ITEMS_STRING = "data_items";
        public const string DAY_STRING = "day";
        public const string MONTH_STRING = "month";
        public const string YEAR_STRING = "year";
        public const string CATEGORY_TYPE_STRING = "category_type";
        public const string AMOUNT_STRING = "amount";
        public const string CURRENCY_STRING = "currency";
        public const string COMMENT_STRING = "comment";
        public const string SETTINGS_STRING = "settings";
        public const string COLOR_STRING = "color";
    }

    public static class Paths
    {
        public static string DATABASE_PATH = string.Empty;
        public static string DATABASE_EXTERNAL_STORAGE_PATH = string.Empty;
        public static string DEFAULT_DATABASE_EXTERNAL_STORAGE_PATH = string.Empty;
        public static string EXCEL_STORAGE_PATH = string.Empty;
    }
    
    public static class SharedObject
    {
        public static Services.DataManager dbManager;
        public static string currencySymbol = string.Empty;
    }
}
