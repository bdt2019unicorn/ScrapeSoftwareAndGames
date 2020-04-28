using System;
using System.Collections.Generic;
using System.Text;
using ClosedXML.Excel;
using System.Data;

namespace ScrapeSoftwareAndGames
{
    class DictionaryToExcel
    {
        static string file_name = "Auckland Games.xlsx";
        static string[] remove_words = { "of New Zealand", "in New Zealand" };
        
        private static DataTable BuildTable(Dictionary<string,string>data)
        {
            DataTable table = new DataTable();
            table.Columns.Add("Name");
            table.Columns.Add("Link");
            foreach (KeyValuePair<string,string> item in data)
            {
                DataRow row = table.NewRow();
                row["Name"] = item.Key;
                row["Link"] = item.Value;
                table.Rows.Add(row); 
            }
            return table; 
        }

        public static void SaveToExcel(Dictionary<string,Dictionary<string,string>>collection)
        {
            XLWorkbook wb = new XLWorkbook();
            foreach (KeyValuePair<string,Dictionary<string,string>> item in collection)
            {
                string category = item.Key;
                foreach (string word in remove_words)
                {
                    category = category.Replace(word, String.Empty); 
                }
                DataTable data = BuildTable(item.Value);
                wb.Worksheets.Add(data, category);
                GC.Collect();
            }
            wb.SaveAs(file_name); 
        }
    }
}
