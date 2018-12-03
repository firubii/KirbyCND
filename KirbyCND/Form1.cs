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
        public Form1()
        {
            InitializeComponent();
        }

        public void ReadCND(BinaryReader reader)
        {
            reader.BaseStream.Seek(0x20, SeekOrigin.Begin);
            reader.BaseStream.Seek(reader.ReadUInt32(), SeekOrigin.Begin);
            uint modelDataSize = reader.ReadUInt32();
            List<uint> modelDataOffsetList = new List<uint>();
            entryList.Nodes.Add("Visual Data");
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
                uint valLength = reader.ReadUInt32();
                string val = string.Join("", reader.ReadChars((int)valLength));
                reader.BaseStream.Seek(varOffset, SeekOrigin.Begin);
                uint varLength = reader.ReadUInt32();
                string var = string.Join("", reader.ReadChars((int)varLength));
                entryList.Nodes[0].Nodes.Add($"{var} {val}");
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
                    uint nameLength = reader.ReadUInt32();
                    string name = string.Join("", reader.ReadChars((int)nameLength));
                    reader.BaseStream.Seek(typeOffset, SeekOrigin.Begin);
                    uint typeLength = reader.ReadUInt32();
                    string type = string.Join("", reader.ReadChars((int)typeLength));
                    entryList.Nodes[0].Nodes[i].Nodes.Add($"{type} {name}");
                    reader.BaseStream.Seek(variableOffsets[v] + 0xC, SeekOrigin.Begin);
                    uint variableCount = reader.ReadUInt32();
                    if (type != "String")
                    {
                        for (int d = 0; d < variableCount; d++)
                        {
                            switch (type)
                            {
                                case "Int":
                                    {
                                        entryList.Nodes[0].Nodes[i].Nodes[v].Nodes.Add(reader.ReadUInt32().ToString());
                                        break;
                                    }
                                case "Vec3":
                                case "Float":
                                    {
                                        entryList.Nodes[0].Nodes[i].Nodes[v].Nodes.Add(reader.ReadSingle().ToString());
                                        break;
                                    }
                                case "Bool":
                                    {
                                        entryList.Nodes[0].Nodes[i].Nodes[v].Nodes.Add(reader.ReadBoolean().ToString());
                                        v = (int)variableCount;
                                        break;
                                    }
                            }
                        }
                    }
                    else
                    {
                        uint stringLen = reader.ReadUInt32();
                        entryList.Nodes[0].Nodes[i].Nodes[v].Nodes.Add(Encoding.UTF8.GetString(reader.ReadBytes((int)stringLen)));
                    }
                }
            }
            reader.BaseStream.Seek(0x30, SeekOrigin.Begin);
            reader.BaseStream.Seek(reader.ReadUInt32(), SeekOrigin.Begin);
            uint size = reader.ReadUInt32();
            List<uint> offsetList = new List<uint>();
            entryList.Nodes.Add("Render Data");
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
                uint valLength = reader.ReadUInt32();
                string val = Encoding.UTF8.GetString(reader.ReadBytes((int)valLength));
                reader.BaseStream.Seek(varOffset, SeekOrigin.Begin);
                uint varLength = reader.ReadUInt32();
                string var = Encoding.UTF8.GetString(reader.ReadBytes((int)varLength));
                entryList.Nodes[1].Nodes.Add($"{var}: {val}");
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
                    uint variableLength = reader.ReadUInt32();
                    string variable = Encoding.UTF8.GetString(reader.ReadBytes((int)variableLength));
                    reader.BaseStream.Seek(typeOffset, SeekOrigin.Begin);
                    uint typeLength = reader.ReadUInt32();
                    string type = Encoding.UTF8.GetString(reader.ReadBytes((int)typeLength));
                    entryList.Nodes[1].Nodes[i].Nodes.Add($"{type} {variable}");
                    reader.BaseStream.Seek(variableOffsets[f] + 0xC, SeekOrigin.Begin);
                    uint variableCount = reader.ReadUInt32();
                    if (type != "String")
                    {
                        for (int v = 0; v < variableCount; v++)
                        {
                            switch (type)
                            {
                                case "Int":
                                    {
                                        entryList.Nodes[1].Nodes[i].Nodes[f].Nodes.Add(reader.ReadUInt32().ToString());
                                        break;
                                    }
                                case "Vec3":
                                case "Float":
                                    {
                                        entryList.Nodes[1].Nodes[i].Nodes[f].Nodes.Add(reader.ReadSingle().ToString());
                                        break;
                                    }
                                case "Bool":
                                    {
                                        entryList.Nodes[1].Nodes[i].Nodes[f].Nodes.Add(reader.ReadBoolean().ToString());
                                        v = (int)variableCount;
                                        break;
                                    }
                            }
                        }
                    }
                    else
                    {
                        uint stringLen = reader.ReadUInt32();
                        entryList.Nodes[1].Nodes[i].Nodes[f].Nodes.Add(Encoding.UTF8.GetString(reader.ReadBytes((int)stringLen)));
                    }
                }
            }
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog open = new OpenFileDialog();
            open.Filter = "CNDBIN Files|*.cndbin";
            if (open.ShowDialog() == DialogResult.OK)
            {
                if (File.Exists(open.FileName))
                {
                    entryList.Nodes.Clear();
                    entryList.BeginUpdate();
                    BinaryReader reader = new BinaryReader(new FileStream(open.FileName, FileMode.Open));
                    ReadCND(reader);
                    entryList.EndUpdate();
                }
            }
        }
    }
}
