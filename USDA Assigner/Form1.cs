using System;
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
        private static string[] Hashes;

        public Form1()
        {
            InitializeComponent();
            AddOpenList();
        }

        private void AddOpenList()
        {
            string[] filenames = Directory.GetFiles(projectPath).Select(Path.GetFileName).ToArray();
            for (int i = 0; i < filenames.Length; i++)
            {
                openToolStripMenuItem.DropDownItems.Add(filenames[i]);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            selectedFolderPath = ShowFolderDialog("Select a folder for the main path:");
            label1.Text = selectedFolderPath;
            CapturePath = Path.Combine(selectedFolderPath, "captures", "textures");
            usdaPath = Path.Combine(selectedFolderPath, "mods", "gameReadyAssets");
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
            string[] imageExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".dds" }; // Add more extensions if needed

            // Get all files in the folder with the specified extensions
            string[] imageFiles = Directory.GetFiles(folderPath, "*.*", SearchOption.TopDirectoryOnly)
                .Where(file => imageExtensions.Contains(Path.GetExtension(file), StringComparer.OrdinalIgnoreCase))
                .ToArray();

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
                Hashes[i] = imageNames[i];
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
                    if (checkBox1.Checked)
                    {
                        sw.WriteLine("                asset inputs:diffuse_texture = @./" + label2.Text + Hashes[i] + "_diffuse.dds@");
                    }
                    if (checkBox2.Checked)
                    {
                        sw.WriteLine("                asset inputs:reflectionroughness_texture = @./" + label3.Text + Hashes[i] + "_roughness.dds@");
                    }
                    if (checkBox3.Checked)
                    {
                        sw.WriteLine("                asset inputs:metallic_texture = @./" + label4.Text + Hashes[i] + "_metallic.dds@");
                    }
                    if (checkBox4.Checked)
                    {
                        sw.WriteLine("                asset inputs:normalmap_texture = @./" + label5.Text + Hashes[i] + "_normal.dds@");
                    }
                    if (checkBox5.Checked)
                    {
                        sw.WriteLine("                asset inputs:height_texture = @./" + label6.Text + Hashes[i] + "_height.dds@ (\r\n                    colorSpace = \"raw\"\r\n                )\r\n                float inputs:displace_in = 0.05");
                    }
                    sw.WriteLine("            }\r\n        }");
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
                    sw.WriteLine(label2.Text);
                    sw.WriteLine(label3.Text);
                    sw.WriteLine(label4.Text);
                    sw.WriteLine(label5.Text);
                    sw.WriteLine(label6.Text);
                }

            }
            AddOpenList();
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            

        }
       
        
    }
}
