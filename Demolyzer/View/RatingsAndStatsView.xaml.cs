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
using Demolyzer.ViewModel;

namespace Demolyzer.View
{
    /// <summary>
    /// Interaction logic for RatingsAndStatsView.xaml
    /// </summary>
    public partial class RatingsAndStatsView : UserControl
    {
        public RatingsAndStatsView()
        {
            InitializeComponent();

            this.PART_ButtonExport.Click += new RoutedEventHandler(PART_ButtonExport_Click);
        }

        void PART_ButtonExport_Click(object sender, RoutedEventArgs e)
        {
            string date = String.Format("{0}{1:D2}{2:D2}", DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day - 1);
            string matchType = this.RatingsAndStatsViewModel.MatchType;
            string totals = String.Empty;
            if (this.RatingsAndStatsViewModel.IsTotals == true)
            {
                totals = "Total";
            }

            Save(ToImage(this.PART_ListViewRatingResults), String.Format("{0}_{1}_ratings{2}.png", date, matchType, totals));
            Save(ToImage(this.PART_ListViewPointsResults), String.Format("{0}_{1}_points{2}.png", date, matchType, totals));
        }

        private RatingsAndStatsViewModel RatingsAndStatsViewModel
        {
            get
            {
                return (RatingsAndStatsViewModel)this.DataContext;
            }
        }

        public static void Save(BitmapSource bs, string fileName)
        {
            PngBitmapEncoder encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(bs));

            FileStream fs = File.Open(fileName, FileMode.Create);
            encoder.Save(fs);
            fs.Close();
        }

        public static BitmapSource ToImage(FrameworkElement obj)
        {
            // Get the size of canvas
            Size size = new Size(5000, 4000);

            // force control to Update
            obj.Measure(size);
            obj.Arrange(new Rect(size));

            RenderTargetBitmap bmp = new RenderTargetBitmap((int)obj.DesiredSize.Width + 30, (int)obj.DesiredSize.Height + 10, 96, 96, PixelFormats.Pbgra32);

            bmp.Render(obj);

            return bmp;
        }
    }
}
