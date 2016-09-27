using CsvHelper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompletedGamesToCSV {
    class CompletionData : IComparable<CompletionData> {
        public string GameName { get; set; }
        public bool is100 { get; set; }
        public string Platform { get; set; }
        public string Comm { get; set; }

        public int CompareTo(CompletionData other) {
            if(Platform.CompareTo(other.Platform) == 0) {
                return GameName.CompareTo(other.GameName);
            } else {
                return Platform.CompareTo(other.Platform);
            }
        }

        public override string ToString() {
            return Platform + "," + GameName + "," + is100 + "," + (Comm!=null ? Comm: "None");
        }

        public string RenderString() {
            return "-" + GameName + (is100 ? " :100percent:":String.Empty) + " " + (Comm != null ? Comm:String.Empty);
        }

        public CompletionData(string csvline) {
            string[] w = csvline.Split(',');
            Platform = w[0];
            GameName = w[1];
            is100 = bool.Parse(w[2]);
            Comm = w[3] == "None" ? String.Empty : w[3];
        }

        public CompletionData() { }
    }

    class GenerateList {
        private static string filename = "games.csv", destinationfile = "description.txt", headerFile = "header.txt";
        public void Run() {
            Dictionary<string, List<CompletionData>> data = new Dictionary<string, List<CompletionData>>();
            /*using (FileStream fs = File.Open(filename, FileMode.Open)) {
                using(StreamReader sr = new StreamReader(fs)) {
                    CompletionData game;
                    while (!sr.EndOfStream) {
                        game = new CompletionData(sr.ReadLine());
                        if(!data.ContainsKey(game.Platform)) {
                            data[game.Platform] = new List<CompletionData>();
                        }
                        data[game.Platform].Add(game);
                    }
                }
            }*/
            var csv = new CsvReader(File.OpenText(filename));
            foreach(var game in csv.GetRecords<CompletionData>()) {
                if (!data.ContainsKey(game.Platform)) {
                    data[game.Platform] = new List<CompletionData>();
                }
                data[game.Platform].Add(game);
            }

            foreach (var x in data) {
                x.Value.Sort();
            }

            using(FileStream fs = File.Open(destinationfile, FileMode.Create)) {
                using (StreamWriter sw = new StreamWriter(fs)) {
                    using (FileStream headerStream = File.Open(headerFile, FileMode.Open)) {
                        using (StreamReader headerReader = new StreamReader(headerStream)) {
                            sw.WriteLine(headerReader.ReadLine());
                        }
                    }

                    foreach (var plat in data) {
                        sw.WriteLine("[b]" + plat.Key + "[/b]");
                        foreach(var game in plat.Value) {
                            sw.WriteLine(game.RenderString());
                        }
                        sw.WriteLine(String.Empty);
                    }
                }
            }
        }
    }

    class Program {
        static void Main(string[] args) {
            new GenerateList().Run();
        }
    }
}
