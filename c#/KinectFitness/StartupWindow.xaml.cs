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
using System.Threading;
using System.Media;

namespace KinectFitness
{
    public partial class StartupWindow : Window
    {
        //Global Variables for Kinect
        bool closing = false;
        const int skeletonCount = 6;
        Skeleton[] allSkeletons = new Skeleton[skeletonCount];
        KinectSensor ksensor;
        Skeleton first;

        Stopwatch hoverTimer;
        System.Windows.Threading.DispatcherTimer dispatcherTimer;
        System.Windows.Threading.DispatcherTimer canvasAnimator;


        //Sound effects
        SoundPlayer hoverSound;
        SoundPlayer clickSound;
        SoundPlayer goBackSound;
        SoundPlayer doneSound;
        SoundPlayer suggestionSound;

        //Start Up Buttons
        Rect playButton;
        Rect recordButton;
        Rect quitButton;
        Rect optionsButton;
        Rect rightHandPos;

        //Select Level Buttons
        Rect warmUp;
        Rect moderateCardio;
        Rect intenseCardio;
        Rect selectLevelBackButton;

        //Kinect Buttons
        Rect playIconPos;
        Rect kinectBackButton;
        Rect bigPlayIcon;
        Rect doneButton;

        //Kinect Data Stuff
        bool videoPlaying;
        bool timerInitialized;
        //boolean to indicate if the patient is standing still
        bool isStandingStill;

        double numberOfPts;
        double totalMovieTime;
        Stopwatch standingStillTimer;
        Random r;

        //Lists for data collection
        List<Joint> previousFrameJoints;
        List<JointAngles> loadedSkeletonAngles;
        List<JointSpeeds> loadedSkeletonSpeeds;
        List<JointAngles> patientAnglesData;
        List<JointSpeeds> patientSpeedData;


        //Timers for matching skeleton and tracking video progress
        System.Windows.Threading.DispatcherTimer skeletonMatcherTimer;
        System.Windows.Threading.DispatcherTimer videoProgressBarTracker;
        System.Windows.Threading.DispatcherTimer accuracyChecker;
        System.Windows.Threading.DispatcherTimer suggestionAnimator;
        System.Windows.Threading.DispatcherTimer doneAnimator;  
     

        //Controller variables
        private Controller control;
        private Thread newThread;
        int buttons;
        bool startUpScreenIsActive;
        bool selectLevelScreenIsActive = false;
        bool kinectScreenIsActive = false;
        bool doneScreenIsActive = false;

        //List of buttons that user can press
        List<Rect> buttonsList = new List<Rect>();
        //List of corresponding actions that each button performs
        List<Action<object, RoutedEventArgs>> actionsList = new List<Action<object, RoutedEventArgs>>();

        public StartupWindow()
        {
            //Kinect2JavaClient client = new Kinect2JavaClient();
            //client.receiveData();

            control = new Controller();

            InitializeComponent();
            InitializeUI();
            InitializeStartUpUI();

           //Kinect2JavaClient data = new Kinect2JavaClient("Start");
            //data.sendFlag();


            var navWindow = Window.GetWindow(this) as NavigationWindow;
            if (navWindow != null) navWindow.ShowsNavigationUI = false;
            this.WindowState = System.Windows.WindowState.Maximized;            
        }


        /****************************************************
         * Controller Code
         ***************************************************/
        // 0 == play; 1 == options; 2 == record; 3 == quit 
        private void updateHighlights(int result) {
           
            /**************
             * Start Up Screen
             * ************/
            if (startUpScreenIsActive)
            {
                
                //button play
                if (buttons == 0)
                {                    
                    if (result == 0)
                    {
                        hoverSound.Play();
                        playborder.Opacity = 0;
                        optionsborder.Opacity = 1;
                        buttons = 1;
                    }
                    else if (result == 9000)
                    {
                        hoverSound.Play();
                        playborder.Opacity = 0;
                        quitborder.Opacity = 1;
                        buttons = 3;
                    }
                }

                //button options
                if (buttons == 1)
                {
                    if (result == 18000)
                    {
                        hoverSound.Play();
                        playborder.Opacity = 1;
                        optionsborder.Opacity = 0;
                        buttons = 0;
                    }
                    else if (result == 9000)
                    {
                        hoverSound.Play();
                        optionsborder.Opacity = 0;
                        recordborder.Opacity = 1;
                        buttons = 2;
                    }
                }

                //button record
                if (buttons == 2)
                {
                    if (result == 27000)
                    {
                        hoverSound.Play();
                        recordborder.Opacity = 0;
                        optionsborder.Opacity = 1;
                        buttons = 1;
                    }
                    else if (result == 18000)
                    {
                        hoverSound.Play();
                        recordborder.Opacity = 0;
                        quitborder.Opacity = 1;
                        buttons = 3;
                    }
                }

                //button quit
                if (buttons == 3)
                {
                    if (result == 0)
                    {
                        hoverSound.Play();
                        quitborder.Opacity = 0;
                        recordborder.Opacity = 1;
                        buttons = 2;
                    }
                    else if (result == 27000)
                    {
                        hoverSound.Play();
                        quitborder.Opacity = 0;
                        playborder.Opacity = 1;
                        buttons = 0;
                    }
                }
            }
            else if (selectLevelScreenIsActive)
            {
                if (buttons == 0)
                {
                    if (result == 9000)
                    {
                        hoverSound.Play();
                        warmUpImgBorder.Opacity = 0;
                        moderateImgBorder.Opacity = 1;
                        buttons = 1;
                        Thread.Sleep(250);
                    }
                }
                else if (buttons == 1)
                {
                    if (result == 9000)
                    {
                        hoverSound.Play();
                        moderateImgBorder.Opacity = 0;
                        intenseImgBorder.Opacity = 1;
                        buttons = 2;
                        Thread.Sleep(250);
                    }
                    else if (result == 27000)
                    {
                        hoverSound.Play();
                        moderateImgBorder.Opacity = 0;
                        warmUpImgBorder.Opacity = 1;
                        buttons = 0;
                        Thread.Sleep(250);
                    }
                }
                else if (buttons == 2)
                {
                    if (result == 27000)
                    {
                        hoverSound.Play();
                        intenseImgBorder.Opacity = 0;
                        moderateImgBorder.Opacity = 1;
                        buttons = 1;
                        Thread.Sleep(250);
                    }
                }
            }
            else if (kinectScreenIsActive)
            {

            }

        }

