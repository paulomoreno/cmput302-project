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
            //InitializeAudioCommands();
            this.WindowState = System.Windows.WindowState.Maximized;
        }
        
        bool closing = false;
        bool videoPlaying;
        bool timerInitialized;
        
        //Kinect Variables
        const int skeletonCount = 6;
        Skeleton[] allSkeletons = new Skeleton[skeletonCount];
        KinectSensor ksensor;
        double numberOfPts;
        int numberOfPtsBar;
        double totalMovieTime;
        Stopwatch hoverTimer;
        Stopwatch standingStillTimer;
        Random r;
        Skeleton first;

        //Lists for data collection
        List<Joint> previousFrameJoints;
        List<JointAngles> loadedSkeletonAngles;
        List<JointSpeeds> loadedSkeletonSpeeds;
        List<JointAngles> patientAnglesData;
        List<JointSpeeds> patientSpeedData;

        //Audio Command Listener
        //AudioCommands myCommands;

        //Timers for matching skeleton and tracking video progress
        System.Windows.Threading.DispatcherTimer skeletonMatcherTimer;
        System.Windows.Threading.DispatcherTimer videoProgressBarTracker;
        System.Windows.Threading.DispatcherTimer dispatcherTimer;
        System.Windows.Threading.DispatcherTimer accuracyChecker;

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
            //Set the Video Progress Bar Width to Zero 
            videoProgressBar.Width = 0;
            //Data to record and display after
            patientAnglesData = new List<JointAngles>();
            patientSpeedData = new List<JointSpeeds>();
            //Data for getting speed of joints
            previousFrameJoints = new List<Joint>();
            initializePreviousFrameJoints();
            //Stopwatch to check if patient is actually exercising
            standingStillTimer = new Stopwatch();

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

        /*
        private void InitializeAudioCommands()
        {
            myCommands = new AudioCommands(false, 0.5, "play", "pause", "back");//instantiate an AudioCommands object with the possible commands
            myCommands.setFunction("play", btnPlay_Click);//tell AudioCommands what to do when the speech "play" is recognized. The second parameter is a function
            myCommands.setFunction("pause", btnPlay_Click);
            myCommands.setFunction("back", leavePage);
        }
         */
         

        private void loadExercise(String exercise)
        {

            string line;

            String path = System.AppDomain.CurrentDomain.BaseDirectory;
            path = System.IO.Directory.GetParent(path).FullName;
            path = System.IO.Directory.GetParent(path).FullName;
            path = System.IO.Directory.GetParent(path).FullName;
            path = System.IO.Directory.GetParent(path).FullName;

            JointAngles ja = new JointAngles();
            JointSpeeds js = new JointSpeeds();
            loadedSkeletonAngles = new List<JointAngles>();
            loadedSkeletonSpeeds = new List<JointSpeeds>();
            // Read the file and display it line by line.
            System.IO.StreamReader file =
               new System.IO.StreamReader(path + exercise);
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
                else if (line.Contains("LHV"))
                {
                    js.leftHand = Convert.ToInt32(file.ReadLine());
                }
                else if (line.Contains("LSV"))
                {
                    js.leftShoulder = Convert.ToInt32(file.ReadLine());
                }
                else if (line.Contains("LHipV"))
                {
                    js.leftHip = Convert.ToInt32(file.ReadLine());
                }
                else if (line.Contains("LKV"))
                {
                    js.leftKnee = Convert.ToInt32(file.ReadLine());
                }
                else if (line.Contains("LFV"))
                {
                    js.leftFoot = Convert.ToInt32(file.ReadLine());
                }
                else if (line.Contains("RHV"))
                {
                    js.rightHand = Convert.ToInt32(file.ReadLine());
                }
                else if (line.Contains("RSV"))
                {
                    js.rightShoulder = Convert.ToInt32(file.ReadLine());
                }
                else if (line.Contains("RHipV"))
                {
                    js.rightHip = Convert.ToInt32(file.ReadLine());
                }
                else if (line.Contains("RKV"))
                {
                    js.rightKnee = Convert.ToInt32(file.ReadLine());
                }
                else if (line.Contains("RFV"))
                {
                    js.rightFoot = Convert.ToInt32(file.ReadLine());
                }
                else if (line.Contains("End"))
                {
                    loadedSkeletonAngles.Add(ja);
                    loadedSkeletonSpeeds.Add(js);
                    ja = new JointAngles();
                    js = new JointSpeeds();
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

                //Check Angles
                if (MatchSkeletonAngles(loadedSkeletonAngles.ElementAt(secondsPassedInVideo)))
                {
                    //debugger.Text = "Success";
                }
                else
                {
                    //debugger.Text = "Failure";
                }
                //Check Speeds
                if (MatchSkeletonSpeeds(loadedSkeletonSpeeds.ElementAt(secondsPassedInVideo)))
                {

                }
                else
                {
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
        private bool MatchSkeletonAngles(JointAngles ja)
        {
            patientAnglesData.Add(new JointAngles(1,1,1,1,1,1,1,1));
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
                
            //Check if patient's joint angle is within +- 20 degrees of the exercise
            if (leftElbow < (ja.leftElbow - 20) || leftElbow > (ja.leftElbow + 20))
            {
                patientAnglesData.Last().leftElbow = 0;
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
            if (rightElbow < (ja.rightElbow - 20) || rightElbow > (ja.rightElbow + 20))
            {
                patientAnglesData.Last().rightElbow = 0;
            }
            else
            {
                numberOfMatches += 1;
                numberOfPtsBar += 2;
                numberOfPts += 5;
                setPoints();
            }
            if (leftHip < (ja.leftHip - 20) || leftHip > (ja.leftHip + 20))
            {
                patientAnglesData.Last().leftHip = 0;
            }
            else
            {
                numberOfMatches += 1;
                numberOfPtsBar += 2;
                numberOfPts += 5;
                setPoints();
            }
            if (rightHip < (ja.rightHip - 20) || rightHip > (ja.rightHip + 20))
            {
                patientAnglesData.Last().rightHip = 0;
            }
            else
            {
                numberOfMatches += 1;
                numberOfPtsBar += 2;
                numberOfPts += 5;
                setPoints();
            }
            if (rightKnee < (ja.rightKnee - 20) || rightKnee > (ja.rightKnee + 20))
            {
                patientAnglesData.Last().rightKnee = 0;
            }
            else
            {
                numberOfMatches += 1;
                numberOfPtsBar += 2;
                numberOfPts += 5;
                setPoints();
            }
            if (leftKnee < (ja.leftKnee - 20) || leftKnee > (ja.leftKnee + 20))
            {
                patientAnglesData.Last().leftKnee = 0;
            }
            else
            {
                numberOfMatches += 1;
                numberOfPtsBar += 2;
                numberOfPts += 5;
                setPoints();
            }
            if (rightShoulder < (ja.rightShoulder - 20) || rightShoulder > (ja.rightShoulder + 20))
            {
                patientAnglesData.Last().rightShoulder = 0;
            }
            else
            {
                numberOfMatches += 1;
                numberOfPtsBar += 2;
                numberOfPts += 5;
                setPoints();
            }
            if (leftShoulder < (ja.leftShoulder - 20) || leftShoulder > (ja.leftShoulder + 20))
            {
                patientAnglesData.Last().leftShoulder = 0;
            }
            else
            {
                numberOfMatches += 1;
                numberOfPtsBar += 2;
                numberOfPts += 5;
                setPoints();
            }
            //debugger.Text = "Left Elbow:" + patientData.Last().leftElbow + "\nLeft Shoulder:" + patientData.Last().leftShoulder + "\nLeft Hip:" + patientData.Last().leftHip + "\nKnee:" + patientData.Last().leftKnee;
            
            //Check for Pause Gesture
            if ((leftShoulder < 100) && (rightShoulder < 100) && (leftElbow > 155) && (rightElbow > 155)
                && (leftHip > 155) && (rightHip > 155) && (leftKnee > 155) && (rightKnee > 155))
            {
                btnPlay_Click(new object(), new RoutedEventArgs());
            }

            //If Patient gets 6 or more angles correct
            //Consider it a success
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
         * Returns true if the speeds of the patients
         * match the recorded skeleton data sufficiently well
         */
        private bool MatchSkeletonSpeeds(JointSpeeds js)
        {
            patientSpeedData.Add(new JointSpeeds(1, 1, 1, 1, 1, 1, 1, 1, 1, 1));
            if (first == null)
            {
                return false;
            }
            int leftHand = SpeedOfJoint(first.Joints[JointType.HandLeft], previousFrameJoints.ElementAt(0));
            int rightHand = SpeedOfJoint(first.Joints[JointType.HandRight], previousFrameJoints.ElementAt(1));
            int leftShoulder = SpeedOfJoint(first.Joints[JointType.ShoulderLeft], previousFrameJoints.ElementAt(2));
            int rightShoulder = SpeedOfJoint(first.Joints[JointType.ShoulderRight], previousFrameJoints.ElementAt(3));
            int leftHip = SpeedOfJoint(first.Joints[JointType.HipLeft], previousFrameJoints.ElementAt(4));
            int rightHip = SpeedOfJoint(first.Joints[JointType.HipRight], previousFrameJoints.ElementAt(5));
            int leftKnee = SpeedOfJoint(first.Joints[JointType.KneeLeft], previousFrameJoints.ElementAt(6));
            int rightKnee = SpeedOfJoint(first.Joints[JointType.KneeRight], previousFrameJoints.ElementAt(7));
            int leftFoot = SpeedOfJoint(first.Joints[JointType.FootLeft], previousFrameJoints.ElementAt(8));
            int rightFoot = SpeedOfJoint(first.Joints[JointType.FootRight], previousFrameJoints.ElementAt(9));

            //Check to see if participant is standing still
            //If they are standing still for more than 10 seconds, start penalizing them
            if (leftHand < 5 && rightHand < 5 && leftFoot < 5 && rightFoot < 5 && leftHip < 5 && rightHip < 5)
            {
                //Start the timer if it has not started
                if (!standingStillTimer.IsRunning)
                {
                    standingStillTimer.Start();
                }
                //If the person has been standing still for 10 seconds or more
                //Give them 0 points for this frame
                if (standingStillTimer.ElapsedMilliseconds > 10000)
                {
                    debugger.Text = "Standing Still!";
                    patientSpeedData[patientSpeedData.Count - 1] = new JointSpeeds(0, 0, 0, 0, 0, 0, 0, 0, 0, 0);
                }
            }
                //If patient is moving at least a little
            else
            {
                debugger.Text = "Moving!";
                standingStillTimer.Reset();
            }

            if ((leftHand >= 5 && js.leftHand >= 5) || (leftHand < 5 && js.leftHand < 5) || ((leftHand > js.leftHand - 3) && (leftHand < js.leftHand + 3)) )
            {                
            }
            else if (leftHand >= 5)
            {
                //Half Points for effort
                patientSpeedData.Last().leftHand = 0.5;
            }
            else
            {
                //Give patient a 0 score for this frame for left hand speed
                patientSpeedData.Last().leftHand = 0;
            }
            if ((rightHand >= 5 && js.rightHand >= 5) || (rightHand <= 5 && js.rightHand <= 5) || ((rightHand > js.rightHand - 3) && (rightHand < js.rightHand + 3)) )
            {

            }
            else if (rightHand >= 5)
            {
                patientSpeedData.Last().rightHand = 0.5;
            }
            else
            {
                //Give patient a 0 score for this frame for right hand speed
                patientSpeedData.Last().rightHand = 0;
            }
            if ((leftShoulder >= 5 && js.leftShoulder >= 5) || (leftShoulder <= 5 && js.leftShoulder <= 5) || ((leftShoulder > js.leftShoulder - 3) && (leftShoulder < js.leftShoulder + 3)) )
            {                
            }
            else if (leftShoulder >= 5)
            {
                patientSpeedData.Last().leftShoulder = 0.5;
            }
            else
            {
                //Give patient a 0 score for this frame for left shoulder speed
                patientSpeedData.Last().leftShoulder = 0;
            }
            if ((rightShoulder >= 5 && js.rightShoulder >= 5) || (rightShoulder <= 5 && js.rightShoulder <= 5) || ((rightShoulder > js.rightShoulder - 3) && (rightShoulder < js.rightShoulder + 3)))
            {
            }
            else if (rightShoulder >= 5)
            {
                patientSpeedData.Last().rightShoulder = 0.5;
            }
            else
            {
                //Give patient a 0 score for this frame for right shoulder speed
                patientSpeedData.Last().rightShoulder = 0;
            }
            if ((leftHip >= 5 && js.leftHip >= 5) || (leftHip <= 5 && js.leftHip <= 5) || ((leftHip > js.leftHip - 3) && (leftHip < js.leftHip + 3)))
            {
            }
            else if (leftHip >= 5)
            {
                patientSpeedData.Last().leftHip = 0.5;
            }
            else
            {
                //Give patient a 0 score for this frame for left hip speed
                patientSpeedData.Last().leftHip = 0;
            }
            if ((rightHip >= 5 && js.rightHip >= 5) || (rightHip <= 5 && js.rightHip <= 5) || ((rightHip > js.rightHip - 3) && (rightHip < js.rightHip + 3)))
            {
                
            }
            else if (rightHip >= 5)
            {
                patientSpeedData.Last().rightHip = 0.5;
            }
            else
            {
                //Give patient a 0 score for this frame for right hip speed
                patientSpeedData.Last().rightHip = 0;
            }
            if ((leftKnee >= 5 && js.leftKnee >= 5) || (leftKnee <= 5 && js.leftKnee <= 5) || ((leftKnee > js.leftKnee - 3) && (leftKnee < js.leftKnee + 3)))
            {                
            }
            else if (leftKnee >= 5)
            {
                patientSpeedData.Last().leftKnee = 0.5;
            }
            else
            {
                //Give patient a 0 score for this frame for left Knee speed
                patientSpeedData.Last().leftKnee = 0;
            }
            if ((rightKnee >= 5 && js.rightKnee >= 5) || (rightKnee <= 5 && js.rightKnee <= 5) || ((rightKnee > js.rightKnee - 3) && (rightKnee < js.rightKnee + 3)))
            {

            }
            else if (rightKnee >= 5)
            {
                patientSpeedData.Last().rightKnee = 0.5;
            }
            else
            {
                //Give patient a 0 score for this frame for right Knee speed
                patientSpeedData.Last().rightKnee = 0;
            }
            if ((leftFoot >= 5 && js.leftFoot >= 5) || (leftFoot <= 5 && js.leftFoot <= 5) || ((leftFoot > js.leftFoot - 3) && (leftFoot < js.leftFoot + 3)))
            {
                //debugger.Foreground = Brushes.Green;
            }
            else if (leftFoot >= 5)
            {
                patientSpeedData.Last().leftFoot = 0.5;
            }
            else
            {
                //Give patient a 0 score for this frame for leftFoot speed
                patientSpeedData.Last().leftFoot = 0;
                //debugger.Foreground = Brushes.Red;
            }
            if ((rightFoot >= 5 && js.rightFoot >= 5) || (rightFoot <= 5 && js.rightFoot <= 5) || ((rightFoot > js.rightFoot - 3) && (rightFoot < js.rightFoot + 3)))
            {

            }
            else if (rightFoot >= 5)
            {
                patientSpeedData.Last().rightFoot = 0.5;
            }
            else
            {
                //Give patient a 0 score for this frame for right foot speed
                patientSpeedData.Last().rightFoot = 0;
            }

            previousFrameJoints[0] = first.Joints[JointType.HandLeft];
            previousFrameJoints[1] = first.Joints[JointType.HandRight];
            previousFrameJoints[2] = first.Joints[JointType.ShoulderLeft];
            previousFrameJoints[3] = first.Joints[JointType.ShoulderRight];
            previousFrameJoints[4] = first.Joints[JointType.HipLeft];
            previousFrameJoints[5] = first.Joints[JointType.HipRight];
            previousFrameJoints[6] = first.Joints[JointType.KneeLeft];
            previousFrameJoints[7] = first.Joints[JointType.KneeRight];
            previousFrameJoints[8] = first.Joints[JointType.FootLeft];
            previousFrameJoints[9] = first.Joints[JointType.FootRight];
            return true;
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

        

        private void initializePreviousFrameJoints()
        {
            SkeletonPoint sp = new SkeletonPoint();
            sp.X = 0;
            sp.Y = 0;
            sp.Z = 0;

            Joint leftFoot = new Joint();
            leftFoot.Position = sp;
            Joint rightFoot = new Joint();
            rightFoot.Position = sp;
            Joint leftShoulder = new Joint();
            leftShoulder.Position = sp;
            Joint rightShoulder = new Joint();
            rightShoulder.Position = sp;
            Joint leftHand = new Joint();
            leftHand.Position = sp;
            Joint rightHand = new Joint();
            rightHand.Position = sp;
            Joint leftKnee = new Joint();
            leftKnee.Position = sp;
            Joint rightKnee = new Joint();
            rightKnee.Position = sp;
            Joint leftHip = new Joint();
            leftHip.Position = sp;
            Joint rightHip = new Joint();
            rightHip.Position = sp;            
            
            previousFrameJoints.Add(leftHand);
            previousFrameJoints.Add(rightHand);
            previousFrameJoints.Add(leftShoulder);
            previousFrameJoints.Add(rightShoulder);
            previousFrameJoints.Add(leftHip);
            previousFrameJoints.Add(rightHip);
            previousFrameJoints.Add(leftKnee);
            previousFrameJoints.Add(rightKnee);
            previousFrameJoints.Add(leftFoot);
            previousFrameJoints.Add(rightFoot);
        }

        /*
         *Loads the exercise video 
         */
        private void LoadVideo()
        {
            string line;
            // Read the file and display it line by line.

            String path = System.AppDomain.CurrentDomain.BaseDirectory;
            path = System.IO.Directory.GetParent(path).FullName;
            path = System.IO.Directory.GetParent(path).FullName;
            path = System.IO.Directory.GetParent(path).FullName;
            path = System.IO.Directory.GetParent(path).FullName;


            System.IO.StreamReader file =
               new System.IO.StreamReader(path + "\\KinectFitness\\FitnessVideos\\video.txt");
            while ((line = file.ReadLine()) != null)
            {
                if (line.Contains("warmUp"))
                {
                    loadExercise("\\KinectFitness\\FitnessVideos\\WarmUp5Min\\exercise.txt");
                    FitnessPlayer.Source = new Uri(path + "\\KinectFitness\\FitnessVideos\\WarmUp5Min\\warmUpVideo.mp4", UriKind.Relative);
                }
                else if (line.Contains("moderate"))
                {
                    loadExercise("\\KinectFitness\\FitnessVideos\\ModerateCardio5Min\\exercise.txt");
                    FitnessPlayer.Source = new Uri(path + "\\KinectFitness\\FitnessVideos\\ModerateCardio5Min\\moderateVideo.mp4", UriKind.Relative);
                }
                else if (line.Contains("intense"))
                {
                    loadExercise("\\KinectFitness\\FitnessVideos\\IntenseCardio5Min\\exercise.txt");
                    FitnessPlayer.Source = new Uri(path + "\\KinectFitness\\FitnessVideos\\IntenseCardio5Min\\intenseVideo.mp4", UriKind.Relative);
                }
            }
            file.Close();

            // Suspend the screen.
            Console.ReadLine();
        }

        //Find length of video
        void media_MediaOpened(object sender, System.Windows.RoutedEventArgs e)
        {
            debugger.Text = "Video Opened";
            totalMovieTime = FitnessPlayer.NaturalDuration.TimeSpan.TotalSeconds;
        }

        

        /**
        * Checks to see if hands are hovering over a button
         * And if they are hovering on a button for 2 seconds
         * take the appropriate action
        */
        private void checkHands(object sender, EventArgs e)
        {
            //Check if hand intersects with the play button
            if (rightHandPos.IntersectsWith(playIconPos))
            {
                //Timer to check how long hand is hovering on button
                hoverTimer.Start();
                //Method to change icon's look as it is being hovered over
                hoverButton(playicon, new RoutedEventArgs());
                //Set progress bar to increase on hands to indicate if hand is hovering on button                
                    setHandProgressBar(false, hoverTimer.ElapsedMilliseconds);                
                
                //Check if hand has been hovering on target for 2 seconds or more   
                if (hoverTimer.ElapsedMilliseconds >= 2000)
                {
                    //Presses the play button
                    btnPlay_Click(sender, new RoutedEventArgs());

                    //Removes the big play button
                    removeBigPlayButton();

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
                        leavePage(new object(), new RoutedEventArgs());
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
         * Removes the big play button
         */
        private void removeBigPlayButton()
        {
                    bigPlayIcon.Size = new Size(0,0);
                    bigPlayIconImg.Height = 0;
                    bigPlayIconHoverImg.Height = 0;
        }

        /**
         * Go back to the Select Level Page
         */
        private void leavePage(object sender, RoutedEventArgs e)
        {
            if (videoPlaying)
            {           
                FitnessPlayer.Stop(); 
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
            StopKinect(kinectSensorChooser1.Kinect);
            dispatcherTimer.Stop();            
            //myCommands.StopSpeechRecognition();
            SelectLevelWindow sw = new SelectLevelWindow();
            this.Close();
            sw.Show();
            hoverTimer.Reset();            
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

        /**
         * Returns the speed of the joint
         */
        private int SpeedOfJoint(Joint second, Joint first)
        {
            double dx, dy, dz, speed;
            dx = second.Position.X - first.Position.X;
            dy = second.Position.Y - first.Position.Y;
            dz = second.Position.Z - first.Position.Z;

            speed = Math.Sqrt((dx * dx) + (dy * dy) + (dz * dz)) * 100;
            int speedRounded = Convert.ToInt32(speed);
            return speedRounded;
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
                stopHoverChecker();
                
                if (!timerInitialized)
                { 

                    timerInitialized = true;
                    //Start playing back the recorded skeleton
                    startPlaybackSkeleton();
                    //Start the video progress bar
                    startVideoProgressBar();
                    //Start the declining points bar
                    startPointsBarDecliner();
                    //Remove Big Play Button
                    removeBigPlayButton();
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
                //Pause the Fitness Player
                FitnessPlayer.Pause();
                videoPlaying = false;

                //Video is paused, so allow user to use hand
                //to click buttons again
                startHoverChecker();

                //Stop matching the skeleton since video is not playing
                skeletonMatcherTimer.Stop();
            }
        }

        /**
         * Stops the hover checker
         */
        private void stopHoverChecker()
        {
            //Stop the hover checker
            dispatcherTimer.Stop();
            //Hides the hand and the progress bar
            rightHand.Opacity = 0;
            rightHandProgressBar.Opacity = 0;
        }

        /**
         * Starts the hover checker
         */
        private void startHoverChecker()
        {
            //Starts the hover checker again
            dispatcherTimer.Start();

            //Makes the hand and progress bar visible again
            rightHand.Opacity = 1;
            rightHandProgressBar.Opacity = 1;
        }

        /**
         * Go back to the home screen
         */
        private void goHome()
        {
            if (videoPlaying)
            {
                FitnessPlayer.Stop();
                FitnessPlayer.Source = null;
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
            //myCommands.StopSpeechRecognition();
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
            //Start up the hover checker
            startHoverChecker();
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
            playIconPos.Location = new Point(-200, -200);
            backButton.Size = new Size(0, 0);
            backButton.Location = new Point(-200, -200);
            bigPlayIcon.Size = new Size(0, 0);
            bigPlayIcon.Location = new Point(-200, -200);
        }

        /**
         * Compile the patients stats on how they did 
         * with the exercise
         */
        private void compileStats()
        {
            double leftElbowAngle = 0;
            double rightElbowAngle = 0;
            double leftShoulderAngle = 0;
            double rightShoulderAngle = 0;
            double leftHipAngle = 0;
            double rightHipAngle = 0;
            double leftKneeAngle = 0;
            double rightKneeAngle = 0;

            double leftHandSpeed = 0;
            double rightHandSpeed = 0;
            double leftShoulderSpeed = 0;
            double rightShoulderSpeed = 0;
            double leftHipSpeed = 0;
            double rightHipSpeed = 0;
            double leftKneeSpeed = 0;
            double rightKneeSpeed = 0;
            double leftFootSpeed = 0;
            double rightFootSpeed = 0;

            //Get Number of corrrect comparisons for each joint
            for (int i = 0; i < patientAnglesData.Count(); i++)
            {
                leftElbowAngle += patientAnglesData.ElementAt(i).leftElbow;
                rightElbowAngle += patientAnglesData.ElementAt(i).rightElbow;
                leftShoulderAngle += patientAnglesData.ElementAt(i).leftShoulder;
                rightShoulderAngle += patientAnglesData.ElementAt(i).rightShoulder;
                leftHipAngle += patientAnglesData.ElementAt(i).leftHip;
                rightHipAngle += patientAnglesData.ElementAt(i).rightHip;
                leftKneeAngle += patientAnglesData.ElementAt(i).leftKnee;
                rightKneeAngle += patientAnglesData.ElementAt(i).rightKnee;
            }

            //Get Number of corrrect comparisons for each joint
            for (int i = 0; i < patientSpeedData.Count(); i++)
            {
                leftHandSpeed += patientSpeedData.ElementAt(i).leftHand;
                rightHandSpeed += patientSpeedData.ElementAt(i).rightHand;
                leftShoulderSpeed += patientSpeedData.ElementAt(i).leftShoulder;
                rightShoulderSpeed += patientSpeedData.ElementAt(i).rightShoulder;
                leftHipSpeed += patientSpeedData.ElementAt(i).leftHip;
                rightHipSpeed += patientSpeedData.ElementAt(i).rightHip;
                leftKneeSpeed += patientSpeedData.ElementAt(i).leftKnee;
                rightKneeSpeed += patientSpeedData.ElementAt(i).rightKnee;
                leftFootSpeed += patientSpeedData.ElementAt(i).leftFoot;
                rightFootSpeed += patientSpeedData.ElementAt(i).rightFoot;
            } 

            //Get Percentage of correct comparisons for each joint
            double totalAngleComparisons = patientAnglesData.Count();
            int leftElbowAngleStat = Convert.ToInt16(Math.Round((leftElbowAngle / totalAngleComparisons * 100), 0));
            int rightElbowAngleStat = Convert.ToInt16(Math.Round((rightElbowAngle / totalAngleComparisons * 100), 0));
            int leftShoulderAngleStat = Convert.ToInt16(Math.Round((leftShoulderAngle / totalAngleComparisons * 100), 0));
            int rightShoulderAngleStat = Convert.ToInt16(Math.Round((rightShoulderAngle / totalAngleComparisons * 100), 0));
            int leftHipAngleStat = Convert.ToInt16(Math.Round((leftHipAngle / totalAngleComparisons * 100), 0));
            int rightHipAngleStat = Convert.ToInt16(Math.Round((rightHipAngle / totalAngleComparisons * 100), 0));
            int leftKneeAngleStat = Convert.ToInt16(Math.Round((leftKneeAngle / totalAngleComparisons * 100), 0));
            int rightKneeAngleStat = Convert.ToInt16(Math.Round((rightKneeAngle / totalAngleComparisons * 100), 0));

            double totalSpeedComparisons = patientSpeedData.Count();
            int leftHandSpeedStat = Convert.ToInt16(Math.Round((leftHandSpeed / totalSpeedComparisons * 100), 0));
            int rightHandSpeedStat = Convert.ToInt16(Math.Round((rightHandSpeed / totalSpeedComparisons * 100), 0));
            int leftShoulderSpeedStat = Convert.ToInt16(Math.Round((leftShoulderSpeed / totalSpeedComparisons * 100), 0));
            int rightShoulderSpeedStat = Convert.ToInt16(Math.Round((rightShoulderSpeed / totalSpeedComparisons * 100), 0));
            int leftHipSpeedStat = Convert.ToInt16(Math.Round((leftHipSpeed / totalSpeedComparisons * 100), 0));
            int rightHipSpeedStat = Convert.ToInt16(Math.Round((rightHipSpeed / totalSpeedComparisons * 100), 0));
            int leftKneeSpeedStat = Convert.ToInt16(Math.Round((leftKneeSpeed / totalSpeedComparisons * 100), 0));
            int rightKneeSpeedStat = Convert.ToInt16(Math.Round((rightKneeSpeed / totalSpeedComparisons * 100), 0));
            int leftFootSpeedStat = Convert.ToInt16(Math.Round((leftFootSpeed / totalSpeedComparisons * 100), 0));
            int rightFootSpeedStat = Convert.ToInt16(Math.Round((rightFootSpeed / totalSpeedComparisons * 100), 0));

            
            // Put this into the Stats box at the end of the video
            angleStatsBox.Text = "Angle Accuracy \nLeft Elbow: " + leftElbowAngleStat +
                "%\nRight Elbow: " + rightElbowAngleStat +
                "%\nLeft Shoulder: " + leftShoulderAngleStat +
                "%\nRight Shoulder: " + rightShoulderAngleStat +
                "%\nLeft Hip: " + leftHipAngleStat +
                "%\nRight Hip: " + rightHipAngleStat +
                "%\nLeft Knee: " + leftKneeAngleStat +
                "%\nRight Knee: " + rightKneeAngleStat + "%";

            speedStatsBox.Text = "Speed Accuracy \nLeft Hand: " + leftHandSpeedStat +
                "%\nRight Hand: " + rightHandSpeedStat +
                "%\nLeft Shoulder: " + leftShoulderSpeedStat +
                "%\nRight Shoulder: " + rightShoulderSpeedStat +
                "%\nLeft Hip: " + leftHipSpeedStat +
                "%\nRight Hip: " + rightHipSpeedStat +
                "%\nLeft Knee: " + leftKneeSpeedStat +
                "%\nRight Knee: " + rightKneeSpeedStat + 
                "%\nLeft Foot: " + leftFootSpeedStat + 
                "%\nRight Foot: " + rightFootSpeedStat + "%";
        }

        /**
         * Show the patients stats
         */
        private void showStats()
        {
            statsBackground.Opacity = 1;
            angleStatsBox.Opacity = 1;
            speedStatsBox.Opacity = 1;
            statsTitle.Opacity = 1;
            doneButtonImg.Opacity = 1;
            doneButtonHoverImg.Opacity = 0;
            Canvas.SetZIndex(statsBackground, 9);
            Canvas.SetZIndex(angleStatsBox, 9);
            Canvas.SetZIndex(speedStatsBox, 9);
            Canvas.SetZIndex(statsTitle, 9);
            Canvas.SetZIndex(doneButtonImg, 9);
            Canvas.SetZIndex(doneButtonHoverImg, 9);
        }


        private void backButton_Click(object sender, MouseButtonEventArgs e)
        {
            leavePage(new object(), new RoutedEventArgs());
        }

    }
}
