﻿using System;
using System.Net;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Imaging;
using System.IO;
using ModelsWorker.model;
using ModelsWorker;
using RavSoft.GoogleTranslator;

namespace __Model_Studio
{
    public partial class Form1 : Form
    {

        #region Variables

        Translator tl = new Translator();
        ModelsBin.ModelFile mf = new ModelsBin.ModelFile();
        ModelContainer MCon = new ModelContainer();
        ModelParser ModelParser = new ModelParser();
        ModelBuilder ModelBuilder = new ModelBuilder();
        List<byte[]> EntryData = new List<byte[]>();
        bool HasFileOpen = false;


        static string[] IconSheetIndex =
{"unknown",
"creeper",
"skeleton",
"spider",
"zombie",
"slime",
"ghast",
"pigzombie",
"enderman",
"cavespider",
"silverfish",
"blaze",
"lavaslime",
"enderdragon",
"witherBoss",
"bat",
"villager.witch",
"endermite",
"guardian",
"shulker",
"pig",
"sheep",
"cow",
"chicken",
"squid",
"wolf",
"redcow",
"snowgolem",
"ocelot",
"irongolem",
"horse",
"donkey",
"mule",
"skeletonhorse",
"zombiehorse",
"rabbit",
"polarbear",
"llama",
"parrot",
"villager",
"vindicator",
"evoker",
"vex",
"skeleton.stray",
"illusioner",
"endcrystal",
"zombie.villager",
"elderguardian",
"zombie.husk",
"skeleton.wither",
"package",
"folder",
"single",
"float",
"text",
"integer",
"dolphin",
"dragon",
"trident",
"boat",
"minecart",
"dragon_head",
"creeper_head",
"sheep.sheared"};

        #endregion

        #region functions

        public void OpenModels()
        {
            OpenFileDialog opf = new OpenFileDialog();
            opf.Filter = "Models File|*.bin";
            opf.InitialDirectory = Environment.CurrentDirectory;
            if (opf.ShowDialog() == DialogResult.OK)
            {
                FileNodeTree.Nodes.Clear();
                EntryNodeTree.Nodes.Clear();
                //mf.OpenModel(opf.FileName, FileNodeTree, EntryNodeTree);
                MCon = ModelParser.Parse(opf.FileName);
                GetNodes(FileNodeTree, EntryNodeTree);
                saveToolStripMenuItem.Enabled = true;
                HasFileOpen = true;
            }
            FileNodeTree.Nodes[0].Expand();
        }

        public void SaveModels()
        {
            SaveFileDialog opf = new SaveFileDialog();
            opf.Filter = "Models File|*.bin";
            opf.InitialDirectory = Environment.CurrentDirectory;
            if (opf.ShowDialog() == DialogResult.OK)
            {
                mf.SaveModel(opf.FileName, FileNodeTree.Nodes[0]);
            }
        }

        public static byte[] StringToByteArrayFastest(string hex)
        {
            if (hex.Length % 2 == 1)
                throw new Exception("The binary key cannot have an odd number of digits");

            byte[] arr = new byte[hex.Length >> 1];

            for (int i = 0; i < hex.Length >> 1; ++i)
            {
                arr[i] = (byte)((GetHexVal(hex[i << 1]) << 4) + (GetHexVal(hex[(i << 1) + 1])));
            }

            return arr;
        }

        public static int GetHexVal(char hex)
        {
            int val = (int)hex;
            //For uppercase A-F letters:
            //return val - (val < 58 ? 48 : 55);
            //For lowercase a-f letters:
            //return val - (val < 58 ? 48 : 87);
            //Or the two combined, but a bit slower:
            return val - (val < 58 ? 48 : (val < 97 ? 55 : 87));
        }