        private void checkButtonPressed(int button_number, bool button, bool button_2) 
        {
            if (startUpScreenIsActive)
            {

                if (button_2 == true)
                {

                }
                //Console.WriteLine("Hey, I'm here!");
                if (button == true)
                {
                    //Console.WriteLine("HEy, I'm here as well!: " + button_number);
                    switch (button_number)
                    {

                        case 0:
                            clickSound.Play();
                            Button_Play(new object(), new RoutedEventArgs());
                            break;

                        case 1:
                            clickSound.Play();
                            Button_Options(new object(), new RoutedEventArgs());
                            break;

                        case 2:
                            clickSound.Play();
                            Button_Record(new object(), new RoutedEventArgs());
                            break;
                        case 3:
                            goBackSound.Play();
                            Button_Quit(new object(), new RoutedEventArgs());
                            break;

                        default:
                            break;
                    }
                    Thread.Sleep(250);
                }
            }
            else if (selectLevelScreenIsActive)
            {
                if (button_2 == true)
                {
                    goBackSound.Play();
                    backButtonPressed(new object(), new RoutedEventArgs());
                    Thread.Sleep(250);
                }

                if (button == true)
                {
                    //Console.WriteLine("HEy, I'm here as well!: " + button_number);
                    switch (buttons)
                    {
                        case 0:
                            clickSound.Play();
                            warmUpWorkout(new object(), new RoutedEventArgs());
                            break;

                        case 1:
                            clickSound.Play();
                            moderateWorkout(new object(), new RoutedEventArgs());
                            break;

                        case 2:
                            clickSound.Play();
                            intenseWorkout(new object(), new RoutedEventArgs());
                            break;

                        default:
                            break;
                    }
                    Thread.Sleep(250);
                }
            }
            else if (kinectScreenIsActive)
            {
                if (button_2 == true)
                {
                    goBackSound.Play();
                    KinectButton_Back(new object(), new RoutedEventArgs());
                    Thread.Sleep(250);
                }
                else if (button == true)
                {
                    clickSound.Play();
                    btnPlay_Click(new object(), new RoutedEventArgs());
                    Thread.Sleep(150);
                }
            }
            else if (doneScreenIsActive)
                if (button == true)
                {
                    doneSound.Play();
                    goHome(new object(), new RoutedEventArgs());
                    Thread.Sleep(250);
                }
        }


        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            kinectSensorChooser1.KinectSensorChanged += new DependencyPropertyChangedEventHandler(kinectSensorChooser1_KinectSensorChanged);
        }


        private void InitializeUI()
        {
            loadBackground();

            //Check if controller is connected
            if (control.isConnected() == true)
            {
                InitializeHoverChecker(0);
                int result = 10;

                buttons = 0; // 0 == play; 1 == options; 2 == record; 3 == quit 
                playborder.Opacity = 1;

                newThread = new Thread(() =>
                {
                    control.updateStates();
                    while (true && control != null)
                    {
                        Application.Current.Dispatcher.Invoke((Action)(() =>
                        {

                            result = control.getPOV();
                            updateHighlights(result);
                            checkButtonPressed(buttons, control.getButton(0), control.getButton(1));
                            // Console.WriteLine(control.getButton());

                        }));

                    }

                });

                newThread.Start();
            }
            else InitializeHoverChecker(0);

            loadSounds();

            //Get position of hand
            rightHandPos = new Rect();
            rightHandPos.Size = new Size(rightHand.Width, rightHand.Height);

            //Set other screens to correct position so they are not seen
            SelectLevel.Margin = new Thickness(1500, 0, 0, 0);
            Kinect.Margin = new Thickness(3000, 0, 0, 0);
            Stats.Margin = new Thickness(4500, 0, 0, 0);
        }    
    
        /**
         * Loads the sound effects for buttons
         */
        private void loadSounds()
        {
            String path = System.AppDomain.CurrentDomain.BaseDirectory;
            path = System.IO.Directory.GetParent(path).FullName;
            path = System.IO.Directory.GetParent(path).FullName;
            path = System.IO.Directory.GetParent(path).FullName;
            path = System.IO.Directory.GetParent(path).FullName;

            hoverSound = new SoundPlayer(path + "\\KinectFitness\\hoverSound.wav");
            clickSound = new SoundPlayer(path + "\\KinectFitness\\clickSound.wav");
            goBackSound = new SoundPlayer(path + "\\KinectFitness\\goBackSound.wav");
            doneSound = new SoundPlayer(path + "\\KinectFitness\\doneSound.wav");
            suggestionSound = new SoundPlayer(path + "\\KinectFitness\\suggestionSound.wav");

        }

        /**
         * Initializer for StartUp UI
         */
        private void InitializeStartUpUI()
        {
            startUpScreenIsActive = true;
            rightHandProgressBar.Width = 0;

            

            //Get positions of buttons
            playButton = new Rect();
            playButton.Location = new Point(Canvas.GetLeft(playButtonImg), Canvas.GetTop(playButtonImg));
            playButton.Size = new Size(playButtonImg.Width, playButtonImg.Height);
            recordButton = new Rect();
            recordButton.Location = new Point(Canvas.GetLeft(recordButtonImg), Canvas.GetTop(recordButtonImg));
            recordButton.Size = new Size(recordButtonImg.Width, recordButtonImg.Height);
            quitButton = new Rect();
            quitButton.Location = new Point(Canvas.GetLeft(quitButtonImg), Canvas.GetTop(quitButtonImg));
            quitButton.Size = new Size(quitButtonImg.Width, quitButtonImg.Height);
            optionsButton = new Rect();
            optionsButton.Location = new Point(Canvas.GetLeft(optionsButtonImg), Canvas.GetTop(optionsButtonImg));
            optionsButton.Size = new Size(optionsButtonImg.Width, optionsButtonImg.Height);

            //Add the buttons to the 
            //button list
            buttonsList.Add(playButton);
            buttonsList.Add(recordButton);
            buttonsList.Add(quitButton);
            buttonsList.Add(optionsButton);


            //Add the corresponding actions
            //to the action list
            actionsList.Add(Button_Play);
            actionsList.Add(Button_Record);
            actionsList.Add(Button_Quit);
            actionsList.Add(Button_Options);
        }

        private void deInitializeStartUpUI()
        {
            buttonsList.Clear();
            actionsList.Clear();
            startUpScreenIsActive = false;
        }

        private void initializeSelectLevelUI()
        {
            selectLevelScreenIsActive = true;
            rightHandProgressBar.Width = 0;


            warmUpImgBorder.Opacity = 1;

            //Get positions of buttons
            warmUp = new Rect();            
            warmUp.Location = new Point(Canvas.GetLeft(warmUpImg), Canvas.GetTop(warmUpImg));
            warmUp.Size = new Size(warmUpImg.Width, warmUpImg.Height);
            moderateCardio = new Rect();
            moderateCardio.Location = new Point(Canvas.GetLeft(moderateImg), Canvas.GetTop(moderateImg));
            moderateCardio.Size = new Size(moderateImg.Width, moderateImg.Height);
            intenseCardio = new Rect();
            intenseCardio.Location = new Point(Canvas.GetLeft(intenseImg), Canvas.GetTop(intenseImg));
            intenseCardio.Size = new Size(intenseImg.Width, intenseImg.Height);
            selectLevelBackButton = new Rect();
            selectLevelBackButton.Location = new Point(Canvas.GetLeft(backButtonImg), Canvas.GetTop(backButtonImg));
            selectLevelBackButton.Size = new Size(backButtonImg.Width, backButtonImg.Height);

            buttonsList.Add(warmUp);
            buttonsList.Add(moderateCardio);
            buttonsList.Add(intenseCardio);
            buttonsList.Add(selectLevelBackButton);
            
            actionsList.Add(warmUpWorkout);
            actionsList.Add(moderateWorkout);
            actionsList.Add(intenseWorkout);
            actionsList.Add(backButtonPressed);

            /*PatientTcpServer.getHeartRate();*/
        }

        private void deInitializeSelectLevelUI()
        {
            selectLevelScreenIsActive = false;
            buttonsList.Clear();
            actionsList.Clear();
        }

