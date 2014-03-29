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
using Microsoft.Kinect;
using Coding4Fun.Kinect.Wpf;
using System.Diagnostics;

namespace KinectFitness
{
    /// <summary>
    /// Interaction logic for SelectLevelWindow.xaml
    /// </summary>
    public partial class SelectLevelWindow : Page
    {
        bool closing = false;
        const int skeletonCount = 6;
        Skeleton[] allSkeletons = new Skeleton[skeletonCount];
        KinectSensor ksensor;
        Skeleton first;
        Stopwatch hoverTimer;

        //Hand Positions
        Rect rightHandPos;

        //Button Positions
        Rect warmUp;
        Rect moderateCardio;
        Rect intenseCardio;
        Rect backButton;


        public SelectLevelWindow()
        {
            InitializeComponent();
            InitializeUI();
            initializeHoverChecker();
            //addExercises();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            kinectSensorChooser1.KinectSensorChanged += new DependencyPropertyChangedEventHandler(kinectSensorChooser1_KinectSensorChanged);
        }

        private void InitializeUI()
        {
            rightHandProgressBar.Width = 0;


            //Get positions of buttons and hands
            rightHandPos = new Rect();
            rightHandPos.Location = new Point(Canvas.GetLeft(rightHand), Canvas.GetTop(rightHand));
            rightHandPos.Size = new Size(rightHand.Width, rightHand.Height);
            warmUp = new Rect();            
            warmUp.Location = new Point(Canvas.GetLeft(warmUpImg), Canvas.GetTop(warmUpImg));
            warmUp.Size = new Size(warmUpImg.Width, warmUpImg.Height);
            moderateCardio = new Rect();
            moderateCardio.Location = new Point(Canvas.GetLeft(moderateImg), Canvas.GetTop(moderateImg));
            moderateCardio.Size = new Size(moderateImg.Width, moderateImg.Height);
            intenseCardio = new Rect();
            intenseCardio.Location = new Point(Canvas.GetLeft(intenseImg), Canvas.GetTop(intenseImg));
            intenseCardio.Size = new Size(intenseImg.Width, intenseImg.Height);
            backButton = new Rect();
            backButton.Location = new Point(Canvas.GetLeft(backButtonImg), Canvas.GetTop(backButtonImg));
            backButton.Size = new Size(backButtonImg.Width, backButtonImg.Height);

        }


        void initializeHoverChecker()
        {
            System.Windows.Threading.DispatcherTimer dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
            //Timer to check for hand positions
            dispatcherTimer.Tick += new EventHandler(checkHands);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, 50);
            dispatcherTimer.Start();

            hoverTimer = new Stopwatch();
        }

        /**
       * Checks to see if hands are hovering over a button
       */
        private void checkHands(object sender, EventArgs e)
        {
            try
            {

                if (rightHandPos.IntersectsWith(warmUp))
                {
                    hoverTimer.Start();
                    //Highlight the correct image and unhighlight the others
                    hoverImage(warmUpImg, new RoutedEventArgs());
                    leaveImage(intenseImg, new RoutedEventArgs());
                    leaveImage(moderateImg, new RoutedEventArgs());

                    //Set progress bar to increase on hands to indicate if hand is hovering on button

                        setHandProgressBar(false, hoverTimer.ElapsedMilliseconds);
                    

                    //Check if hand has been hovering on target for 2 seconds or more   
                    if (hoverTimer.ElapsedMilliseconds >= 2000)
                    {
                        //Tells the Kinect Window which file to load
                        SetFile(warmUpImg);
                        KinectWindow kw = new KinectWindow();
                        this.NavigationService.Navigate(kw);
                        hoverTimer.Reset();
                    }
                }
                else if (rightHandPos.IntersectsWith(moderateCardio))
                {
                    hoverTimer.Start();
                    //Highlight the correct image and unhighlight the others
                    hoverImage(moderateImg, new RoutedEventArgs());
                    leaveImage(intenseImg, new RoutedEventArgs());
                    leaveImage(warmUpImg, new RoutedEventArgs());

                    //Set progress bar to increase on hands to indicate if hand is hovering on button

                        setHandProgressBar(false, hoverTimer.ElapsedMilliseconds);
                    

                    //Check if hand has been hovering on target for 2 seconds or more   
                    if (hoverTimer.ElapsedMilliseconds >= 2000)
                    {
                        //Tells the Kinect Window which file to load
                        SetFile(moderateImg);
                        KinectWindow kw = new KinectWindow();
                        this.NavigationService.Navigate(kw);
                        hoverTimer.Reset();
                    }
                }
                else if (rightHandPos.IntersectsWith(intenseCardio))
                {
                    hoverTimer.Start();
                    //Highlight the correct image and unhighlight the others
                    hoverImage(intenseImg, new RoutedEventArgs());
                    leaveImage(moderateImg, new RoutedEventArgs());
                    leaveImage(warmUpImg, new RoutedEventArgs());


                    //Set progress bar to increase on hands to indicate if hand is hovering on button
                        setHandProgressBar(false, hoverTimer.ElapsedMilliseconds);
                   

                    //Check if hand has been hovering on target for 2 seconds or more   
                    if (hoverTimer.ElapsedMilliseconds >= 2000)
                    {
                        //Tells the Kinect Window which file to load
                        SetFile(intenseImg);
                        KinectWindow kw = new KinectWindow();
                        this.NavigationService.Navigate(kw);
                        hoverTimer.Reset();
                    }
                }
                else if (rightHandPos.IntersectsWith(backButton))
                {
                    hoverTimer.Start();
                    //Highlight the correct image and unhighlight the others
                    hoverImage(backButtonImg, new RoutedEventArgs());
                    leaveImage(moderateImg, new RoutedEventArgs());
                    leaveImage(warmUpImg, new RoutedEventArgs());
                    leaveImage(intenseImg, new RoutedEventArgs());


                    //Set progress bar to increase on hands to indicate if hand is hovering on button

                        setHandProgressBar(false, hoverTimer.ElapsedMilliseconds);
                    

                    //Check if hand has been hovering on target for 2 seconds or more   
                    if (hoverTimer.ElapsedMilliseconds >= 2000)
                    {
                        StartupWindow sw = new StartupWindow();
                        this.NavigationService.Navigate(sw);
                        hoverTimer.Reset();
                    }
                }
                else  //If hand is not hovering on any button.  Reset timer.
                {
                    resetHandProgressBars();
                    hoverTimer.Reset();
                    //Unhighlight all images
                    leaveImage(warmUpImg, new RoutedEventArgs());
                    leaveImage(moderateImg, new RoutedEventArgs());
                    leaveImage(intenseImg, new RoutedEventArgs());
                    leaveImage(backButtonImg, new RoutedEventArgs());

                }
            }
            catch (NullReferenceException)
            {
                //Do Nothing
            }
        }