        public void ResetNodeCount(TreeNode tn)
        {
            byte[] data = StringToByteArrayFastest(tn.Tag.ToString().Replace("-", ""));
            List<byte> newdat = new List<byte>();
            newdat.AddRange(data.Take(data.Length - 4));
            newdat.AddRange(BitConverter.GetBytes(tn.Nodes.Count).Reverse().ToArray());
            tn.Tag = BitConverter.ToString(newdat.ToArray());
        }

        public void RebuildFileNode()
        {

            int g = ModelsBin.GetNodeparents(FileNodeTree.SelectedNode);
            List<byte> OutputBytes = new List<byte>();

            switch (g)
            {
                case (4):
                    {
                        foreach(TreeNode tn in EntryNodeTree.Nodes[0].Nodes)
                        {
                            byte[] data = StringToByteArrayFastest(tn.Tag.ToString().Replace("-", "")).Reverse().ToArray();
                            OutputBytes.AddRange(data);
                        }
                        FileNodeTree.SelectedNode.Tag = BitConverter.ToString(OutputBytes.ToArray());
                    }
                    break;
                case (3):
                    {

                        byte[] nomLen = BitConverter.GetBytes(Int16.Parse(FileNodeTree.SelectedNode.Text.Length.ToString())).Reverse().ToArray();
                        byte[] nom = Encoding.Default.GetBytes(FileNodeTree.SelectedNode.Text);
                        byte[] TranslationX = StringToByteArrayFastest(EntryNodeTree.Nodes[0].Nodes[1].Tag.ToString().Replace("-", "")).ToArray();
                        byte[] TranslationY = StringToByteArrayFastest(EntryNodeTree.Nodes[0].Nodes[2].Tag.ToString().Replace("-", "")).ToArray();
                        byte[] TranslationZ = StringToByteArrayFastest(EntryNodeTree.Nodes[0].Nodes[3].Tag.ToString().Replace("-", "")).ToArray();
                        byte[] OffsetX = StringToByteArrayFastest(EntryNodeTree.Nodes[0].Nodes[4].Tag.ToString().Replace("-", "")).ToArray();
                        byte[] OffsetY = StringToByteArrayFastest(EntryNodeTree.Nodes[0].Nodes[5].Tag.ToString().Replace("-", "")).ToArray();
                        byte[] RotationX = StringToByteArrayFastest(EntryNodeTree.Nodes[0].Nodes[6].Tag.ToString().Replace("-", "")).ToArray();
                        byte[] RotationY = StringToByteArrayFastest(EntryNodeTree.Nodes[0].Nodes[7].Tag.ToString().Replace("-", "")).ToArray();
                        byte[] RotationZ = StringToByteArrayFastest(EntryNodeTree.Nodes[0].Nodes[8].Tag.ToString().Replace("-", "")).ToArray();

                        OutputBytes.AddRange(nomLen);
                        OutputBytes.AddRange(nom);
                        OutputBytes.AddRange(new byte[] { 0x00, 0x00, 0x00, 0x00 });
                        OutputBytes.AddRange(TranslationX);
                        OutputBytes.AddRange(TranslationY);
                        OutputBytes.AddRange(TranslationZ);
                        OutputBytes.AddRange(OffsetX);
                        OutputBytes.AddRange(OffsetY);
                        OutputBytes.AddRange(RotationX);
                        OutputBytes.AddRange(RotationY);
                        OutputBytes.AddRange(RotationZ);
                        OutputBytes.AddRange(new byte[] { 0x00, 0x00, 0x00, 0x00 });
                        FileNodeTree.SelectedNode.Tag = BitConverter.ToString(OutputBytes.ToArray());
                        ResetNodeCount(FileNodeTree.SelectedNode);
                    }
                    break;
                case (2):
                    {
                        byte[] nomLen = BitConverter.GetBytes(Int16.Parse(FileNodeTree.SelectedNode.Text.Length.ToString())).Reverse().ToArray();
                        byte[] nom = Encoding.Default.GetBytes(FileNodeTree.SelectedNode.Text);
                        byte[] wid = StringToByteArrayFastest(EntryNodeTree.Nodes[0].Nodes[1].Tag.ToString().Replace("-", "")).ToArray();
                        byte[] hei = StringToByteArrayFastest(EntryNodeTree.Nodes[0].Nodes[2].Tag.ToString().Replace("-", "")).ToArray();
                        OutputBytes.AddRange(nomLen);
                        OutputBytes.AddRange(nom);
                        OutputBytes.AddRange(wid);
                        OutputBytes.AddRange(hei);
                        OutputBytes.AddRange(new byte[] { 0x00, 0x00, 0x00, 0x00 });
                        FileNodeTree.SelectedNode.Tag = BitConverter.ToString(OutputBytes.ToArray());
                        ResetNodeCount(FileNodeTree.SelectedNode);
                    }
                    break;
            }

        }