        private void InitializeKinectUI()
        {
            kinectScreenIsActive = true;
            //Set the number of points for the patient to 0
            numberOfPts = 0;

            isStandingStill = false;

            points.Text = numberOfPts + "Pts.";
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

            //Hide Done Button
            doneButtonHoverImg.Opacity = 1;
            doneButtonImg.Opacity = 1;
            doneButtonImg.Width = 1;
            doneButtonHoverImg.Width = 1;

            playIconPos = new Rect();
            playIconPos.Location = new Point(Canvas.GetLeft(playicon), Canvas.GetTop(playicon));
            playIconPos.Size = new Size(playicon.Width, playicon.Height);
            kinectBackButton = new Rect();
            kinectBackButton.Location = new Point(Canvas.GetLeft(backButtonKinectImg), Canvas.GetTop(backButtonKinectImg));
            kinectBackButton.Size = new Size(backButtonKinectImg.Width, backButtonKinectImg.Height);
            bigPlayIcon = new Rect();
            bigPlayIcon.Location = new Point(Canvas.GetLeft(bigPlayIconImg), Canvas.GetTop(bigPlayIconImg));
            bigPlayIcon.Size = new Size(bigPlayIconImg.Width, bigPlayIconImg.Height);
            doneButton = new Rect();
            doneButton.Location = new Point(-900, -900);

            //Set video source for video player
            LoadVideo();
            //Get Length of Video
            FitnessPlayer.MediaOpened += new System.Windows.RoutedEventHandler(media_MediaOpened);

            buttonsList.Add(playIconPos);
            buttonsList.Add(kinectBackButton);
            buttonsList.Add(bigPlayIcon);
            buttonsList.Add(doneButton);

            actionsList.Add(btnPlay_Click);
            actionsList.Add(KinectButton_Back);
            actionsList.Add(btnPlay_Click);
            actionsList.Add(goHome);
        
        }

        private void deInitializeKinectUI()
        {
            kinectScreenIsActive = false;
            actionsList.Clear();
            buttonsList.Clear();

            //Check if video is playing 
            //and stop it if it is
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
            if (accuracyChecker != null)
            {
                accuracyChecker.Stop();
            }
        }

        /**
         * Initialize the final Screen
         */
        private void initializeDoneUI()
        {
            doneScreenIsActive = true;

            
            startDoneAnimator();


            angleStatsBox.Width = 0;
            speedStatsBox.Width = 0;
        }

        private void startDoneAnimator()
        {            
            doneAnimator = new System.Windows.Threading.DispatcherTimer();
            doneAnimator.Tick += (s, args) => animateStats();
            doneAnimator.Interval = new TimeSpan(0, 0, 0, 0, 25);
            doneAnimator.Start();
        }

        /**
         * Animate the stats boxes after exercising
         */
        private void animateStats()
        {
            if (angleStatsBox.Width < 350 || speedStatsBox.Width < 350)
            {
                angleStatsBox.Width += 8;
                speedStatsBox.Width += 8;
            }
            
            else
            {
                doneAnimator.Stop();
            }
        
        }

        /**
         * De-initialize the final screen
         */
        private void deInitializeDoneUI()
        {
            doneScreenIsActive = false;
        }



        private void loadBackground()
        {
            String path = System.AppDomain.CurrentDomain.BaseDirectory;
            path = System.IO.Directory.GetParent(path).FullName;
            path = System.IO.Directory.GetParent(path).FullName;
            path = System.IO.Directory.GetParent(path).FullName;
            path = System.IO.Directory.GetParent(path).FullName;

            background.Source = new Uri(path + "\\KinectFitness\\background.mp4");
            background.Play();
        }

        /**
        * Take the old screen and hide it
        * Take the new screen and show it
        */
        private void startNewCanvas(Canvas newScreen, Canvas oldScreen, bool goBack)
        {
            //If not going back a screen
            //then do the forward animation
            if (goBack == false)
            {
                canvasAnimator = new System.Windows.Threading.DispatcherTimer();
                canvasAnimator.Tick += (s, args) => animateShowNewCanvas(newScreen, oldScreen);
                canvasAnimator.Interval = new TimeSpan(0, 0, 0, 0, 10);
                canvasAnimator.Start();
            }
            //Else do the backward animation
            else
            {
                canvasAnimator = new System.Windows.Threading.DispatcherTimer();
                canvasAnimator.Tick += (s, args) => animateShowNewCanvasBack(newScreen, oldScreen);
                canvasAnimator.Interval = new TimeSpan(0, 0, 0, 0, 10);
                canvasAnimator.Start();
            }
        }

        private void animateShowNewCanvas(Canvas newScreen, Canvas oldScreen)
        {
            double distanceToGoal = Math.Abs(newScreen.Margin.Left - 0);
            double sqrt = Math.Sqrt(distanceToGoal);
            if (newScreen.Margin.Left > 40)
            {
                StartUp.Margin = new Thickness(StartUp.Margin.Left - 2 * sqrt, 0, 0, 0);
                SelectLevel.Margin = new Thickness(SelectLevel.Margin.Left - 2 * sqrt, 0, 0, 0);
                Kinect.Margin = new Thickness(Kinect.Margin.Left - 2 * sqrt, 0, 0, 0);
                Stats.Margin = new Thickness(Stats.Margin.Left - 2 * sqrt, 0, 0, 0);                
            }
            else
            {
                canvasAnimator.Stop();
            }
        }

        private void animateShowNewCanvasBack(Canvas newScreen, Canvas oldScreen)
        {
            double distanceToGoal = Math.Abs(newScreen.Margin.Left - 0);
            double sqrt = Math.Sqrt(distanceToGoal);
            if (newScreen.Margin.Left < 0)
            {
                StartUp.Margin = new Thickness(StartUp.Margin.Left + 2*sqrt, 0, 0, 0);
                SelectLevel.Margin = new Thickness(SelectLevel.Margin.Left + 2 * sqrt, 0, 0, 0);
                Kinect.Margin = new Thickness(Kinect.Margin.Left + 2 * sqrt, 0, 0, 0);
                Stats.Margin = new Thickness(Stats.Margin.Left + 2 * sqrt, 0, 0, 0);   
            }
            else
            {
                canvasAnimator.Stop();
            }
        }

        /**
         * Loop the background
         */
        private void Media_Ended(object sender, EventArgs e)
        {
            background.Position = new TimeSpan(0, 0, 0);
            background.Play();
        }


