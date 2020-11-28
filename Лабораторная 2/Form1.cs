using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.Util;

namespace Лабораторная_2
{
    public partial class Form1 : Form
    {

        private Func func = new Func();

        public Form1()
        {
            InitializeComponent();
        }

       
        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            var result = openFileDialog.ShowDialog(); // открытие диалога выбора файла
            if (result == DialogResult.OK) // открытие выбранного файла
            {
                string fileName = openFileDialog.FileName;
                func.Source(fileName);

                imageBox1.Image = func.sourceImage;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            int th = int.Parse(textBox1.Text);

            imageBox2.Image = func.Binarization(th);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            int th = int.Parse(textBox1.Text);

            imageBox2.Image = func.Circuits(th);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            int th = int.Parse(textBox1.Text);
            int minArea = int.Parse(textBox2.Text);
            Label label = label3;
            imageBox2.Image = func.Search(th, minArea, label);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            imageBox2.Image = func.SearchColor();
        }
    }
}
