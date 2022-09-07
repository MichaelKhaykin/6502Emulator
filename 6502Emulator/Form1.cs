using _6502Emulator.FancyWrappers;
using _6502Emulator.Instructions.Arthimetic;
using _6502Emulator.VisualizationClasses;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace _6502Emulator
{
    public partial class Form1 : Form
    {
        PropertyChangeTracker<MemoryData, byte> MemoryObserver;
        PropertyChangeTracker<string, short> ShortRegisterObserver;
        PropertyChangeTracker<string, byte> ByteRegisterObserver;
        PropertyChangeTracker<FlagData, bool> FlagsObserver;

        MappingsHelper helper;

        Chip chip;

        CheckBox debugInHex;
        CheckBox debugMode;

        string currentFileSelected = "";

        PictureBox mmioBox;

        ColorWheelControl cw;
        Label encodedValueLabel;
        public Form1()
        {
            InitializeComponent();

            this.WindowState = FormWindowState.Maximized;
        }

        private void Form1_Load(object sender, EventArgs e)
        {

            cw = new ColorWheelControl()
            {
                Radius = 100,
                Location = new Point(0, ClientSize.Height - 200),
            };


            encodedValueLabel = new Label()
            {
                Location = new Point(cw.Location.X, cw.Location.Y - 20),
            };

            Controls.Add(cw);
            Controls.Add(encodedValueLabel);

            MemoryObserver = new PropertyChangeTracker<MemoryData, byte>();
            ShortRegisterObserver = new PropertyChangeTracker<string, short>();
            ByteRegisterObserver = new PropertyChangeTracker<string, byte>();
            FlagsObserver = new PropertyChangeTracker<FlagData, bool>();

            Computer.Ram = new Ram(MemoryObserver.OnPropChanged);

            mmioBox = new PictureBox()
            {
                Image = MMIO.ScaledMap(),
                SizeMode = PictureBoxSizeMode.AutoSize,
            };

            mmioBox.Location = new Point(ClientSize.Width - mmioBox.Width - 50, 0);

            Controls.Add(mmioBox);

            fileComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            fileComboBox.FormattingEnabled = true;

            var path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            string[] files =
                Directory.GetFiles(path, "*.asm", SearchOption.TopDirectoryOnly);

            fileComboBox.Items.AddRange(files.Select(x => x.Remove(0, path.Length + 1)).ToArray());

            debugInHex = new CheckBox()
            {
                Location = new Point(fileComboBox.Right, BuildButton.Location.Y),
                Text = "Display in Hex",
                Checked = true
            };
            debugInHex.CheckedChanged += DebugInHex_CheckedChanged;

            debugMode = new CheckBox()
            {
                Location = new Point(debugInHex.Location.X, debugInHex.Bottom),
                Text = "Debug",
                Checked = false,
            };
            debugMode.CheckedChanged += DebugMode_CheckedChanged;
            
            Controls.Add(debugInHex);
            Controls.Add(debugMode);

            
            Helper.Font = new Font(FontFamily.GenericMonospace, 14);
            codeTextBox.SelectionChanged += codeTextBox_SelectionChanged;

            PopulateDescriptions();
        }

        private void DebugMode_CheckedChanged(object sender, EventArgs e)
        {
            DebugButton.Visible = debugMode.Checked;
        }

        private void DebugInHex_CheckedChanged(object sender, EventArgs e)
        {
            DataGridView view = null;
            foreach(Control control in Controls)
            {
                if(control.Name == "DebugView")
                {
                    view = (DataGridView)control;
                    break;
                }
            }

            if (view == null) return;

            var source = (DataTable)view.DataSource;

            for(int i = 0; i < source.Rows.Count; i++)
            {
                if(debugInHex.Checked == true)
                {
                    var decimalString = (string)source.Rows[i][1];

                    if (int.TryParse(decimalString, out int decimalValue) == false) continue;

                    source.Rows[i][1] = "0x" + Convert.ToString(decimalValue, 16);
                }
                else
                {
                    var hexValue = (string)source.Rows[i][1];

                    if (hexValue.Contains("0x"))
                    {
                        hexValue = hexValue.Substring(2);
                    }

                    bool worked = hexValue.ToDecimal(out int @decimal);
                    if (!worked) continue;

                    source.Rows[i][1] = @decimal;
                }
            }
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

            ShortRegisterObserver.ChangedPropValues.Clear();
            ByteRegisterObserver.ChangedPropValues.Clear();
            FlagsObserver.ChangedPropValues.Clear();
            MemoryObserver.ChangedPropValues.Clear();

            chip = new Chip(ShortRegisterObserver.OnPropChanged, ByteRegisterObserver.OnPropChanged, FlagsObserver.OnPropChanged, instructions);
            RunButton.Visible = true;
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
            MMIO.Clear();

            for(int i = 0; i < Controls.Count; i++)
            {
                if(Controls[i].Tag as string == "RemoveMe")
                {
                    Controls.RemoveAt(i);
                    break;
                }
            }

            fileComboBox.SelectedIndex = -1;

            System.IO.File.WriteAllText("temp.txt", codeTextBox.Text);

            helper = new MappingsHelper(System.IO.File.ReadAllLines("temp.txt"));

            Display("temp.txt");
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
            DebugStep();
        }

        private void DebugStep()
        {
            if (chip == null)
            {
                MessageBox.Show("Please build code first");
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

            Visualize();
            chip.EmulateSingleInstruction();
        }

        public void Visualize()
        {
            ResetCellColors();
            ResetTextColor();

            var index = chip.OffsetToIndexMap[chip.ProgramCounter.Value];
            index = helper.DissassemblyIndexToLineIndex[index];

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

            DataTable table = new DataTable();
            table.Columns.Add();
            table.Columns.Add();

            foreach (var memorykvp in MemoryObserver.ChangedPropValues)
            {
                var strKey = MemoryObserver.KeyDisplayMethod(memorykvp.Key);

                table.Rows.Add(strKey, "0x" + Convert.ToString(memorykvp.Value, 16));
            }

            foreach (var flagvaluekvp in FlagsObserver.ChangedPropValues)
            {
                var strKey = FlagsObserver.KeyDisplayMethod(flagvaluekvp.Key);

                table.Rows.Add(strKey, flagvaluekvp.Value);
            }

            foreach (var registershortkvp in ShortRegisterObserver.ChangedPropValues)
            {
                var strKey = ShortRegisterObserver.KeyDisplayMethod(registershortkvp.Key);

                table.Rows.Add(strKey, "0x" + Convert.ToString(registershortkvp.Value, 16));
            }

            foreach (var registerbytekvp in ByteRegisterObserver.ChangedPropValues)
            {
                var strKey = ByteRegisterObserver.KeyDisplayMethod(registerbytekvp.Key);

                table.Rows.Add(strKey, "0x" + Convert.ToString(registerbytekvp.Value, 16));
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
                Name = "DebugView",
                BackgroundColor = BackColor,
            };

            foreach (DataGridViewColumn col in newDataGridView.Columns)
            {
                col.SortMode = DataGridViewColumnSortMode.NotSortable;
                col.ReadOnly = true;
            }

            newDataGridView.ClearSelection();

            mmioBox.Image = MMIO.ScaledMap();
            mmioBox.Invalidate();

            Controls.Add(newDataGridView);

        }
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            var currentFileSelected = fileComboBox.Text;

            if (currentFileSelected == "") return;

            var lines = System.IO.File.ReadAllLines(currentFileSelected);

            helper = new MappingsHelper(lines);

            codeTextBox.Text = System.IO.File.ReadAllText(currentFileSelected);
            Display(currentFileSelected);
        }

        private void RunButton_Click(object sender, EventArgs e)
        {
            MMIO.Clear();

            while (!chip.Finished)
            {
                chip.EmulateSingleInstruction();
                //DebugStep();
            }

            Visualize();
        }

        private void ColorTimer_Tick(object sender, EventArgs e)
        {
            encodedValueLabel.Text = $"{Encode(cw.ColorSelected)}";
        }

        private byte Encode(Color color)
        {
            var highbytes = (color.R * 7 / 255) << 5;
            var midbytest = (color.G * 7 / 255) << 2;
            var lowbytes = (color.B * 3 / 255);

            return (byte)(highbytes + midbytest + lowbytes);
        }
    }
}
