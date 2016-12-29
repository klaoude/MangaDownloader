using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Text.RegularExpressions;
using System.IO;
using System.Diagnostics;

namespace MangaDownloaderV2
{
    public partial class Form1 : Form
    {
        FrenchManga m_fm;
        EnglishManga m_em;

        public Form1()
        {
            InitializeComponent();

            m_fm = new FrenchManga(listBox1, listBox2, progressBar1, label1, label2, label3, label4, label5, label6, label7, label8, richTextBox1, webBrowser1);
            m_em = new EnglishManga(listBox1, listBox2, progressBar1, label1, label2, label3, label4, label5, label6, richTextBox1);

            comboBox1.SelectedIndex = 0;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex == 0)
                m_fm.loadMangaList();
            else
                m_em.loadMangaList();
            textBox2.Text = "./Download";
            comboBox1.SelectedIndex = 0;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            var itemList = m_fm.getMangeNames();

            if (comboBox1.SelectedIndex != 0)
                itemList = m_em.getMangeNames();            

            if (itemList.Count > 0)
            {
                listBox1.Items.Clear();

                listBox1.Items.AddRange(
                    itemList.Where(i => i.IndexOf(textBox1.Text, StringComparison.OrdinalIgnoreCase) >= 0).ToArray());
            }
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex == 0)
            {
                m_fm.updateMangaHtml(m_fm.getName());
                m_fm.setName();
                m_fm.setInfo();
                m_fm.loadChapitre();
            }
            else
            {
                m_em.updateMangaHtml(m_em.getName());
                m_em.setName();
                m_em.setInfo();
                m_em.loadChapitre();
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();

            DialogResult result = fbd.ShowDialog();

            textBox2.Text = fbd.SelectedPath;
            if (comboBox1.SelectedIndex == 0)
                m_fm.setPath(textBox2.Text);
            else
                m_em.setPath(textBox2.Text);
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex == 0)
                m_fm.setPath(textBox2.Text);
            else
                m_em.setPath(textBox2.Text);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if(textBox2.Text == "")
            {
                MessageBox.Show("Please enter a path !");
                return;
            }
            foreach(string s in listBox2.SelectedItems)
            {
                if (comboBox1.SelectedIndex == 0)
                    m_fm.DownloadChapter(s);
                else
                    m_em.DownloadChapter(s);
            }
            MessageBox.Show("All download are finished !");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (textBox2.Text == "")
            {
                MessageBox.Show("Please enter a path !");
                return;
            }
            foreach (string s in listBox2.Items)
            {
                if (comboBox1.SelectedIndex == 0)
                    m_fm.DownloadChapter(s);
                else
                    m_em.DownloadChapter(s);
            }
                
            MessageBox.Show("All download are finished !");
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            textBox1.Text = "";
            textBox2.Text = "";
            listBox1.Items.Clear();
            listBox2.Items.Clear();
            //if (comboBox1.SelectedIndex == 0)
            //    m_fm.loadMangaList();
            //else
            //    m_em.loadMangaList();
        }

        private void webBrowser1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {

        }
    }
}
