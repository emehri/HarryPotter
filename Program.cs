using System.IO;
using System.Text;
using System.Diagnostics;
namespace HarryPotter
{
    internal class Program
    {
        static StreamWriter sw;
        static void Main(string[] args)
        {
            string source = "salam_mdx.txt";
            sw = new StreamWriter(source, false, Encoding.UTF8);
            sw.WriteLine($"-\r\n-\r\n</>");
            Make_MDX("part1", "salam");
            Make_MDX("part2", "salam");
            Make_MDX("part3", "salam");
            sw.Close();
            string Title = "Gratification (Audio Book)";
            File.WriteAllText("title.html", Title);
            File.WriteAllText("description.html", "ebrahim.mehri@gmail.com");
            File.WriteAllText("run.bat", 
                $"mdict --title title.html --description description.html -a {source} \"{Title}.mdx\"\r\n" +
                $"mdict --title title.html --description description.html -a Audio \"{Title}.mdd\"");
            Process.Start("run.bat");
        }
        static void Make_MDX(string FilmName, string Title)
        {
            List<string> Diologue = File.ReadAllLines("Text of " + FilmName + ".txt", Encoding.UTF8).ToList();
            int k = 1;
            sw.WriteLine(Title);
            sw.WriteLine($"<h1 style='text-align:center;'>{Title}</h1>");
            foreach (string ss in Diologue)
            {
                sw.WriteLine($"{k}. <a href='sound://{FilmName}-{k}.mp3' style='font-size:larger;'><img src='lips.png'></a> {ss}<br><br>");
                k++;
            }
            sw.WriteLine("</>");
        }
        static void MakeTxtMP3(string FilmName, string Format)
        {
            //string begin = Times[i].Substring(0, 12).Replace(",", ".");
            //string end = Times[i].Substring(17, 12).Replace(",", ".");
            //00:01:56,141 --> 00:02:00,062
            //sw.WriteLine($"ffmpeg -ss {begin}  -i \"{FilmName}.mp4\" -t {finish - start} -c:v libx264 -c:a aac \"{fn}\"");
            List<string> Original_SRT = File.ReadAllLines(FilmName + ".srt", Encoding.UTF8).ToList();
            double duration = 0;
            bool Dialogue = false;
            string Sentence = "";
            StreamWriter sw = new StreamWriter("Text of " + FilmName + ".txt", false, Encoding.UTF8);
            string bat = $"CutAudio-{FilmName}.bat";
            StreamWriter ffmpeg = new StreamWriter(bat, false, Encoding.ASCII);
            int n = 0;
            string begin = "";
            if (!Directory.Exists(FilmName))
                Directory.CreateDirectory(FilmName);
            foreach (string ss in Original_SRT)
            {
                if (ss.IndexOf(" --> ") != -1)
                {
                    begin = ss.Substring(0, 12).Replace(",", ".");
                    duration = AbsoluteTime(ss.Substring(17, 12)) - AbsoluteTime(ss.Substring(0, 12));
                    Dialogue = true;
                    continue;
                }
                if (ss == "")
                {
                    Dialogue = false;
                    if (string.IsNullOrWhiteSpace(Sentence)) continue;
                    if (Sentence.Length < 20 || Sentence.IndexOf("(") != -1 || Sentence.IndexOf("[") != -1)
                    {
                        Sentence = "";
                        continue;
                    }
                    n++;
                    sw.WriteLine(Sentence);
                    Sentence = "";
                    ffmpeg.WriteLine($"ffmpeg -ss {begin}  -i \"{FilmName}.{Format}\" -t {String.Format("{0:0.00}", duration)} \"Audio\\{FilmName}-{n}.mp3\"");
                    continue;
                }
                if (!Dialogue)
                    continue;
                Sentence += ss + " ";
            }
            sw.Flush();
            ffmpeg.Close();
            Process.Start(bat);
        }
        static int Capitals(string s)
        {
            int n = 0;
            foreach (char c in s)
                if (char.IsUpper(c))
                    n++;
            return n;
        }
        static double AbsoluteTime(string time)
        {
            //00:01:56,141
            int k = 0;
            List<double> coefficient = new List<double> { 36000, 3600, 600, 60, 10, 1, .1, .01, .001 };
            double total = 0;
            for (int i = 0; i < time.Length; i++)
            {
                if (!Char.IsDigit(time[i]))
                    continue;
                total += coefficient[k] * Digit(time[i]);
                k++;
            }
            return total;
        }
        static double Digit(char a)
        {
            return double.Parse(a.ToString());
        }
    }
}
