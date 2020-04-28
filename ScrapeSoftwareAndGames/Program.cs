using System;
using System.Linq;
using System.Collections.Generic;

using System.Threading.Tasks;

using HtmlAgilityPack; 

namespace ScrapeSoftwareAndGames
{



    class Program
    {
        static void Main(string[] args)
        {
            Dictionary<string, Dictionary<string, string>> companies_by_categories = ProcessToDictionary.CompaniesUrlByCategories();
            DictionaryToExcel.SaveToExcel(companies_by_categories); 
        }
    }
}
