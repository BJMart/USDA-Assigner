using System;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Windows.Forms;


namespace USDA_Assigner
{

    public partial class Form1 : Form
    {
        private static string selectedFolderPath = "";
        private static string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        private static string projectPath = Path.Combine(documentsPath, "USDAAssigner");
        private static string usdaPath = "";
        private string CapturePath = "";
        private string DiffusePath = "";
        private string RoughPath = "";
        private string MetalPath = "";
        private string NormalPath = "";
        private string HeightPath = "";
        private string relativePath = "";
        private static int imageCount = 0;
        private string diffusename = "diffuse";
        private string normalname = "normal";
        private string metalname = "metal";
        private string roughname = "rough";
        private string heightname = "height";
        private static string[] Hashes;
        private ToolStripTextBox txtbxDiffuse = new ToolStripTextBox();
        private ToolStripTextBox txtbxNormal = new ToolStripTextBox();
        private ToolStripTextBox txtbxRough = new ToolStripTextBox();
        private ToolStripTextBox txtbxMetal = new ToolStripTextBox();
        private ToolStripTextBox txtbxHeight = new ToolStripTextBox();


        public Form1()
        {
            InitializeComponent();
            AddOpenList();
            AddMapNames();

        }

        private void AddOpenList()
        {
            string[] filenames = Directory.GetFiles(projectPath).Select(Path.GetFileName).ToArray();
            openToolStripMenuItem.DropDownItems.Clear();
            for (int i = 0; i < filenames.Length; i++)
            {
                string filename = filenames[i];

                // Create a ToolStripButton for each file and attach the common Click event handler
                ToolStripButton fileButton = new ToolStripButton(filename);
                fileButton.Click += FileButton_Click;

                openToolStripMenuItem.DropDownItems.Add(fileButton);
            }

            // Common Click event handler for all file buttons
        }

        private void button1_Click(object sender, EventArgs e)
        {
            selectedFolderPath = ShowFolderDialog("Select a folder for the main path:");
            label1.Text = selectedFolderPath;
            string modsFolder = Path.Combine(selectedFolderPath, "mods");
            string[] modsSubfolders = Directory.GetDirectories(modsFolder);
            string ModFolderName = Path.GetFileName(modsSubfolders[0]);

            CapturePath = Path.Combine(selectedFolderPath, "captures", "textures");
            usdaPath = Path.Combine(selectedFolderPath, "mods", ModFolderName);
            TextureHashing();



        }

        private void button2_Click(object sender, EventArgs e)
        {
            DiffusePath = ShowFolderDialog("Select a folder for Diffuse:");
            CalculateAndDisplayRelativePath(DiffusePath);
            label2.Text = relativePath.Replace('\\', '/');
           
        }

        private void button3_Click(object sender, EventArgs e)
        {
            RoughPath = ShowFolderDialog("Select a folder for Rough:");
            CalculateAndDisplayRelativePath(RoughPath);
            label3.Text = relativePath.Replace('\\', '/');
        }

        private void button4_Click(object sender, EventArgs e)
        {
            MetalPath = ShowFolderDialog("Select a folder for Metal:");
            CalculateAndDisplayRelativePath(MetalPath);
            label4.Text = relativePath.Replace('\\', '/');
        }

        private void button5_Click(object sender, EventArgs e)
        {
            NormalPath = ShowFolderDialog("Select a folder for Normal:");
            CalculateAndDisplayRelativePath(NormalPath);
            label5.Text = relativePath.Replace('\\', '/');
        }

        private void button6_Click(object sender, EventArgs e)
        {
            HeightPath = ShowFolderDialog("Select a folder for Height:");
            CalculateAndDisplayRelativePath(HeightPath);
            label6.Text = relativePath.Replace('\\', '/');
        }

        private string ShowFolderDialog(string description)
        {
            using (FolderBrowserDialog folderDialog = new FolderBrowserDialog())
            {
                folderDialog.SelectedPath = usdaPath; // You can set your desired initial path here
                folderDialog.Description = description;

                DialogResult result = folderDialog.ShowDialog();

                if (result == DialogResult.OK)
                {
                    return folderDialog.SelectedPath;
                }

                return "";
            }
        }

