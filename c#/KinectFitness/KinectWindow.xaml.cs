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
        int numberOfPtsBar;
        double totalMovieTime;
        Stopwatch hoverTimer;
        Random r;
        Skeleton first;
        List<string> jointAngles;
        List<JointAngles> loadedSkeleton;
        System.Windows.Threading.DispatcherTimer skeletonMatcherTimer;
        System.Windows.Threading.DispatcherTimer videoProgressBarTracker;

        //Hand Positions
        Rect leftHandPos;
        Rect rightHandPos;

        //Button Positions
        Rect playIconPos;

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            kinectSensorChooser1.KinectSensorChanged += new DependencyPropertyChangedEventHandler(kinectSensorChooser1_KinectSensorChanged);
            initializeUI();
            initializeHoverChecker();
            loadExercise();
        }

        private void loadExercise()
        {
            string line;
            JointAngles ja = new JointAngles();
            loadedSkeleton = new List<JointAngles>();
            // Read the file and display it line by line.
            System.IO.StreamReader file =
               new System.IO.StreamReader("..\\..\\Recordings\\chosenExercise.txt");
            while ((line = file.ReadLine()) != null)
            {
                if(line.Contains("Left Shoulder"))
                {
                    ja.leftShoulder = Convert.ToInt32(file.ReadLine());
                }
                else if (line.Contains("Right Shoulder"))
                {
                    ja.rightShoulder = Convert.ToInt32(file.ReadLine());
                }
                else if (line.Contains("Left Elbow"))
                {
                    ja.leftElbow = Convert.ToInt32(file.ReadLine());
                }
                else if (line.Contains("Left Hip"))
                {
                    ja.leftHip = Convert.ToInt32(file.ReadLine());
                }
                else if (line.Contains("Left Knee"))
                {
                    ja.leftKnee = Convert.ToInt32(file.ReadLine());
                }
                else if (line.Contains("Right Elbow"))
                {
                    ja.rightElbow = Convert.ToInt32(file.ReadLine());
                }
                else if (line.Contains("Right Hip"))
                {
                    ja.rightHip = Convert.ToInt32(file.ReadLine());
                }
                else if (line.Contains("Right Knee"))
                {
                    ja.rightKnee = Convert.ToInt32(file.ReadLine());
                }
                else if (line.Contains("End"))
                {
                    loadedSkeleton.Add(ja);
                    ja = new JointAngles();
                }
            }

            file.Close();

            // Suspend the screen.
            Console.ReadLine();
        }

        void initializeHoverChecker()
        {
            System.Windows.Threading.DispatcherTimer dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
            //Timer to check for hand positions
            dispatcherTimer.Tick += new EventHandler(checkHands);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, 100);
            dispatcherTimer.Start();

            hoverTimer = new Stopwatch();
        }

        /*
        void initializeTimer()
        {
            System.Windows.Threading.DispatcherTimer dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
            
            //Timer for Pseudo Heart Rate
            dispatcherTimer.Tick += new EventHandler(heartRate);
            dispatcherTimer.Interval = new TimeSpan(0,0,1);
            dispatcherTimer.Start();

            stopwatch = new Stopwatch();
            stopwatch.Start();
            r = new Random();
        }
        */

        /**
         * Starts playing back the skeleton and matching it against the user
         */
        private void startPlaybackSkeleton()
        {
            skeletonMatcherTimer = new System.Windows.Threading.DispatcherTimer();
            skeletonMatcherTimer.Tick += new EventHandler(matchSkeleton);
            skeletonMatcherTimer.Interval = new TimeSpan(0, 0, 1);
            skeletonMatcherTimer.Start();
        }

        /**
         * Starts the timer for the progress bar
         */
        private void startVideoProgressBar()
        {
            videoProgressBarTracker = new System.Windows.Threading.DispatcherTimer();
            videoProgressBarTracker.Tick += new EventHandler(updateVideoProgressBar);
            videoProgressBarTracker.Interval = new TimeSpan(0, 0, 1);
            videoProgressBarTracker.Start();
        }

        /**
         * Updates the video progress bar every second
         */
        private void updateVideoProgressBar(object sender, EventArgs e)
        {            
            videoProgressBar.Width = (FitnessPlayer.Position.Seconds / totalMovieTime) * 855;
        }

        /**
         * Checks if the loaded skeleton data matches the users skeleton 
         * periodically every second
         */
        private void matchSkeleton(object sender, EventArgs e)
        {
            numberOfPtsBar = 0;
            setPoints();
            //Get amount of seconds passed in video
            TimeSpan timePassedInVideo = FitnessPlayer.Position;
            int secondsPassedInVideo = timePassedInVideo.Seconds;

            //Check if loaded skeleton at this point matches the users current data within +- 1 second of the video
            try
            {
                if (SkeletonMatchesCloselyEnough(loadedSkeleton.ElementAt(secondsPassedInVideo)))
                {
                    
                }
                else
                {
                    
                }
            }
            catch (ArgumentOutOfRangeException)
            {
                //Do nothing
            }
        }

        /**
         * Checks if the skeleton data matches within certain
         * angles to the users
         */
        private bool SkeletonMatchesCloselyEnough(JointAngles ja)
        {
            bool everythingMatches = true;
            if (first == null)
            {
                return false;
            }
            int leftElbow = AngleBetweenJoints(first.Joints[JointType.HandLeft], first.Joints[JointType.ElbowLeft], first.Joints[JointType.ShoulderLeft]);
            int rightElbow = AngleBetweenJoints(first.Joints[JointType.HandRight], first.Joints[JointType.ElbowRight], first.Joints[JointType.ShoulderRight]);
            int rightShoulder = AngleBetweenJoints(first.Joints[JointType.ElbowRight], first.Joints[JointType.ShoulderRight], first.Joints[JointType.ShoulderCenter]);          
            int leftHip = AngleBetweenJoints(first.Joints[JointType.ShoulderLeft], first.Joints[JointType.HipLeft], first.Joints[JointType.KneeLeft]);
            int rightHip = AngleBetweenJoints(first.Joints[JointType.ShoulderRight], first.Joints[JointType.HipRight], first.Joints[JointType.KneeRight]); 
            int leftKnee = AngleBetweenJoints(first.Joints[JointType.HipLeft], first.Joints[JointType.KneeLeft], first.Joints[JointType.FootLeft]);
            int rightKnee = AngleBetweenJoints(first.Joints[JointType.HipRight], first.Joints[JointType.KneeRight], first.Joints[JointType.FootRight]);
                
            //Check if patient's joint angle is +- 30 degrees of the exercise
            if (leftElbow < (ja.leftElbow - 30) || leftElbow > (ja.leftElbow + 30))
            {
                everythingMatches = false;
            }
            else
            {
                numberOfPtsBar += 15;
                numberOfPts += 50;
                setPoints();
            }            
            if (rightElbow < (ja.rightElbow - 30) || rightElbow > (ja.rightElbow + 30))
            {
                everythingMatches = false;
            }
            else
            {
                numberOfPtsBar += 15;
                numberOfPts += 50;
                setPoints();
            }
            if (leftHip < (ja.leftHip - 30) || leftHip > (ja.leftHip + 30))
            {
                everythingMatches = false;
            }
            else
            {
                numberOfPtsBar += 15;
                numberOfPts += 50;
                setPoints();
            }
            if (rightHip < (ja.rightHip - 30) || rightHip > (ja.rightHip + 30))
            {
                everythingMatches = false;
            }
            else
            {
                numberOfPtsBar += 15;
                numberOfPts += 50;
                setPoints();
            }
            if (rightKnee < (ja.rightKnee - 30) || rightKnee > (ja.rightKnee + 30))
            {
                everythingMatches = false;
            }
            else
            {
                numberOfPtsBar += 15;
                numberOfPts += 50;
                setPoints();
            }
            if (leftKnee < (ja.leftKnee - 30) || leftKnee > (ja.leftKnee + 30))
            {
                everythingMatches = false;
            }
            else
            {
                numberOfPtsBar += 15;
                numberOfPts += 50;
                setPoints();
            }
            if (rightShoulder < (ja.rightShoulder - 30) || rightShoulder > (ja.rightShoulder + 30))
            {
                everythingMatches = false;
            }
            else
            {
                numberOfPtsBar += 15;
                numberOfPts += 50;
                setPoints();
            }
            return everythingMatches;
        }
        /**
         * Pause the playback of the skeleton
         */
        private void pausePlaybackSkeleton()
        {
            skeletonMatcherTimer.Stop();
        }

        /**
         * Pseudo Heart Rate Simulator
         */
        /*
        private void heartRate(object sender, EventArgs e)
        {            
            int heartRate = r.Next(50,130);
            heartRateToPoints(heartRate);            
        }
        */

        /**
         * Checks the heart rate and converts it to points
         */
        //This method has been discontinued for this part of the program
        /*
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
        */

        /**
         * Sets the points to the progress bar and number of points
         */
        void setPoints()
        {            
            points.Text = numberOfPts + "Pts.";
            pointsBar.Value = numberOfPtsBar;
        }

        /**
         * Initializes the UI
         */
        void initializeUI()
        {
            numberOfPts = 0;
            numberOfPtsBar = 0;
            points.Text = numberOfPts + "Pts.";
            pointsBar.Value = numberOfPtsBar;
            videoPlaying = false;
            timerInitialized = false;
            videoProgressBar.Width = 0;
          
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

            //Set video source for video player
            FitnessPlayer.Source = new Uri("..\\..\\FitnessVideos\\Workout2.mp4", UriKind.Relative);
            //Get Length of Video
            FitnessPlayer.MediaOpened += new System.Windows.RoutedEventHandler(media_MediaOpened);
        }

        //Find length of video
        void media_MediaOpened(object sender, System.Windows.RoutedEventArgs e)
        {
            totalMovieTime = FitnessPlayer.NaturalDuration.TimeSpan.TotalSeconds;
        }

        /**
        * Checks to see if hands are hovering over a button
        */
        private void checkHands(object sender, EventArgs e)
        {
            
            if (leftHandPos.IntersectsWith(playIconPos) || rightHandPos.IntersectsWith(playIconPos))
            {
                hoverTimer.Start();
                hoverPlay(playicon, new RoutedEventArgs());
                //Check if hand has been hovering on target for 1 second or more   
                if (hoverTimer.ElapsedMilliseconds >= 1000)
                {
                    //Presses the play button
                    btnPlay_Click(sender, new RoutedEventArgs());
                    //Resets hoverTimer
                    hoverTimer.Reset();
                }
            }
            else  //If hand is not hovering on any button.  Reset timer.
            {
                hoverTimer.Reset();                
                leavePlay(playicon, new RoutedEventArgs());
            }                       
        }


        /**
         * Highlights play when the mouse or hand hovers over them
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
            first = GetFirstSkeleton(e);

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

        //Returns the angle between the joints
        public static int AngleBetweenJoints(Joint j1, Joint j2, Joint j3)
        {
            double Angulo = 0;
            double shrhX = j1.Position.X - j2.Position.X;
            double shrhY = j1.Position.Y - j2.Position.Y;
            double shrhZ = j1.Position.Z - j2.Position.Z;
            double hsl = vectorNorm(shrhX, shrhY, shrhZ);
            double unrhX = j3.Position.X - j2.Position.X;
            double unrhY = j3.Position.Y - j2.Position.Y;
            double unrhZ = j3.Position.Z - j2.Position.Z;
            double hul = vectorNorm(unrhX, unrhY, unrhZ);
            double mhshu = shrhX * unrhX + shrhY * unrhY + shrhZ * unrhZ;
            double x = mhshu / (hul * hsl);
            if (x != Double.NaN)
            {
                if (-1 <= x && x <= 1)
                {
                    double angleRad = Math.Acos(x);
                    Angulo = angleRad * (180.0 / Math.PI);
                }
                else
                    Angulo = 0;


            }
            else
                Angulo = 0;


            return Convert.ToInt32(Angulo);

        }


        private static double vectorNorm(double x, double y, double z)
        {

            return Math.Sqrt(Math.Pow(x, 2) + Math.Pow(y, 2) + Math.Pow(z, 2));

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
            catch (NullReferenceException)
            {
                MessageBox.Show("No Kinect Attached");
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
            catch (NullReferenceException)
            {
                MessageBox.Show("No Kinect Attached");
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
            if (!videoPlaying)
            {               
                FitnessPlayer.Play();                
                videoPlaying = true;
                if (!timerInitialized)
                {
                    //initializeTimer();
                    timerInitialized = true;
                    startPlaybackSkeleton();
                    startVideoProgressBar();
                }                
            }
            else
            {                
                FitnessPlayer.Pause();
                videoPlaying = false;
                //pausePlaybackSkeleton();
            }
        }




    }
}
