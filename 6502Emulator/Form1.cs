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

            Helper.Font = new Font(FontFamily.GenericMonospace, 14);

        }

        private void Display(string file)
        {
            AssemblyParser parser = new AssemblyParser(file);
            var instructions = parser.Parse();

            var addresses = Helper.GetAddresses(instructions);
            var hexdump = Helper.HexDumpStr(instructions);

            HexCodeParser hexCodeParser = new HexCodeParser(Helper.HexDumpList(instructions), Helper.GetAddressesList(instructions));
            var dissassembler = hexCodeParser.Parse();
            var disassemblerStr = Helper.DissassemblyStr(dissassembler);

            var adSplit = addresses.Split('\n').Where((x) => !string.IsNullOrEmpty(x)).ToArray();
            var hexSplit = hexdump.Split('\n').Where((x) => !string.IsNullOrEmpty(x)).ToArray();
            var disSplit = disassemblerStr.Split('\n').Where((x) => !string.IsNullOrEmpty(x)).ToArray();

            DataTable table = new DataTable();
            table.Columns.Add("Address:");
            table.Columns.Add("HexDump:");
            table.Columns.Add("Dissassembly:");

            var correctlines = System.IO.File.ReadAllLines("CorrectAssembly.txt");
            for (int i = 0; i < adSplit.Length; i++)
            {
                table.Rows.Add(adSplit[i], hexSplit[i], disSplit[i]);
                
                string currLine = adSplit[i].PadRight(4, ' ') + "    " + hexSplit[i].PadRight(8, ' ') + "  " + disSplit[i];
                
                if(correctlines[i].ToLower() != currLine.ToLower())
                {

                }
            }
            
            dataGridView1.DataSource = table;
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridView1.Height = this.Height;
            dataGridView1.RowHeadersVisible = false;
            dataGridView1.BorderStyle = BorderStyle.None;
            dataGridView1.CellBorderStyle = DataGridViewCellBorderStyle.None;

            dataGridView1.BackgroundColor = this.BackColor;

            foreach (DataGridViewColumn col in dataGridView1.Columns)
            {
                col.SortMode = DataGridViewColumnSortMode.NotSortable;
            }

            dataGridView1.ClearSelection();

            dataGridView1.CellMouseMove += dataGridView1_CellMouseMove;
            dataGridView1.CellMouseLeave += dataGridView1_CellMouseLeave;
        }

        private void BuildButton_Click(object sender, EventArgs e)
        {
            System.IO.File.WriteAllText("temp.txt", codeTextBox.Text);
            Display("temp.txt");
        }

        private void BuildFile_Click(object sender, EventArgs e)
        {
            Display("6502Code.txt");
        }

        private void dataGridView1_CellMouseMove(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.RowIndex < 0) return;
            if (e.ColumnIndex != 2) return;

            dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Style.BackColor = Color.Blue;
        }
        private void dataGridView1_CellMouseLeave(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            if (e.ColumnIndex != 2) return;

            dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Style.BackColor = Color.White;
        }
    }
}
