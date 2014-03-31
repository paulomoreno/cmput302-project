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


   

    public partial class KinectWindow : Window
    {
        public KinectWindow()
        {
            InitializeComponent();
            initializeUI();
            initializeHoverChecker();
            this.WindowState = System.Windows.WindowState.Maximized;
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
        List<JointAngles> patientData;
        System.Windows.Threading.DispatcherTimer skeletonMatcherTimer;
        System.Windows.Threading.DispatcherTimer videoProgressBarTracker;
        System.Windows.Threading.DispatcherTimer dispatcherTimer;

        //Hand Position
        Rect rightHandPos;

        //Button Positions
        Rect playIconPos;
        Rect backButton;
        Rect bigPlayIcon;
        Rect doneButton;

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            kinectSensorChooser1.KinectSensorChanged += new DependencyPropertyChangedEventHandler(kinectSensorChooser1_KinectSensorChanged);          
        }

        private void loadExercise(String exercise)
        {
            string line;
            JointAngles ja = new JointAngles();
            loadedSkeleton = new List<JointAngles>();
            // Read the file and display it line by line.
            System.IO.StreamReader file =
               new System.IO.StreamReader("..\\..\\" + exercise);
            while ((line = file.ReadLine()) != null)
            {
                if(line.Contains("LSA"))
                {
                    ja.leftShoulder = Convert.ToInt32(file.ReadLine());
                }
                else if (line.Contains("RSA"))
                {
                    ja.rightShoulder = Convert.ToInt32(file.ReadLine());
                }
                else if (line.Contains("LEA"))
                {
                    ja.leftElbow = Convert.ToInt32(file.ReadLine());
                }
                else if (line.Contains("LHA"))
                {
                    ja.leftHip = Convert.ToInt32(file.ReadLine());
                }
                else if (line.Contains("LKA"))
                {
                    ja.leftKnee = Convert.ToInt32(file.ReadLine());
                }
                else if (line.Contains("REA"))
                {
                    ja.rightElbow = Convert.ToInt32(file.ReadLine());
                }
                else if (line.Contains("RHA"))
                {
                    ja.rightHip = Convert.ToInt32(file.ReadLine());
                }
                else if (line.Contains("RKA"))
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

        /**
         * Initializes the timer to check 20 times / second
         * if the hand is hovering over a button
         */
        void initializeHoverChecker()
        {
            dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
            //Timer to check for hand positions
            dispatcherTimer.Tick += new EventHandler(checkHands);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, 50);
            dispatcherTimer.Start();
            
            hoverTimer = new Stopwatch();
        }


        /**
         * Starts playing back the skeleton and matching it against the user
         */
        private void startPlaybackSkeleton()
        {
            skeletonMatcherTimer = new System.Windows.Threading.DispatcherTimer();
            skeletonMatcherTimer.Tick += new EventHandler(matchSkeleton);
            skeletonMatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, 100);
            skeletonMatcherTimer.Start();
        }

        private void startPointsBarDecliner()
        {
            System.Windows.Threading.DispatcherTimer pointsBarDecliner;
            pointsBarDecliner = new System.Windows.Threading.DispatcherTimer();
            pointsBarDecliner.Tick += new EventHandler(declinePointsBar);
            pointsBarDecliner.Interval = new TimeSpan(0, 0, 0, 0, 34);
            pointsBarDecliner.Start();
        }


        /**
         * Slowly reduce points bar if player is not getting any points
         * so player knows if they are not doing well
         */
        private void declinePointsBar(object sender, EventArgs e)
        {
            numberOfPtsBar -= 5;
            setPoints();
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
            videoProgressBar.Width = (FitnessPlayer.Position.TotalSeconds / totalMovieTime) * 668;
        }

        /**
         * Checks if the loaded skeleton data matches the users skeleton 
         * periodically ten times every second
         */
        private void matchSkeleton(object sender, EventArgs e)
        {
            if (numberOfPtsBar > 100)
            {
                numberOfPtsBar = 100;
            }
            setPoints();
            //Get amount of seconds passed in video
            TimeSpan timePassedInVideo = FitnessPlayer.Position;
            int secondsPassedInVideo = Convert.ToInt16(timePassedInVideo.TotalMilliseconds / 100);
            //debugger.Text = secondsPassedInVideo.ToString();
            //Check if loaded skeleton at this point matches the users current data within +- 1 second of the video
            try
            {
                if (SkeletonMatchesCloselyEnough(loadedSkeleton.ElementAt(secondsPassedInVideo), secondsPassedInVideo))
                {
                    //debugger.Text = "Success";
                }
                else
                {
                    //debugger.Text = "Failure";
                }
            }
            catch (ArgumentOutOfRangeException)
            {
                debugger.Text = "done";
            }
        }

        /**
         * Checks if the skeleton data matches within certain
         * angles to the users.  If 80% of the skeleton matches 
         * within 20 degrees, then this returns true.
         */
        private bool SkeletonMatchesCloselyEnough(JointAngles ja, int secondsPassed)
        {
            patientData.Add(new JointAngles(1,1,1,1,1,1,1,1));
            int numberOfMatches = 0;
            if (first == null)
            {
                return false;
            }
            int leftElbow = AngleBetweenJoints(first.Joints[JointType.HandLeft], first.Joints[JointType.ElbowLeft], first.Joints[JointType.ShoulderLeft]);
            int rightElbow = AngleBetweenJoints(first.Joints[JointType.HandRight], first.Joints[JointType.ElbowRight], first.Joints[JointType.ShoulderRight]);
            int rightShoulder = AngleBetweenJoints(first.Joints[JointType.ElbowRight], first.Joints[JointType.ShoulderRight], first.Joints[JointType.ShoulderCenter]);          
            int leftShoulder = AngleBetweenJoints(first.Joints[JointType.ElbowLeft], first.Joints[JointType.ShoulderLeft], first.Joints[JointType.ShoulderCenter]);
            int leftHip = AngleBetweenJoints(first.Joints[JointType.ShoulderLeft], first.Joints[JointType.HipLeft], first.Joints[JointType.KneeLeft]);
            int rightHip = AngleBetweenJoints(first.Joints[JointType.ShoulderRight], first.Joints[JointType.HipRight], first.Joints[JointType.KneeRight]); 
            int leftKnee = AngleBetweenJoints(first.Joints[JointType.HipLeft], first.Joints[JointType.KneeLeft], first.Joints[JointType.FootLeft]);
            int rightKnee = AngleBetweenJoints(first.Joints[JointType.HipRight], first.Joints[JointType.KneeRight], first.Joints[JointType.FootRight]);
                
            //Check if patient's joint angle is within 20 degrees of the exercise
            if (leftElbow < (ja.leftElbow - 10) || leftElbow > (ja.leftElbow + 10))
            {
                patientData.Last().leftElbow = 0;
            }
            else
            {
                //Number of matches goes up 1
                numberOfMatches += 1;
                //Points are added to the progress bar
                numberOfPtsBar += 2;
                //Points are added 
                numberOfPts += 5;
                setPoints();
            }            
            if (rightElbow < (ja.rightElbow - 10) || rightElbow > (ja.rightElbow + 10))
            {
                patientData.Last().rightElbow = 0;
            }
            else
            {
                numberOfMatches += 1;
                numberOfPtsBar += 2;
                numberOfPts += 5;
                setPoints();
            }
            if (leftHip < (ja.leftHip - 10) || leftHip > (ja.leftHip + 10))
            {
                patientData.Last().leftHip = 0;
            }
            else
            {
                numberOfMatches += 1;
                numberOfPtsBar += 2;
                numberOfPts += 5;
                setPoints();
            }
            if (rightHip < (ja.rightHip - 10) || rightHip > (ja.rightHip + 10))
            {
                patientData.Last().rightHip = 0;
            }
            else
            {
                numberOfMatches += 1;
                numberOfPtsBar += 2;
                numberOfPts += 5;
                setPoints();
            }
            if (rightKnee < (ja.rightKnee - 10) || rightKnee > (ja.rightKnee + 10))
            {
                patientData.Last().rightKnee = 0;
            }
            else
            {
                numberOfMatches += 1;
                numberOfPtsBar += 2;
                numberOfPts += 5;
                setPoints();
            }
            if (leftKnee < (ja.leftKnee - 10) || leftKnee > (ja.leftKnee + 10))
            {
                patientData.Last().leftKnee = 0;
            }
            else
            {
                numberOfMatches += 1;
                numberOfPtsBar += 2;
                numberOfPts += 5;
                setPoints();
            }
            if (rightShoulder < (ja.rightShoulder - 10) || rightShoulder > (ja.rightShoulder + 10))
            {
                patientData.Last().rightShoulder = 0;
            }
            else
            {
                numberOfMatches += 1;
                numberOfPtsBar += 2;
                numberOfPts += 5;
                setPoints();
            }
            if (leftShoulder < (ja.leftShoulder - 10) || leftShoulder > (ja.leftShoulder + 10))
            {
                patientData.Last().leftShoulder = 0;
            }
            else
            {
                numberOfMatches += 1;
                numberOfPtsBar += 2;
                numberOfPts += 5;
                setPoints();
            }
            debugger.Text = "Left Elbow:" + patientData.Last().leftElbow + "\nLeft Shoulder:" + patientData.Last().leftShoulder + "\nLeft Hip:" + patientData.Last().leftHip + "\nKnee:" + patientData.Last().leftKnee;
            
            if (numberOfMatches >= 6)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /**
         * Sets the points to the progress bar and number of points
         */
        void setPoints()
        {            
            points.Text = numberOfPts + "Pts.";

            pointsBar.Value = numberOfPtsBar;

            //Change color of the Points Bar depending on how many points they have
            if (numberOfPtsBar > 80)
            {
                pointsBar.Foreground = Brushes.Green;
            }
            else if (numberOfPtsBar > 60)
            {
                pointsBar.Foreground = Brushes.GreenYellow;
            }
            else if (numberOfPtsBar > 40)
            {
                pointsBar.Foreground = Brushes.Yellow;
            }
            else if (numberOfPtsBar > 20)
            {
                pointsBar.Foreground = Brushes.Orange;
            }
            else
            {
                pointsBar.Foreground = Brushes.Red;
            }
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
            patientData = new List<JointAngles>();
          
            //Get positions of buttons
            rightHandPos = new Rect();
            rightHandPos.Location = new Point(Canvas.GetLeft(rightHand), Canvas.GetTop(rightHand));
            rightHandPos.Size = new Size(rightHand.Width, rightHand.Height);
            playIconPos = new Rect();
            playIconPos.Location = new Point(Canvas.GetLeft(playicon), Canvas.GetTop(playicon));
            playIconPos.Size = new Size(playicon.Width, playicon.Height);
            backButton = new Rect();
            backButton.Location = new Point(Grid.GetRow(backButtonImg), Grid.GetColumn(backButtonImg));
            backButton.Size = new Size(backButtonImg.Width, backButtonImg.Height);
            bigPlayIcon = new Rect();
            bigPlayIcon.Location = new Point(Canvas.GetLeft(bigPlayIconImg), Canvas.GetTop(bigPlayIconImg));
            bigPlayIcon.Size = new Size(bigPlayIconImg.Width, bigPlayIconImg.Height);
            doneButton = new Rect();
            doneButton.Location = new Point(-900, -900);

            //Set video source for video player
            LoadVideo();
            //Get Length of Video
            FitnessPlayer.MediaOpened += new System.Windows.RoutedEventHandler(media_MediaOpened);
        }

        private void LoadVideo()
        {
            string line;
            // Read the file and display it line by line.
            System.IO.StreamReader file =
               new System.IO.StreamReader("..\\..\\FitnessVideos\\video.txt");
            while ((line = file.ReadLine()) != null)
            {
                if (line.Contains("warmUp"))
                {
                    loadExercise("FitnessVideos\\WarmUp5Min\\exercise.txt");
                    FitnessPlayer.Source = new Uri("..\\..\\FitnessVideos\\WarmUp5Min\\warmUpVideo.mp4", UriKind.Relative);
                }
                else if (line.Contains("moderate"))
                {
                    loadExercise("FitnessVideos\\ModerateCardio5Min\\exercise.txt");
                    FitnessPlayer.Source = new Uri("..\\..\\FitnessVideos\\ModerateCardio5Min\\moderateVideo.mp4", UriKind.Relative);
                }
                else if (line.Contains("intense"))
                {
                    loadExercise("FitnessVideos\\IntenseCardio5Min\\exercise.txt");
                    FitnessPlayer.Source = new Uri("..\\..\\FitnessVideos\\IntenseCardio5Min\\intenseVideo.mp4", UriKind.Relative);
                }
            }

            file.Close();

            // Suspend the screen.
            Console.ReadLine();
        }

        //Find length of video
        void media_MediaOpened(object sender, System.Windows.RoutedEventArgs e)
        {
            totalMovieTime = FitnessPlayer.NaturalDuration.TimeSpan.TotalSeconds;
            debugger2.Text = totalMovieTime.ToString();
        }

        

        /**
        * Checks to see if hands are hovering over a button
        */
        private void checkHands(object sender, EventArgs e)
        {
            
            if (rightHandPos.IntersectsWith(playIconPos))
            {
                hoverTimer.Start();
                hoverButton(playicon, new RoutedEventArgs());

                //Set progress bar to increase on hands to indicate if hand is hovering on button                
                    setHandProgressBar(false, hoverTimer.ElapsedMilliseconds);                
                
                //Check if hand has been hovering on target for 2 seconds or more   
                if (hoverTimer.ElapsedMilliseconds >= 2000)
                {
                    //Presses the play button
                    btnPlay_Click(sender, new RoutedEventArgs());
                    //Resets hoverTimer
                    hoverTimer.Reset();
                }
            }
            else if (rightHandPos.IntersectsWith(backButton))
            {
                hoverTimer.Start();
                hoverButton(backButtonImg, new RoutedEventArgs());

                //Set progress bar to increase on hands to indicate if hand is hovering on button
                    setHandProgressBar(false, hoverTimer.ElapsedMilliseconds);

                //Check if hand has been hovering on target for 1 second or more   
                if (hoverTimer.ElapsedMilliseconds >= 2000)
                {
                        leavePage();
                        //Resets hoverTimer
                        hoverTimer.Reset();
                }
            }
            else if (rightHandPos.IntersectsWith(bigPlayIcon)  && !videoPlaying)
            {
                hoverTimer.Start();
                hoverButton(bigPlayIconImg, new RoutedEventArgs());

                //Set progress bar to increase on hands to indicate if hand is hovering on button
                setHandProgressBar(false, hoverTimer.ElapsedMilliseconds);

                //Check if hand has been hovering on target for 1 second or more   
                if (hoverTimer.ElapsedMilliseconds >= 2000)
                {
                    //Presses the play button
                    btnPlay_Click(sender, new RoutedEventArgs());
                    
                    //Remove Big Play Icon after it is clicked once 
                    bigPlayIcon.Size = new Size(0,0);
                    bigPlayIconImg.Height = 0;
                    bigPlayIconHoverImg.Height = 0;

                    //Resets hoverTimer
                    hoverTimer.Reset();
                }
            }
            else if (rightHandPos.IntersectsWith(doneButton))
            {
                hoverTimer.Start();
                hoverButton(doneButtonImg, new RoutedEventArgs());

                //Set progress bar to increase on hands to indicate if hand is hovering on button
                setHandProgressBar(false, hoverTimer.ElapsedMilliseconds);

                //Check if hand has been hovering on target for 1 second or more   
                if (hoverTimer.ElapsedMilliseconds >= 2000)
                {
                    goHome();
                    //Resets hoverTimer
                    hoverTimer.Reset();
                }
            }
            else  //If hand is not hovering on any button.  Reset timer.
            {
                resetHandProgressBars();
                hoverTimer.Reset();                
                leaveButton(playicon, new RoutedEventArgs());
                leaveButton(backButtonImg, new RoutedEventArgs());
                leaveButton(bigPlayIconImg, new RoutedEventArgs());
                leaveButton(doneButtonImg, new RoutedEventArgs());
            }                       
        }

        /**
         * Go back to the Select Level Page
         */
        private void leavePage()
        {
            if (videoPlaying)
            {
                FitnessPlayer.Stop();
                FitnessPlayer.Close();     
            }
            if (skeletonMatcherTimer != null)
            {
                skeletonMatcherTimer.Stop();
            }
            if (videoProgressBarTracker != null)
            {
                videoProgressBarTracker.Stop();
            }
            closing = true;           
            dispatcherTimer.Stop();
            StopKinect(kinectSensorChooser1.Kinect);
            SelectLevelWindow sw = new SelectLevelWindow();
            this.Close();
            sw.Show();
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

        /**
         * Highlights play when the mouse or hand hovers over them
         */
        private void hoverButton(object sender, RoutedEventArgs e)
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
            else if (i.Name.Equals(backButtonImg.Name))
            {
                backButtonImg.Opacity = 0;
                backButtonHoverImg.Opacity = 1;
            }
            else if (i.Name.Equals(bigPlayIconImg.Name))
            {
                bigPlayIconHoverImg.Opacity = 1;
                bigPlayIconImg.Opacity = 0;
            }
            else if (i.Name.Equals(doneButtonImg.Name))
            {
                doneButtonImg.Opacity = 0;
                doneButtonHoverImg.Opacity = 1;
            }
            
        }

        /**
         * Stops highlighting the images when the mouse leaves
         */
        private void leaveButton(object sender, RoutedEventArgs e)
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
            else if (i.Name.Equals(backButtonImg.Name))
            {
                backButtonImg.Opacity = 1;
                backButtonHoverImg.Opacity = 0;
            }
            else if (i.Name.Equals(bigPlayIconImg.Name))
            {
                bigPlayIconHoverImg.Opacity = 0;
                bigPlayIconImg.Opacity = 1;
            }
            else if (i.Name.Equals(doneButtonImg.Name))
            {
                doneButtonHoverImg.Opacity = 0;
                doneButtonImg.Opacity = 1;
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
                Smoothing = 0.7f,
                Correction = 0.3f,
                Prediction = 1.0f,
                JitterRadius = 1.0f,
                MaxDeviationRadius = 1.0f
            };
            sensor.SkeletonStream.Enable(parameters);

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
                    kinectSensorChooser1.Kinect == null || !kinectSensorChooser1.Kinect.IsRunning)
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
                    startPointsBarDecliner();
                }
                //Check if the Skeleton Matcher is running
                //If not, start it back again
                if (!skeletonMatcherTimer.IsEnabled)
                {
                    skeletonMatcherTimer.Start();
                }
            }
            else
            {                
                FitnessPlayer.Pause();
                videoPlaying = false;
                skeletonMatcherTimer.Stop();
            }
        }

        /**
         * Go back to the home screen
         */
        private void goHome()
        {
            if (videoPlaying)
            {
                FitnessPlayer.Stop();
                FitnessPlayer.Close();     
            }
            if (skeletonMatcherTimer != null)
            {
                skeletonMatcherTimer.Stop();
            }
            if (videoProgressBarTracker != null)
            {
                videoProgressBarTracker.Stop();
            }
            closing = true;           
            dispatcherTimer.Stop();
            StopKinect(kinectSensorChooser1.Kinect);
            StartupWindow sw = new StartupWindow();
            this.Close();
            sw.Show();        
        }

        /**
         * Fires when the video ends
         */
        private void videoEnd(object sender, RoutedEventArgs e)
        {
            //Create Done Button and remove other buttons
            createStatsView();
            //Compile the stats for the patient
            compileStats();
            //Display the stats
            showStats();
        }

        /**
         * Creates and removes appropriate buttons
         * to help display the statistics 
         * at the end of the patients exercise
         */
        private void createStatsView()
        {
            doneButton.Size = new Size(doneButtonImg.Width, doneButtonImg.Height);
            doneButton.Location = new Point(Canvas.GetLeft(doneButtonImg), Canvas.GetTop(doneButtonImg));
            playIconPos.Size = new Size(0, 0);
            backButton.Size = new Size(0, 0);
            bigPlayIcon.Size = new Size(0, 0);
        }

        /**
         * Compile the patients stats on how they did 
         * with the exercise
         */
        private void compileStats()
        {
            double leftElbow = 0;
            double rightElbow = 0;
            double leftShoulder = 0;
            double rightShoulder = 0;
            double leftHip = 0;
            double rightHip = 0;
            double leftKnee = 0;
            double rightKnee = 0;

            //Get Number of corrrect comparisons for each joint
            for (int i = 0; i < patientData.Count(); i++)
            {
                leftElbow += patientData.ElementAt(i).leftElbow;
                rightElbow += patientData.ElementAt(i).rightElbow;
                leftShoulder += patientData.ElementAt(i).leftShoulder;
                rightShoulder += patientData.ElementAt(i).rightShoulder;
                leftHip += patientData.ElementAt(i).leftHip;
                rightHip += patientData.ElementAt(i).rightHip;
                leftKnee += patientData.ElementAt(i).leftKnee;
                rightKnee += patientData.ElementAt(i).rightKnee;
            }            

            //Get Percentage of correct comparisons for each joint
            double totalComparisons = patientData.Count();
            int leftElbowStat = Convert.ToInt16(Math.Round((leftElbow / totalComparisons * 100), 0));
            int rightElbowStat = Convert.ToInt16(Math.Round((rightElbow / totalComparisons * 100), 0));
            int leftShoulderStat = Convert.ToInt16(Math.Round((leftShoulder / totalComparisons * 100), 0));
            int rightShoulderStat = Convert.ToInt16(Math.Round((rightShoulder / totalComparisons * 100), 0));
            int leftHipStat = Convert.ToInt16(Math.Round((leftHip / totalComparisons * 100), 0));
            int rightHipStat = Convert.ToInt16(Math.Round((rightHip / totalComparisons * 100), 0));
            int leftKneeStat = Convert.ToInt16(Math.Round((leftKnee / totalComparisons * 100), 0));
            int rightKneeStat = Convert.ToInt16(Math.Round((rightKnee / totalComparisons * 100), 0));
            
            // Put this into the Stats box at the end of the video
            statsBox.Text = "Left Elbow: " + leftElbowStat +
                "%\nRight Elbow: " + rightElbowStat +
                "%\nLeft Shoulder: " + leftShoulderStat +
                "%\nRight Shoulder: " + rightShoulderStat +
                "%\nLeft Hip: " + leftHipStat +
                "%\nRight Hip: " + rightHipStat +
                "%\nLeft Knee: " + leftKneeStat +
                "%\nRight Knee: " + rightKneeStat + "%";
        }

        /**
         * Show the patients stats
         */
        private void showStats()
        {
            statsBackground.Opacity = 1;
            statsBox.Opacity = 1;
            statsTitle.Opacity = 1;
            doneButtonImg.Opacity = 1;
            doneButtonHoverImg.Opacity = 0;
            Canvas.SetZIndex(statsBackground, 9);
            Canvas.SetZIndex(statsBox, 9);
            Canvas.SetZIndex(statsTitle, 9);
            Canvas.SetZIndex(doneButtonImg, 9);
            Canvas.SetZIndex(doneButtonHoverImg, 9);
        }
    }
}
