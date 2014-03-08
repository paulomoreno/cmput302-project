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

namespace KinectFitness
{
    /// <summary>
    /// Interaction logic for StartupWindow.xaml
    /// </summary>
    public partial class StartupWindow : Page
    {
        public StartupWindow()
        {
            InitializeComponent();
        }

        private void Button_Play(object sender, RoutedEventArgs e)
        {
            // View Expense Report
            KinectWindow kw = new KinectWindow();
            this.NavigationService.Navigate(kw);

        }

        private void Button_Options(object sender, RoutedEventArgs e)
        {

        }

        private void Change_Opacity(object sender, RoutedEventArgs e)
        {
            textBlock1.Opacity = 0.5;
        }

    }
}
