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

namespace QuantumBitDesigns.Charting
{
    /// <summary>
    /// Interaction logic for ChartView.xaml
    /// </summary>
    public partial class ChartView : UserControl
    {
        public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register("ViewModel", typeof(ChartViewModel), typeof(ChartView), new UIPropertyMetadata(null));

        public ChartView()
        {
            InitializeComponent();

            this.PreviewMouseMove += new MouseEventHandler(ChartView_PreviewMouseMove);
        }

        void ChartView_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            Point p = e.GetPosition(this);
            this.ViewModel.UpdateSelectedPosition(p.X);
        }

        public ChartViewModel ViewModel
        {
            get { return (ChartViewModel)GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }
    }
}
