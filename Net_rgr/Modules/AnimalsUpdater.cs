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
        //стартовый url
        private const string urlStart = "https://ru.wikipedia.org/w/index.php?title=Категория:Животные_по_алфавиту&pagefrom=а#mw-pages";
        //Количество животоных
        private readonly int steps;
        //Счетчик пропарсенных животоных в DoParseOnePage
        private int step;
        //Максимальное количество шагов
        private const int maxStep = 37000; 
        //Счетчик занесенных животных в базу данных (нужен для прогресса)
        private int i = 0;
        //Таблица данных
        private rgrNetDataSet.AnimalDataTable animalTable;
        //Интерфейчс для калбека прогерсса
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

        /// <summary>
        /// Парсинг страницы с категориями животных
        /// </summary>
        /// <param name="url">utr страницы (https://ru.wikipedia.org/w/index.php?title=Категория:Животные_по_алфавиту&pagefrom=Имя животоного#mw-pages)</param>
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
                
                //Массив тасков
                List<Task> tasks = new List<Task>();
                for (int i = 0; i < querySelectorAll.Length; i++)
                {
                    string animalName = querySelectorAll[i].InnerHtml;
                    lastAnimal = animalName.Trim().Replace(" ", "_");
                    
                    //Парсим страницу животного ассинхронно
                    var task = ParseOneAnimal(animalName);
                    //Добавляем таск в массив тасков
                    tasks.Add(task);
                    step++;
                    
                    
                    if (step >= steps || step >= maxStep)
                    {
                        //Ждем пока парсинг всех страниц животных в виде таска закончится
                        Task.WaitAll(tasks.ToArray());
                        return;
                    }
                    else if (i == querySelectorAll.Length - 1)
                    {
                        //Ждем пока парсинг всех страниц животных в виде таска закончится
                        Task.WaitAll(tasks.ToArray());
                        break;
                    }
                    Console.WriteLine(animalName);
                }
            }
            response.Close();
            //Парсим следующие 200 животоных
            DoParseOnePage(urlStart.Replace("pagefrom=а", "pagefrom=" + lastAnimal));
        }
        
        /// <summary>
        /// Парсинг страницы животного (https://ru.wikipedia.org/wiki/Название животного)
        /// </summary>
        /// <param name="animalName">Имя животного</param>
        /// <returns>Task</returns>
        private async Task ParseOneAnimal(string animalName) {
            
            string url = "https://ru.wikipedia.org/wiki/" + animalName.Trim().Replace(" ", "_");
            WebRequest webRequest = WebRequest.Create(url);
            webRequest.Method = "GET";
            WebResponse response = await webRequest.GetResponseAsync();
            using (StreamReader reader = new StreamReader(response.GetResponseStream()))
            {
                //Парсим Html
                string html = await reader.ReadToEndAsync();
                HtmlParser parser = new HtmlParser();
                IHtmlDocument document = parser.ParseDocument(html);
                
                //Создаем временный класс для удобной работы
                Animal animal = new Animal
                {
                    Name = document.QuerySelector("#firstHeading").InnerHtml,
                    About = Regex.Replace(
                        document.QuerySelector(".mw-parser-output > p").TextContent.Trim().Normalize(), @"\[\d+\]", ""),
                    Img = document.QuerySelector(".infobox-image img")?.GetAttribute("src").Trim().Substring(2)
                };

                //Есть ил уже в таблице животное с таким именем.
                var animalRow = animalTable.AsEnumerable().FirstOrDefault(row => animalName.Contains(row.Field<String>("name")));
                //Если уже такое животное есть - то обнвоялем его
                if (animalRow != null)
                {
                    animalRow.about = animal.About;
                    animalRow.name = animal.Name;
                    animalRow.img = animal.Img == null ? null : new WebClient().DownloadData("https://" + animal.Img);

                }
                //Если такого жвотного нет - то создаем его
                else
                {
                    lock (this)
                    {
                        object[] rowArray = new object[]
                        {
                            null,
                            animal.Name,
                            animal.About,
                            animal.Img == null ? null : new WebClient().DownloadData("https://" + animal.Img)
                        };
                        DataRow currentRow = animalTable.NewRow();
                        currentRow.ItemArray = rowArray;
                        animalTable.Rows.Add(currentRow);
                    }
                }

                //Синхронизируем потоки и калбечим прогресс
                lock (this)
                {
                    i++;
                    progress.Report((int)(Math.Floor((double)i / ((double)steps / 100.0))));
                }
            }
        }
    }
    
    public class Animal
    {
        public string Name { get; set; }
        public string Img { get; set; }
        public string About { get; set; }
        
    }
}