        private static int GetIndex(string nom)
        {
            //Console.WriteLine(nom + " -- " + Array.IndexOf(IconSheetIndex, nom));
            return Array.IndexOf(IconSheetIndex, nom);
        }

        public void GetNodes(TreeView treeView0, TreeView treeView1)
        {


            #region AddImageList

            // Get the inputs.
            Bitmap bm = (Bitmap)Properties.Resources.mobs;
            int wid = 32;
            int hgt = 32;


            ImageList imageList = new ImageList();
            imageList.ColorDepth = ColorDepth.Depth32Bit;
            imageList.ImageSize = new Size(20, 20);


            // Start splitting the Bitmap.
            Bitmap piece = new Bitmap(wid, hgt);
            Rectangle dest_rect = new Rectangle(0, 0, wid, hgt);
            using (Graphics gr = Graphics.FromImage(piece))
            {
                int num_rows = bm.Height / hgt;
                int num_cols = bm.Width / wid;
                Rectangle source_rect = new Rectangle(0, 0, wid, hgt);
                for (int row = 0; row < num_rows; row++)
                {
                    source_rect.X = 0;
                    for (int col = 0; col < num_cols; col++)
                    {
                        // Copy the piece of the image.
                        gr.Clear(Color.Transparent);
                        gr.DrawImage(bm, dest_rect, source_rect,
                            GraphicsUnit.Pixel);

                        // Save the piece.
                        MemoryStream ms = new MemoryStream();
                        piece.Save(ms, ImageFormat.Png);
                        Image imag = Image.FromStream(ms);
                        imageList.Images.Add(imag);

                        // Move to the next column.
                        source_rect.X += wid;
                    }
                    source_rect.Y += hgt;
                }
            }
            Console.WriteLine(imageList.Images.Count + "--");
            treeView0.ImageList = imageList;
            treeView1.ImageList = imageList;

            #endregion

            TreeNode treeNode = new TreeNode();
            treeNode.Text = "Models.bin [" + MCon.models.Count + " Models]";

            treeNode.ImageIndex = GetIndex("package");
            treeNode.SelectedImageIndex = GetIndex("package");

            foreach (KeyValuePair<string, ModelsWorker.model.ModelPiece> Piece in MCon.models)
            {
                TreeNode treeNodeModel = new TreeNode(Piece.Key);
                treeNodeModel.Text = Piece.Key.ToString();
                treeNodeModel.ImageIndex = GetIndex(Piece.Key.ToString());
                treeNodeModel.SelectedImageIndex = GetIndex(Piece.Key.ToString());

                foreach (KeyValuePair<string, ModelsWorker.model.ModelPart> Part in Piece.Value.Parts)
                {
                    TreeNode treeNodeModelPiece = new TreeNode(Part.Key);
                    treeNodeModelPiece.Text = Part.Key.ToString();
                    treeNodeModelPiece.ImageIndex = GetIndex(Part.Key.ToString());
                    treeNodeModelPiece.SelectedImageIndex = GetIndex(Part.Key.ToString());


                    foreach (KeyValuePair<string, ModelsWorker.model.ModelBox> Box in Part.Value.Boxes)
                    {
                        TreeNode treeNodeModelPieceBox = new TreeNode();
                        treeNodeModelPieceBox.Text = "Box " + Box.Key.ToString();
                        treeNodeModelPieceBox.ImageIndex = GetIndex(Part.Key.ToString());
                        treeNodeModelPieceBox.SelectedImageIndex = GetIndex(Part.Key.ToString());

                        treeNodeModelPiece.Nodes.Add(treeNodeModelPieceBox);
                    }

                    treeNodeModel.Nodes.Add(treeNodeModelPiece);
                }

                treeNode.Nodes.Add(treeNodeModel);
            }

            treeView0.Nodes.Add(treeNode);
        }

