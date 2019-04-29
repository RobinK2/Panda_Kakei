using System;
using System.Collections.Generic;
using System.Linq;
using SQLite;
using Panda_Kakei.Models;

namespace Panda_Kakei.Services
{
    public enum SortType
    {
        DAY_ASCENDING,
        DAY_DESCENDING,
        CATEGORY_ASCENING,
        CATEGORY_DESCENDING,
        AMOUNT_ASCENDING,
        AMOUNT_DESCENDING
    }

    /// <summary>
    /// Manages SQLite database
    /// </summary>
    public class DataManager
    {
        private SQLiteConnection dbConnection;
        public string ErrorStatusMessage { get; set; }

        public DataManager(string dbPath)
        {
            try
            {
                dbConnection = new SQLiteConnection(dbPath);
                dbConnection.CreateTable<DataSettings>();
                dbConnection.CreateTable<Data>();
                dbConnection.CreateTable<AppSettings>();
                dbConnection.CreateTable<RegularData>();
            }
            catch(Exception e)
            {
                ErrorStatusMessage = string.Format("Failed to open database. Error: {0}", e.Message);
            }
        }

        /// <summary>
        //  Add app settings to settings table. This shall only be called once when the application executes first time.
        /// </summary>
        /// <param name="currencySymbol"></param>
        public void AddAppSettings(string currencySymbol)
        {
            try
            {
                dbConnection.Insert(new AppSettings
                {
                    Currency = currencySymbol
                });
            }
            catch (Exception e)
            {
                ErrorStatusMessage = string.Format("Failed to add {0}. Error: {1}", currencySymbol, e.Message);
            }
        }

        public void AddDataSettingsItem(string categoryType, string name)
        {
            try
            {
                dbConnection.Insert(new DataSettings
                {
                    CategoryType = categoryType,
                    Name = name
                });
            }
            catch (Exception e)
            {
                ErrorStatusMessage = string.Format("Failed to add {0}. Error: {1}", name, e.Message);
            }
        }

        public void AddRegularDataItem(RegularData regularData)
        {
            try
            {
                dbConnection.Insert(regularData);
            }
            catch (Exception e)
            {
                ErrorStatusMessage = string.Format("Failed to add regular data item. Error: {0}", e.Message);
            }
        }

        public void Commit()
        {
            this.dbConnection.Commit();
        }

        public void Close()
        {
            this.dbConnection.Close();
        }

        public void ModifySettingsItem(DataSettings settingsItem)
        {
            try
            {
                dbConnection.Update(settingsItem);
            }
            catch (Exception e)
            {
                ErrorStatusMessage = string.Format("Failed to Modify data. {0}", e.Message);
            }
        }

        public void DeleteSettingsItem(DataSettings settingsItem)
        {
            try
            {
                dbConnection.Delete(settingsItem);
            }
            catch (Exception e)
            {
                ErrorStatusMessage = string.Format("Failed to delete data. {0}", e.Message);
            }
        }

        public void DeleteSettingsItem(string categoryType, string name)
        {
            try
            {
                string query = string.Format("DELETE FROM {0} WHERE {1} = \"{2}\" AND {3} = \"{4}\"",
                    Constants.CATEGORIES_STRING, Constants.CATEGORY_TYPE_STRING, categoryType,
                    Constants.NAME_STRING, name);

                dbConnection.Query<DataSettings>(query);
            }
            catch (Exception e)
            {
                ErrorStatusMessage = string.Format("Failed to delete data. {0}", e.Message);
            }
        }

        public List<DataSettings> GetAllIncomeSettingsItems()
        {
            try
            {
                string query = string.Format("SELECT * FROM {0} WHERE {1} = \"{2}\"", Constants.CATEGORIES_STRING,
                    Constants.CATEGORY_TYPE_STRING, Constants.INCOME_STRING);

                return dbConnection.Query<DataSettings>(query);
            }
            catch (Exception e)
            {
                ErrorStatusMessage = string.Format("Failed to receive data. {0}", e.Message);
            }
            return new List<DataSettings>();
        }

