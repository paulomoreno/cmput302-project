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
    public partial class StartupWindow : Page
    {
        public StartupWindow()
        {
            InitializeComponent();
            var navWindow = Window.GetWindow(this) as NavigationWindow;
            if (navWindow != null) navWindow.ShowsNavigationUI = false;
        }

        private void Button_Play(object sender, RoutedEventArgs e)
        {
            KinectWindow kw = new KinectWindow();
            this.NavigationService.Navigate(kw);
            
        }

        private void Button_Record(object sender, RoutedEventArgs e)
        {
            RecordWindow rw = new RecordWindow();
            this.NavigationService.Navigate(rw);
        }

        private void Button_Options(object sender, RoutedEventArgs e)
        {

        }

        /**
         * Highlights images when the mouse hovers over them
         */
        private void mouse_hover(object sender, RoutedEventArgs e)
        {
            Image i = (Image)sender;

            if (i.Name.Equals(playButton.Name))
            {
                playborder.Opacity = 1;
            }
            else if (i.Name.Equals(optionsButton.Name))
            {
                optionsborder.Opacity = 1;
            }
            else if (i.Name.Equals(tutorialbutton.Name))
            {
                tutorialborder.Opacity = 1;
            }
            else if (i.Name.Equals(quitbutton.Name))
            {
                quitborder.Opacity = 1;
            }
            else if (i.Name.Equals(recordbutton.Name))
            {
                recordborder.Opacity = 1;
            }
        }

        /**
         * Stops highlighting the images when the mouse leaves
         */
        private void mouse_leave(object sender, RoutedEventArgs e)
        {
                Image i = (Image)sender;   
                if(i.Name.Equals(playButton.Name))
                {
                    playborder.Opacity = 0;
                }
                else if (i.Name.Equals(optionsButton.Name))
                {
                    optionsborder.Opacity = 0;
                }
                else if (i.Name.Equals(tutorialbutton.Name))
                {
                    tutorialborder.Opacity = 0;
                }
                else if (i.Name.Equals(quitbutton.Name))
                {
                    quitborder.Opacity = 0;
                }
                else if (i.Name.Equals(recordbutton.Name))
                {
                    recordborder.Opacity = 0;
                }

        }

    }
}