        public void GetNodeData(TreeNode Node, int NodeLevel)
        {
            ModelBox box = new ModelBox();
            ModelPart part = new ModelPart();
            ModelPiece piece = new ModelPiece();

            string[] Path = Node.FullPath.Split(new[] { "\\" }, StringSplitOptions.None);
            Console.WriteLine(Node.FullPath);

            switch (NodeLevel)
            {
                case 4:
                    MCon.models.TryGetValue(Path[1], out piece);
                    piece.Parts.TryGetValue(Path[2], out part);
                    part.Boxes.TryGetValue(Path[3].Split(' ')[1], out box);
                    ReadPieceToNode(NodeLevel, box, part, piece);
                    break;
                case 3:
                    MCon.models.TryGetValue(Path[1], out piece);
                    piece.Parts.TryGetValue(Path[2], out part);
                    ReadPieceToNode(NodeLevel, box, part, piece);
                    break;
                case 2:
                    MCon.models.TryGetValue(Path[1], out piece);
                    ReadPieceToNode(NodeLevel, box, part, piece);
                    break;
            }
        }

        public void ReadPieceToNode(int NodeLevel, ModelBox box, ModelPart part, ModelPiece piece)
        {

            TreeNode TN0 = new TreeNode();
            TN0.ImageIndex = GetIndex("package");
            TN0.SelectedImageIndex = GetIndex("package");
            EntryData.Clear();
            switch (NodeLevel)
            {
                case 2:
                    TN0.Text = "MODEL";
                    AddNode(TN0, "TextureWidth : " + piece.TextureWidth, "integer");
                    AddNode(TN0, "TextureHeight : " + piece.TextureHeight, "integer");
                    EntryData.Add(BitConverter.GetBytes(piece.TextureWidth));
                    EntryData.Add(BitConverter.GetBytes(piece.TextureHeight));
                    break;
                case 3:
                    TN0.Text = "PART";
                    AddNode(TN0, "TranslationX : " + part.TranslationX, "float");
                    AddNode(TN0, "TranslationY : " + part.TranslationY, "float");
                    AddNode(TN0, "TranslationZ : " + part.TranslationZ, "float");
                    AddNode(TN0, "TextureOffsetX : " + part.TextureOffsetX, "float");
                    AddNode(TN0, "TextureOffsetY : " + part.TextureOffsetY, "float");
                    AddNode(TN0, "RotationX : " + part.RotationX, "float");
                    AddNode(TN0, "RotationY : " + part.RotationY, "float");
                    AddNode(TN0, "RotationZ : " + part.RotationZ, "float");
                    EntryData.Add(BitConverter.GetBytes(part.TranslationX));
                    EntryData.Add(BitConverter.GetBytes(part.TranslationY));
                    EntryData.Add(BitConverter.GetBytes(part.TranslationZ));
                    EntryData.Add(BitConverter.GetBytes(part.TextureOffsetX));
                    EntryData.Add(BitConverter.GetBytes(part.TextureOffsetY));
                    EntryData.Add(BitConverter.GetBytes(part.RotationX));
                    EntryData.Add(BitConverter.GetBytes(part.RotationY));
                    EntryData.Add(BitConverter.GetBytes(part.RotationZ));
                    break;
                case 4:
                    TN0.Text = "BOX";
                    AddNode(TN0, "PositionX : " + box.PositionX, "float");
                    AddNode(TN0, "PositionY : " + box.PositionY, "float");
                    AddNode(TN0, "PositionZ : " + box.PositionZ, "float");
                    AddNode(TN0, "Length : " + box.Length, "integer");
                    AddNode(TN0, "Width : " + box.Width, "integer");
                    AddNode(TN0, "Height : " + box.Height, "integer");
                    AddNode(TN0, "UvX : " + box.UvX, "float");
                    AddNode(TN0, "UvY : " + box.UvY, "float");
                    AddNode(TN0, "Scale : " + box.Scale, "float");
                    EntryData.Add(BitConverter.GetBytes(box.PositionX));
                    EntryData.Add(BitConverter.GetBytes(box.PositionY));
                    EntryData.Add(BitConverter.GetBytes(box.PositionZ));
                    EntryData.Add(BitConverter.GetBytes(box.Length));
                    EntryData.Add(BitConverter.GetBytes(box.Height));
                    EntryData.Add(BitConverter.GetBytes(box.Width));
                    EntryData.Add(BitConverter.GetBytes(box.UvX));
                    EntryData.Add(BitConverter.GetBytes(box.UvY));
                    EntryData.Add(BitConverter.GetBytes(box.Scale));
                    break;
            }

            EntryNodeTree.Nodes.Add(TN0);
            TN0.Expand();
        }

