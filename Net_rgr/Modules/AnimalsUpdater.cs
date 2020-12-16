using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Net;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;

namespace Net_rgr.Modules
{
    public class AnimalsUpdater
    {
        private const string urlStart = "https://ru.wikipedia.org/w/index.php?title=Категория:Животные_по_алфавиту&pagefrom=а#mw-pages";
        private readonly int steps;
        private int step;
        private const int maxStep = 37000; 
        
        public AnimalsUpdater(int steps = 100)
        {
            this.steps = steps;
            this.step = 0;
        }

        public void DoWork()
        {
            DoParseOnePage(urlStart);
        }

        private void DoParseOnePage(string url)
        {
            string lastAnimal = "a";
            WebRequest webRequest = WebRequest.Create(url);
            webRequest.Method = "GET";
            WebResponse response = webRequest.GetResponse();
            using (StreamReader reader = new StreamReader(response.GetResponseStream()))
            {
                //Парсим html
                string html = reader.ReadToEnd();
                HtmlParser parser = new HtmlParser();
                IHtmlDocument document = parser.ParseDocument(html);
                var querySelectorAll = document.QuerySelectorAll(".mw-category-group ul > li > a");
                for (int i = 0; i < querySelectorAll.Length; i++)
                {
                    string animalName = querySelectorAll[i].InnerHtml;
                    lastAnimal = animalName.Trim().Replace(" ", "_");
                    
                    
                    
                    
                    step++;
                    if (step >= steps || step >= maxStep)
                    {
                        return;
                    }
                    if (i == querySelectorAll.Length - 1)
                    {
                        break;
                    }
                    Console.WriteLine(animalName);
                }
            }
            response.Close();
            DoParseOnePage(urlStart.Replace("pagefrom=а", "pagefrom=" + lastAnimal));
        }
        
        
    }




    public class Animal
    {
        public string Name { get; set; }
        public string Img { get; set; }
        public string About { get; set; }
    }
}