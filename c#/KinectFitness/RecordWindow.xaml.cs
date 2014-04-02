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
    /// Interaction logic for RecordWindow.xaml
    /// </summary>
    public partial class RecordWindow : Window
    {
        public RecordWindow()
        {
            InitializeComponent();
            stopButton.Opacity = 0;
            recording = false;
        }

        bool closing = false;
        const int skeletonCount = 6;
        Skeleton[] allSkeletons = new Skeleton[skeletonCount];
        KinectSensor ksensor;
        Skeleton first;
        List<string> jointAngles;
        Stopwatch jointTime;
        System.Windows.Threading.DispatcherTimer dispatcherTimer;
        bool recording = false;
        int currentTime = 1;
        int previousTime = 0;
        Joint HL = new Joint();
        Joint HR = new Joint();
        Joint SL = new Joint();
        Joint SR = new Joint();
        Joint KL = new Joint();
        Joint KR = new Joint();
        Joint HipL = new Joint();
        Joint HipR = new Joint();
        Joint FL = new Joint();
        Joint FR = new Joint();

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            kinectSensorChooser1.KinectSensorChanged += new DependencyPropertyChangedEventHandler(kinectSensorChooser1_KinectSensorChanged);     
        }
        
        //Initializes recorder
        void initializeRecorder()
        {
            jointAngles = new List<string>();
            dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
            jointTime = new Stopwatch();
            //Timer to record joint angles every tenth of a second
            dispatcherTimer.Tick += new EventHandler(recordSkeletonData);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, 100);
            dispatcherTimer.Start();
            jointTime.Start();
        }
        //Records skeleton data
        private void recordSkeletonData(object sender, EventArgs e)
        {
            currentTime = Convert.ToInt16(jointTime.ElapsedMilliseconds / 100);
            try
            {
                if (first == null)
                    return;
                jointAngles.Add("Time: " + currentTime.ToString());
                
                //Add Angles
                jointAngles.Add("LEA");
                jointAngles.Add(AngleBetweenJoints(first.Joints[JointType.HandLeft], first.Joints[JointType.ElbowLeft], first.Joints[JointType.ShoulderLeft]).ToString());
                jointAngles.Add("LSA");
                jointAngles.Add(AngleBetweenJoints(first.Joints[JointType.ElbowLeft], first.Joints[JointType.ShoulderLeft], first.Joints[JointType.ShoulderCenter]).ToString());
                jointAngles.Add("REA");
                jointAngles.Add(AngleBetweenJoints(first.Joints[JointType.HandRight], first.Joints[JointType.ElbowRight], first.Joints[JointType.ShoulderRight]).ToString());
                jointAngles.Add("RSA");
                jointAngles.Add(AngleBetweenJoints(first.Joints[JointType.ElbowRight], first.Joints[JointType.ShoulderRight], first.Joints[JointType.ShoulderCenter]).ToString());
                jointAngles.Add("LHA");
                jointAngles.Add(AngleBetweenJoints(first.Joints[JointType.ShoulderLeft], first.Joints[JointType.HipLeft], first.Joints[JointType.KneeLeft]).ToString());
                jointAngles.Add("RHA");
                jointAngles.Add(AngleBetweenJoints(first.Joints[JointType.ShoulderRight], first.Joints[JointType.HipRight], first.Joints[JointType.KneeRight]).ToString());
                jointAngles.Add("LKA");
                jointAngles.Add(AngleBetweenJoints(first.Joints[JointType.HipLeft], first.Joints[JointType.KneeLeft], first.Joints[JointType.FootLeft]).ToString());
                jointAngles.Add("RKA");
                jointAngles.Add(AngleBetweenJoints(first.Joints[JointType.HipRight], first.Joints[JointType.KneeRight], first.Joints[JointType.FootRight]).ToString());

                //Add Speeds
                jointAngles.Add("LHV");
                jointAngles.Add(SpeedOfJoint(first.Joints[JointType.HandLeft], HL));
                jointAngles.Add("RHV");
                jointAngles.Add(SpeedOfJoint(first.Joints[JointType.HandRight], HR));
                jointAngles.Add("LSV");
                jointAngles.Add(SpeedOfJoint(first.Joints[JointType.ShoulderLeft], SL));
                jointAngles.Add("RSV");
                jointAngles.Add(SpeedOfJoint(first.Joints[JointType.ShoulderRight], SR));
                jointAngles.Add("LHipV");
                jointAngles.Add(SpeedOfJoint(first.Joints[JointType.HipLeft], HipL));
                jointAngles.Add("RHipV");
                jointAngles.Add(SpeedOfJoint(first.Joints[JointType.HipRight], HipR));
                jointAngles.Add("LFV");
                jointAngles.Add(SpeedOfJoint(first.Joints[JointType.FootLeft], FL));
                jointAngles.Add("RFV");
                jointAngles.Add(SpeedOfJoint(first.Joints[JointType.FootRight], FR));
                jointAngles.Add("LKV");
                jointAngles.Add(SpeedOfJoint(first.Joints[JointType.KneeLeft], KL));
                jointAngles.Add("RKV");
                jointAngles.Add(SpeedOfJoint(first.Joints[JointType.KneeRight], KR));

                //Mark the end of a frame
                jointAngles.Add("End");

                if (currentTime != (previousTime + 1))
                {
                    jointAngles.Add("Time: " + currentTime.ToString());
                    //Add Angles
                    jointAngles.Add("LEA");
                    jointAngles.Add(AngleBetweenJoints(first.Joints[JointType.HandLeft], first.Joints[JointType.ElbowLeft], first.Joints[JointType.ShoulderLeft]).ToString());
                    jointAngles.Add("LSA");
                    jointAngles.Add(AngleBetweenJoints(first.Joints[JointType.ElbowLeft], first.Joints[JointType.ShoulderLeft], first.Joints[JointType.ShoulderCenter]).ToString());
                    jointAngles.Add("REA");
                    jointAngles.Add(AngleBetweenJoints(first.Joints[JointType.HandRight], first.Joints[JointType.ElbowRight], first.Joints[JointType.ShoulderRight]).ToString());
                    jointAngles.Add("RSA");
                    jointAngles.Add(AngleBetweenJoints(first.Joints[JointType.ElbowRight], first.Joints[JointType.ShoulderRight], first.Joints[JointType.ShoulderCenter]).ToString());
                    jointAngles.Add("LHA");
                    jointAngles.Add(AngleBetweenJoints(first.Joints[JointType.ShoulderLeft], first.Joints[JointType.HipLeft], first.Joints[JointType.KneeLeft]).ToString());
                    jointAngles.Add("RHA");
                    jointAngles.Add(AngleBetweenJoints(first.Joints[JointType.ShoulderRight], first.Joints[JointType.HipRight], first.Joints[JointType.KneeRight]).ToString());
                    jointAngles.Add("LKA");
                    jointAngles.Add(AngleBetweenJoints(first.Joints[JointType.HipLeft], first.Joints[JointType.KneeLeft], first.Joints[JointType.FootLeft]).ToString());
                    jointAngles.Add("RKA");
                    jointAngles.Add(AngleBetweenJoints(first.Joints[JointType.HipRight], first.Joints[JointType.KneeRight], first.Joints[JointType.FootRight]).ToString());

                    //Add Speeds
                    jointAngles.Add("LHV");
                    jointAngles.Add(SpeedOfJoint(first.Joints[JointType.HandLeft], HL));
                    jointAngles.Add("RHV");
                    jointAngles.Add(SpeedOfJoint(first.Joints[JointType.HandRight], HR));
                    jointAngles.Add("LSV");
                    jointAngles.Add(SpeedOfJoint(first.Joints[JointType.ShoulderLeft], SL));
                    jointAngles.Add("RSV");
                    jointAngles.Add(SpeedOfJoint(first.Joints[JointType.ShoulderRight], SR));
                    jointAngles.Add("LHipV");
                    jointAngles.Add(SpeedOfJoint(first.Joints[JointType.HipLeft], HipL));
                    jointAngles.Add("RHipV");
                    jointAngles.Add(SpeedOfJoint(first.Joints[JointType.HipRight], HipR));
                    jointAngles.Add("LFV");
                    jointAngles.Add(SpeedOfJoint(first.Joints[JointType.FootLeft], FL));
                    jointAngles.Add("RFV");
                    jointAngles.Add(SpeedOfJoint(first.Joints[JointType.FootRight], FR));
                    jointAngles.Add("LKV");
                    jointAngles.Add(SpeedOfJoint(first.Joints[JointType.KneeLeft], KL));
                    jointAngles.Add("RKV");
                    jointAngles.Add(SpeedOfJoint(first.Joints[JointType.KneeRight], KR));

                    //Mark the end of a frame
                    jointAngles.Add("End");
                }

                previousTime = currentTime;

                //Save joint positions from this frame to calculate speed for next frame
                HL = first.Joints[JointType.HandLeft];
                HR = first.Joints[JointType.HandRight];
                SL = first.Joints[JointType.ShoulderLeft];
                SR = first.Joints[JointType.ShoulderRight];
                KL = first.Joints[JointType.KneeLeft];
                KR = first.Joints[JointType.KneeRight];
                HipL = first.Joints[JointType.HipLeft];
                HipR = first.Joints[JointType.HipRight];
                FL = first.Joints[JointType.FootLeft];
                FR = first.Joints[JointType.FootRight];

            }
            catch (NullReferenceException)
            {

            }
        }

        private string SpeedOfJoint(Joint second, Joint first)
        {
            double dx, dy, dz, speed;
            dx = second.Position.X - first.Position.X;
            dy = second.Position.Y - first.Position.Y;
            dz = second.Position.Z - first.Position.Z;

            speed = Math.Sqrt( (dx * dx) + (dy * dy) + (dz * dz)) * 100;
            int speedRounded = Convert.ToInt32(speed);
            return speedRounded.ToString();
        }

        //Stop recording skeleton data and save the 'jointAngles' string collection to a text file
        private void stopRecordSkeletonData()
        {
            jointTime.Reset();
            dispatcherTimer.Stop();            
        }


        void Record_Button(object sender, RoutedEventArgs e)
        {
            if (!recording)
            {
                recordButton.Opacity = 0;
                stopButton.Opacity = 1;
                initializeRecorder();
                recording = true;
            }
            else
            {
                recordButton.Opacity = 1;
                stopButton.Opacity = 0;
                // WriteAllLines creates a file, writes a collection of strings to the file, 
                // and then closes the file.
                stopRecordSkeletonData();
                recording = false;
                SaveFileAs.Visibility = System.Windows.Visibility.Visible;
            }
        }


        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            // YesButton Clicked! Let's hide our InputBox and handle the input text.
            SaveFileAs.Visibility = System.Windows.Visibility.Collapsed;

            // Do something with the Input
            String input = InputTextBox.Text;

            String path = System.AppDomain.CurrentDomain.BaseDirectory;
            path = System.IO.Directory.GetParent(path).FullName;
            path = System.IO.Directory.GetParent(path).FullName;
            path = System.IO.Directory.GetParent(path).FullName;
            path = System.IO.Directory.GetParent(path).FullName;
            
            System.IO.File.WriteAllLines(path + "\\Recordings\\" + input + ".txt", jointAngles);

            // Clear InputBox.
            InputTextBox.Text = String.Empty;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            // NoButton Clicked! Let's hide our InputBox.
            SaveFileAs.Visibility = System.Windows.Visibility.Collapsed;

            // Clear InputBox.
            InputTextBox.Text = String.Empty;
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
    }
}