        public void AddNode(TreeNode AttachNode, string text, string icon)
        {
            TreeNode TN1 = new TreeNode();
            TN1.Text = text;
            TN1.ImageIndex = GetIndex(icon);
            TN1.SelectedImageIndex = GetIndex(icon);
            AttachNode.Nodes.Add(TN1);

        }

        public static int GetNodeparents(TreeNode tn)
        {
            Console.WriteLine("NodeLevel - " + tn.FullPath.Split(new[] { "\\" }, StringSplitOptions.None).Length);
            return tn.FullPath.Split(new[] { "\\" }, StringSplitOptions.None).Length;
        }

        #endregion

        #region Form Functions

        public Form1()
        {
            Classes.UserInteraction.GenerateDocumentData();
            InitializeComponent();
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (HasFileOpen)
            {
                if (MessageBox.Show("You have an open model!\ndo you want to discard it?", "Warning!", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    OpenModels();
            }
            else
                OpenModels();
            try
            {
                FileNodeTree.SelectedNode = FileNodeTree.Nodes[0];
            }
            catch
            {

            }

        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            EntryNodeTree.Nodes.Clear();
            int g = GetNodeparents(FileNodeTree.SelectedNode);
            if(g == 4) // BOX
            {
                GetNodeData(FileNodeTree.SelectedNode, g);
                ModelStrip.Items[0].Enabled = false;
                ModelStrip.Items[1].Enabled = true;
                ModelStrip.Items[2].Enabled = false;
            }
            if(g == 3) // PART
            {
                GetNodeData(FileNodeTree.SelectedNode, g);
                ModelStrip.Items[0].Enabled = true;
                ModelStrip.Items[1].Enabled = true;
                ModelStrip.Items[2].Enabled = false;
            }
            if(g == 2) // MODEL
            {
                GetNodeData(FileNodeTree.SelectedNode, g);
                ModelStrip.Items[0].Enabled = true;
                ModelStrip.Items[1].Enabled = true;
                ModelStrip.Items[2].Enabled = true;
            }
            if(g == 1) // FILE
            {
                ModelStrip.Items[0].Enabled = true;
                ModelStrip.Items[1].Enabled = false;
                ModelStrip.Items[2].Enabled = false;
            }
        }

        private void EntryNodeTree_AfterSelect(object sender, EventArgs e)
        {

            if (EntryNodeTree.SelectedNode.ImageIndex == 53)
            {
                Forms.ValueEditor ve = new Forms.ValueEditor(EntryNodeTree.SelectedNode, 1, EntryData, EntryNodeTree.SelectedNode.Index);
                ve.ShowDialog();
                //RebuildFileNode();
            }
            else if (EntryNodeTree.SelectedNode.ImageIndex == 55)
            {
                Forms.ValueEditor ve = new Forms.ValueEditor(EntryNodeTree.SelectedNode, 2, EntryData, EntryNodeTree.SelectedNode.Index);
                ve.ShowDialog();
                //RebuildFileNode();
            }


            ModelBox box = new ModelBox();
            ModelPart part = new ModelPart();
            ModelPiece piece = new ModelPiece();

            string[] Path = FileNodeTree.SelectedNode.FullPath.Split(new[] { "\\" }, StringSplitOptions.None);

            switch (EntryNodeTree.Nodes[0].Text)
            {
                case "MODEL":
                    MCon.models.TryGetValue(Path[1], out piece);
                    piece.TextureWidth = BitConverter.ToInt32(EntryData[0], 0);
                    piece.TextureHeight = BitConverter.ToInt32(EntryData[1], 0);
                    break;
                case "PART":
                    MCon.models.TryGetValue(Path[1], out piece);
                    piece.Parts.TryGetValue(Path[2], out part);
                    part.TranslationX = BitConverter.ToInt32(EntryData[0], 0);
                    part.TranslationY = BitConverter.ToInt32(EntryData[1], 0);
                    part.TranslationZ = BitConverter.ToInt32(EntryData[2], 0);
                    part.TextureOffsetX = BitConverter.ToInt32(EntryData[3], 0);
                    part.TextureOffsetY = BitConverter.ToInt32(EntryData[4], 0);
                    part.RotationX = BitConverter.ToInt32(EntryData[5], 0);
                    part.RotationY = BitConverter.ToInt32(EntryData[6], 0);
                    part.RotationZ = BitConverter.ToInt32(EntryData[7], 0);
                    break;
                case "BOX":
                    MCon.models.TryGetValue(Path[1], out piece);
                    piece.Parts.TryGetValue(Path[2], out part);
                    part.Boxes.TryGetValue(Path[3].Split(' ')[1], out box);
                    box.PositionX = BitConverter.ToInt32(EntryData[0], 0);
                    box.PositionY = BitConverter.ToInt32(EntryData[1], 0);
                    box.PositionZ = BitConverter.ToInt32(EntryData[2], 0);
                    box.Length = BitConverter.ToInt32(EntryData[3], 0);
                    box.Height = BitConverter.ToInt32(EntryData[4], 0);
                    box.Width = BitConverter.ToInt32(EntryData[5], 0);
                    box.UvX = BitConverter.ToInt32(EntryData[6], 0);
                    box.UvY = BitConverter.ToInt32(EntryData[7], 0);
                    box.Scale = BitConverter.ToInt32(EntryData[8], 0);
                    break;
            }
        }

        private void viewItemHexToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Forms.HexBox ve = new Forms.HexBox(EntryNodeTree.SelectedNode);
            ve.ShowDialog();
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Forms.About abt = new Forms.About();
            abt.ShowDialog();
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "Models File|*.bin";
            sfd.Title = "Save Models";
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                ModelBuilder.Build(MCon, sfd.FileName);
            }
        }

