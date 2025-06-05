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

namespace Chisldiferen
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void LeftMehtod_Click(object sender, RoutedEventArgs e)
        {
            Left left= new Left();
            left.Show();
        }

        private void QuadricMethod_Click(object sender, RoutedEventArgs e)
        {
            Quadric quadric= new Quadric();
            quadric.Show();
        }

        private void RungeMethod_Click(object sender, RoutedEventArgs e)
        {
            Runge runge= new Runge();
            runge.Show();
        }

        private void NutonMethod_Click(object sender, RoutedEventArgs e)
        {
            Nuton nuton= new Nuton();
            nuton.Show();
        }

        private void CentreMethod_Click(object sender, RoutedEventArgs e)
        {
            Centre centre= new Centre();
            centre.Show();
        }

        private void MnkMethod_Click(object sender, RoutedEventArgs e)
        {
            Mnk mnk= new Mnk();
            mnk.Show();
        }

        private void CubicMethod_Click(object sender, RoutedEventArgs e)
        {
            Cubic cubic= new Cubic();
            cubic.Show();
        }

        private void RightMethod_Click(object sender, RoutedEventArgs e)
        {
            Right right= new Right();
            right.Show();
        }
    }
}