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
using Demolyzer.ViewModel;

namespace Demolyzer.View
{
    /// <summary>
    /// Interaction logic for MatchProgressIndicator.xaml
    /// </summary>
    public partial class MatchProgressIndicator : UserControl
    {
        public MatchProgressIndicator()
        {
            InitializeComponent();

            this.SizeChanged += MatchProgressIndicator_SizeChanged;
        }

        void MatchProgressIndicator_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            this.PART_Line.Y2 = this.ActualHeight;
            if (this.ViewModel != null)
            {
                this.ViewModel.MatchProgressIndicatorWidth = this.ActualWidth;
            }
        }

        private MatchResultsViewModel ViewModel
        {
            get
            {
                return this.DataContext as MatchResultsViewModel;
            }
        }
    }
}