        private void removeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (FileNodeTree.SelectedNode == FileNodeTree.Nodes[0])
                MessageBox.Show("Cannot remove head node!");
            else
                FileNodeTree.Nodes.Remove(FileNodeTree.SelectedNode);
        }

        private void addToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int g = ModelsBin.GetNodeparents(FileNodeTree.SelectedNode);

            switch (g)
            {
                case (3):
                    List<byte> OutputList = new List<byte>();
                    byte[] Arr1 = new byte[0x24];
                    for (int i = 0; i < Arr1.Length; i++)
                    {
                        Arr1[i] = 0x00;
                    }
                    TreeNode tn0 = new TreeNode("Box" + (FileNodeTree.SelectedNode.Nodes.Count + 1));
                    tn0.Tag = BitConverter.ToString(Arr1);
                    FileNodeTree.SelectedNode.Nodes.Add(tn0);
                    ResetNodeCount(FileNodeTree.SelectedNode);
                    break;
                case (2):
                    Forms.NewEntry ne = new Forms.NewEntry(FileNodeTree.SelectedNode, g);
                    ne.ShowDialog();
                    ResetNodeCount(FileNodeTree.SelectedNode);
                    break;
                case (1):
                    Forms.NewEntry ne1 = new Forms.NewEntry(FileNodeTree.SelectedNode, g);
                    ne1.ShowDialog();
                    ResetNodeCount(FileNodeTree.SelectedNode);
                    break;
                default:
                    break;
            }

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Classes.Network.CheckUpdate();
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (HasFileOpen)
            {
                if (MessageBox.Show("You have an open model!\ndo you want to discard it?", "Warning!", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    FileNodeTree.Nodes.Clear();
                    EntryNodeTree.Nodes.Clear();
                    MCon = ModelParser.Parse(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\ModelStudio\\TemplateModels\\models.bin");
                    GetNodes(FileNodeTree, EntryNodeTree);
                    saveToolStripMenuItem.Enabled = true;
                    HasFileOpen = true;
                }
            }
            else
            {
                FileNodeTree.Nodes.Clear();
                EntryNodeTree.Nodes.Clear();
                MCon = ModelParser.Parse(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\ModelStudio\\TemplateModels\\models.bin");
                GetNodes(FileNodeTree, EntryNodeTree);
                saveToolStripMenuItem.Enabled = true;
                HasFileOpen = true;
            }
            try
            {
                FileNodeTree.SelectedNode = FileNodeTree.Nodes[0];
                FileNodeTree.Nodes[0].Expand();
            }
            catch
            {

            }
        }

        private void convertToCSMToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "CustomSkinModel| *.CSM";
            if(sfd.ShowDialog() == DialogResult.OK)
            {
                Classes.CSM_Actions.ModelToCSM(sfd.FileName, FileNodeTree.SelectedNode);
                //Classes.JSONActions.ModelToJSON(sfd.FileName, FileNodeTree.SelectedNode);
            }
        }

        private void changeLogToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Forms.ChangeLog cl = new Forms.ChangeLog();
            cl.ShowDialog();
        }

