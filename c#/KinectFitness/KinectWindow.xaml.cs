﻿using System;
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
    /// Interaction logic for KinectWindow.xaml
    /// </summary>
    /// 


   

    public partial class KinectWindow : Page
    {
        public KinectWindow()
        {
            InitializeComponent();
            
        }
        
        bool closing = false;
        bool videoPlaying;
        bool timerInitialized;
        const int skeletonCount = 6;
        Skeleton[] allSkeletons = new Skeleton[skeletonCount];
        KinectSensor ksensor;
        double numberOfPts;
        Stopwatch stopwatch;
        Random r;

        //Hand Positions
        Rect leftHandPos;
        Rect rightHandPos;

        //Button Positions
        Rect playIconPos;

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            kinectSensorChooser1.KinectSensorChanged += new DependencyPropertyChangedEventHandler(kinectSensorChooser1_KinectSensorChanged);
            initializeUI();           
        }

        void initializeTimer()
        {
            System.Windows.Threading.DispatcherTimer dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
            
            //Timer for Pseudo Heart Rate
            dispatcherTimer.Tick += new EventHandler(heartRate);
            dispatcherTimer.Interval = new TimeSpan(0,0,1);
            dispatcherTimer.Start();

            //Timer to check for hand positions
            dispatcherTimer.Tick += new EventHandler(checkHands);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, 1000);
            dispatcherTimer.Start();


            stopwatch = new Stopwatch();
            stopwatch.Start();
            r = new Random();
        }

        /**
         * Pseudo Heart Rate Simulator
         */
        private void heartRate(object sender, EventArgs e)
        {            
            int heartRate = r.Next(50,130);
            heartRateToPoints(heartRate);            
        }

        /**
         * Checks the heart rate and converts it to points
         */
        void heartRateToPoints(int heartRate)
        {
            stopwatch.Stop();
            long elapsed = stopwatch.ElapsedMilliseconds;
            if (heartRate > 90 && heartRate < 110)
            {
                numberOfPts += elapsed;
                setPoints();
            }

            var heartarrowangle = heartarrow.RenderTransform as RotateTransform;
            heartarrowangle.Angle = heartRate*3;                  
            
            stopwatch.Reset();
            stopwatch.Start();
        }

        /**
         * Sets the points to the progress bar and number of points
         */
        void setPoints()
        {            
            points.Text = numberOfPts + "Pts.";            
        }

        /**
         * Initializes the UI
         */
        void initializeUI()
        {
            numberOfPts = 0;
            points.Text = numberOfPts + "Pts.";
            videoPlaying = false;
            timerInitialized = false;
          
            //Get positions of buttons
            leftHandPos = new Rect();
            leftHandPos.Location = new Point(Canvas.GetLeft(leftHand), Canvas.GetTop(leftHand));
            leftHandPos.Size = new Size(leftHand.Width, leftHand.Height);
            rightHandPos = new Rect();
            rightHandPos.Location = new Point(Canvas.GetLeft(rightHand), Canvas.GetTop(rightHand));
            rightHandPos.Size = new Size(rightHand.Width, rightHand.Height);
            playIconPos = new Rect();
            playIconPos.Location = new Point(Canvas.GetLeft(playicon), Canvas.GetTop(playicon));
            playIconPos.Size = new Size(playicon.Width, playicon.Height);
        }

        /**
         * Checks to see if hands are hovering over a button
         */
        private void checkHands(object sender, EventArgs e)
        {
            points.Text = leftHandPos.Top.ToString();
            if (leftHandPos.IntersectsWith(playIconPos) || rightHandPos.IntersectsWith(playIconPos))
            {                
                btnPlay_Click(sender, new RoutedEventArgs());
            }
        }


        /**
         * Highlights play when the mouse hovers over them
         */
        private void hoverPlay(object sender, RoutedEventArgs e)
        {
            Image i = (Image)sender;

            if (i.Name.Equals(playicon.Name))
            {
                if (!videoPlaying)
                {
                    playicon.Opacity = 0;
                    hoverplayicon.Opacity = 1;
                    pauseicon.Opacity = 0;
                    hoverpauseicon.Opacity = 0;                    
                }
                else
                {
                    playicon.Opacity = 0;
                    hoverplayicon.Opacity = 0;
                    pauseicon.Opacity = 0;
                    hoverpauseicon.Opacity = 1;
                }
            }
            
        }

        /**
         * Stops highlighting the images when the mouse leaves
         */
        private void leavePlay(object sender, RoutedEventArgs e)
        {
            Image i = (Image)sender;

            if (i.Name.Equals(playicon.Name))
            {
                if (!videoPlaying)
                {
                    playicon.Opacity = 1;
                    hoverplayicon.Opacity = 0;
                    pauseicon.Opacity = 0;
                    hoverpauseicon.Opacity = 0;
                }
                else
                {
                    playicon.Opacity = 0;
                    hoverplayicon.Opacity = 0;
                    pauseicon.Opacity = 1;
                    hoverpauseicon.Opacity = 0;
                }
            }
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
                Smoothing = 0.3f,
                Correction = 0.0f,
                Prediction = 0.0f,
                JitterRadius = 1.0f,
                MaxDeviationRadius = 0.5f
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
            Skeleton first = GetFirstSkeleton(e);

            if (first == null)
            {
                return;
            }

            GetCameraPoint(first, e);
            //set scaled position
            ScalePosition(leftHand, first.Joints[JointType.HandLeft]);
            ScalePosition(rightHand, first.Joints[JointType.HandRight]);
            leftHandPos.Location = new Point(Canvas.GetLeft(leftHand), Canvas.GetTop(leftHand));
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
                CameraPosition(leftHand, leftColorPoint);
                CameraPosition(rightHand, rightColorPoint);
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

        private void motorUp_Click(object sender, RoutedEventArgs e)
        {
            
            try
            {
                int x = ksensor.ElevationAngle;
                ksensor.ElevationAngle += 5;
            }
            catch (InvalidOperationException)
            {
                MessageBox.Show("Failed to move Kinect motor.");
            }
            catch (ArgumentOutOfRangeException)
            {
                MessageBox.Show("Elevation angle is out of range.");
            }
        }

        private void motorDown_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int x = ksensor.ElevationAngle;
                ksensor.ElevationAngle -= 5;
            }
            catch (InvalidOperationException)
            {
                MessageBox.Show("Failed to move Kinect motor.");
            }
            catch (ArgumentOutOfRangeException)
            {
                MessageBox.Show("Elevation angle is out of range.");
            }
        }


        private void kinectSkeletonViewer1_Unloaded(object sender, RoutedEventArgs e)
        {
            closing = true;
            StopKinect(kinectSensorChooser1.Kinect); 
        }


        /*
         * Media Player Stuff
         */


        private void btnPlay_Click(object sender, RoutedEventArgs e)
        {
            MediaPlayer.Source = new Uri("C:\\Users\\Public\\Videos\\Sample Videos\\Wildlife.wmv");
            

            if (!videoPlaying)
            {
                MediaPlayer.Play();                
                videoPlaying = true;
                if (!timerInitialized)
                {
                    initializeTimer();
                    timerInitialized = true;
                }
            }
            else
            {
                MediaPlayer.Stop();
                videoPlaying = false;
            }
        }




    }
}