using _6502Emulator.FancyWrappers;
using _6502Emulator.Instructions.Arthimetic;
using _6502Emulator.VisualizationClasses;
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
        Action<FancyMemory<byte>, PropertyChangedEventArgs> onMemoryValueChanged;
        Dictionary<int, byte> memoryValues = new Dictionary<int, byte>();

        Action<FancyRegister<short>, PropertyChangedEventArgs> onRegisterValueChangedShort;
        Dictionary<string, short> registerShortValues = new Dictionary<string, short>();
        
        Action<FancyRegister<byte>, PropertyChangedEventArgs> onRegisterValueChangedByte;
        Dictionary<string, byte> registerByteValues = new Dictionary<string, byte>();

        Action<FancyFlag, PropertyChangedEventArgs> onFlagChanged;
        Dictionary<FlagType, bool> flagValues = new Dictionary<FlagType, bool>();

        MappingsHelper helper;

        Chip chip;
        public Form1()
        {
            InitializeComponent();

            

            onMemoryValueChanged = new Action<FancyMemory<byte>, PropertyChangedEventArgs>((obj, args) =>
            {
                if (memoryValues.ContainsKey(obj.Index) == false)
                {
                    memoryValues.Add(obj.Index, 0);
                }
                memoryValues[obj.Index] = obj.Value;
            });
            onRegisterValueChangedShort = new Action<FancyRegister<short>, PropertyChangedEventArgs>((obj, args) =>
            {
                if (registerShortValues.ContainsKey(obj.Name) == false)
                {
                    registerShortValues.Add(obj.Name, 0);
                }
                registerShortValues[obj.Name] = obj.Value;
            });
            onRegisterValueChangedByte = new Action<FancyRegister<byte>, PropertyChangedEventArgs>((obj, args) =>
            {
                if (registerByteValues.ContainsKey(obj.Name) == false)
                {
                    registerByteValues.Add(obj.Name, 0);
                }
                registerByteValues[obj.Name] = obj.Value;
            });
            onFlagChanged = new Action<FancyFlag, PropertyChangedEventArgs>((obj, args) =>
            {
                if (flagValues.ContainsKey(obj.Type) == false)
                {
                    flagValues.Add(obj.Type, obj.HasValue);
                }
                flagValues[obj.Type] = obj.HasValue;
            });

            Computer.Ram = new Ram(onMemoryValueChanged);
        }


        private void Form1_Load(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Maximized;

            Helper.Font = new Font(FontFamily.GenericMonospace, 14);
            codeTextBox.SelectionChanged += codeTextBox_SelectionChanged;

            PopulateDescriptions();
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

            dataGridView1.BackgroundColor = this.BackColor;

            chip = new Chip(onRegisterValueChangedShort, onRegisterValueChangedByte, onFlagChanged, instructions);
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

            var textboxLine = helper.DissassemblyIndexToLineIndex[e.RowIndex];
            var length = helper.LineIndexToLength[textboxLine];

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

                    if (dataGridView1[e.ColumnIndex, e.RowIndex].Value == null) return;

                    var nameOfInstruction = dataGridView1[e.ColumnIndex, e.RowIndex].Value.ToString().Substring(0, 3);
                    InstructionType type = (InstructionType)Enum.Parse(typeof(InstructionType), nameOfInstruction);

                    dataGridView1[e.ColumnIndex, e.RowIndex].ToolTipText = Helper.InstructionDescriptions[type];
                    break;
            }
        }

        private void BuildButton_Click(object sender, EventArgs e)
        {
            System.IO.File.WriteAllText("temp.txt", codeTextBox.Text);

            helper = new MappingsHelper(System.IO.File.ReadAllLines("temp.txt"));

            Display("temp.txt");
        }

        private void BuildFile_Click(object sender, EventArgs e)
        {
            var lines = System.IO.File.ReadAllLines("6502Code.txt");

            helper = new MappingsHelper(lines);

            codeTextBox.Text = System.IO.File.ReadAllText("6502Code.txt");
            Display("6502Code.txt");
        }

        private void codeTextBox_SelectionChanged(object sender, EventArgs e)
        {
            ResetCellColors();

            int index = codeTextBox.SelectionStart;
            int line = codeTextBox.GetLineFromCharIndex(index);

            if (helper == null) return;

            if (helper.LineIndexToDissassemblyIndex.ContainsKey(line))
            {
                var row = dataGridView1.Rows[helper.LineIndexToDissassemblyIndex[line]];
                foreach (DataGridViewCell cell in row.Cells)
                {
                    cell.Style.BackColor = Color.Yellow;
                }
            }
        }

        private void DebugButton_Click(object sender, EventArgs e)
        {
            if (chip == null)
            {
                MessageBox.Show("Please build code first");
                return;
            }
            if (helper.LineIndicies.Count == 0)
            {
                MessageBox.Show("Program has finished");
                return;
            }

            for (int i = 0; i < Controls.Count; i++)
            {
                if ((string)Controls[i].Tag == "RemoveMe")
                {
                    Controls.RemoveAt(i);
                    break;
                }
            }

            ResetCellColors();
            ResetTextColor();

            var index = helper.LineIndicies.Dequeue();

            if (helper.LineIndexToDissassemblyIndex.ContainsKey(index))
            {
                var row = dataGridView1.Rows[helper.LineIndexToDissassemblyIndex[index]];
                foreach (DataGridViewCell cell in row.Cells)
                {
                    cell.Style.BackColor = Color.Green;
                }
            }

            codeTextBox.Select(codeTextBox.GetFirstCharIndexFromLine(index), helper.LineIndexToLength[index]);
            codeTextBox.SelectionColor = Color.Red;

            chip.EmulateSingleInstruction(helper.LineIndexToDissassemblyIndex[index]);

            DataTable table = new DataTable();
            table.Columns.Add();
            table.Columns.Add();

            foreach (var memorykvp in memoryValues)
            {
                table.Rows.Add(memorykvp.Key, "0x" + Convert.ToString(memorykvp.Value, 16));
            }

            foreach (var flagvaluekvp in flagValues)
            {
                table.Rows.Add(flagvaluekvp.Key, flagvaluekvp.Value);
            }

            foreach (var registershortkvp in registerShortValues)
            {
                table.Rows.Add(registershortkvp.Key, "0x" + Convert.ToString(registershortkvp.Value, 16));
            }

            foreach (var registerbytekvp in registerByteValues)
            {
                table.Rows.Add(registerbytekvp.Key, "0x" + Convert.ToString(registerbytekvp.Value, 16));
            }

            var newDataGridView = new DataGridView()
            {
                Location = new Point(codeTextBox.Right, 0),
                DataSource = table,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                Height = this.Height,
                RowHeadersVisible = false,
                ColumnHeadersVisible = false,
                BorderStyle = BorderStyle.None,
                CellBorderStyle = DataGridViewCellBorderStyle.None,
                Tag = "RemoveMe",
                BackgroundColor = BackColor,
            };

            foreach (DataGridViewColumn col in newDataGridView.Columns)
            {
                col.SortMode = DataGridViewColumnSortMode.NotSortable;
                col.ReadOnly = true;
            }

            newDataGridView.ClearSelection();

            Controls.Add(newDataGridView);
        }
    }
}