        public List<DataSettings> GetAllExpenseSettingsItems()
        {
            try
            {
                string query = string.Format("SELECT * FROM {0} WHERE {1} = \"{2}\"", Constants.CATEGORIES_STRING,
                    Constants.CATEGORY_TYPE_STRING, Constants.EXPENSE_STRING);

                return dbConnection.Query<DataSettings>(query);
            }
            catch (Exception e)
            {
                ErrorStatusMessage = string.Format("Failed to receive data. {0}", e.Message);
            }
            return new List<DataSettings>();
        }

        public List<DataSettings> GetAllSettingsItems(string categoryType)
        {
            try
            {
                string query = string.Format("SELECT * FROM {0} WHERE {1} = \"{2}\"", Constants.CATEGORIES_STRING,
                    Constants.CATEGORY_TYPE_STRING, categoryType);

                return dbConnection.Query<DataSettings>(query);
            }
            catch (Exception e)
            {
                ErrorStatusMessage = string.Format("Failed to receive data. {0}", e.Message);
            }
            return new List<DataSettings>();
        }

        public List<RegularData> GetAllRegularDataItems(string categoryType)
        {
            try
            {
                string query = string.Format("SELECT * FROM {0} WHERE {1} = \"{2}\"", Constants.REGULAR_DATA_ITEMS_STRING,
                    Constants.CATEGORY_TYPE_STRING, categoryType);

                return dbConnection.Query<RegularData>(query);
            }
            catch (Exception e)
            {
                ErrorStatusMessage = string.Format("Failed to receive regular data. Error: {0}", e.Message);
            }
            return new List<RegularData>();
        }

        public string GetCurrencySettings()
        {
            try
            {
                return dbConnection.Get<AppSettings>(1).Currency;
            }
            catch (Exception e)
            {
                ErrorStatusMessage = string.Format("Failed to receive data. {0}", e.Message);
            }
            return string.Empty;
        }

        public int GetAppSettingsCount()
        {
            return dbConnection.Table<AppSettings>().Count();
        }

        public void AddDataItem(string month, string year, string categoryType, string category,
            int amount, string currency, string comment)
        {
            try
            {
                dbConnection.Insert(new Data
                {
                    Month = month,
                    Year = year,
                    CategoryType = categoryType,
                    Category = category,
                    Amount = amount,
                    Currency = currency,
                    Comment = comment
                });
            }
            catch (Exception e)
            {
                ErrorStatusMessage = string.Format("Failed to add data item. Error: {0}", e.Message);
            }
        }

        public void AddDataItem(Data item)
        {
            try
            {
                dbConnection.Insert(item);
            }
            catch (Exception e)
            {

                ErrorStatusMessage = string.Format("Failed to add data item. Error: {0}", e.Message);
            }
        }

        public void ModifyDataItem(Data dataItem)
        {
            try
            {
                dbConnection.Update(dataItem);
            }
            catch (Exception e)
            {
                ErrorStatusMessage = string.Format("Failed to modify data item. Error: {0}", e.Message);
            }
        }

        public void ModifyRegularDataItem(RegularData regularData)
        {
            try
            {
                dbConnection.Update(regularData);
            }
            catch (Exception e)
            {
                ErrorStatusMessage = string.Format("Failed to modify regular data item. Error: {0}", e.Message);
            }
        }

        public void ModifyCurrencySettings(string newCurrencySymbol)
        {
            AppSettings appSettings = dbConnection.Get<AppSettings>(1);
            appSettings.Currency = newCurrencySymbol;
            dbConnection.Update(appSettings);
        }

        public void DeleteDataItem(Data dataItem)
        {
            try
            {
                dbConnection.Delete(dataItem);
            }
            catch (Exception e)
            {
                ErrorStatusMessage = string.Format("Failed to delete data item. Error: {0}", e.Message);
            }
        }

