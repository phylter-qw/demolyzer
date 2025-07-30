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

namespace Demolyzer.View
{
    /// <summary>
    /// Interaction logic for MatchResultsView.xaml
    /// </summary>
    public partial class MatchResultsView : UserControl
    {
        public MatchResultsView()
        {
            InitializeComponent();
        }

        private void EOGDataGrid_Initialized(object sender, EventArgs e)
        {
            var EOGColumns = Properties.Settings.Default.EOGColumns;
            if (EOGColumns != null)
            {
                for (int i = 0; i < EOGColumns.Count; i++)
                {
                    int index = Convert.ToInt32(EOGColumns[i]);
                    EOGDataGrid.Columns[i].DisplayIndex = index;
                }
            }
        }

        private void EOGDataGrid_ColumnReordered(object sender, DataGridColumnEventArgs e)
        {
            var EOGColumns = Properties.Settings.Default.EOGColumns;
            if (EOGColumns == null)
            {
                EOGColumns = Properties.Settings.Default.EOGColumns = new System.Collections.Specialized.StringCollection();
            }
            EOGColumns.Clear();
            foreach (DataGridColumn col in EOGDataGrid.Columns)
            {
                EOGColumns.Add(col.DisplayIndex.ToString());
            }
            Properties.Settings.Default.Save();
        }
    }
}