        void InitializeHoverChecker(int control)
        {

            dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
            //Timer to check for hand positions

            if (control == 1)
            {
                dispatcherTimer.Tick += new EventHandler(checkHands);
                dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, 50);
            }         
          
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
                //Boolean to check if a button is being hovered
                bool noButtonsBeingHovered = true;
                for (int i = 0; i < buttonsList.Count(); i++)
                {
                    if (rightHandPos.IntersectsWith(buttonsList.ElementAt(i)))
                    {
                        hoverTimer.Start();
                        //Set to false
                        noButtonsBeingHovered = false;

                        hoverImage(buttonsList.ElementAt(i), new RoutedEventArgs());

                        setHandProgressBar(false, hoverTimer.ElapsedMilliseconds);

                        if (hoverTimer.ElapsedMilliseconds >= 2000)
                        {
                            //Call the appropriate function for the button being pressed
                            buttonPressed(buttonsList.ElementAt(i), new RoutedEventArgs());
                            hoverTimer.Reset();
                        }
                    }
                }
                if (noButtonsBeingHovered)
                {
                    resetHandProgressBars();
                    hoverTimer.Reset();
                    resetImages(new object(), new RoutedEventArgs());
                }
            }
            catch (NullReferenceException)
            {
                //Do Nothing
            }
        }

        private void buttonPressed(object sender, RoutedEventArgs e)
        {
            Rect r = (Rect)sender;
            for (int i = 0; i < buttonsList.Count(); i++)
            {
                if (r.Equals(buttonsList.ElementAt(i)))
                {
                    actionsList.ElementAt(i)(new object(), new RoutedEventArgs());
                }
            }
        }

        private void Button_Quit(object sender, RoutedEventArgs e)
        {
            //MessageBox.Show("Quit");
            StopKinect(kinectSensorChooser1.Kinect);
            dispatcherTimer.Stop();
            //myCommands.StopSpeechRecognition();
            this.Close();

            try
            {
                /*
                Kinect2JavaClient quitMessage = new Kinect2JavaClient("quit");
                quitMessage.sendFlag();

                if(Kinect2JavaClient.socketForServer.Connected)
                {
                    Kinect2JavaClient.socketForServer.Close();
                }/**/
                newThread.Abort();
                control.ReleaseDevice();
            }
            catch (Exception ex) { }

            Process.GetCurrentProcess().Kill();
        }
        private void Button_Play(object sender, RoutedEventArgs e)
        {

            startNewCanvas(SelectLevel, StartUp, false);
            deInitializeStartUpUI();
            initializeSelectLevelUI();

        }

        private void Button_Record(object sender, RoutedEventArgs e)
        {
            closing = true; 
            StopKinect(kinectSensorChooser1.Kinect);
            dispatcherTimer.Stop();
            RecordWindow rw = new RecordWindow();
            this.Close();
            rw.Show();
            //this.NavigationService.Navigate(rw);
        }

        private void Button_Options(object sender, RoutedEventArgs e)
        {

        }

        /**
         * Highlights images when the mouse hovers over them
         */
        private void hoverImage(object sender, RoutedEventArgs e)
        {
            Rect r = new Rect();
                r = (Rect)sender;


            if (r.Equals(playButton))
            {
                playborder.Opacity = 1;
            }
            else if (r.Equals(optionsButton))
            {
                optionsborder.Opacity = 1;
            }
            else if (r.Equals(quitButton))
            {
                quitborder.Opacity = 1;
            }
            else if (r.Equals(recordButton))
            {
                recordborder.Opacity = 1;
            }
            else if (r.Equals(warmUp))
            {
                warmUpImgBorder.Opacity = 1;
            }
            else if (r.Equals(moderateCardio))
            {
                moderateImgBorder.Opacity = 1;
            }
            else if (r.Equals(intenseCardio))
            {
                intenseImgBorder.Opacity = 1;
            }
            else if (r.Equals(selectLevelBackButton))
            {
                backButtonImg.Opacity = 0;
                backButtonHoverImg.Opacity = 1;
            }
            else if (r.Equals(playIconPos))
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
            else if (r.Equals(bigPlayIcon))
            {
                bigPlayIconImg.Opacity = 0;
                bigPlayIconHoverImg.Opacity = 1;
            }
            else if (r.Equals(kinectBackButton))
            {
                backButtonKinectImg.Opacity = 0;
                backButtonKinectHoverImg.Opacity = 1;
            }
            else if (r.Equals(doneButton))
            {
                doneButtonHoverImg.Opacity = 1;
                doneButtonImg.Opacity = 0;
            }
        }

        /**
         * Stops highlighting the images when the mouse leaves
         */
        private void resetImages(object sender, RoutedEventArgs e)
        {
                    playborder.Opacity = 0;
                    optionsborder.Opacity = 0;
                    quitborder.Opacity = 0;
                    recordborder.Opacity = 0;

                warmUpImgBorder.Opacity = 0;

                moderateImgBorder.Opacity = 0;

                intenseImgBorder.Opacity = 0;

                backButtonImg.Opacity = 1;
                backButtonHoverImg.Opacity = 0;

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

                bigPlayIconImg.Opacity = 1;
                bigPlayIconHoverImg.Opacity = 0;

                backButtonKinectImg.Opacity = 1;
                backButtonKinectHoverImg.Opacity = 0;

                doneButtonHoverImg.Opacity = 0;
                doneButtonImg.Opacity = 1;                      

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
            rightHandProgressBar.Margin = new Thickness(rightHandProgressBar.Margin.Left, rightHandProgressBar.Margin.Top + 60, 0, 0);
            rightHandPos.Location = new Point(rightHand.Margin.Left, rightHand.Margin.Top);
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
            Joint scaledJoint = joint.ScaleTo(900, 800, .3f, .3f);
            element.Margin = new Thickness(scaledJoint.Position.X, scaledJoint.Position.Y, 0, 0);

        }

        private void kinectSkeletonViewer1_Unloaded(object sender, RoutedEventArgs e)
        {
            closing = true;
            StopKinect(kinectSensorChooser1.Kinect);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try { 
                newThread.Abort(); 
                control.ReleaseDevice(); 
            } 
            catch (Exception ex) { }
            
        }

        private void SetFile(Image exercise)
        {
            List<string> video = new List<string>();

            String path = System.AppDomain.CurrentDomain.BaseDirectory;
            path = System.IO.Directory.GetParent(path).FullName;
            path = System.IO.Directory.GetParent(path).FullName;
            path = System.IO.Directory.GetParent(path).FullName;
            path = System.IO.Directory.GetParent(path).FullName;

            if (exercise.Name == warmUpImg.Name)
            {
                video.Add(warmUpImg.Name);
                System.IO.File.WriteAllLines(path + "\\KinectFitness\\FitnessVideos\\video.txt", video);
            }
            else if (exercise.Name == moderateImg.Name)
            {
                video.Add(moderateImg.Name);
                System.IO.File.WriteAllLines(path + "\\KinectFitness\\FitnessVideos\\video.txt", video);
            }
            else if (exercise.Name == intenseImg.Name)
            {
                video.Add(intenseImg.Name);
                System.IO.File.WriteAllLines(path + "\\KinectFitness\\FitnessVideos\\video.txt", video);
            }
        }

        private void intenseWorkout(object sender, RoutedEventArgs e)
        {
            SetFile(intenseImg);
        }

        private void moderateWorkout(object sender, RoutedEventArgs e)
        {
            
            SetFile(moderateImg);
            startNewCanvas(Kinect, SelectLevel, false);

            deInitializeSelectLevelUI();
            InitializeKinectUI();
        }

        private void warmUpWorkout(object sender, RoutedEventArgs e)
        {
            SetFile(warmUpImg);
            startNewCanvas(Kinect, SelectLevel, false);
            
            deInitializeSelectLevelUI();
            InitializeKinectUI();
        }

        private void backButtonPressed(object sender, RoutedEventArgs e)
        {
            /*
            if(PatientTcpServer.tcpListener.Pending())
            {
                PatientTcpServer.thread.Abort();
            }/**/

            startNewCanvas(StartUp, SelectLevel, true);
            deInitializeSelectLevelUI();
            InitializeStartUpUI();
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
            //Console.ReadLine();
        }

        //Find length of video
        void media_MediaOpened(object sender, System.Windows.RoutedEventArgs e)
        {
            debugger.Text = "Video Opened";
            totalMovieTime = FitnessPlayer.NaturalDuration.TimeSpan.TotalSeconds;
        }

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
                if (line.Contains("LSA"))
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
            //Console.ReadLine();
        }

        /*
         * Media Player Stuff
         */

        private void btnPlay_Play(object sender, RoutedEventArgs e)
        {
            if (!videoPlaying)
                VideoNotPlaying();
        }

        private void btnPlay_Pause(object sender, RoutedEventArgs e)
        {
            if (videoPlaying)
                VideoPlaying();
        }
        private void btnPlay_Click(object sender, RoutedEventArgs e)
        {
            if (!videoPlaying)
                VideoNotPlaying();

            else
                VideoPlaying();
        }

        private void VideoNotPlaying()
        {
            FitnessPlayer.Play();
            videoPlaying = true;
            stopHoverChecker();
            startAccuracyChecker();

            if (!timerInitialized)
            {
                timerInitialized = true;
                //Start playing back the recorded skeleton
                startPlaybackSkeleton();
                //Start the video progress bar
                startVideoProgressBar();
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

        private void VideoPlaying()
        {
            //Pause the Fitness Player
            FitnessPlayer.Pause();
            videoPlaying = false;

            //Video is paused, so allow user to use hand
            //to click buttons again
            startHoverChecker();

            //Stop matching the skeleton since video is not playing
            skeletonMatcherTimer.Stop();

            stopAccuracyChecker();
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
         * Starts playing back the skeleton and matching it against the user
         */
        private void startPlaybackSkeleton()
        {
            skeletonMatcherTimer = new System.Windows.Threading.DispatcherTimer();
            skeletonMatcherTimer.Tick += new EventHandler(matchSkeleton);
            skeletonMatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, 100);
            skeletonMatcherTimer.Start();
        }

        private void startAccuracyChecker()
        {
            accuracyChecker = new System.Windows.Threading.DispatcherTimer();
            accuracyChecker.Tick += new EventHandler(checkAccuracy);
            accuracyChecker.Interval = new TimeSpan(0, 0, 7);
            accuracyChecker.Start();
        }

        private void stopAccuracyChecker()
        {
            accuracyChecker.Stop();
        }

        /**
         * Check the Accuracy of the player periodically and display it
         */
        private void checkAccuracy(object sender, EventArgs e)
        {
            //Get angle and speed accuracy for last 70 frames
            double anglePrecision = angleAccuracy(70);
            double speedPrecision = speedAccuracy(70);

            //Used to tell if patient is moving too fast
            //or too slow on average
            JointSpeeds speedComparisons = speedComparisonsAccuracy(70);


            //debugger.Text = "Speed: " + speedPrecision.ToString();
            //debugger.Text = "Angle: " + anglePrecision.ToString();
            if (suggestionBox.Width <= 0)
            {
                //Get a suggestions based on all of the data
                getSuggestion(anglePrecision, speedPrecision, speedComparisons);
                //Start Animation to show suggestion
                startAnimation();
            }
            else
            {
                hideAnimation();
            }
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

        private void calibrateSkeletonSpeeds()
        {
            if (first == null)
                return;
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
        }

        /**
         * Checks if the loaded skeleton data matches the users skeleton 
         * periodically ten times every second
         */
        private void matchSkeleton(object sender, EventArgs e)
        {

            setPoints();
            //Get amount of seconds passed in video
            TimeSpan timePassedInVideo = FitnessPlayer.Position;
            int secondsPassedInVideo = Convert.ToInt16(timePassedInVideo.TotalMilliseconds / 100);
            //debugger.Text = secondsPassedInVideo.ToString();
            //Check if loaded skeleton at this point matches the users current data within +- 1 second of the video
            try
            {

                //Check Speeds
                MatchSkeletonSpeeds(loadedSkeletonSpeeds.ElementAt(secondsPassedInVideo));

                //Check Angles
                MatchSkeletonAngles(loadedSkeletonAngles.ElementAt(secondsPassedInVideo));

                //Needed for getting skeleton speed in next frame
                calibrateSkeletonSpeeds();
            }
            catch (ArgumentOutOfRangeException)
            {
                debugger.Text = "done";
            }
        }



        /**
         * Checks if the skeleton data matches within certain
         * angles to the users.
         */
        private void MatchSkeletonAngles(JointAngles ja)
        {
            patientAnglesData.Add(new JointAngles(0, 0, 0, 0, 0, 0, 0, 0));
            if (first == null || isStandingStill)
            {
                return;
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

            }
            else
            {
                patientAnglesData.Last().leftElbow = 1;
                //Points are added 
                numberOfPts += 5;
                setPoints();
            }
            if (rightElbow < (ja.rightElbow - 20) || rightElbow > (ja.rightElbow + 20))
            {

            }
            else
            {
                patientAnglesData.Last().rightElbow = 1;
                numberOfPts += 5;
                setPoints();
            }
            if (leftHip < (ja.leftHip - 20) || leftHip > (ja.leftHip + 20))
            {

            }
            else
            {
                patientAnglesData.Last().leftHip = 1;
                numberOfPts += 5;
                setPoints();
            }
            if (rightHip < (ja.rightHip - 20) || rightHip > (ja.rightHip + 20))
            {

            }
            else
            {
                patientAnglesData.Last().rightHip = 1;
                numberOfPts += 5;
                setPoints();
            }
            if (rightKnee < (ja.rightKnee - 20) || rightKnee > (ja.rightKnee + 20))
            {

            }
            else
            {
                patientAnglesData.Last().rightKnee = 1;
                numberOfPts += 5;
                setPoints();
            }
            if (leftKnee < (ja.leftKnee - 20) || leftKnee > (ja.leftKnee + 20))
            {

            }
            else
            {
                patientAnglesData.Last().leftKnee = 1;
                numberOfPts += 5;
                setPoints();
            }
            if (rightShoulder < (ja.rightShoulder - 20) || rightShoulder > (ja.rightShoulder + 20))
            {

            }
            else
            {
                patientAnglesData.Last().rightShoulder = 1;
                numberOfPts += 5;
                setPoints();
            }
            if (leftShoulder < (ja.leftShoulder - 20) || leftShoulder > (ja.leftShoulder + 20))
            {

            }
            else
            {
                patientAnglesData.Last().leftShoulder = 1;
                numberOfPts += 5;
                setPoints();
            }
            //debugger.Text = "Left Elbow:" + patientData.Last().leftElbow + "\nLeft Shoulder:" + patientData.Last().leftShoulder + "\nLeft Hip:" + patientData.Last().leftHip + "\nKnee:" + patientData.Last().leftKnee;

            //Check for Pause Gesture
            if ((leftShoulder < 105) && (rightShoulder < 105) && (leftElbow > 140) && (rightElbow > 140)
                && (leftHip > 140) && (rightHip > 140) && (leftKnee > 140) && (rightKnee > 140))
            {
                btnPlay_Click(new object(), new RoutedEventArgs());
            }
        }

        /**
         * Returns true if the speeds of the patients
         * match the recorded skeleton data sufficiently well
         */
        private void MatchSkeletonSpeeds(JointSpeeds js)
        {
            patientSpeedData.Add(new JointSpeeds(0, 0, 0, 0, 0, 0, 0, 0, 0, 0));
            if (first == null)
            {
                return;
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

            /*
            if (js.leftFoot < -3)
                debugger.Text = "Left";
            else if (js.leftFoot <= 3)
                debugger.Text = "Stopped";
            else
                debugger.Text = "Right";
            if (leftFoot < -3)
                debugger.Text += "\nLeft";
            else if (leftFoot <= 3)
                debugger.Text += "\nStopped";
            else
                debugger.Text += "\nRight";
            */


            //Check to see if patient is standing still
            checkIfStandingStill(leftHand, rightHand, leftFoot, rightFoot, leftHip, rightHip);

            if (isStandingStill)
            {
                return;
            }

            //Checks skeleton and compares it to see if it is going 
            //too fast or slow or just right
            getComparisons(leftHand, rightHand, leftShoulder, rightShoulder, leftHip, rightHip,
                leftKnee, rightKnee, leftFoot, rightFoot, js);


            //Checks skeleton to see if it is within the appropriate error window
            if ((leftHand > js.leftHand - 5) && (leftHand < js.leftHand + 5))
            {
                //Give user full marks (1/1) for this frame for matching 
                //their left hand with the recorded left hand
                patientSpeedData.Last().leftHand = 1;
                numberOfPts += 5;
                setPoints();

            }
            else if (leftHand >= 5 || leftHand <= -5)
            {
                //Half Points for effort
                patientSpeedData.Last().leftHand = 0.5;
                numberOfPts += 2;
                setPoints();
            }
            else
            {
            }
            if (((rightHand > js.rightHand - 5) && (rightHand < js.rightHand + 5)))
            {
                patientSpeedData.Last().rightHand = 1;
                numberOfPts += 5;
                setPoints();
            }
            else if (rightHand >= 5 || rightHand <= -5)
            {
                numberOfPts += 2;
                setPoints();
                patientSpeedData.Last().rightHand = 0.5;
            }
            else
            {
            }
            if (((leftShoulder > js.leftShoulder - 5) && (leftShoulder < js.leftShoulder + 5)))
            {
                numberOfPts += 5;
                setPoints();
                patientSpeedData.Last().leftShoulder = 1;
            }
            else if (leftShoulder >= 5 || leftShoulder <= -5)
            {
                numberOfPts += 2;
                setPoints();
                patientSpeedData.Last().leftShoulder = 0.5;
            }
            else
            {
            }
            if (((rightShoulder > js.rightShoulder - 5) && (rightShoulder < js.rightShoulder + 5)))
            {
                numberOfPts += 5;
                setPoints();
                patientSpeedData.Last().rightShoulder = 1;
            }
            else if (rightShoulder >= 5 || rightShoulder <= -5)
            {
                numberOfPts += 2;
                setPoints();
                patientSpeedData.Last().rightShoulder = 0.5;
            }
            else
            {
            }
            if (((leftHip > js.leftHip - 5) && (leftHip < js.leftHip + 5)))
            {
                numberOfPts += 5;
                setPoints();
                patientSpeedData.Last().leftHip = 1;
            }
            else if (leftHip >= 5 || leftHip <= -5)
            {
                numberOfPts += 2;
                setPoints();
                patientSpeedData.Last().leftHip = 0.5;
            }
            else
            {
            }
            if (((rightHip > js.rightHip - 5) && (rightHip < js.rightHip + 5)))
            {
                numberOfPts += 5;
                setPoints();
                patientSpeedData.Last().rightHip = 1;
            }
            else if (rightHip >= 5 || rightHip <= -5)
            {
                numberOfPts += 2;
                setPoints();
                patientSpeedData.Last().rightHip = 0.5;
            }
            else
            {
            }
            if (((leftKnee > js.leftKnee - 5) && (leftKnee < js.leftKnee + 5)))
            {
                numberOfPts += 5;
                setPoints();
                patientSpeedData.Last().leftKnee = 1;
            }
            else if (leftKnee >= 5 || leftKnee <= -5)
            {
                numberOfPts += 2;
                setPoints();
                patientSpeedData.Last().leftKnee = 0.5;
            }
            else
            {
            }
            if (((rightKnee > js.rightKnee - 5) && (rightKnee < js.rightKnee + 5)))
            {
                numberOfPts += 5;
                setPoints();
                patientSpeedData.Last().rightKnee = 1;
            }
            else if (rightKnee >= 5 || rightKnee <= -5)
            {
                numberOfPts += 2;
                setPoints();
                patientSpeedData.Last().rightKnee = 0.5;
            }
            else
            {
            }
            if ((leftFoot > js.leftFoot - 5) && (leftFoot < js.leftFoot + 5))
            {
                debugger.Foreground = Brushes.Green;
                numberOfPts += 5;
                setPoints();
                patientSpeedData.Last().leftFoot = 1;
            }
            else if (leftFoot >= 5 || leftFoot <= -5)
            {
                debugger.Foreground = Brushes.Green;
                numberOfPts += 2;
                setPoints();
                patientSpeedData.Last().leftFoot = 0.5;
            }
            else
            {
                debugger.Foreground = Brushes.Red;
            }
            if (((rightFoot > js.rightFoot - 5) && (rightFoot < js.rightFoot + 5)))
            {
                numberOfPts += 5;
                setPoints();
                patientSpeedData.Last().rightFoot = 1;
            }
            else if (rightFoot >= 5 || rightFoot <= -5)
            {
                numberOfPts += 2;
                setPoints();
                patientSpeedData.Last().rightFoot = 0.5;
            }
            else
            {
            }

        }

        /**
         * Gets the comparison of speeds between skeletons 
         * so that user can get feedback to know if they are 
         * going too fast or slow
         */
        private void getComparisons(int leftHand, int rightHand, int leftShoulder, int rightShoulder,
            int leftHip, int rightHip, int leftKnee, int rightKnee, int leftFoot, int rightFoot, JointSpeeds js)
        {
            //Get differences in absolute values of joint speeds
            patientSpeedData.Last().leftHandComparison = Math.Abs(leftHand) - Math.Abs(js.leftHand);
            patientSpeedData.Last().rightHandComparison = Math.Abs(rightHand) - Math.Abs(js.rightHand);
            patientSpeedData.Last().leftShoulderComparison = Math.Abs(leftShoulder) - Math.Abs(js.leftShoulder);
            patientSpeedData.Last().rightShoulderComparison = Math.Abs(rightShoulder) - Math.Abs(js.rightShoulder);
            patientSpeedData.Last().leftHipComparison = Math.Abs(leftHip) - Math.Abs(js.leftHip);
            patientSpeedData.Last().rightHipComparison = Math.Abs(rightHip) - Math.Abs(js.rightHip);
            patientSpeedData.Last().leftKneeComparison = Math.Abs(leftKnee) - Math.Abs(js.leftKnee);
            patientSpeedData.Last().rightKneeComparison = Math.Abs(rightKnee) - Math.Abs(js.rightKnee);
            patientSpeedData.Last().leftFootComparison = Math.Abs(leftFoot) - Math.Abs(js.leftFoot);
            patientSpeedData.Last().rightFootComparison = Math.Abs(rightFoot) - Math.Abs(js.rightFoot);
        }

        /**
         * Check to see if patient is standing still
         */
        private void checkIfStandingStill(int leftHand, int rightHand, int leftFoot, int rightFoot,
            int leftHip, int rightHip)
        {
            //Check to see if participant is standing still
            //If they are standing still for more than 7 seconds, start penalizing them
            if (leftHand < 5 && rightHand < 5 && leftFoot < 5 && rightFoot < 5 && leftHip < 5 && rightHip < 5)
            {
                //Start the timer if it has not started
                if (!standingStillTimer.IsRunning)
                {
                    standingStillTimer.Start();
                }
                //If the person has been standing still for 7 seconds or more
                //Give them 0 points for this frame
                if (standingStillTimer.ElapsedMilliseconds > 7000)
                {
                    //debugger.Text = "Standing Still!";
                    patientSpeedData[patientSpeedData.Count - 2] = new JointSpeeds(0, 0, 0, 0, 0, 0, 0, 0, 0, 0);
                    patientAnglesData[patientAnglesData.Count - 2] = new JointAngles(0, 0, 0, 0, 0, 0, 0, 0);
                    isStandingStill = true;
                }
                else
                {
                    isStandingStill = false;
                }

            }
            //If patient is moving at least a little
            else
            {
                //debugger.Text = "Moving!";
                standingStillTimer.Reset();
            }
        }

        /**
         * Sets the points to the progress bar and number of points
         */
        void setPoints()
        {
            points.Text = numberOfPts + "Pts.";
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

            int i;
            if (dx < 0)
                i = -1;
            else
                i = 1;


            speed = Math.Sqrt((dx * dx) + (dy * dy) + (dz * dz)) * 100;
            int speedRounded = Convert.ToInt32(speed);
            return i * speedRounded;
        }


        private static double vectorNorm(double x, double y, double z)
        {

            return Math.Sqrt(Math.Pow(x, 2) + Math.Pow(y, 2) + Math.Pow(z, 2));

        }

        /**
         * This method takes the all of the patient exercise 
         * data and decides what to tell the patient in the 
         * suggestion box (i.e "You are doing great!" or
         * "Try and speed up a little")
         */
        private void getSuggestion(double anglePrecision, double speedPrecision, JointSpeeds js)
        {
            double averagePrecision = (anglePrecision + speedPrecision) / 2;
            if (averagePrecision > 80)
            {
                if (anglePrecision > speedPrecision)
                {
                    if (!patientSpeedComparisons(js))
                    {
                        if (anglePrecision > 94)
                            suggestionBox.Text = "Perfect Form!";
                        else if (anglePrecision >= 85)
                        {
                            suggestionBox.Text = "Wonderful!";
                        }
                        else if (anglePrecision > 80)
                            suggestionBox.Text = "You are \nlooking great!";
                    }
                }
                else
                    suggestionBox.Text = "Great Pace!";
                suggestionBox.Background = (SolidColorBrush)new BrushConverter().ConvertFromString("#FF19B419");
            }
            else if (averagePrecision > 60)
            {
                //Compare the patient's speed and 
                //write out a suggestion
                //If the patient's speed is doing okay
                //then check if the angle precision 
                // or speed precision is worse
                if (!patientSpeedComparisons(js))
                {
                    if (anglePrecision > speedPrecision)
                    {
                        suggestionBox.Text = "Nice Form!";
                    }
                    else
                        suggestionBox.Text = "Make sure to match\nthe trainer's form";
                }
                suggestionBox.Background = (SolidColorBrush)new BrushConverter().ConvertFromString("#FFFFEB00");
            }
            else if (averagePrecision > 40)
            {
                if (!patientSpeedComparisons(js))
                {
                    if (anglePrecision < speedPrecision)
                    {
                        suggestionBox.Text = "Try and match the \ntrainer's form better";
                    }
                    else
                        suggestionBox.Text = "Try and match the \ntrainer's pace better";
                }
                suggestionBox.Background = (SolidColorBrush)new BrushConverter().ConvertFromString("#FFF87200");
            }
            else if (averagePrecision > 20)
            {
                if (!patientSpeedComparisons(js))
                {
                    if (anglePrecision < speedPrecision)
                    {
                        suggestionBox.Text = "Try and improve \nthe leg work";
                    }
                    else
                        suggestionBox.Text = "Try and match the \ntrainer's pace better";
                }
                suggestionBox.Background = (SolidColorBrush)new BrushConverter().ConvertFromString("#FFF87200");
            }
            else if (averagePrecision > 0)
            {
                suggestionBox.Background = (SolidColorBrush)new BrushConverter().ConvertFromString("#FFF80000");
                suggestionBox.Text = "Are you \neven trying?";
            }
            else
            {
                suggestionBox.Background = (SolidColorBrush)new BrushConverter().ConvertFromString("#FFF80000");
                suggestionBox.Text = "Start Moving!";
            }
        }


        //Returns true and writes a suggestion to 
        //the suggestion box if the patient's speed is 
        //significantly off
        private bool patientSpeedComparisons(JointSpeeds js)
        {

            if (js.leftFootComparison < -2 && js.rightFootComparison < -2 ||
                js.leftKneeComparison < -2 && js.rightKneeComparison < -2)
            {
                if (js.leftHandComparison < -2 && js.rightHandComparison < -2)
                    suggestionBox.Text = "Try and keep pace \nwith the trainer!";
                else
                    suggestionBox.Text = "Get those \nlegs moving!";
                //Return true since a suggestion had to 
                //be made 
                return true;
            }
            else if (js.leftFootComparison > 2 && js.rightFootComparison > 2 ||
                js.leftKneeComparison > 2 && js.rightKneeComparison > 2)
            {
                if (js.leftHandComparison > 2 && js.rightHandComparison > 2)
                    suggestionBox.Text = "Try and slow \ndown your pace";
                else
                    suggestionBox.Text = "Slow down those legs!";
                //Return true since a suggestion had to
                //be made
                return true;
            }
            //No suggestion was made so return false
            else
                return false;

        }

        //When called, it starts the Suggestion Box Animation
        private void startAnimation()
        {
            suggestionSound.Play();
            suggestionAnimator = new System.Windows.Threading.DispatcherTimer();
            suggestionAnimator.Tick += new EventHandler(animateShowSuggestionBox);
            suggestionAnimator.Interval = new TimeSpan(0, 0, 0, 0, 10);
            suggestionAnimator.Start();
        }

        private void hideAnimation()
        {
            suggestionAnimator = new System.Windows.Threading.DispatcherTimer();
            suggestionAnimator.Tick += new EventHandler(animateHideSuggestionBox);
            suggestionAnimator.Interval = new TimeSpan(0, 0, 0, 0, 10);
            suggestionAnimator.Start();
        }

        /**
         * Show the suggestion box through animation
         */
        private void animateShowSuggestionBox(object sender, EventArgs e)
        {
            if (suggestionBox.Width < 254)
            {
                suggestionBox.Width += 8;
            }
            else
            {
                suggestionAnimator.Stop();
            }
        }

        /**
         * Hide the suggestion box through animation
         */
        private void animateHideSuggestionBox(object sender, EventArgs e)
        {
            if (suggestionBox.Width > 0)
            {
                suggestionBox.Width -= 8;
            }
            else
            {
                suggestionAnimator.Stop();
            }
        }

        /**
         * Fires when the video ends
         */
        private void videoEnd(object sender, RoutedEventArgs e)
        {
            //Create Done Button and remove other buttons
            createStatsView();
            //Display the stats
            showStats();
            //Start up the hover checker
            startHoverChecker();
            //Video is not playing
            videoPlaying = false;

            /*
            if(PatientTcpServer.tcpListener.Pending())
            {
                PatientTcpServer.thread.Abort();
                PatientTcpServer.tcpListener.Stop();
            }/**/
        }

        /**
         * Creates and removes appropriate buttons
         * to help display the statistics 
         * at the end of the patients exercise
         */
        private void createStatsView()
        {
            //Initialize Done Button
            doneButton.Size = new Size(doneButtonImg.Width, doneButtonImg.Height);
            doneButton.Location = new Point(Canvas.GetLeft(doneButtonImg), Canvas.GetTop(doneButtonImg));
            doneButtonImg.Width = 200;
            doneButtonHoverImg.Width = 200;
        }

        /**
         * Get the average speed of the joints and returns 
         * a JointSpeeds object with the average difference in
         * speed for each joint compared with the recorded skeleton
         */
        private JointSpeeds speedComparisonsAccuracy(int numberOfComparisons)
        {
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

            //Get average speed relative to recorded skeleton
            for (int i = patientSpeedData.Count() - 1; (i >= patientSpeedData.Count() - numberOfComparisons) && (i >= 0); i--)
            {
                leftHandSpeed += patientSpeedData.ElementAt(i).leftHandComparison;
                rightHandSpeed += patientSpeedData.ElementAt(i).rightHandComparison;
                leftShoulderSpeed += patientSpeedData.ElementAt(i).leftShoulderComparison;
                rightShoulderSpeed += patientSpeedData.ElementAt(i).rightShoulderComparison;
                leftHipSpeed += patientSpeedData.ElementAt(i).leftHipComparison;
                rightHipSpeed += patientSpeedData.ElementAt(i).rightHipComparison;
                leftKneeSpeed += patientSpeedData.ElementAt(i).leftKneeComparison;
                rightKneeSpeed += patientSpeedData.ElementAt(i).rightKneeComparison;
                leftFootSpeed += patientSpeedData.ElementAt(i).leftFootComparison;
                rightFootSpeed += patientSpeedData.ElementAt(i).rightFootComparison;
            }

            //Get average speed for each joint
            JointSpeeds js = new JointSpeeds();
            try
            {
                js.leftHandComparison = leftHandSpeed / numberOfComparisons;
                js.rightHandComparison = rightHandSpeed / numberOfComparisons;
                js.leftShoulderComparison = leftShoulderSpeed / numberOfComparisons;
                js.rightShoulderComparison = rightShoulderSpeed / numberOfComparisons;
                js.leftHipComparison = leftHipSpeed / numberOfComparisons;
                js.rightHipComparison = rightHipSpeed / numberOfComparisons;
                js.leftKneeComparison = leftKneeSpeed / numberOfComparisons;
                js.rightKneeComparison = rightKneeSpeed / numberOfComparisons;
                js.leftFootComparison = leftFootSpeed / numberOfComparisons;
                js.rightFootComparison = rightFootSpeed / numberOfComparisons;
            }
            catch (OverflowException)
            {

            }

            return js;

        }

        /**
         * Get Speed Accuracy and return as double between 0% and 100%
         */
        private double speedAccuracy(int numberOfSpeedData)
        {
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
            for (int i = patientSpeedData.Count() - 1; (i >= patientSpeedData.Count() - numberOfSpeedData) && (i >= 0); i--)
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

            try
            {
                int leftHandSpeedStat = Convert.ToInt16(Math.Round((leftHandSpeed / numberOfSpeedData * 100), 0));
                int rightHandSpeedStat = Convert.ToInt16(Math.Round((rightHandSpeed / numberOfSpeedData * 100), 0));
                int leftShoulderSpeedStat = Convert.ToInt16(Math.Round((leftShoulderSpeed / numberOfSpeedData * 100), 0));
                int rightShoulderSpeedStat = Convert.ToInt16(Math.Round((rightShoulderSpeed / numberOfSpeedData * 100), 0));
                int leftHipSpeedStat = Convert.ToInt16(Math.Round((leftHipSpeed / numberOfSpeedData * 100), 0));
                int rightHipSpeedStat = Convert.ToInt16(Math.Round((rightHipSpeed / numberOfSpeedData * 100), 0));
                int leftKneeSpeedStat = Convert.ToInt16(Math.Round((leftKneeSpeed / numberOfSpeedData * 100), 0));
                int rightKneeSpeedStat = Convert.ToInt16(Math.Round((rightKneeSpeed / numberOfSpeedData * 100), 0));
                int leftFootSpeedStat = Convert.ToInt16(Math.Round((leftFootSpeed / numberOfSpeedData * 100), 0));
                int rightFootSpeedStat = Convert.ToInt16(Math.Round((rightFootSpeed / numberOfSpeedData * 100), 0));

                double speedAccuracy = (leftHandSpeedStat + rightHandSpeedStat + leftShoulderSpeedStat + rightShoulderSpeedStat
                + leftHipSpeedStat + rightHipSpeedStat + leftKneeSpeedStat + rightKneeSpeedStat + leftFootSpeedStat
                + rightFootSpeedStat) / 10;

                return speedAccuracy;
            }
            catch (OverflowException)
            {

            }
            //Get the Speed Accuracy
            return 0;

            
        }

        /**
         * Get the angle accuracy and return as a double between 0% and 100%
         */
        private double angleAccuracy(int numberOfAngleData)
        {
            double leftElbowAngle = 0;
            double rightElbowAngle = 0;
            double leftShoulderAngle = 0;
            double rightShoulderAngle = 0;
            double leftHipAngle = 0;
            double rightHipAngle = 0;
            double leftKneeAngle = 0;
            double rightKneeAngle = 0;

            //Get Number of corrrect comparisons for each joint
            for (int i = patientAnglesData.Count() - 1; (i >= patientAnglesData.Count() - numberOfAngleData) && (i >= 0); i--)
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

            try
            {
                //Get Percentage of correct comparisons for each joint
                int leftElbowAngleStat = Convert.ToInt16(Math.Round((leftElbowAngle / numberOfAngleData * 100), 0));
                int rightElbowAngleStat = Convert.ToInt16(Math.Round((rightElbowAngle / numberOfAngleData * 100), 0));
                int leftShoulderAngleStat = Convert.ToInt16(Math.Round((leftShoulderAngle / numberOfAngleData * 100), 0));
                int rightShoulderAngleStat = Convert.ToInt16(Math.Round((rightShoulderAngle / numberOfAngleData * 100), 0));
                int leftHipAngleStat = Convert.ToInt16(Math.Round((leftHipAngle / numberOfAngleData * 100), 0));
                int rightHipAngleStat = Convert.ToInt16(Math.Round((rightHipAngle / numberOfAngleData * 100), 0));
                int leftKneeAngleStat = Convert.ToInt16(Math.Round((leftKneeAngle / numberOfAngleData * 100), 0));
                int rightKneeAngleStat = Convert.ToInt16(Math.Round((rightKneeAngle / numberOfAngleData * 100), 0));

                //Get the Joint Accuracy
                double angleAccuracy = (leftElbowAngleStat + rightElbowAngleStat + leftShoulderAngleStat + rightShoulderAngleStat
                    + leftHipAngleStat + rightHipAngleStat + leftKneeAngleStat + rightKneeAngleStat) / 8;

                return angleAccuracy;
            }
            catch (OverflowException)
            {

            }

            return 0;
            
        }

        /**
         * Show the patients stats
         */
        private void showStats()
        {
            double anglePrecision = angleAccuracy(patientAnglesData.Count());
            double speedPrecision = speedAccuracy(patientSpeedData.Count());
            // Put this into the Stats box at the end of the video
            angleStatsBox.Text = "Angle Accuracy: " + anglePrecision;
            speedStatsBox.Text = "Speed Accuracy: " + speedPrecision;


            startNewCanvas(Stats, Kinect, false);
            deInitializeKinectUI();
            initializeDoneUI();
        }

        /**
         * Removes the big play button
         */
        private void removeBigPlayButton()
        {
            bigPlayIcon.Size = new Size(0, 0);
            bigPlayIconImg.Height = 0;
            bigPlayIconHoverImg.Height = 0;
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

        /**
         * Go back to the home screen
         */
        private void goHome(object sender, RoutedEventArgs e)
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
            if (accuracyChecker != null)
            {
                accuracyChecker.Stop();
            }
            if (suggestionAnimator != null)
            {
                suggestionAnimator.Stop();
            }

            /*
            MessageBox.Show("go home method");
            if (PatientTcpServer.tcpListener.Pending())
            {
                PatientTcpServer.thread.Abort();
            }/**/

            startNewCanvas(StartUp, Stats, true);
            deInitializeDoneUI();
            InitializeStartUpUI();
        }

        private void KinectButton_Back(object sender, RoutedEventArgs e)
        {
            /*
            MessageBox.Show("back button");
            if (PatientTcpServer.tcpListener.Pending())
            {
                Kinect2JavaClient backmessage = new Kinect2JavaClient("back");
                backmessage.sendFlag();
                PatientTcpServer.thread.Abort();
            }/**/

            startNewCanvas(SelectLevel, Kinect, true);
            deInitializeKinectUI();
            initializeSelectLevelUI();
        }

    }
}
