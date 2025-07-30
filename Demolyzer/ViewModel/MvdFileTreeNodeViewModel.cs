using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using QuantumBitDesigns.Mvvm;
using System.IO;
using System.Collections.ObjectModel;

namespace Demolyzer.ViewModel
{
    public class MvdListViewNodeViewModel : ViewModelBase
    {
        public string Name { get; set; }
        public string FileName { get; set; }
        public DateTime Date { get; set; }
        public MvdListViewNodeViewModel(string name, string fileName, DateTime date)
        {
            this.Name = name;
            this.FileName = fileName;
            this.Date = date;
        }
    }

    public abstract class TreeNodeViewModel : ViewModelBase
    {
        public TreeNodeViewModel()
        {
            this.Children = new ObservableCollection<TreeNodeViewModel>();
        }

        public abstract string Name { get; }
        public ObservableCollection<TreeNodeViewModel> Children { get; private set; }
    }

    public class MvdFileTreeNodeViewModel : TreeNodeViewModel
    {
        private FileInfo _fileInfo;

        public MvdFileTreeNodeViewModel(FileInfo fileInfo)
        {
            this._fileInfo = fileInfo;
        }

        public override string Name
        {
            get
            {
                return this._fileInfo.Name;
            }
        }

        public string FullName
        {
            get
            {
                return this._fileInfo.FullName;
            }
        }
    }

    public class MvdDirectoryTreeNodeViewModel : TreeNodeViewModel
    {
        private DirectoryInfo _directoryInfo;

        public MvdDirectoryTreeNodeViewModel(DirectoryInfo directoryInfo, bool sortByDate)
        {
            this._directoryInfo = directoryInfo;

            DirectoryInfo[] subDirectories = directoryInfo.GetDirectories();
            foreach (DirectoryInfo subDirectory in subDirectories)
            {
                MvdDirectoryTreeNodeViewModel directoryViewModel = new MvdDirectoryTreeNodeViewModel(subDirectory, sortByDate);

                //only add directories that contain children (directory with mvds, or subdirectories with mvds)
                if (directoryViewModel.Children.Count > 0)
                {
                    this.Children.Add(directoryViewModel);
                }
            }
            FileInfo[] mvds = directoryInfo.GetFiles("*.mvd", SearchOption.TopDirectoryOnly);
            this.MvdCount = mvds.Length;

            if (sortByDate == true)
            {
                Array.Sort(mvds, (a, b) => b.CreationTime.CompareTo(a.CreationTime));
            }
            
            foreach (FileInfo mvd in mvds)
            {
                this.Children.Add(new MvdFileTreeNodeViewModel(mvd));
            }
        }

        public List<MvdFileTreeNodeViewModel> GetFiles()
        {
            return GetFiles(true);
        }

        public List<MvdFileTreeNodeViewModel> GetFiles(bool includeSubdirectories)
        {
            List<MvdFileTreeNodeViewModel> files = new List<MvdFileTreeNodeViewModel>();
            foreach (var child in this.Children)
            {
                if (includeSubdirectories == true)
                {
                    if (child is MvdDirectoryTreeNodeViewModel)
                    {
                        files.AddRange(((MvdDirectoryTreeNodeViewModel)child).GetFiles());
                    }
                }
                if (child is MvdFileTreeNodeViewModel)
                {
                    files.Add((MvdFileTreeNodeViewModel)child);
                }
            }
            return files;
        }

        public int MvdCount { get; private set; }

        public override string Name
        {
            get
            {
                return this._directoryInfo.Name;
            }
        }

        public string FullName
        {
            get
            {
                return this._directoryInfo.FullName;
            }
        }
    }
}
