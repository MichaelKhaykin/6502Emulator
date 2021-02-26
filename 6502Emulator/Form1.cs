using _6502Emulator.Instructions.Arthimetic;
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
        Dictionary<int, int> matching = new Dictionary<int, int>();
        Dictionary<int, int> reverseMatching = new Dictionary<int, int>();
        Dictionary<int, int> lineToLength = new Dictionary<int, int>();

        public Form1()
        {
            InitializeComponent();
        }


        private void Form1_Load(object sender, EventArgs e)
        {
            Helper.Font = new Font(FontFamily.GenericMonospace, 14);
            codeTextBox.SelectionChanged += codeTextBox_SelectionChanged;

            PopulateDescriptions();

            Chip chip = new Chip();

        }

        private void PopulateDescriptions()
        {
            var instructionFile = System.IO.File.ReadAllText("6502InstructionDefinitions.json");

            var definition = new[]
            {
                new
                {
                    Name = "",
                    Description = "",
                }
            };

            var result = Newtonsoft.Json.JsonConvert.DeserializeAnonymousType(instructionFile, definition);

            foreach (var item in result)
            {
                InstructionType type = (InstructionType)Enum.Parse(typeof(InstructionType), item.Name);
                Helper.InstructionDescriptions.Add(type, item.Description);
            }
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

          
            foreach (DataGridViewColumn col in dataGridView1.Columns)
            {
                col.SortMode = DataGridViewColumnSortMode.NotSortable;
                col.ReadOnly = true;
            }

            dataGridView1.ClearSelection();

            dataGridView1.CellClick += DataGridView1_CellClick;
            dataGridView1.DefaultCellStyle.SelectionBackColor = dataGridView1.DefaultCellStyle.BackColor;
            dataGridView1.DefaultCellStyle.SelectionForeColor = dataGridView1.DefaultCellStyle.ForeColor;
            dataGridView1.CellFormatting += DataGridView1_CellFormatting;
        }
        private void ResetTextColor()
        {
            codeTextBox.Select();
            codeTextBox.SelectionColor = Color.Black;
            codeTextBox.SelectionBackColor = codeTextBox.BackColor;
        }

        private void ResetCellColors()
        {
            dataGridView1.ClearSelection();
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                foreach (DataGridViewCell cell in row.Cells)
                {
                    cell.Style.BackColor = dataGridView1.DefaultCellStyle.BackColor;
                    cell.Style.ForeColor = dataGridView1.DefaultCellStyle.ForeColor;
                }
            }
        }


        private void DataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex != 0 || e.RowIndex < 0 || e.RowIndex >= dataGridView1.Rows.Count)
            {
                dataGridView1.DefaultCellStyle.SelectionBackColor = dataGridView1.DefaultCellStyle.BackColor;
                dataGridView1.DefaultCellStyle.SelectionForeColor = dataGridView1.DefaultCellStyle.ForeColor;

                return;
            }
            
            ResetTextColor();
            
            dataGridView1.CurrentRow.DefaultCellStyle.SelectionBackColor = Color.Yellow;

            var textboxLine = reverseMatching[e.RowIndex];
            var length = lineToLength[textboxLine];

            int startIndex = codeTextBox.GetFirstCharIndexFromLine(textboxLine);
            codeTextBox.Select(startIndex, length);
        }

        private void DataGridView1_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0 || e.RowIndex >= dataGridView1.Rows.Count) return;

            switch (e.ColumnIndex)
            {
                case 0:
                    break;

                case 1:
                    break;

                case 2:

                    if (e.RowIndex < 0 || e.RowIndex >= dataGridView1.Rows.Count) return;
                    if (e.ColumnIndex != 2) return;

                    var nameOfInstruction = dataGridView1[e.ColumnIndex, e.RowIndex].Value.ToString().Substring(0, 3);
                    InstructionType type = (InstructionType)Enum.Parse(typeof(InstructionType), nameOfInstruction);

                    dataGridView1[e.ColumnIndex, e.RowIndex].ToolTipText = Helper.InstructionDescriptions[type];
                    break;
            }
        }

        private void BuildButton_Click(object sender, EventArgs e)
        {
            System.IO.File.WriteAllText("temp.txt", codeTextBox.Text);
            Display("temp.txt");
        }

        private void BuildFile_Click(object sender, EventArgs e)
        {
            var lines = System.IO.File.ReadAllLines("6502Code.txt");
            var copy = lines;

            lines = AssemblyParser.ReplaceDefines(lines);
            lines = AssemblyParser.RemoveLabels(lines);
            lines = AssemblyParser.GetRidOfCommentsAndEmptyLines(lines);

            var copyOfCopy = copy;

            copy = copy.Select((x) =>
            {
                if (!x.Contains(';')) return x.Trim();

                return x.Substring(0, x.IndexOf(';')).Trim();
            }).ToArray();


            var table = AssemblyParser.GenerateDefineReplacementTable(copy);
            for (int i = 0; i < copy.Length; i++)
            {
                if (copy.Contains("define")) continue;

                foreach (var defineValue in table)
                {
                    if (copy[i].Contains(defineValue.Key))
                    {
                        copy[i] = copy[i].Replace(defineValue.Key, defineValue.Value);
                    }
                }
            }

            int internalCount = 0;
            for (int i = 0; i < copy.Length; i++)
            {
                if (lines.Contains(copy[i].Trim()))
                {
                    matching.Add(i, internalCount);
                    reverseMatching.Add(internalCount, i);
                    lineToLength.Add(i, copyOfCopy[i].Length);

                    internalCount++;
                }
            }

            codeTextBox.Text = System.IO.File.ReadAllText("6502Code.txt");
            Display("6502Code.txt");
        }

        private void codeTextBox_SelectionChanged(object sender, EventArgs e)
        {
            ResetCellColors();

            int index = codeTextBox.SelectionStart;
            int line = codeTextBox.GetLineFromCharIndex(index);

            if (matching.ContainsKey(line))
            {
                var row = dataGridView1.Rows[matching[line]];
                foreach (DataGridViewCell cell in row.Cells)
                {
                    cell.Style.BackColor = Color.Yellow;
                }
            }

            this.Text = line.ToString();
        }
    }
}
