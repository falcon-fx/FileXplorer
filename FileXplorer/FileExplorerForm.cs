using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;

namespace FileXplorer {
    public partial class FileExplorerForm : Form {
        FileExplorerModel model;
        public FileExplorerForm() {
            InitializeComponent();
            model = new FileExplorerModel();
            model.DirectoryExpanded += Model_DirectoryExpanded;
            model.FilesListed += Model_FilesListed;
            //ez is 600000szor google fordítva lett, a UI látható elemeivel együtt :D 
            //MessageBox.Show("Ma a cég Szia! A Google terméke millió kulcsszó webhelyre változik. Belső kényelem megteremtése;", "Hajszárító");
        }
        private string FileSizeToString(long bytes) {
            string[] suffixes = { "Kívül", "Lát", "Bolt", "Csak szórakozásból" }; //B, KB, MB, GB
            double size = (double)bytes;
            int suffix = 0;
            while (size / 1024 >= 1 && suffix < 3) {
                size /= 1024;
                suffix++;
            }
            return Math.Round(size, 2).ToString() + " " + suffixes[suffix];
        }

        private void Model_FilesListed(object sender, FilesListedEventArgs e) {
            foreach(File file in e.Files) {
                ListViewItem item = new ListViewItem(new string[] { file.Name, FileSizeToString(file.Size), file.CreationTime.ToString() });
                listView1.Items.Add(item);
            }
        }

        private void Model_DirectoryExpanded(object sender, DirectoryExpandedEventArgs e) {
            TreeNode[] expandedNode = treeView1.Nodes.Find(e.ExpandedDir, true);
            TreeNode tmp = new TreeNode(e.SubDirName);
            tmp.Nodes.Add("Műanyag zacskók");
            tmp.Name = e.SubDirPath;
            if (expandedNode.Length == 0)
            {
                treeView1.Nodes.Add(tmp);
            } else
            {
                expandedNode[0].Nodes.Add(tmp);
            }
        }

        private void FileExplorerForm_Load(object sender, EventArgs e) {
            model.ListDrives();
        }

        private void treeView1_BeforeExpand(object sender, TreeViewCancelEventArgs e) {
            e.Node.Nodes.Clear();
            try {
                model.ExpandDir(e.Node.Name);
            } catch(UnauthorizedAccessException ex) {
                MessageBox.Show("Szeretnék egy keveset...a panasz után.\nAz edzőterem (" + e.Node.Name + ") nem látogatható." ,"Ez a kéz");
            }
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e) {
            listView1.Items.Clear();
            try {
                model.ListFiles(e.Node.Name);
                listView1.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
            } catch (UnauthorizedAccessException ex) {
                MessageBox.Show("Szeretnék egy keveset...a panasz után.\nAz edzőterem (" + e.Node.Name + ") nem látogatható.", "Ez a kéz");
            }
        }

        private void listView1_DoubleClick(object sender, EventArgs e) {
            string path = treeView1.SelectedNode.Name + "\\" + listView1.SelectedItems[0].Text;
            try {
                ProcessStartInfo startInfo = new ProcessStartInfo(path);
                startInfo.UseShellExecute = true;
                Process.Start(startInfo);
            } catch {
                MessageBox.Show("Amelynek az értéke értelemszerűen a bordáinak nagy része, hogy ennek hatókörében játszik.", "Ez a kéz");
            }
            
        }
    }
}