        private void CalculateAndDisplayRelativePath(string selectedPath)
        {
            if (!string.IsNullOrEmpty(usdaPath))
            {
                // Calculate the relative path
                relativePath = Path.GetRelativePath(usdaPath, selectedPath) + "\\";
            }
            else
            {
                MessageBox.Show("Please select the main path first.", "Main Path not selected");
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            TextureHashing();
            WriteUSDA();
        }

        private void TextureHashing()
        {
            string folderPath = Path.Combine(selectedFolderPath, "captures", "textures");

            // Get all files in the folder with the .dds extension
            string[] imageFiles = Directory.GetFiles(folderPath, "*.dds", SearchOption.TopDirectoryOnly);

            imageCount = imageFiles.Length;

            // Create an array to store the image file names
            string[] imageNames = new string[imageCount];

            // Fill the array with image file names
            for (int i = 0; i < imageCount; i++)
            {
                imageNames[i] = Path.GetFileName(imageFiles[i]);
            }

            Hashes = new string[imageCount];
            for (int i = 0; i < imageCount; i++)
            {
                Hashes[i] = imageNames[i].Replace(".dds", "");
            }
        }


        private void WriteUSDA()
        {
            string filePath = Path.Combine(usdaPath, "USDAGEN.usda");
            using (StreamWriter sw = new StreamWriter(filePath))
            {
                sw.WriteLine("#usda 1.0");
                sw.WriteLine();
                sw.WriteLine("over \"RootNode\"\r\n{");
                sw.WriteLine("    over \"Looks\"\r\n    {");
                for (int i = 0; i < imageCount; i++)
                {
                    sw.WriteLine("        over \"mat_" + Hashes[i] + "\"\r\n        {\r\n            over \"Shader\"\r\n            {");
                    if (checkBox1.Checked == true)
                    {
                        if (File.Exists(DiffusePath + "\\" + Hashes[i].Replace(".dds", "") + "_" + txtbxDiffuse + ".a.rtex.dds"))
                        {
                          
                            sw.WriteLine("                asset inputs:diffuse_texture = @./" + label2.Text + Hashes[i].Replace(".dds", "") + "_" + txtbxDiffuse +".a.rtex.dds@");
                        }
                    }
                    if (checkBox4.Checked == true)

                    {
                      
                                
                                if (File.Exists(NormalPath + "\\" + Hashes[i].Replace(".dds", "") + "_" + txtbxNormal + "_DX_Normal.n.rtex.dds"))
                                {
                                    sw.WriteLine("                asset inputs:normalmap_texture = @./" + label5.Text + Hashes[i].Replace(".dds", "") + "_" + txtbxNormal + "_DX_Normal.n.rtex.dds@");
                                }
                          
                                if (File.Exists(NormalPath + "\\" + Hashes[i].Replace(".dds", "") + "_" + txtbxNormal + "_OGL_Normal.n.rtex.dds"))
                                {
                                    sw.WriteLine("                asset inputs:normalmap_texture = @./" + label5.Text + Hashes[i].Replace(".dds", "") + "_" + txtbxNormal + "_OGL_Normal.n.rtex.dds@");
                                }
                         
                                if (File.Exists(NormalPath + "\\" + Hashes[i].Replace(".dds", "") + "_" + txtbxNormal + ".n.rtex.dds"))
                                {
                                    sw.WriteLine("                asset inputs:normalmap_texture = @./" + label5.Text + Hashes[i].Replace(".dds", "") + "_" + txtbxNormal + ".n.rtex.dds@");
                                }

                                 if (File.Exists(NormalPath + "\\" + Hashes[i].Replace(".dds", "") + "_" + txtbxNormal + "_OTH_Normal.n.rtex.dds"))
                                {
                                    sw.WriteLine("                asset inputs:normalmap_texture = @./" + label5.Text + Hashes[i].Replace(".dds", "") + "_" + txtbxNormal + "_OTH_Normal.n.rtex.dds@");
                                }
                           
                        
                    }
                    if (checkBox2.Checked == true)
                    {
                        if (File.Exists(RoughPath + "\\" + Hashes[i].Replace(".dds", "") + "_" + txtbxRough + ".r.rtex.dds"))
                        {
                            sw.WriteLine("                asset inputs:reflectionroughness_texture = @./" + label3.Text + Hashes[i].Replace(".dds", "") + "_" + txtbxRough + ".r.rtex.dds@");
                        }
                    }
                    if (checkBox3.Checked == true)
                    {
                        if (File.Exists(MetalPath + "\\" + Hashes[i].Replace(".dds", "") + "_" + txtbxMetal + ".m.rtex.dds"))
                        {
                            sw.WriteLine("                asset inputs:metallic_texture = @./" + label4.Text + Hashes[i].Replace(".dds", "") + "_" + txtbxMetal + ".m.rtex.dds@");
                        }
                    }

                    if (checkBox5.Checked == true)
                    {
                        if (File.Exists(HeightPath + "\\" + Hashes[i].Replace(".dds", "") + "_" + txtbxHeight + ".h.rtex.dds"))
                        {
                            sw.WriteLine("                asset inputs:height_texture = @./" + label6.Text + Hashes[i].Replace(".dds", "") + "_" + txtbxHeight + ".h.rtex.dds@ (\r\n                    colorSpace = \"raw\"\r\n                )\r\n                float inputs:displace_in = 0.05");
                        }
                    }

                    sw.WriteLine("            }\r\n        }\r\n");
                }

                sw.WriteLine("    }\r\n}");
                sw.WriteLine("#Generated with usda Generator from Cheev");

            }
        }

        private void toolStripLabel1_Click(object sender, EventArgs e)
        {

        }

        private void toolStripTextBox1_Click(object sender, EventArgs e)
        {

        }



        private void toolStripButton1_Click(object sender, EventArgs e)
        {



        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtName.Text))
            {
                string projectsPath = Path.Combine(projectPath, txtName.Text + ".uap");
                string directoryPath = Path.GetDirectoryName(projectsPath);
                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }
                using (StreamWriter sw = new StreamWriter(projectsPath))
                {
                    sw.WriteLine(label1.Text);
                    sw.WriteLine(DiffusePath);
                    sw.WriteLine(RoughPath);
                    sw.WriteLine(MetalPath);
                    sw.WriteLine(NormalPath);
                    sw.WriteLine(HeightPath);
                    sw.WriteLine(txtbxDiffuse.Text);
                    sw.WriteLine(txtbxNormal.Text);
                    sw.WriteLine(txtbxRough.Text);
                    sw.WriteLine(txtbxMetal.Text);
                    sw.WriteLine(txtbxHeight.Text);
                }

            }
            AddOpenList();
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {


        }

