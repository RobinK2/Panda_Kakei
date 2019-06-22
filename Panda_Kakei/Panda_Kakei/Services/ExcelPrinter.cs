using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Panda_Kakei.Models;
using Syncfusion.XlsIO;
using Panda_Kakei.Resources;
using System.Globalization;

namespace Panda_Kakei.Services
{
    class ExcelPrinter
    {
        private uint monthStart;
        private uint yearStart;
        private uint monthEnd;
        private uint yearEnd;
        
        public ExcelPrinter(string monthStart, string yearStart,
            string monthEnd, string yearEnd)
        {
            this.monthStart = uint.Parse(monthStart);
            this.yearStart = uint.Parse(yearStart);
            this.monthEnd = uint.Parse(monthEnd);
            this.yearEnd = uint.Parse(yearEnd);
        }
        
        public async Task PrintExcel(string filename)
        {
            //Create an instance of ExcelEngine.
            using (ExcelEngine excelEngine = new ExcelEngine())
            {
                //Set the default application version as Excel 2013.
                excelEngine.Excel.DefaultVersion = ExcelVersion.Excel2013;

                //Create a workbook with a worksheet
                IWorkbook workbook = excelEngine.Excel.Workbooks.Create(1);
                workbook.CalculationOptions.CalculationMode = ExcelCalculationMode.Automatic;

                //Access first worksheet from the workbook instance.
                IWorksheet worksheetTotal = workbook.Worksheets[0];
                worksheetTotal.Name = AppResource.ExcelTotalizationString;

                worksheetTotal.EnableSheetCalculations();
                uint monthCursor = this.monthStart;
                uint yearCursor = this.yearStart;
                Dictionary<string, List<string>> incomeByCategoryCellMap = new Dictionary<string, List<string>>();
                Dictionary<string, List<string>> expenseByCategoryCellMap = new Dictionary<string, List<string>>();

                while (checkStartPeriodExcess(monthCursor, yearCursor, this.monthEnd, this.yearEnd))
                {
                    IWorksheet worksheetMonth = workbook.Worksheets.Create();
                    worksheetMonth.EnableSheetCalculations();

                    printMonthDetail(monthCursor, yearCursor, SharedObject.currencySymbol,
                        worksheetMonth, incomeByCategoryCellMap, expenseByCategoryCellMap);

                    if (12 == monthCursor)
                    {
                        monthCursor = 1;
                        yearCursor++;
                    }
                    else
                    {
                        monthCursor++;
                    }
                }

                printSummaryCells(worksheetTotal, incomeByCategoryCellMap, expenseByCategoryCellMap,
                    SharedObject.currencySymbol);

                //Save the workbook to stream in xlsx format. 
                MemoryStream stream = new MemoryStream();
                workbook.SaveAs(stream);

                workbook.Close();

                //Save the stream as a file in the device and invoke it for viewing
                await Xamarin.Forms.DependencyService.Get<ISave>().SaveAndView(filename, "application/msexcel", stream);
            }
        }

        private string generateSumString(in Dictionary<string, List<string>> cellMap, string key)
        {
            string sum = "=";
            foreach(string cell in cellMap[key])
            {
                sum += cell + " + ";
            }
            sum = sum.Remove(sum.Length - 3, 3);

            return sum;
        }

