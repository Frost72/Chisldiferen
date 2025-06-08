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
    /// Логика взаимодействия для Mnk.xaml
    /// </summary>
    public partial class Mnk : Window
    {
        private List<MyPointData> points = new List<MyPointData>();
        public Mnk()
        {
            InitializeComponent();
        }
        // Добавление точек из текстового поля
        private void AddPoints_Click(object sender, RoutedEventArgs e)
        {
            var lines = txtInputPoints.Text.Split('\n', StringSplitOptions.RemoveEmptyEntries);
            points.Clear();
            lstPoints.Items.Clear();

            foreach (var line in lines)
            {
                var parts = line.Trim().Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);

                if (parts.Length < 2)
                {
                    MessageBox.Show($"Неверный формат строки: \"{line}\". Требуется пара чисел x y.");
                    return;
                }

                if (!double.TryParse(parts[0], out double x))
                {
                    MessageBox.Show($"Неверное значение x: \"{parts[0]}\".");
                    return;
                }

                if (!double.TryParse(parts[1], out double y))
                {
                    MessageBox.Show($"Неверное значение y: \"{parts[1]}\".");
                    return;
                }

                points.Add(new MyPointData { X = x, Y = y });
            }

            if (points.Count < 4)
            {
                MessageBox.Show("Введите минимум 4 точки.");
                return;
            }

            // Сортируем по X
            points = points.OrderBy(p => p.X).ToList();

            // Отображаем точки
            for (int i = 0; i < points.Count; i++)
            {
                var p = points[i];
                lstPoints.Items.Add($"x{i} = {p.X:F2}, y{i} = {p.Y:F2}");
            }

            txtStep.Text = "Точки добавлены";
        }

        // Расчёт производной методом неопределённых коэффициентов
        private void CalculateDerivative_Click(object sender, RoutedEventArgs e)
        {
            if (points.Count < 4)
            {
                MessageBox.Show("Для этой формулы требуется минимум 4 точки.");
                return;
            }

            if (!double.TryParse(txtDerivativeX.Text, out double xTarget))
            {
                MessageBox.Show("Введите корректное значение x для расчёта производной.");
                return;
            }

            // Находим ближайшую точку
            int k = points.FindIndex(p => p.X >= xTarget);
            if (k == -1 || k < 1 || k > points.Count - 2)
                k = 1; // по умолчанию для второй точки

            // Берём 4 соседние точки: k-1, k, k+1, k+2
            var p0 = points[k - 1];
            var p1 = points[k];
            var p2 = points[k + 1];
            var p3 = points[k + 2];

            double h = p1.X - p0.X;

            // Проверяем равномерность сетки
            if (Math.Abs(p2.X - p1.X - h) > 1e-8 ||
                Math.Abs(p3.X - p2.X - h) > 1e-8)
            {
                MessageBox.Show("Формула требует равномерной сетки.");
                return;
            }

            // Формула (19): y1’ = 1/(6h)(–2y0 –3y1 +6y2 –y3)
            double y0 = p0.Y;
            double y1 = p1.Y;
            double y2 = p2.Y;
            double y3 = p3.Y;

            double derivative = (-2 * y0 - 3 * y1 + 6 * y2 - y3) / (6 * h);

            txtResult.Text = $"Производная f'({p1.X:F2}) ≈ {derivative:F4}\n(по формуле (19))";
        }
    }
}