        private void testToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void toolStripDropDownButton2_Click(object sender, EventArgs e)
        {

        }

        private void AddMapNames()
        {
            txtbxDiffuse.Text = diffusename;
            txtbxNormal.Text = normalname;
            txtbxRough.Text = roughname;
            txtbxMetal.Text = metalname;
            txtbxHeight.Text = heightname;
            diffuseToolStripMenuItem.DropDownItems.Add(txtbxDiffuse);
            normalToolStripMenuItem.DropDownItems.Add(txtbxNormal);
            roughnessToolStripMenuItem.DropDownItems.Add(txtbxRough);
            metalnessToolStripMenuItem.DropDownItems.Add(txtbxMetal);
            heightToolStripMenuItem.DropDownItems.Add(txtbxHeight);
        }

        private void FileButton_Click(object sender, EventArgs e)
        {
            // Get the sender (button) and extract the file name from the button's text
            ToolStripButton button = (ToolStripButton)sender;
            string fileName = button.Text;

            txtName.Text = fileName.Replace(".uap", "");
            StreamReader sr = new StreamReader(projectPath + "\\" + fileName);
            string line = "temp";
            for (int i = 0; i < 11; i++)
            {
                line = sr.ReadLine();
                if (line != null)
                {


                    if (i == 0)
                    {
                        selectedFolderPath = line;
                        label1.Text = selectedFolderPath;
                        string modsFolder = Path.Combine(selectedFolderPath, "mods");
                        string[] modsSubfolders = Directory.GetDirectories(modsFolder);
                        string ModFolderName = Path.GetFileName(modsSubfolders[0]);
                        CapturePath = Path.Combine(selectedFolderPath, "captures", "textures");
                        usdaPath = Path.Combine(selectedFolderPath, "mods", ModFolderName);
                    }
                    if (i == 1)
                    {
                        DiffusePath = line;
                        CalculateAndDisplayRelativePath(DiffusePath);
                        label2.Text = relativePath.Replace('\\', '/');
                    }
                    if (i == 2)
                    {
                        RoughPath = line;
                        CalculateAndDisplayRelativePath(RoughPath);
                        label3.Text = relativePath.Replace('\\', '/');
                    }
                    if (i == 3)
                    {
                        MetalPath = line;
                        CalculateAndDisplayRelativePath(MetalPath);
                        label4.Text = relativePath.Replace('\\', '/');
                    }
                    if (i == 4)
                    {
                        NormalPath = line;
                        CalculateAndDisplayRelativePath(NormalPath);
                        label5.Text = relativePath.Replace('\\', '/');
                    }
                    if (i == 5)
                    {
                        HeightPath = line;
                        CalculateAndDisplayRelativePath(HeightPath);
                        label6.Text = relativePath.Replace('\\', '/');
                    }
                    if (i == 6)
                    {
                        diffusename = line;
                    }
                    if (i == 7)
                    {
                        normalname = line;
                    }
                    if (i == 8)
                    {
                        roughname = line;
                    }
                    if (i == 9)
                    {
                        metalname = line;
                    }
                    if (i == 10)
                    {
                        heightname = line;
                    }
                }

                AddMapNames();

            }
            sr.Close();
        }

        private void metalToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void toolStripSplitButton1_ButtonClick(object sender, EventArgs e)
        {

        }

       
    }
}