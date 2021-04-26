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

namespace KirbyCND
{
    public partial class Form1 : Form
    {
        string filePath;

        public Form1()
        {
            InitializeComponent();
        }

        public void WriteCND(BinaryWriter writer)
        {
            
        }

        public void ReadCND(BinaryReader reader)
        {
            entryList.Nodes.Clear();
            entryList.BeginUpdate();
            reader.BaseStream.Seek(0x28, SeekOrigin.Begin);
            reader.BaseStream.Seek(reader.ReadUInt32(), SeekOrigin.Begin);
            string n = Encoding.UTF8.GetString(reader.ReadBytes(reader.ReadInt32()));
            reader.BaseStream.Seek(0x2C, SeekOrigin.Begin);
            reader.BaseStream.Seek(reader.ReadUInt32(), SeekOrigin.Begin);
            string n2 = Encoding.UTF8.GetString(reader.ReadBytes(reader.ReadInt32()));
            entryList.Nodes.Add($"{n}: {n2}");
            reader.BaseStream.Seek(0x20, SeekOrigin.Begin);
            reader.BaseStream.Seek(reader.ReadUInt32(), SeekOrigin.Begin);
            uint modelDataSize = reader.ReadUInt32();
            List<uint> modelDataOffsetList = new List<uint>();
            entryList.Nodes[0].Nodes.Add("Visual Data");
            for (int i = 0; i < modelDataSize; i++)
            {
                modelDataOffsetList.Add(reader.ReadUInt32());
            }
            for (int i = 0; i < modelDataSize; i++)
            {
                reader.BaseStream.Seek(modelDataOffsetList[i], SeekOrigin.Begin);
                uint valOffset = reader.ReadUInt32();
                uint varOffset = reader.ReadUInt32();
                reader.BaseStream.Seek(valOffset, SeekOrigin.Begin);
                string val = Encoding.UTF8.GetString(reader.ReadBytes(reader.ReadInt32()));
                reader.BaseStream.Seek(varOffset, SeekOrigin.Begin);
                string var = Encoding.UTF8.GetString(reader.ReadBytes(reader.ReadInt32()));
                entryList.Nodes[0].Nodes[0].Nodes.Add($"{var} {val}");
                reader.BaseStream.Seek(modelDataOffsetList[i] + 0xC, SeekOrigin.Begin);
                reader.BaseStream.Seek(reader.ReadUInt32(), SeekOrigin.Begin);
                uint variableListCount = reader.ReadUInt32();
                List<uint> variableOffsets = new List<uint>();
                for (int v = 0; v < variableListCount; v++)
                {
                    variableOffsets.Add(reader.ReadUInt32());
                }
                for (int v = 0; v < variableListCount; v++)
                {
                    reader.BaseStream.Seek(variableOffsets[v], SeekOrigin.Begin);
                    uint nameOffset = reader.ReadUInt32();
                    uint typeOffset = reader.ReadUInt32();
                    reader.BaseStream.Seek(nameOffset, SeekOrigin.Begin);
                    string name = Encoding.UTF8.GetString(reader.ReadBytes(reader.ReadInt32()));
                    reader.BaseStream.Seek(typeOffset, SeekOrigin.Begin);
                    string type = Encoding.UTF8.GetString(reader.ReadBytes(reader.ReadInt32()));
                    entryList.Nodes[0].Nodes[0].Nodes[i].Nodes.Add($"{type} {name}");
                    reader.BaseStream.Seek(variableOffsets[v] + 0xC, SeekOrigin.Begin);
                    uint variableSize = reader.ReadUInt32();

                    byte[] data = reader.ReadBytes((int)variableSize);
                    switch (type)
                    {
                        default:
                        case "Int":
                            {
                                entryList.Nodes[0].Nodes[0].Nodes[i].Nodes[v].Nodes.Add(BitConverter.ToInt32(data, 0).ToString());
                                break;
                            }
                        case "Float":
                            {
                                entryList.Nodes[0].Nodes[0].Nodes[i].Nodes[v].Nodes.Add(BitConverter.ToSingle(data, 0).ToString());
                                break;
                            }
                        case "Bool":
                            {
                                entryList.Nodes[0].Nodes[0].Nodes[i].Nodes[v].Nodes.Add(Convert.ToBoolean(BitConverter.ToInt32(data, 0)).ToString());
                                break;
                            }
                        case "Color4":
                            {
                                entryList.Nodes[0].Nodes[0].Nodes[i].Nodes[v].Nodes.Add(data[0].ToString());
                                entryList.Nodes[0].Nodes[0].Nodes[i].Nodes[v].Nodes.Add(data[1].ToString());
                                entryList.Nodes[0].Nodes[0].Nodes[i].Nodes[v].Nodes.Add(data[2].ToString());
                                entryList.Nodes[0].Nodes[0].Nodes[i].Nodes[v].Nodes.Add(data[3].ToString());
                                break;
                            }
                        case "Vec3":
                            {
                                entryList.Nodes[0].Nodes[0].Nodes[i].Nodes[v].Nodes.Add(BitConverter.ToSingle(data, 0).ToString());
                                entryList.Nodes[0].Nodes[0].Nodes[i].Nodes[v].Nodes.Add(BitConverter.ToSingle(data, 4).ToString());
                                entryList.Nodes[0].Nodes[0].Nodes[i].Nodes[v].Nodes.Add(BitConverter.ToSingle(data, 8).ToString());
                                break;
                            }
                        case "String":
                            {
                                Console.WriteLine(reader.BaseStream.Position);
                                entryList.Nodes[0].Nodes[0].Nodes[i].Nodes[v].Nodes.Add(Encoding.UTF8.GetString(reader.ReadBytes(reader.ReadInt32())));
                                break;
                            }
                    }
                }
            }
            reader.BaseStream.Seek(0x30, SeekOrigin.Begin);
            reader.BaseStream.Seek(reader.ReadUInt32(), SeekOrigin.Begin);
            uint size = reader.ReadUInt32();
            List<uint> offsetList = new List<uint>();
            entryList.Nodes[0].Nodes.Add("Render Data");
            for (int i = 0; i < size; i++)
            {
                offsetList.Add(reader.ReadUInt32());
            }
            for (int i = 0; i < size; i++)
            {
                reader.BaseStream.Seek(offsetList[i], SeekOrigin.Begin);
                uint valOffset = reader.ReadUInt32();
                uint varOffset = reader.ReadUInt32();
                reader.BaseStream.Seek(valOffset, SeekOrigin.Begin);
                string val = Encoding.UTF8.GetString(reader.ReadBytes(reader.ReadInt32()));
                reader.BaseStream.Seek(varOffset, SeekOrigin.Begin);
                string var = Encoding.UTF8.GetString(reader.ReadBytes(reader.ReadInt32()));
                entryList.Nodes[0].Nodes[1].Nodes.Add($"{var}: {val}");
                reader.BaseStream.Seek(offsetList[i] + 0xC, SeekOrigin.Begin);
                reader.BaseStream.Seek(reader.ReadUInt32(), SeekOrigin.Begin);
                uint variableListCount = reader.ReadUInt32();
                List<uint> variableOffsets = new List<uint>();
                for (int f = 0; f < variableListCount; f++)
                {
                    variableOffsets.Add(reader.ReadUInt32());
                }
                for (int f = 0; f < variableListCount; f++)
                {
                    reader.BaseStream.Seek(variableOffsets[f], SeekOrigin.Begin);
                    uint variableOffset = reader.ReadUInt32();
                    uint typeOffset = reader.ReadUInt32();
                    reader.BaseStream.Seek(variableOffset, SeekOrigin.Begin);
                    string variable = Encoding.UTF8.GetString(reader.ReadBytes(reader.ReadInt32()));
                    reader.BaseStream.Seek(typeOffset, SeekOrigin.Begin);
                    string type = Encoding.UTF8.GetString(reader.ReadBytes(reader.ReadInt32()));
                    entryList.Nodes[0].Nodes[1].Nodes[i].Nodes.Add($"{type} {variable}");
                    reader.BaseStream.Seek(variableOffsets[f] + 0xC, SeekOrigin.Begin);
                    uint variableSize = reader.ReadUInt32();

                    byte[] data = reader.ReadBytes((int)variableSize);
                    switch (type)
                    {
                        default:
                        case "Int":
                            {
                                entryList.Nodes[0].Nodes[1].Nodes[i].Nodes[f].Nodes.Add(BitConverter.ToInt32(data, 0).ToString());
                                break;
                            }
                        case "Float":
                            {
                                entryList.Nodes[0].Nodes[1].Nodes[i].Nodes[f].Nodes.Add(BitConverter.ToSingle(data, 0).ToString());
                                break;
                            }
                        case "Bool":
                            {
                                entryList.Nodes[0].Nodes[1].Nodes[i].Nodes[f].Nodes.Add(Convert.ToBoolean(BitConverter.ToInt32(data, 0)).ToString());
                                break;
                            }
                        case "Color4":
                            {
                                entryList.Nodes[0].Nodes[1].Nodes[i].Nodes[f].Nodes.Add(data[0].ToString());
                                entryList.Nodes[0].Nodes[1].Nodes[i].Nodes[f].Nodes.Add(data[1].ToString());
                                entryList.Nodes[0].Nodes[1].Nodes[i].Nodes[f].Nodes.Add(data[2].ToString());
                                entryList.Nodes[0].Nodes[1].Nodes[i].Nodes[f].Nodes.Add(data[3].ToString());
                                break;
                            }
                        case "Vec3":
                            {
                                entryList.Nodes[0].Nodes[1].Nodes[i].Nodes[f].Nodes.Add(BitConverter.ToSingle(data, 0).ToString());
                                entryList.Nodes[0].Nodes[1].Nodes[i].Nodes[f].Nodes.Add(BitConverter.ToSingle(data, 4).ToString());
                                entryList.Nodes[0].Nodes[1].Nodes[i].Nodes[f].Nodes.Add(BitConverter.ToSingle(data, 8).ToString());
                                break;
                            }
                        case "String":
                            {
                                Console.WriteLine(reader.BaseStream.Position);
                                entryList.Nodes[0].Nodes[1].Nodes[i].Nodes[f].Nodes.Add(Encoding.UTF8.GetString(reader.ReadBytes(reader.ReadInt32())));
                                break;
                            }
                    }
                }
            }
            entryList.Nodes[0].Expand();
            entryList.Nodes[0].Nodes[1].Expand();
            entryList.EndUpdate();
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog open = new OpenFileDialog();
            open.Filter = "CNDBIN Files|*.cndbin";
            if (open.ShowDialog() == DialogResult.OK)
            {
                if (File.Exists(open.FileName))
                {
                    filePath = open.FileName;
                    this.Enabled = false;
                    this.Cursor = Cursors.WaitCursor;
                    this.Text = $"KirbyCND - Reading {filePath.Split('\\').Last()}...";
                    BinaryReader reader = new BinaryReader(new FileStream(filePath, FileMode.Open));
                    ReadCND(reader);
                    this.Enabled = true;
                    this.Cursor = Cursors.Arrow;
                    this.Text = $"KirbyCND - {filePath.Split('\\').Last()}";
                    saveToolStripMenuItem.Enabled = true;
                    saveAsToolStripMenuItem.Enabled = true;
                }
            }
        }

        private void entryList_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (entryList.SelectedNode.Nodes.Count == 0)
            {
                entryList.SelectedNode.BeginEdit();
            }
        }
    }
}
