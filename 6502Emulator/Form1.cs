using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace _6502Emulator
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            string maxstr = "";
            for(int i = 0; i < Helper.MemorySize; i++)
            {
                string str = Convert.ToString(i, 16);
                if(maxstr.Length <= str.Length)
                {
                    maxstr = str;
                }
            }

            Parser pa = new Parser("6502Code.txt", "configFile.json");



            var bytes = new byte[] { Convert.ToByte("0xFF", 16) };
            //System.IO.File.WriteAllBytes("poop.txt", bytes);
            string s = BitConverter.ToString(bytes);
        }
    }
}