        private void tESTINGToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }

        private void sourceCodeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.com/PhoenixARC/Model-Studio");
        }

        private void sendABugReportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string ver = Application.ProductVersion;
            string trace = Environment.StackTrace;
            string link = "mailto:phoenixarc.canarynotifs@gmail.com?subject=Spark%20Model%20Editor%20BUGREPORT&body=Version%3A%20$s%0A%0AStack%20Trace%3A%0A$b";
            System.Diagnostics.Process.Start(link.Replace("$s", ver).Replace("$b", trace));
        }

        #endregion

        private void FileNodeTree_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

            if (HasFileOpen)
            {
                if (MessageBox.Show("You have an open model!\ndo you want to discard it?", "Warning!", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    FileNodeTree.Nodes.Clear();
                    EntryNodeTree.Nodes.Clear();
                    mf.OpenModel(files[0], FileNodeTree, EntryNodeTree);
                    saveToolStripMenuItem.Enabled = true;
                    HasFileOpen = true;
                }
            }
            else
            {
                FileNodeTree.Nodes.Clear();
                EntryNodeTree.Nodes.Clear();
                mf.OpenModel(files[0], FileNodeTree, EntryNodeTree);
                saveToolStripMenuItem.Enabled = true;
                HasFileOpen = true;
            }
            try
            {
                FileNodeTree.SelectedNode = FileNodeTree.Nodes[0];
            }
            catch
            {

            }
        }

        private void FileNodeTree_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.Copy;
        }
    }
}
