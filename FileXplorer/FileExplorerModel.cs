using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace FileXplorer {

    public class File {
        public File(string n, long s, DateTime c) {
            Name = n;
            Size = s;
            CreationTime = c;
        }
        public string Name;
        public long Size;
        public DateTime CreationTime;

    }
    public class DirectoryExpandedEventArgs {
        public DirectoryExpandedEventArgs(string xpDir, string sdPath, string sdName) {
            ExpandedDir = xpDir;
            SubDirPath = sdPath;
            SubDirName = sdName;
        }
        public string ExpandedDir;
        public string SubDirPath;
        public string SubDirName;
    }

    public class FilesListedEventArgs {
        public FilesListedEventArgs(List<File> f) {
            Files = f;
        }
        public List<File> Files;
    }

    public class FileExplorerModel {
        private Dictionary<string, DirectoryInfo> listedDirectories;
        private List<File> listedFiles;
        public event EventHandler<DirectoryExpandedEventArgs> DirectoryExpanded;
        public event EventHandler<FilesListedEventArgs> FilesListed;
        public FileExplorerModel() {
            listedDirectories = new Dictionary<string, DirectoryInfo>();
            listedFiles = new List<File>();
        }
        public void ListDrives() {
            DriveInfo[] drives = DriveInfo.GetDrives();
            foreach (DriveInfo drive in drives) {
                string rootPath = drive.RootDirectory.FullName;
                listedDirectories.Add(rootPath, drive.RootDirectory);
                OnDirectoryExpanded("/", rootPath, drive.Name);
            }
        }

        public void ExpandDir(string path) {
            foreach(DirectoryInfo dirInf in listedDirectories[path].GetDirectories()) {
                if(!listedDirectories.ContainsKey(dirInf.FullName)) {
                    listedDirectories.Add(dirInf.FullName, dirInf);
                }
                OnDirectoryExpanded(path, dirInf.FullName, dirInf.Name);
            }
        }
        public void ListFiles(string path) {
            FileInfo[] files = listedDirectories[path].GetFiles();
            listedFiles.Clear();
            foreach(FileInfo file in files) {
                listedFiles.Add(new File(file.Name, file.Length, file.CreationTime));
            }
            OnFilesListed(listedFiles);
        }

        public void Execute(string s) {

        }

        private void OnDirectoryExpanded(string expandedDir, string subDirPath, string subDirName) {
            if (DirectoryExpanded != null) {
                DirectoryExpanded(this, new DirectoryExpandedEventArgs(expandedDir, subDirPath, subDirName));
            }
        }

        private void OnFilesListed(List<File> f) {
            if(FilesListed != null) {
                FilesListed(this, new FilesListedEventArgs(f));
            }
        }
    }
}
