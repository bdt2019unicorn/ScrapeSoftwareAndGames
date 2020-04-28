using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ScrapeSoftwareAndGames
{
    class ProcessToDictionary
    {
        static string[] base_url_game_software = { "https://en.wikipedia.org/wiki/Category:Video_game_companies_of_New_Zealand", "https://en.wikipedia.org/wiki/Category:Software_companies_of_New_Zealand" };
        static string video_games_nz = "https://en.wikipedia.org/wiki/Category:Video_games_developed_in_New_Zealand";

        const string base_url = "https://en.wikipedia.org";


        private static HtmlDocument PageDocument(string url)
        {
            HtmlWeb web = new HtmlWeb();
            HtmlDocument doc = web.Load(url);
            return doc;
        }

        private static Dictionary<string, string> WikipediaLinks(string page_url, out string category, string company_vs_game)
        {
            HtmlDocument page = PageDocument(page_url);
            Dictionary<string, string> links = new Dictionary<string, string>();
            HtmlNodeCollection all_links = page.GetElementbyId("mw-pages").SelectSingleNode("//div[@class='mw-category']").SelectNodes(".//li");


            Parallel.ForEach
                (
                    all_links, (li) =>
                    {
                        string href = li.SelectSingleNode(".//a").Attributes["href"].Value;
                        href = base_url + href;
                        string company_url = CompanyUrl(href, company_vs_game); 
                        if(company_url!=null)
                        {
                            links[li.InnerText] = company_url; 
                        }
                    }
                );



            category = page.GetElementbyId("firstHeading").InnerText.Replace("Category:", String.Empty);
            GC.Collect(); 
            return links;
        }


        private static string CompanyUrl(string wikipedia_link, string company_vs_game)
        {
            HtmlDocument page = PageDocument(wikipedia_link);
            Dictionary<string, string> table_class = new Dictionary<string, string>()
            {
                {"company","vcard" },
                {"game","hproduct" }
            }; 

            HtmlNodeCollection information_table = page.GetElementbyId("mw-content-text").SelectNodes(".//table[@class='infobox "+table_class[company_vs_game] +"']//tr");
            string company_url = null;

            try
            {
                foreach (HtmlNode tr in information_table)
                {
                    try
                    {
                        string th = tr.SelectSingleNode("./th").InnerText;
                        string td = tr.SelectSingleNode("./td").InnerText;
                        if(company_vs_game=="company")
                        {
                            if (th == "Fate")
                            {
                                return null;
                            }
                            else if ((th == "Website") || (th == "URL"))
                            {
                                company_url = td;
                            }
                        }
                        else
                        {
                            if(th== "Publisher(s)")
                            {
                                return td; 
                            }
                        }
                    }
                    catch { }
                }
            }
            catch
            {
                return null; 
            }

            
            return company_url; 
        }

        private static Dictionary<string,string> GamePublisher(Dictionary<string,string>game_vs_publisher)
        {
            Dictionary<string, string> game_publisher = new Dictionary<string, string>();
            foreach (KeyValuePair<string,string> item in game_vs_publisher)
            {
                game_publisher[item.Value] = item.Value; 
            }
            return game_publisher; 
        }
        public static Dictionary<string, Dictionary<string, string>> CompaniesUrlByCategories()
        {
            Dictionary<string, Dictionary<string, string>> companies_by_categories = new Dictionary<string, Dictionary<string, string>>();
            Parallel.ForEach
                (
                    base_url_game_software,
                    (url) =>
                    {
                        string category = "";
                        Dictionary<string, string> companies = WikipediaLinks(url, out category, "company");
                        companies_by_categories[category] = companies;
                        GC.Collect(); 
                    }
                );
            string game_category = "";
            Dictionary<string, string> game_by_publisher = WikipediaLinks(video_games_nz, out game_category,"game");
            companies_by_categories[game_category] = GamePublisher(game_by_publisher);
            GC.Collect(); 



            return companies_by_categories;
        }
    }
}
