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
    /// Логика взаимодействия для Nuton.xaml
    /// </summary>
    public partial class Nuton : Window
    {
        private List<MyPointData> points = new List<MyPointData>();
        private double xTarget;
        public Nuton()
        {
            InitializeComponent();
        }

        private void BtnAddPoint_Click(object sender, RoutedEventArgs e)
        {
            if (!double.TryParse(txtX.Text, out double x) || !double.TryParse(txtY.Text, out double y))
            {
                MessageBox.Show("Введите корректные числа для X и Y.");
                return;
            }

            points.Add(new MyPointData { X = x, Y = y });
            lstPoints.Items.Add(new MyPointData { X = x, Y = y });

            txtX.Clear();
            txtY.Clear();
        }

        private void BtnCalculate_Click(object sender, RoutedEventArgs e)
        {
            if (points.Count < 4)
            {
                MessageBox.Show("Для расчёта производной по многочлену Ньютона нужно минимум 4 точки.");
                return;
            }

            var sortedPoints = points.OrderBy(p => p.X).ToList();

            // Проверяем, является ли сетка равномерной
            double h = sortedPoints[1].X - sortedPoints[0].X;

            bool isUniform = true;
            for (int i = 0; i < sortedPoints.Count - 1; i++)
            {
                double currentH = sortedPoints[i + 1].X - sortedPoints[i].X;
                if (Math.Abs(currentH - h) > 1e-8)
                {
                    isUniform = false;
                    break;
                }
            }

            if (!isUniform)
            {
                MessageBox.Show("Метод Ньютона требует равномерной сетки. Введите точки с одинаковым шагом.");
                return;
            }

            // Ближайшая точка к xTarget
            int index = FindNearestIndex(sortedPoints, xTarget);
            if (index < 0 || index + 3 >= sortedPoints.Count)
            {
                MessageBox.Show("Выберите точку, для которой можно построить интерполяцию (минимум 4 точки подряд)");
                return;
            }

            var p0 = sortedPoints[index];
            var p1 = sortedPoints[index + 1];
            var p2 = sortedPoints[index + 2];
            var p3 = sortedPoints[index + 3];

            // Строим таблицу конечных разностей
            var yValues = new List<double> { p0.Y, p1.Y, p2.Y, p3.Y };
            var finiteDiffs = BuildFiniteDifferenceTable(yValues);

            // Вычисляем q
            double x0 = p0.X;
            double q = (xTarget - x0) / h;

            // Формула производной из вашей задачи
            double delta1 = finiteDiffs[0][1];
            double delta2 = finiteDiffs[0][2];
            double delta3 = finiteDiffs[0][3];

            double derivative = (1 / h) * (
                delta1 +
                ((2 * q - 1) / 2.0) * delta2 +
                ((3 * q * q - 6 * q + 2) / 6.0) * delta3
            );

            // Обновляем производную у центральной точки
            p1.Derivative = "—";
            p2.Derivative = derivative.ToString("F4");

            lstPoints.Items.Clear();
            foreach (var point in points)
            {
                lstPoints.Items.Add(point.ToString());
            }

            txtResult.Text = $"Производная f'(x) ≈ {derivative:F4}\n" +
                             $"Параметры:\n" +
                             $"• x = {xTarget:F2}\n" +
                             $"• x₀ = {x0:F2}\n" +
                             $"• q = {q:F2}\n" +
                             $"• h = {h:F2}";
        }
        // Метод построения таблицы конечных разностей
        private List<List<double>> BuildFiniteDifferenceTable(List<double> yValues)
        {
            int n = yValues.Count;
            var table = new List<List<double>>();

            table.Add(yValues.ToList());

            for (int i = 1; i < n; i++)
            {
                var diffRow = new List<double>();
                for (int j = 0; j < n - i; j++)
                {
                    double diff = table[i - 1][j + 1] - table[i - 1][j];
                    diffRow.Add(diff);
                }
                table.Add(diffRow);
            }

            return table;
        }
        // Поиск ближайшей точки к xTarget
        private int FindNearestIndex(List<MyPointData> points, double target)
        {
            int idx = points.FindIndex(p => p.X >= target);
            if (idx == -1 || idx == 0)
                return 0;
            if (idx >= points.Count)
                return points.Count - 1;
            return idx - 1;
        }
        private void BtnClear_Click(object sender, RoutedEventArgs e)
        {
            points.Clear();
            lstPoints.Items.Clear();
        }
    }
}
