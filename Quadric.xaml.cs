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
    /// Логика взаимодействия для Quadric.xaml
    /// </summary>
    public partial class Quadric : Window
    {
        private List<MyPointData> points = new List<MyPointData>();
        private double xTarget;
        public Quadric()
        {
            InitializeComponent();
        }

        private void BtnAddPoint_Click(object sender, RoutedEventArgs e)
        {
            if (!double.TryParse(txtX.Text, out double x) ||
                !double.TryParse(txtY.Text, out double y))
            {
                MessageBox.Show("Введите корректные значения X и Y");
                return;
            }

            var point = new MyPointData { X = x, Y = y };
            points.Add(point);
            lstPoints.Items.Add(point);

            txtX.Clear();
            txtY.Clear();
        }

        private void BtnCalculate_Click(object sender, RoutedEventArgs e)
        {
            if (points.Count < 3)
            {
                MessageBox.Show("Для квадратичной интерполяции необходимо минимум 3 точки.");
                return;
            }

            var sortedPoints = points.OrderBy(p => p.X).ToList();

            for (int i = 0; i < sortedPoints.Count - 2; i++)
            {
                var p0 = sortedPoints[i];
                var p1 = sortedPoints[i + 1];
                var p2 = sortedPoints[i + 2];

                double[,] matrix = new double[,]
                {
            { p0.X * p0.X, p0.X, 1, p0.Y },
            { p1.X * p1.X, p1.X, 1, p1.Y },
            { p2.X * p2.X, p2.X, 1, p2.Y }
                };

                if (SolveSystem(matrix, out double a, out double b, out _))
                {
                    p1.Derivative = (2 * a * p1.X + b).ToString("F4");
                }
                else
                {
                    p1.Derivative = "Ошибка";
                }
            }

            // Очищаем и заново выводим все точки с новыми значениями производных
            lstPoints.Items.Clear();
            foreach (var point in points)
            {
                lstPoints.Items.Add(point.ToString());
            }
        }

        private void BtnClear_Click(object sender, RoutedEventArgs e)
        {
            points.Clear();
            lstPoints.Items.Clear();
        }
        private bool SolveSystem(double[,] matrix, out double a, out double b, out double c)
        {
            a = b = c = 0;
            int n = matrix.GetLength(0);

            // Прямой ход метода Гаусса
            for (int i = 0; i < n; i++)
            {
                int maxRow = i;
                for (int k = i + 1; k < n; k++)
                {
                    if (Math.Abs(matrix[k, i]) > Math.Abs(matrix[i, i]))
                        maxRow = k;
                }

                // Перестановка строк
                if (maxRow != i)
                {
                    for (int k = i; k < n + 1; k++)
                    {
                        double tmp = matrix[maxRow, k];
                        matrix[maxRow, k] = matrix[i, k];
                        matrix[i, k] = tmp;
                    }
                }

                // Приведение к треугольному виду
                double factor = matrix[i, i];
                if (Math.Abs(factor) < 1e-10)
                {
                    return false; // Вырожденная матрица
                }

                for (int k = i + 1; k < n; k++)
                {
                    double f = matrix[k, i] / factor;
                    for (int j = i; j < n + 1; j++)
                    {
                        matrix[k, j] -= f * matrix[i, j];
                    }
                }
            }

            // Обратный ход
            double[] solution = new double[n];
            for (int i = n - 1; i >= 0; i--)
            {
                solution[i] = matrix[i, n];
                for (int j = i + 1; j < n; j++)
                {
                    solution[i] -= matrix[i, j] * solution[j];
                }
                solution[i] /= matrix[i, i];
            }

            a = solution[0];
            b = solution[1];
            c = solution[2];

            return true;
        }
    }
}