        private void SetFile(Image exercise)
        {
            List<string> video = new List<string>();

            if (exercise.Name == warmUpImg.Name)
            {
                video.Add(warmUpImg.Name);                
                System.IO.File.WriteAllLines(@"C:\Users\Brad\302\c#\KinectFitness\FitnessVideos\video.txt", video);
            }
            else if (exercise.Name == moderateImg.Name)
            {
                video.Add(moderateImg.Name);
                System.IO.File.WriteAllLines(@"C:\Users\Brad\302\c#\KinectFitness\FitnessVideos\video.txt", video);
            }
            else if (exercise.Name == intenseImg.Name)
            {
                video.Add(intenseImg.Name);
                System.IO.File.WriteAllLines(@"C:\Users\Brad\302\c#\KinectFitness\FitnessVideos\video.txt", video);
            }
        }


        /**
        * Highlights play when the mouse or hand hovers over them
        */
        private void hoverImage(object sender, RoutedEventArgs e)
        {
            Image i = (Image)sender;

            if (i.Name.Equals(warmUpImg.Name))
            {
                warmUpImgBorder.Opacity = 1;
            }
            else if (i.Name.Equals(moderateImg.Name))
            {
                moderateImgBorder.Opacity = 1;
            }
            else if (i.Name.Equals(intenseImg.Name))
            {
                intenseImgBorder.Opacity = 1;
            }
            else if (i.Name.Equals(backButtonImg.Name))
            {
                backButtonImg.Opacity = 0;
                backButtonHoverImg.Opacity = 1;
            }

        }

        /**
         * Stops highlighting the images when the mouse leaves
         */
        private void leaveImage(object sender, RoutedEventArgs e)
        {
            Image i = (Image)sender;

            if (i.Name.Equals(warmUpImg.Name))
            {
                warmUpImgBorder.Opacity = 0;
            }
            else if (i.Name.Equals(moderateImg.Name))
            {
                moderateImgBorder.Opacity = 0;
            }
            else if (i.Name.Equals(intenseImg.Name))
            {
                intenseImgBorder.Opacity = 0;
            }
            else if (i.Name.Equals(backButtonImg.Name))
            {
                backButtonHoverImg.Opacity = 0;
                backButtonImg.Opacity = 1;
            }
        }


        private void setHandProgressBar(bool leftHand, long timeElapsed)
        {
            double t = timeElapsed;
            double w = t / 2000 * 50;

                rightHandProgressBar.Width = w;
            
        }

        /**
         * Resets the progress bar on the hands
         */
        private void resetHandProgressBars()
        {

            rightHandProgressBar.Width = 0;
        }


        void kinectSensorChooser1_KinectSensorChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            KinectSensor old = (KinectSensor)e.OldValue;

            StopKinect(old);

            KinectSensor sensor = (KinectSensor)e.NewValue;

            if (sensor == null)
            {
                return;
            }

            var parameters = new TransformSmoothParameters
            {
                Smoothing = 0.7f,
                Correction = 0.3f,
                Prediction = 1.0f,
                JitterRadius = 1.0f,
                MaxDeviationRadius = 1.0f
            };
            //sensor.SkeletonStream.Enable(parameters);

