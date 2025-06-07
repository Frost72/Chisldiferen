using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Chisldiferen
{
    /// <summary>
    /// Логика взаимодействия для Centre.xaml
    /// </summary>
    public partial class Centre : Window
    {
        public Centre()
        {
            InitializeComponent();
        }

        private void CalculateButton_Click(object sender, RoutedEventArgs e)
        {
            var lines = InputTextBox.Text.Split('\n', StringSplitOptions.RemoveEmptyEntries);
            var points = new List<MyPointData>();

            foreach (var line in lines)
            {
                var parts = line.Trim().Split(' ');
                if (parts.Length >= 2 && double.TryParse(parts[0], out double x) && double.TryParse(parts[1], out double y))
                {
                    points.Add(new MyPointData { X = x, Y = y });
                }
            }

            if (points.Count < 3)
            {
                MessageBox.Show("Введите минимум 3 точки.");
                return;
            }

            for (int i = 0; i < points.Count; i++)
            {
                if (i > 0 && i < points.Count - 1)
                {
                    double dx = points[i + 1].X - points[i - 1].X;
                    if (dx != 0)
                        points[i].Derivative = ((points[i + 1].Y - points[i - 1].Y) / dx).ToString("F4");
                }
            }

            ResultDataGrid.ItemsSource = points;
        }
    }
}