        private void printSummaryCells(IWorksheet worksheetTotal, Dictionary<string, List<string>> incomeByCategorySummaryMap,
            Dictionary<string, List<string>> expenseByCategorySummarylMap, string currencySymbol)
        {
            const int INCOME_BY_CATEGORY_START_ROW = 5;
            const int EXPENSE_BY_CATEGORY_START_ROW = 5;
            const string INCOME_CATEGORY_COL = "A";
            const string INCOME_AMOUNT_COL = "B";
            const string INCOME_CURRENCY_COL = "C";
            const string EXPENSE_CATEGORY_COL = "E";
            const string EXPENSE_AMOUNT_COL = "F";
            const string EXPENSE_CURRENCY_COL = "G";

            int incomeRowIndex = INCOME_BY_CATEGORY_START_ROW;
            int expenseRowIndex = EXPENSE_BY_CATEGORY_START_ROW;
            string title;
            if (CultureInfo.CurrentCulture.Name == "ja")
            {
                if ((this.monthStart == this.monthEnd) && (this.yearStart == this.yearEnd))
                {
                    title = this.monthStart + AppResource.MonthText + this.yearStart + AppResource.YearText;
                }
                else
                {
                    title = this.monthStart + AppResource.MonthText + this.yearStart + AppResource.YearText + " - " + 
                        this.monthEnd + AppResource.MonthText + this.yearEnd + AppResource.YearText;
                }
            }
            else
            {
                if ((this.monthStart == this.monthEnd) && (this.yearStart == this.yearEnd))
                {
                    title = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName((int)this.monthStart) + " " +
                        this.yearStart;
                }
                else
                {
                    title = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName((int)this.monthStart) + " " +
                        this.yearStart + " - " + CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName((int)this.monthEnd) +
                        " " + this.yearEnd;
                }
            }

            worksheetTotal.Range[INCOME_CATEGORY_COL + 3].Text = AppResource.IncomeByCategoryText;
            worksheetTotal.Range[INCOME_CATEGORY_COL + 4].Text = AppResource.CategoryText;
            worksheetTotal.Range[INCOME_CATEGORY_COL + 4].CellStyle.Font.Bold = true;
            worksheetTotal.Range[INCOME_AMOUNT_COL + 4].Text = AppResource.AmountText;
            worksheetTotal.Range[INCOME_AMOUNT_COL + 4].CellStyle.Font.Bold = true;
            worksheetTotal.Range[INCOME_CURRENCY_COL + 4].Text = AppResource.ExcelCurrencyColumnText;
            worksheetTotal.Range[INCOME_CURRENCY_COL + 4].CellStyle.Font.Bold = true;

            worksheetTotal.Range[EXPENSE_CATEGORY_COL + 3].Text = AppResource.ExpenseByCategoryText;
            worksheetTotal.Range[EXPENSE_CATEGORY_COL + 4].Text = AppResource.CategoryText;
            worksheetTotal.Range[EXPENSE_CATEGORY_COL + 4].CellStyle.Font.Bold = true;
            worksheetTotal.Range[EXPENSE_AMOUNT_COL + 4].Text = AppResource.AmountText;
            worksheetTotal.Range[EXPENSE_AMOUNT_COL + 4].CellStyle.Font.Bold = true;
            worksheetTotal.Range[EXPENSE_CURRENCY_COL + 4].Text = AppResource.ExcelCurrencyColumnText;
            worksheetTotal.Range[EXPENSE_CURRENCY_COL + 4].CellStyle.Font.Bold = true;

            foreach (string key in incomeByCategorySummaryMap.Keys)
            {
                string incomeSum = generateSumString(incomeByCategorySummaryMap, key);
                worksheetTotal.Range[INCOME_CATEGORY_COL + incomeRowIndex].Text = key;
                worksheetTotal.Range[INCOME_AMOUNT_COL + incomeRowIndex].Formula = incomeSum;
                worksheetTotal.Range[INCOME_CURRENCY_COL + incomeRowIndex].Text = currencySymbol;
                incomeRowIndex++;
            }
            foreach (string key in expenseByCategorySummarylMap.Keys)
            {
                string expenseSum = generateSumString(expenseByCategorySummarylMap, key);
                worksheetTotal.Range[EXPENSE_CATEGORY_COL + expenseRowIndex].Text = key;
                worksheetTotal.Range[EXPENSE_AMOUNT_COL + expenseRowIndex].Formula = expenseSum;
                worksheetTotal.Range[EXPENSE_CURRENCY_COL + expenseRowIndex].Text = currencySymbol;
                expenseRowIndex++;
            }

            int totalRowIndex = Math.Max(incomeRowIndex, expenseRowIndex) + 1;
            incomeRowIndex--;
            expenseRowIndex--;
            string incomeOverPeriod = "=SUM($" + INCOME_AMOUNT_COL + "$" + INCOME_BY_CATEGORY_START_ROW +
                ":$" + INCOME_AMOUNT_COL + "$" + incomeRowIndex + ")";

            string expenseOverPeriod = "=SUM($" + EXPENSE_AMOUNT_COL + "$" + EXPENSE_BY_CATEGORY_START_ROW +
                ":$" + EXPENSE_AMOUNT_COL + "$" + expenseRowIndex + ")";

            string balanceOverPeriod = "=$" + INCOME_AMOUNT_COL + "$" + totalRowIndex + "-$" +
                EXPENSE_AMOUNT_COL + "$" + totalRowIndex;

            worksheetTotal.Range[INCOME_CATEGORY_COL + 1].Text = title;
            worksheetTotal.Range[INCOME_CATEGORY_COL + 1].CellStyle.Font.Bold = true;

            worksheetTotal.Range[INCOME_CATEGORY_COL + totalRowIndex].Text = AppResource.TotalText;
            worksheetTotal.Range[INCOME_CATEGORY_COL + totalRowIndex].CellStyle.Font.Bold = true;
            worksheetTotal.Range[INCOME_AMOUNT_COL + totalRowIndex].Formula = incomeOverPeriod;
            worksheetTotal.Range[INCOME_CURRENCY_COL + totalRowIndex].Text = currencySymbol;
            worksheetTotal.Range[EXPENSE_CATEGORY_COL + totalRowIndex].Text = AppResource.TotalText;
            worksheetTotal.Range[EXPENSE_CATEGORY_COL + totalRowIndex].CellStyle.Font.Bold = true;
            worksheetTotal.Range[EXPENSE_AMOUNT_COL + totalRowIndex].Formula = expenseOverPeriod;
            worksheetTotal.Range[EXPENSE_CURRENCY_COL + totalRowIndex].Text = currencySymbol;

            worksheetTotal.Range[INCOME_CATEGORY_COL + (totalRowIndex + 2)].Text = AppResource.BalanceText;
            worksheetTotal.Range[INCOME_CATEGORY_COL + (totalRowIndex + 2)].CellStyle.Font.Bold = true;
            worksheetTotal.Range[INCOME_AMOUNT_COL + (totalRowIndex + 2)].Formula = balanceOverPeriod;
            worksheetTotal.Range[INCOME_CURRENCY_COL + (totalRowIndex + 2)].Text = currencySymbol;
        }

