using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace HangUm.Services
{
    public class WordService
    {

        private List<string> WordsToUse { get; set; } 

        public WordService()
        {
            WordsToUse = new List<string>();

            //load data file
            WordsToUse.AddRange(LoadTextFile());
        }

        private List<string> LoadTextFile()
        {
            List<string> result = new List<string>();
            //make sure the file exists
            if (File.Exists("wwwroot/data/words.dat"))
            {
                try
                {
                    string data = File.ReadAllText("wwwroot/data/words.dat");
                    string[] sdata = data.Replace('\n', ' ').Trim().Split('\r');
                    result.AddRange(sdata.Where(str => !string.IsNullOrEmpty(str)));
                }
                catch
                {

                }
            }
            return result;
        }


        public async Task<string> Get()
        {
            string result = "";

            if (WordsToUse.Any())
            {
                Random r = new Random();
                int index = r.Next(1, WordsToUse.Count - 1);
                try
                {
                    result = WordsToUse.ElementAt(index).Trim();
                }
                catch
                {
                    result = WordsToUse.ElementAt(0).Trim();
                }
            }
            await Task.CompletedTask;
            return result;
        }
    }
}
