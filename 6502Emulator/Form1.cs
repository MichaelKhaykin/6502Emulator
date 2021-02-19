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

            Chip chip = new Chip();

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

            for (int i = 0; i < adSplit.Length; i++)
            {
                table.Rows.Add(adSplit[i], hexSplit[i], disSplit[i]);
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
                col.ReadOnly = true;
            }

            dataGridView1.ClearSelection();

            dataGridView1.DefaultCellStyle.SelectionBackColor = dataGridView1.DefaultCellStyle.BackColor;
            dataGridView1.DefaultCellStyle.SelectionForeColor = dataGridView1.DefaultCellStyle.ForeColor;
            dataGridView1.CellToolTipTextNeeded += DataGridView1_CellToolTipTextNeeded;
        }

        private void DataGridView1_CellToolTipTextNeeded(object sender, DataGridViewCellToolTipTextNeededEventArgs e)
        {
            //e has a row index and cell index we can utilize
            
            e.ToolTipText = "Hello";
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
    }
}