            sensor.SkeletonStream.Enable();

            sensor.AllFramesReady += new EventHandler<AllFramesReadyEventArgs>(sensor_AllFramesReady);
            sensor.DepthStream.Enable(DepthImageFormat.Resolution640x480Fps30);
            sensor.ColorStream.Enable(ColorImageFormat.RgbResolution640x480Fps30);

            try
            {
                sensor.Start();
                ksensor = sensor;
            }
            catch (System.IO.IOException)
            {
                kinectSensorChooser1.AppConflictOccurred();
            }
        }

        void sensor_AllFramesReady(object sender, AllFramesReadyEventArgs e)
        {
            if (closing)
            {
                return;
            }

            //Get a skeleton
            first = GetFirstSkeleton(e);

            if (first == null)
            {
                return;
            }

            GetCameraPoint(first, e);
            //set scaled position

            ScalePosition(rightHand, first.Joints[JointType.HandRight]);

            ScalePosition(rightHandProgressBar, first.Joints[JointType.HandRight]);


            rightHandPos.Location = new Point(Canvas.GetLeft(rightHand), Canvas.GetTop(rightHand));



        }

        void GetCameraPoint(Skeleton first, AllFramesReadyEventArgs e)
        {
            using (DepthImageFrame depth = e.OpenDepthImageFrame())
            {
                if (depth == null ||
                    kinectSensorChooser1.Kinect == null)
                {
                    return;
                }

                //Map a joint location to a point on the depth map
                //left hand
                DepthImagePoint leftDepthPoint =
                    depth.MapFromSkeletonPoint(first.Joints[JointType.HandLeft].Position);
                //right hand
                DepthImagePoint rightDepthPoint =
                    depth.MapFromSkeletonPoint(first.Joints[JointType.HandRight].Position);



                //Map a depth point to a point on the color image
                //left hand
                ColorImagePoint leftColorPoint =
                    depth.MapToColorImagePoint(leftDepthPoint.X, leftDepthPoint.Y,
                    ColorImageFormat.RgbResolution640x480Fps30);
                //right hand
                ColorImagePoint rightColorPoint =
                    depth.MapToColorImagePoint(rightDepthPoint.X, rightDepthPoint.Y,
                    ColorImageFormat.RgbResolution640x480Fps30);


                //Set location
                CameraPosition(rightHand, rightColorPoint);
                CameraPosition(rightHandProgressBar, rightColorPoint);
            }
        }


        Skeleton GetFirstSkeleton(AllFramesReadyEventArgs e)
        {
            using (SkeletonFrame skeletonFrameData = e.OpenSkeletonFrame())
            {
                if (skeletonFrameData == null)
                {
                    return null;
                }


                skeletonFrameData.CopySkeletonDataTo(allSkeletons);

                //get the first tracked skeleton
                Skeleton first = (from s in allSkeletons
                                  where s.TrackingState == SkeletonTrackingState.Tracked
                                  select s).FirstOrDefault();

                return first;

            }
        }

        private void StopKinect(KinectSensor sensor)
        {
            if (sensor != null)
            {
                if (sensor.IsRunning)
                {
                    //stop sensor 
                    sensor.Stop();

                    //stop audio if not null
                    if (sensor.AudioSource != null)
                    {
                        sensor.AudioSource.Stop();
                    }
                }
            }
        }

        private void CameraPosition(FrameworkElement element, ColorImagePoint point)
        {
            //Divide by 2 for width and height so point is right in the middle 
            // instead of in top/left corner
            Canvas.SetLeft(element, point.X - element.Width / 2);
            Canvas.SetTop(element, point.Y - element.Height / 2);
        }

        private void ScalePosition(FrameworkElement element, Joint joint)
        {
            //Joint scaledJoint = joint.ScaleTo(1280, 720); 

            Joint scaledJoint = joint.ScaleTo(900, 800, .3f, .3f);

            Canvas.SetLeft(element, scaledJoint.Position.X);
            Canvas.SetTop(element, scaledJoint.Position.Y);

        }

        private void kinectSkeletonViewer1_Unloaded(object sender, RoutedEventArgs e)
        {
            closing = true;
            StopKinect(kinectSensorChooser1.Kinect);
        }



        /*
        //Add exercises with buttons dynamically
        public void addExercises() 
        {
            int x = 0;
            List<string> exercises;
            exercises = getFiles();
            foreach(string exercise in exercises)
            {
                //Make sure a button is not created for the 'chosenExercise' file
                if (!exercise.Contains("chosenExercise"))
                {
                    System.Windows.Controls.Button newBtn = new Button();
                    x++;
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
                File.Delete("..\\..\\Recordings\\chosenExercise.txt");
                System.IO.File.Copy(exercise, "..\\..\\Recordings\\chosenExercise.txt");
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
                string[] fileEntries = Directory.GetFiles("..\\..\\Recordings");
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
        */
    }
}
