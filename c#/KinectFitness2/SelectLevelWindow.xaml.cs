using System.IO;
using System.Collections;
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
    /// Interaction logic for SelectLevelWindow.xaml
    /// </summary>
    public partial class SelectLevelWindow : Page
    {
        public SelectLevelWindow()
        {
            InitializeComponent();
            addExercises();
        }
        //Add exercises with buttons dynamically
        public void addExercises() 
        {
            int x = 0;
            List<string> exercises;
            exercises = getFiles();
            foreach(string exercise in exercises)
            {
                //Make sure a button is not created for the 'chosenExercise' file
                if (!exercise.Contains("chosenExercise") && exercise.EndsWith(".txt"))
                {
                    System.Windows.Controls.Button newBtn = new Button();
                    x++;
                    //newBtn.Content = "Exercise " + x.ToString();
                    newBtn.Content = exercise;
                    newBtn.Name = "e" + x.ToString();
                    newBtn.Click += new RoutedEventHandler(newBtn_Click);

                    levels.Children.Add(newBtn);
                }
            }
        }

        //Copy the exercise file chosen by user to the "chosenExercise.txt" file 
        //and navigate to Kinect Exercise Window
        private void newBtn_Click(object sender, RoutedEventArgs e)
        {
            String exercise = (sender as Button).Content.ToString();
            try
            {
                String path = System.AppDomain.CurrentDomain.BaseDirectory;
                path = System.IO.Directory.GetParent(path).FullName;
                path = System.IO.Directory.GetParent(path).FullName;
                path = System.IO.Directory.GetParent(path).FullName;
                path = System.IO.Directory.GetParent(path).FullName;

                File.Delete(path + "\\Recordings\\chosenExercise.txt");
                System.IO.File.Copy(exercise, path + "\\Recordings\\chosenExercise.txt");
            }
            catch (IOException)
            {
                MessageBox.Show("Error in loading exercise");
            }
            KinectWindow kw = new KinectWindow();
            this.NavigationService.Navigate(kw);
            
        }

        private List<string> getFiles()
        {
            List<string> exercises = new List<string>();
            try
            {
                String path = System.AppDomain.CurrentDomain.BaseDirectory;
                path = System.IO.Directory.GetParent(path).FullName;
                path = System.IO.Directory.GetParent(path).FullName;
                path = System.IO.Directory.GetParent(path).FullName;
                path = System.IO.Directory.GetParent(path).FullName;

                string[] fileEntries = Directory.GetFiles(path + "\\Recordings");
                foreach (string fileName in fileEntries)
                {
                    exercises.Add(fileName);
                }
            }
            catch(DirectoryNotFoundException)
            {
                MessageBox.Show("Error Finding Exercises");
            }            
            return exercises;
        }
    }
}
