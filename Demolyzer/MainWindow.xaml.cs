using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using System.Net;
using System.Windows.Media.Media3D;
using System.Collections.ObjectModel;
using Demolyzer.ViewModel;
using System.Threading.Tasks;
using System.Threading;
using Demolyzer.Model;
using System.Windows.Forms;
using System.Diagnostics;

namespace Demolyzer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            this.WindowState = System.Windows.WindowState.Maximized;

            InitializeComponent();

            //for (int x = 1; x <= 5000; x += 50)
            //{
            //    string cmd = "cmd dl ";
            //    for (int i = x; i <= x + 50; ++i)
            //    {
            //        cmd += String.Format("{0} ", i);
            //    }
            //    cmd += "\r\n";
            //    File.AppendAllText("cmddls.txt", cmd);
            //}
            //int asdf = 0;

            this.Loaded += new RoutedEventHandler(MainWindow_Loaded);
            this.PART_DemoTree.SelectedItemChanged += PART_DemoTree_SelectedItemChanged;
            this.PART_DemoList.SelectionChanged += PART_DemoList_SelectionChanged;
            this.PART_DemoListToday.SelectionChanged += PART_DemoListToday_SelectionChanged;
            this.PART_ButtonChangeDirectory.Click += PART_ButtonChangeDirectory_Click;

            this.RadioName.Click += RadioName_Click;
            this.RadioDate.Click += RadioDate_Click;

            this.RadioDate.IsChecked = true;

            //this.ButtonRefreshToday.Click += new RoutedEventHandler(ButtonRefreshToday_Click);
        }

        //void ButtonRefreshToday_Click(object sender, RoutedEventArgs e)
        //{
        //    this.AppShell.RefreshToday();
        //}

        void RadioDate_Click(object sender, RoutedEventArgs e)
        {
            this.AppShell.SortByDate();
        }

        void RadioName_Click(object sender, RoutedEventArgs e)
        {
            this.AppShell.SortByName();
        }

        void PART_ButtonChangeDirectory_Click(object sender, RoutedEventArgs e)
        {
            this.AppShell.ChangeDirectory();
        }

        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            this.AppShell.Initialize();
        }

        void PART_DemoListToday_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            MvdListViewNodeViewModel mvdFile = this.PART_DemoListToday.SelectedItem as MvdListViewNodeViewModel;
            if (mvdFile == null)
            {
                return;
            }
            this.AppShell.AnalyzeMvd(mvdFile.FileName);
        }

        void PART_DemoList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            MvdListViewNodeViewModel mvdFile = this.PART_DemoList.SelectedItem as MvdListViewNodeViewModel;
            if (mvdFile == null)
            {
                return;
            }
            this.AppShell.AnalyzeMvd(mvdFile.FileName);
        }

        void PART_DemoTree_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            MvdFileTreeNodeViewModel mvdFile = this.PART_DemoTree.SelectedItem as MvdFileTreeNodeViewModel;
            if (mvdFile == null)
            {
                return;
            }
            this.AppShell.AnalyzeMvd(mvdFile.FullName);
        }

        private ApplicationShellViewModel AppShell
        {
            get
            {
                ObjectDataProvider data = Resources["AppShell"] as ObjectDataProvider;
                return data.Data as ApplicationShellViewModel;
            }
        }
    }
}
