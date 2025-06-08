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
    /// Логика взаимодействия для Cubic.xaml
    /// </summary>
    public partial class Cubic : Window
    {
        private List<MyPointData> points = new List<MyPointData>();
        public Cubic()
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

            // Проверяем равномерность сетки
            bool isUniform = true;
            double h = points[1].X - points[0].X;

            for (int i = 1; i < points.Count - 1; i++)
            {
                double currentH = points[i + 1].X - points[i].X;
                if (Math.Abs(currentH - h) > 1e-8)
                {
                    isUniform = false;
                    break;
                }
            }

            if (isUniform)
            {
                txtStep.Text = $"Шаг h = {h:F4}";
            }
            else
            {
                txtStep.Text = "Сетка неравномерная";
            }
        }

        // Расчёт производной по кубической интерполяции
        private void CalculateDerivative_Click(object sender, RoutedEventArgs e)
        {
            if (points.Count < 4)
            {
                MessageBox.Show("Сначала добавьте минимум 4 точки.");
                return;
            }

            if (!double.TryParse(txtDerivativeX.Text, out double xTarget))
            {
                MessageBox.Show("Введите корректное значение x для расчёта производной.");
                return;
            }

            // Ближайшие 4 точки
            var sortedPoints = points.OrderBy(p => Math.Abs(p.X - xTarget)).Take(4).OrderBy(p => p.X).ToList();

            if (sortedPoints.Count < 4)
            {
                MessageBox.Show("Не удалось найти 4 ближайшие точки.");
                return;
            }

            // Проверяем равномерность
            double h = sortedPoints[1].X - sortedPoints[0].X;
            bool isUniform = true;

            for (int i = 1; i < sortedPoints.Count - 1; i++)
            {
                double currentH = sortedPoints[i + 1].X - sortedPoints[i].X;
                if (Math.Abs(currentH - h) > 1e-8)
                {
                    isUniform = false;
                    break;
                }
            }

            // Получаем значения
            double x0 = sortedPoints[0].X;
            double y0 = sortedPoints[0].Y;
            double y1 = sortedPoints[1].Y;
            double y2 = sortedPoints[2].Y;
            double y3 = sortedPoints[3].Y;

            double derivative = 0;

            if (isUniform)
            {
                // Формула кубической интерполяции
                double delta1 = y1 - y0;
                double delta2 = (y2 - y1) - (y1 - y0);
                double delta3 = y3 - 3 * y2 + 3 * y1 - y0;
                double q = (xTarget - x0) / h;

                derivative = (1.0 / h) * (
                    delta1 +
                    ((2 * q - 1) / 2.0) * delta2 +
                    ((3 * q * q - 6 * q + 2) / 6.0) * delta3
                );

                txtResult.Text = $"Производная f'({xTarget:F2}) ≈ {derivative:F4} (равномерная сетка)";
            }
            else
            {
                // Приближение центральной разностью
                double dx1 = sortedPoints[1].X - sortedPoints[0].X;
                double dx2 = sortedPoints[2].X - sortedPoints[1].X;
                double dy1 = sortedPoints[1].Y - sortedPoints[0].Y;
                double dy2 = sortedPoints[2].Y - sortedPoints[1].Y;

                double centralDiff = (dy2 / dx2 - dy1 / dx1) / ((dx1 + dx2) / 2);

                txtResult.Text = $"Приближённая производная: f'({xTarget:F2}) ≈ {centralDiff:F4} (неравномерная сетка)";
            }
        }
    }
}

