using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
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
        private int i = 0;
        private rgrNetDataSet.AnimalDataTable animalTable;
        public IProgress<int> progress;


        public AnimalsUpdater(rgrNetDataSet.AnimalDataTable animalTable, IProgress<int> p, int steps = 500)
        {
            this.animalTable = animalTable;
            this.steps = steps;
            this.step = 0;
            this.progress = p;
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


                    var task = ParseOneAnimal(animalName);
                    
                    task.Wait();
                    step++;
                    if (step >= steps || step >= maxStep)
                    {
                        Console.WriteLine("STOP: " + step + " of " + steps);
                        task.Wait();
                        return;
                    }
                    if (i == querySelectorAll.Length - 1)
                    {
                        task.Wait();
                        break;
                    }
                    Console.WriteLine(animalName);
                }
            }
            response.Close();
            DoParseOnePage(urlStart.Replace("pagefrom=а", "pagefrom=" + lastAnimal));
        }
        
        private async Task ParseOneAnimal(string animalName) {
            
            string url = "https://ru.wikipedia.org/wiki/" + animalName.Trim().Replace(" ", "_");
            WebRequest webRequest = WebRequest.Create(url);
            webRequest.Method = "GET";
            WebResponse response = await webRequest.GetResponseAsync();
            using (StreamReader reader = new StreamReader(response.GetResponseStream()))
            {
                string html = await reader.ReadToEndAsync();
                HtmlParser parser = new HtmlParser();
                IHtmlDocument document = parser.ParseDocument(html);



                Animal animal = new Animal
                {
                    Name = document.QuerySelector("#firstHeading").InnerHtml,
                    About = Regex.Replace(
                        document.QuerySelector(".mw-parser-output > p").TextContent.Trim().Normalize(), @"\[\d+\]", ""),
                    Img = document.QuerySelector(".infobox-image img")?.GetAttribute("src").Trim().Substring(2)
                };


                var animalRow = animalTable.AsEnumerable().FirstOrDefault(row => animalName.Contains(row.Field<String>("name")));
                if (animalRow != null)
                {
                    animalRow.about = animal.About;
                    animalRow.name = animal.Name;
                    if (animal.Img == null)
                    {
                        animalRow.img = null;
                    }
                    else
                    {
                        animalRow.img = new WebClient().DownloadData("https://" + animal.Img);
                    }

                }
                else
                {
                    if (animal.Img == null)
                    {
                        AppendToDb(animal.Name, animal.About, null);
                    }
                    else
                    {
                        AppendToDb(animal.Name, animal.About, new WebClient().DownloadData("https://" + animal.Img));
                    }
                }



                i++;
                progress.Report((int)(Math.Floor((double)i / ((double)steps / 100.0))));





                Console.WriteLine(animal.Name + " parsed! " + i);
                
            }
        }

        public void AppendToDb(string name, string about, byte[] img)
        {
            lock (this)
            {
                object[] RowArray = new object[] { null, name, about, img };
                DataRow CurrentRow = animalTable.NewRow();
                CurrentRow.ItemArray = RowArray;
                animalTable.Rows.Add(CurrentRow);
            }
        }

    }




    public class Animal
    {
        public string Name { get; set; }
        public string Img { get; set; }
        public string About { get; set; }

        public override string ToString()
        {
            return "Name = " + Name + ", About = " + About + ", Img = " + Img;
        }
    }
}