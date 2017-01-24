using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace GetPrimitive
{
    public partial class Form1 : Form
    {

        string filePath;
        
        public Form1(string filePath)
        {
            InitializeComponent();
            this.filePath = filePath; 
        }


        private void button1_Click(object sender, EventArgs e)
        {
            //this.label1.Text = "The points have been extracted!";
            //FileStream fst = new FileStream(this.filePath, FileMode.Open);
            System.Diagnostics.Process.Start(this.filePath);

            this.Close();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