        public void DeleteRegularDataItem(RegularData regularData)
        {
            try
            {
                dbConnection.Delete(regularData);
            }
            catch (Exception e)
            {
                ErrorStatusMessage = string.Format("Failed to delete regular data item. Error: {0}", e.Message);
            }
        }

        public void DeleteAllDataItems()
        {
            dbConnection.DeleteAll<Data>();
        }

        public List<Data> GetDataItemsOfPeriod(string month, string year)
        {
            try
            {
                string query = string.Format("SELECT * FROM {0} WHERE {1} = \"{2}\" AND {3} = \"{4}\"",
                    Constants.DATA_ITEMS_STRING, Constants.YEAR_STRING, year, Constants.MONTH_STRING, month);

                return dbConnection.Query<Data>(query);
            }
            catch (Exception e)
            {
                ErrorStatusMessage = string.Format("Failed to receive data. {0}", e.Message);
            }
            return new List<Data>();
        }

        public List<Data> GetDataItemsOfPeriodSortedBy(string month, string year, SortType sortType)
        {
            string query;
            switch(sortType)
            {
                case SortType.DAY_ASCENDING:
                    query = string.Format("SELECT * FROM {0} WHERE {1} = \"{2}\" AND {3} = \"{4}\" ORDER BY CAST( {5} AS INT) ASC",
                        Constants.DATA_ITEMS_STRING, Constants.YEAR_STRING, year, Constants.MONTH_STRING, month,
                        Constants.DAY_STRING);
                    break;
                case SortType.DAY_DESCENDING:
                    query = string.Format("SELECT * FROM {0} WHERE {1} = \"{2}\" AND {3} = \"{4}\" ORDER BY CAST( {5} AS INT) DESC",
                        Constants.DATA_ITEMS_STRING, Constants.YEAR_STRING, year, Constants.MONTH_STRING, month,
                        Constants.DAY_STRING);
                    break;
                case SortType.CATEGORY_ASCENING:
                    query = string.Format("SELECT * FROM {0} WHERE {1} = \"{2}\" AND {3} = \"{4}\" ORDER BY {5} ASC",
                        Constants.DATA_ITEMS_STRING, Constants.YEAR_STRING, year, Constants.MONTH_STRING, month,
                        Constants.CATEGORY_STRING);
                    break;
                case SortType.CATEGORY_DESCENDING:
                    query = string.Format("SELECT * FROM {0} WHERE {1} = \"{2}\" AND {3} = \"{4}\" ORDER BY {5} DESC",
                        Constants.DATA_ITEMS_STRING, Constants.YEAR_STRING, year, Constants.MONTH_STRING, month,
                        Constants.CATEGORY_STRING);
                    break;
                case SortType.AMOUNT_ASCENDING:
                    query = string.Format("SELECT * FROM {0} WHERE {1} = \"{2}\" AND {3} = \"{4}\" ORDER BY {5} ASC",
                        Constants.DATA_ITEMS_STRING, Constants.YEAR_STRING, year, Constants.MONTH_STRING, month,
                        Constants.AMOUNT_STRING);
                    break;
                case SortType.AMOUNT_DESCENDING:
                    query = string.Format("SELECT * FROM {0} WHERE {1} = \"{2}\" AND {3} = \"{4}\" ORDER BY {5} DESC",
                        Constants.DATA_ITEMS_STRING, Constants.YEAR_STRING, year, Constants.MONTH_STRING, month,
                        Constants.AMOUNT_STRING);
                    break;
                default:
                    query = string.Empty;
                    break;
            }

            try
            {
                return dbConnection.Query<Data>(query);
            }
            catch (Exception e)
            {
                ErrorStatusMessage = string.Format("Failed to recevie data. {0}", e.Message);
            }
            return new List<Data>();
        }
    }
}
