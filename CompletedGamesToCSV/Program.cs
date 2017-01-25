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
        public void Run(string filename, string destinationFile, string headerFile) {
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

            // Write description txt
            WriteDescription(data, DescriptionFormat.TXT, destinationFile, headerFile);

            // Write description HTML
            WriteDescription(data, DescriptionFormat.HTML, destinationFile, headerFile);
        }

        void WriteDescription(Dictionary<string, List<CompletionData>> data, DescriptionFormat format, string destinationfile, string headerFile) {
            using (FileStream fs = File.Open(destinationfile + format.Ext, FileMode.Create)) {
                using (StreamWriter sw = new StreamWriter(fs)) {
                    sw.WriteLine(format.RenderFileHeader());
                    try {
                        using (FileStream headerStream = File.Open(headerFile, FileMode.Open)) {
                            using (StreamReader headerReader = new StreamReader(headerStream)) {
                                while (!headerReader.EndOfStream)
                                    sw.WriteLine(format.RenderHeaderLine(headerReader.ReadLine()));
                            }
                        }
                    } catch (Exception) {
                        Console.WriteLine("Header cannot be opened");
                    }
                    sw.WriteLine(format.RenderFileHeaderEnd());

                    foreach (var plat in data) {
                        sw.WriteLine(format.RenderPlatform(plat.Key));
                        foreach (var game in plat.Value) {
                            sw.WriteLine(format.RenderGame(game));
                        }
                        sw.WriteLine(format.RenderPlatformEnd());
                    }

                    sw.WriteLine(format.RenderFileEnd());
                }
            }
        }
    }

    /**
     * DescriptionFormat : implements the different rendering methods to display as html or steam text
     **/
    abstract class DescriptionFormat {
        public string Ext { get; set; }
        public abstract string RenderPlatform(string platform);
        public abstract string RenderPlatformEnd();
        public abstract string RenderGame(CompletionData game);
        public abstract string RenderFileHeader();
        public abstract string RenderFileHeaderEnd();
        public abstract string RenderFileEnd();
        public abstract string RenderHeaderLine(string line);
        public static DescriptionFormat TXT { get { return new TextDescriptionFormat(); } }
        public static DescriptionFormat HTML { get { return new HTMLDescriptionFormat(); } }
    }

    /**
     * Text: simple steam description
     **/
    class TextDescriptionFormat : DescriptionFormat {
        public TextDescriptionFormat() {
            Ext = ".txt";
        }

        public override string RenderFileEnd() {
            return string.Empty;
        }

        public override string RenderFileHeader() {
            return string.Empty;
        }

        public override string RenderFileHeaderEnd() {
            return string.Empty;
        }

        public override string RenderGame(CompletionData game) {
            return "-" + game.GameName + (game.is100 ? " :100percent:" : String.Empty) + " " + ((game.Comm != null && game.Comm != "None") ? game.Comm : String.Empty);
        }

        public override string RenderHeaderLine(string line) {
            return line;
        }

        public override string RenderPlatform(string platform) {
            return "[b]" + platform + "[/b]";
        }

        public override string RenderPlatformEnd() {
            return string.Empty;
        }
    }
    
    /**
     * HTML: for a web page
     **/
    class HTMLDescriptionFormat : DescriptionFormat {
        public HTMLDescriptionFormat() {
            Ext = ".html";
        }

        public override string RenderFileEnd() {
            return "</div></body></html>";
        }

        public override string RenderFileHeader() {
            return "<html><head><title=\"Completed Games List\"><link rel=\"stylesheet\" href=\"style.css\"></head><body><div id=\"header\">";
        }

        public override string RenderFileHeaderEnd() {
            return "</div><div id=\"list\">";
        }

        public override string RenderGame(CompletionData game) {
            return "<div class=\"game\">" + game.GameName + (game.is100 ? "<div class=\"percent\"></div>" : String.Empty) + ((game.Comm != null && game.Comm != "None") ? "<div class=\"comment\">" + DecorateComment(game.Comm) + "</div>" : String.Empty) + "</div>";
        }

        public override string RenderHeaderLine(string line) {
            return line + "<br/>";
        }

        public override string RenderPlatform(string platform) {
            return "<div class=\"platform\" id=\"" + platform + "\">";
        }

        public override string RenderPlatformEnd() {
            return "</div>";
        }

        private string DecorateComment(string comment) {
            return comment.Replace(":B1:", "</div><div class=\"b1\"></div><div class=\"comment-resume\">");
        }
    }

    class Program {
        static void Main(string[] args) {
            string filename = "games.csv", destinationfile = "description", headerFile = "header.txt";
            foreach (var x in args) Console.WriteLine(x);

            new GenerateList().Run(filename, destinationfile, headerFile);

            Console.WriteLine("OK");
        }
    }
}
