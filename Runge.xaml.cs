using System;
using System.Collections.Generic;
using System.Globalization;
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
    /// Логика взаимодействия для Runge.xaml
    /// </summary>
    public partial class Runge : Window
    {
        private List<MyPointData> points = new List<MyPointData>();
        private double xTarget;
        public Runge()
        {
            InitializeComponent();
        }
        private void OnNumericTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is TextBox textBox)
            {
                textBox.Text = textBox.Text.Replace(',', '.');
                textBox.CaretIndex = textBox.Text.Length;
            }
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
                    MessageBox.Show($"Неверный формат строки: \"{line}\". Введите точки в формате x y");
                    return;
                }

                // Парсинг x
                if (!double.TryParse(parts[0].Replace(',', '.'), NumberStyles.Any,
                    CultureInfo.InvariantCulture, out double x))
                {
                    MessageBox.Show($"Ошибка в x: \"{parts[0]}\" — неверное число.");
                    return;
                }

                // Парсинг y
                if (!double.TryParse(parts[1].Replace(',', '.'), NumberStyles.Any,
                    CultureInfo.InvariantCulture, out double y))
                {
                    MessageBox.Show($"Ошибка в y: \"{parts[1]}\" — неверное число.");
                    return;
                }

                points.Add(new MyPointData { X = x, Y = y });
            }

            if (points.Count < 2)
            {
                MessageBox.Show("Введите минимум 2 точки.");
                return;
            }

            // Сортируем точки по X
            points = points.OrderBy(p => p.X).ToList();

            // Отображаем точки
            for (int i = 0; i < points.Count; i++)
            {
                lstPoints.Items.Add($"x{i} = {points[i].X:F3}, y{i} = {points[i].Y:F3}");
            }

            txtStep.Text = $"Добавлено точек: {points.Count}";
        }
        // Расчёт производной методом Рунге
        private void CalculateDerivative_Click(object sender, RoutedEventArgs e)
        {
            if (points.Count < 2)
            {
                MessageBox.Show("Введите минимум 2 точки.");
                return;
            }

            if (!double.TryParse(txtXTarget.Text.Replace(',', '.'), NumberStyles.Any,
                CultureInfo.InvariantCulture, out xTarget))
            {
                MessageBox.Show("Введите корректное значение x.");
                return;
            }

            if (!double.TryParse(txtH.Text.Replace(',', '.'), NumberStyles.Any,
                CultureInfo.InvariantCulture, out double h) || h <= 0)
            {
                MessageBox.Show("Введите положительное значение шага h.");
                return;
            }

            if (!int.TryParse(txtK.Text, out int kInt) || kInt < 1)
            {
                MessageBox.Show("Введите корректное значение множителя k ≥ 1.");
                return;
            }

            if (!int.TryParse(txtP.Text, out int p) || p < 1)
            {
                MessageBox.Show("Введите корректный порядок точности p ≥ 1.");
                return;
            }

            double k = kInt;

            // Найти ближайшую точку
            var point = points.FirstOrDefault(p => p.X >= xTarget);

            if (point == null && points.Count > 0)
                point = points.Last();

            if (point == null)
            {
                MessageBox.Show("Не найдено подходящих точек.");
                return;
            }

            // Вычисляем производные
            double f_h = ForwardDifference(point, h);
            double f_kh = ForwardDifference(point, h * k);

            if (double.IsNaN(f_h) || double.IsNaN(f_kh))
            {
                MessageBox.Show("Не хватает соседних точек для расчёта производной.");
                return;
            }

            // Формула Рунге
            double refined = f_h + (f_h - f_kh) / (Math.Pow(k, p) - 1);

            txtResult.Text = $"Исходная производная f(x, h): {f_h:F6}\n" +
                             $"Производная f(x, kh): {f_kh:F6}\n\n" +

                             $"Уточнённая производная:\n" +
                             $"f'(x) ≈ {refined:F6}\n\n" +

                             $"Параметры:\n" +
                             $"• Точка x = {xTarget:F3}\n" +
                             $"• Шаг h = {h:F3}\n" +
                             $"• Множитель k = {k}\n" +
                             $"• Порядок точности p = {p}";
        }

        // Метод расчёта производной
        private double ForwardDifference(MyPointData point, double step)
        {
            int idx = points.IndexOf(point);
            if (idx == -1 || points.Count == 0)
                return double.NaN;

            if (idx == 0 && idx < points.Count - 1)
            {
                return (points[1].Y - points[0].Y) / step;
            }
            else if (idx > 0 && idx < points.Count - 1)
            {
                return (points[idx + 1].Y - points[idx - 1].Y) / (2 * step);
            }
            else if (idx == points.Count - 1 && idx > 0)
            {
                return (points[idx].Y - points[idx - 1].Y) / step;
            }

            return double.NaN;
        }

    }
}