        private void printMonthDetail(uint month, uint year, string currencySymbol, IWorksheet worksheetMonth,
            Dictionary<string, List<string>> incomeByCategoryCellMap, Dictionary<string, List<string>> expenseByCategoryCellMap)
        {
            List<Data> items = SharedObject.dbManager.GetDataItemsOfPeriod(month.ToString(), year.ToString());
            string sheetName = string.Empty;
            if (CultureInfo.CurrentCulture.Name == "ja")
            {
                sheetName = month + AppResource.MonthText + year + AppResource.YearText;
            }
            else
            {
                sheetName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName((int) month) + " " + year;
            }

            worksheetMonth.Name = sheetName;

            const int ROW_START_INDEX = 5;
            const string INCOME_CATEGORY_COL = "A";
            const string INCOME_AMOUNT_COL = "B";
            const string INCOME_CURRENCY_COL = "C";
            const string EXPENSE_CATEGORY_COL = "E";
            const string EXPENSE_AMOUNT_COL = "F";
            const string EXPENSE_CURRENCY_COL = "G";
            const string INCOME_ITEM_CATEGORY_COL = "I";
            const string INCOME_ITEM_AMOUNT_COL = "J";
            const string INCOME_ITEM_CURRENCY_COL = "K";
            const string INCOME_ITEM_MEMO_COL = "L";
            const string EXPENSE_ITEM_CATEGORY_COL = "N";
            const string EXPENSE_ITEM_AMOUNT_COL = "O";
            const string EXPENSE_ITEM_CURRENCY_COL = "P";
            const string EXPENSE_ITEM_MEMO_COL = "Q";

            worksheetMonth.Range["A1"].Text = sheetName;
            worksheetMonth.Range["A1"].CellStyle.Font.Bold = true;

            worksheetMonth.Range[INCOME_CATEGORY_COL + "3"].Text = AppResource.IncomeByCategoryText;
            worksheetMonth.Range[EXPENSE_CATEGORY_COL + "3"].Text = AppResource.ExpenseByCategoryText;
            worksheetMonth[INCOME_ITEM_CATEGORY_COL + "3"].Text = AppResource.IncomeText;
            worksheetMonth[EXPENSE_ITEM_CATEGORY_COL + "3"].Text = AppResource.ExpenseText;

            worksheetMonth.Rows[2].CellStyle.Font.Bold = true;

            worksheetMonth.Range[INCOME_CATEGORY_COL + "4"].Text = AppResource.CategoryText;
            worksheetMonth.Range[INCOME_AMOUNT_COL + "4"].Text = AppResource.AmountText;
            worksheetMonth.Range[INCOME_CURRENCY_COL + "4"].Text = AppResource.ExcelCurrencyColumnText;
            worksheetMonth.Range[EXPENSE_CATEGORY_COL + "4"].Text = AppResource.CategoryText;
            worksheetMonth.Range[EXPENSE_AMOUNT_COL + "4"].Text = AppResource.AmountText;
            worksheetMonth.Range[EXPENSE_CURRENCY_COL + "4"].Text = AppResource.ExcelCurrencyColumnText;
            worksheetMonth.Range[INCOME_ITEM_CATEGORY_COL + "4"].Text = AppResource.CategoryText;
            worksheetMonth.Range[INCOME_ITEM_AMOUNT_COL + "4"].Text = AppResource.AmountText;
            worksheetMonth.Range[INCOME_ITEM_CURRENCY_COL + "4"].Text = AppResource.ExcelCurrencyColumnText;
            worksheetMonth.Range[INCOME_ITEM_MEMO_COL + "4"].Text = AppResource.MemoText;
            worksheetMonth.Range[EXPENSE_ITEM_CATEGORY_COL + "4"].Text = AppResource.CategoryText;
            worksheetMonth.Range[EXPENSE_ITEM_AMOUNT_COL + "4"].Text = AppResource.AmountText;
            worksheetMonth.Range[EXPENSE_ITEM_CURRENCY_COL + "4"].Text = AppResource.ExcelCurrencyColumnText;
            worksheetMonth.Range[EXPENSE_ITEM_MEMO_COL + "4"].Text = AppResource.MemoText;

            worksheetMonth.Rows[3].CellStyle.Font.Bold = true;

            int incomeCategoryIndex = ROW_START_INDEX;
            int expenseCategoryIndex = ROW_START_INDEX;
            int incomeItemIndex = ROW_START_INDEX;
            int expenseItemIndex = ROW_START_INDEX;

            Dictionary<string, List<string>> incomeByCategoryMap = new Dictionary<string, List<string>>();
            Dictionary<string, List<string>> expenseByCategoryMap = new Dictionary<string, List<string>>();
            foreach (Data item in items)
            {
                if (Constants.INCOME_STRING == item.CategoryType)
                {
                    if (!incomeByCategoryMap.ContainsKey(item.Category))
                    {
                        string cell = "$" + INCOME_ITEM_AMOUNT_COL + "$" + incomeItemIndex;
                        List<string> cellList = new List<string>();
                        cellList.Add(cell);
                        incomeByCategoryMap.Add(item.Category, cellList);
                    }
                    else
                    {
                        string cell = "$" + INCOME_ITEM_AMOUNT_COL + "$" + incomeItemIndex;
                        incomeByCategoryMap[item.Category].Add(cell);
                    }

                    worksheetMonth.Range[INCOME_ITEM_CATEGORY_COL + incomeItemIndex.ToString()].Text = item.Category;
                    worksheetMonth.Range[INCOME_ITEM_AMOUNT_COL + incomeItemIndex.ToString()].Number = item.Amount;
                    worksheetMonth.Range[INCOME_ITEM_CURRENCY_COL + incomeItemIndex.ToString()].Text = currencySymbol;
                    worksheetMonth.Range[INCOME_ITEM_MEMO_COL + incomeItemIndex.ToString()].Text = item.Comment;

                    incomeItemIndex++;
                }
                else if (Constants.EXPENSE_STRING == item.CategoryType)
                {
                    if (!expenseByCategoryMap.ContainsKey(item.Category))
                    {
                        string cell = "$" + EXPENSE_ITEM_AMOUNT_COL + "$" + expenseItemIndex;
                        List<string> cellList = new List<string>();
                        cellList.Add(cell);
                        expenseByCategoryMap.Add(item.Category, cellList);
                    }
                    else
                    {
                        string cell = "$" + EXPENSE_ITEM_AMOUNT_COL + "$" + expenseItemIndex;
                        expenseByCategoryMap[item.Category].Add(cell);
                    }

                    worksheetMonth.Range[EXPENSE_ITEM_CATEGORY_COL + expenseItemIndex.ToString()].Text = item.Category;
                    worksheetMonth.Range[EXPENSE_ITEM_AMOUNT_COL + expenseItemIndex.ToString()].Number = item.Amount;
                    worksheetMonth.Range[EXPENSE_ITEM_CURRENCY_COL + expenseItemIndex.ToString()].Text = currencySymbol;
                    worksheetMonth.Range[EXPENSE_ITEM_MEMO_COL + expenseItemIndex.ToString()].Text = item.Comment;

                    expenseItemIndex++;
                }
            }

            foreach (string key in incomeByCategoryMap.Keys)
            {
                string sum = generateSumString(incomeByCategoryMap, key);
                worksheetMonth.Range[INCOME_CATEGORY_COL + incomeCategoryIndex.ToString()].Text = key;
                worksheetMonth.Range[INCOME_AMOUNT_COL + incomeCategoryIndex.ToString()].Formula = sum;
                worksheetMonth.Range[INCOME_CURRENCY_COL + incomeCategoryIndex.ToString()].Text = currencySymbol;

                if (!incomeByCategoryCellMap.ContainsKey(key))
                {
                    string cell = "\'" + sheetName + "\'!$" + INCOME_AMOUNT_COL + "$" + incomeCategoryIndex;
                    List<string> cellList = new List<string>();
                    cellList.Add(cell);
                    incomeByCategoryCellMap.Add(key, cellList);
                }
                else
                {
                    string cell = "\'" + sheetName + "\'!$" + INCOME_AMOUNT_COL + "$" + incomeCategoryIndex;
                    incomeByCategoryCellMap[key].Add(cell);
                }

                incomeCategoryIndex++;
            }
            foreach (string key in expenseByCategoryMap.Keys)
            {
                string sum = generateSumString(expenseByCategoryMap, key);
                worksheetMonth.Range[EXPENSE_CATEGORY_COL + expenseCategoryIndex.ToString()].Text = key;
                worksheetMonth.Range[EXPENSE_AMOUNT_COL + expenseCategoryIndex.ToString()].Formula = sum;
                worksheetMonth.Range[EXPENSE_CURRENCY_COL + expenseCategoryIndex.ToString()].Text = currencySymbol;

                if (!expenseByCategoryCellMap.ContainsKey(key))
                {
                    string cell = "\'" + sheetName + "\'!$" + EXPENSE_AMOUNT_COL + "$" + expenseCategoryIndex;
                    List<string> cellList = new List<string>();
                    cellList.Add(cell);
                    expenseByCategoryCellMap.Add(key, cellList);
                }
                else
                {
                    string cell = "\'" + sheetName + "\'!$" + EXPENSE_AMOUNT_COL + "$" + expenseCategoryIndex;
                    expenseByCategoryCellMap[key].Add(cell);
                }

                expenseCategoryIndex++;
            }

            int totalIndex = Math.Max(incomeCategoryIndex, expenseCategoryIndex) + 1;
            worksheetMonth.Range[INCOME_CATEGORY_COL + totalIndex].Text = AppResource.TotalText;
            worksheetMonth.Range[INCOME_CATEGORY_COL + totalIndex].CellStyle.Font.Bold = true;
            worksheetMonth.Range[INCOME_AMOUNT_COL + totalIndex].Formula = "=SUM($" + INCOME_AMOUNT_COL + "$" +
                ROW_START_INDEX + ":$" + INCOME_AMOUNT_COL + (incomeCategoryIndex - 1) + ")" ;
            worksheetMonth.Range[INCOME_CURRENCY_COL + totalIndex].Text = currencySymbol;

            worksheetMonth.Range[EXPENSE_CATEGORY_COL + totalIndex].Text = AppResource.TotalText;
            worksheetMonth.Range[EXPENSE_CATEGORY_COL + totalIndex].CellStyle.Font.Bold = true;
            worksheetMonth.Range[EXPENSE_AMOUNT_COL + totalIndex].Formula = "=SUM($" + EXPENSE_AMOUNT_COL + "$" +
                ROW_START_INDEX + ":$" + EXPENSE_AMOUNT_COL + "$" + (expenseCategoryIndex - 1) + ")";
            worksheetMonth.Range[EXPENSE_CURRENCY_COL + totalIndex].Text = currencySymbol;

            int balanceIndex = totalIndex + 2;
            worksheetMonth.Range[INCOME_CATEGORY_COL + balanceIndex].Text = AppResource.BalanceText;
            worksheetMonth.Range[INCOME_CATEGORY_COL + balanceIndex].CellStyle.Font.Bold = true;
            worksheetMonth.Range[INCOME_AMOUNT_COL + balanceIndex].Formula = "=$" + INCOME_AMOUNT_COL + "$" +
                totalIndex + " - $" + EXPENSE_AMOUNT_COL + "$" + totalIndex;
            worksheetMonth.Range[INCOME_CURRENCY_COL + balanceIndex].Text = currencySymbol;
        }

        /// <summary>
        /// Check that end period is not before start period.
        /// </summary>
        /// <param name="monthStart"></param>
        /// <param name="yearStart"></param>
        /// <param name="monthEnd"></param>
        /// <param name="yearEnd"></param>
        /// <returns> True if end period is not before start period, otherwise false. </returns>
        private bool checkStartPeriodExcess(uint monthStart, uint yearStart, uint monthEnd, uint yearEnd)
        {
            bool checkOk = true;

            if (yearStart > yearEnd)
            {
                checkOk = false;
            }

            if (monthStart > monthEnd)
            {
                if (yearStart >= yearEnd)
                {
                    checkOk = false;
                }
            }

            return checkOk;
        }
    }
}
