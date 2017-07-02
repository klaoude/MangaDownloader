using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MangaDownloaderV2
{
    class EnglishManga
    {
        ListBox listBox1, listBox2;
        ProgressBar progressBar1;
        Label label1, label2, label3, label4, label5, label6;
        RichTextBox richTextBox1;

        private WebClient m_client;
        private string m_html;
        private string m_mangaHtml;
        private HashSet<string> m_MangaName = new HashSet<string>();
        private string m_path;

        public EnglishManga(ListBox lb1, ListBox lb2,
            ProgressBar pb1,
            Label l1, Label l2, Label l3, Label l4, Label l5, Label l6,
            RichTextBox rtb)
        {
            m_client = new WebClient();
            m_client.Encoding = Encoding.UTF8;
            m_html = WebUtility.HtmlDecode(m_client.DownloadString("http://www.mangareader.net/alphabetical"));

            listBox1 = lb1;
            listBox2 = lb2;

            progressBar1 = pb1;

            label1 = l1;
            label2 = l2;
            label3 = l3;
            label4 = l4;
            label5 = l5;
            label6 = l6;

            richTextBox1 = rtb;
        }

        public void setPath(string path)
        {
            m_path = path;
        }
        public HashSet<string> getMangeNames() { return m_MangaName; }

        public void loadMangaList()
        {
            string dlpatt = "<li>(.*)<a href=\"(.*)\">(.*)<\\/a>";
            MatchCollection resultDl = Regex.Matches(m_html, dlpatt);
            for(int i = 46; i < resultDl.Count; i++)
            {
                if (i >= 46 && i <= resultDl.Count - 10)
                {
                    if(resultDl[i].Groups[3].ToString().Length > 1)
                        m_MangaName.Add(resultDl[i].Groups[3].ToString());
                }
            }
            foreach (string s in m_MangaName)
                listBox1.Items.Add(s);
        }

        public void updateMangaHtml(string name)
        {
            m_mangaHtml = WebUtility.HtmlDecode(m_client.DownloadString("http://www.mangareader.net" + name));
        }

        public void loadChapitre()
        {
            listBox2.Items.Clear();

            string temphtml = Regex.Split(m_mangaHtml, "readmangasum")[1];

            string patern = "<div class=\"chico_manga\"><\\/div>\\n<a href=\"(.*)\">(.*)<\\/a>";
            MatchCollection match = Regex.Matches(temphtml, patern);

            foreach(Match m in match)
            {
                if(m.Groups[1].ToString() != "")
                {
                    listBox2.Items.Add(m.Groups[2].ToString());
                }                    
            }
        }

        public string getImgURL(string name)
        {
            string[] imgLinks = Regex.Split(getImgWrapper(name), "src=\"http://cdn.japscan.com");
            return "http://cdn.japscan.com" + Regex.Split(imgLinks[1], "\"/>")[0];
        }

        public string getImgWrapper(string name)
        {
            string newName = "";
            foreach (char c in name)
            {
                if (c == '[' || c == ']' || c == '(' || c == ')' || c == '!' || c == '?' || c == '*' || c == '+')
                    newName += '\\';
                newName += c;
            }

            string dlpatt = "href=\"//(.*)\">Scan " + newName;

            Match resultDl = Regex.Match(m_mangaHtml, dlpatt);

            return m_client.DownloadString("http://" + resultDl.Groups[1]);
        }

        public void DownloadChapter(string chap)
        {
            string imgLink = getImgURL(chap);

            progressBar1.Minimum = 0;
            progressBar1.Step = 1;
            string[] test = Regex.Split(getImgWrapper(chap), "</option>");
            string t = Regex.Split(test[test.Length - 3], "html")[1];
            t = Regex.Split(t, " ")[1];
            progressBar1.Maximum = int.Parse(t);
            progressBar1.Value = 0;

            Directory.CreateDirectory(m_path + "\\" + getName());

            string folderName = "Chap ";
            try
            {
                int chapNum = int.Parse(Regex.Split(imgLink, "/")[5]);
                if (chapNum <= 9)
                    folderName += "00" + chapNum.ToString();
                else if (chapNum < 100 && chapNum >= 10)
                    folderName += "0" + chapNum.ToString();
                else
                    folderName += chapNum.ToString();
            }
            catch
            {
                folderName += Regex.Split(imgLink, "/")[5];
            }

            Directory.CreateDirectory(m_path + "\\" + getName() + "/" + folderName);

            bool c = true;
            int i = 1;
            int tryy = 0;
            while (c)
            {
                try
                {
                    string extention = imgLink.Substring(imgLink.Length - 4);
                    imgLink = Regex.Split(imgLink, extention)[0];
                    imgLink = imgLink.Remove(imgLink.Length - 1);

                    if (i >= 10 && i < 100)
                        imgLink = imgLink.Remove(imgLink.Length - 1);
                    else if (i >= 100 && i < 999)
                    {
                        imgLink = imgLink.Remove(imgLink.Length - 1);
                        imgLink = imgLink.Remove(imgLink.Length - 1);
                    }
                    else if (i >= 1000)
                    {
                        imgLink = imgLink.Remove(imgLink.Length - 1);
                        imgLink = imgLink.Remove(imgLink.Length - 1);
                        imgLink = imgLink.Remove(imgLink.Length - 1);
                    }

                    if (Regex.Split(imgLink, "/").Length < 7)
                        imgLink += '/';

                    imgLink += i;

                    imgLink += extention;
                    string fileName = Regex.Split(imgLink, "/")[6];
                    m_client.DownloadFile(imgLink, m_path + "\\" + getName() + "/" + folderName + "/" + fileName);
                    progressBar1.PerformStep();
                    i++;
                    tryy = 0;
                }
                catch (WebException ex)
                {
                    Console.WriteLine("Exectption : link = " + imgLink);
                    tryy++;
                    i++;
                    if (tryy >= 10)
                        c = false;
                }
            }
        }

        public string getName()
        {
            string MangaCode = "";
            string dlpatt = "href=\"(.*)\">(.*)</a>";
            MatchCollection resultDl = Regex.Matches(m_html, dlpatt);
            for (int i = 0; i < resultDl.Count; i++)
            {
                if (listBox1.SelectedItem.ToString() == resultDl[i].Groups[2].ToString())
                {
                    MangaCode = resultDl[i].Groups[1].ToString();
                    break;
                }
            }
            return MangaCode;
        }
        public void setName()
        {
            string dlpatt = "aname\">(.*)</h2>";
            MatchCollection resultDl = Regex.Matches(m_mangaHtml, dlpatt);
            label1.Text += resultDl[0].Groups[1].ToString();
        }

        public void setInfo()
        {
            string pattern = "propertytitle(.*)\n<td>(.*)</td>";
            MatchCollection resultDl = Regex.Matches(m_mangaHtml, pattern);

            /*string[] InfoSplit = Regex.Split(m_html, "<div class=\"row\">");
            InfoSplit = Regex.Split(InfoSplit[1], "<div class=\"cell\">");

            label2.Text += Regex.Split(InfoSplit[1], "<")[0];
            if (InfoSplit.Length > 6)
            {
                label3.Text += Regex.Split(InfoSplit[3], "<")[0];
                label4.Text += Regex.Split(InfoSplit[4], "<")[0];
                label6.Text += Regex.Split(Regex.Split(InfoSplit[5], ">")[1], "<")[0];
                label5.Text += Regex.Split(InfoSplit[6], "<")[0];
            }
            else
            {
                label3.Text += Regex.Split(InfoSplit[2], "<")[0];
                label4.Text += Regex.Split(InfoSplit[3], "<")[0];
                label6.Text += Regex.Split(Regex.Split(InfoSplit[4], ">")[1], "<")[0];
                label5.Text += Regex.Split(InfoSplit[5], "<")[0];
            }*/

            pattern = "readmangasum\">\n<h2>.*\n<p>(.*(\n.*)*)</p>";
            resultDl = Regex.Matches(m_mangaHtml, pattern);

            richTextBox1.Text = resultDl[0].Groups[1].ToString();
        }
    }
}
