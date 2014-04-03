using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX;
using SlimDX.DirectInput;
using System.Globalization;
using System.Threading;

namespace KinectFitness
{
    class Controller
    {

        //Control buttons and sticks

        int stateX;
        int stateY;
        int stateZ;

        int stateRotationX;
        int stateRotationY;
        int stateRotationZ;

        //0 .. 1
        int[] slider;

        //0 ..3
        public volatile int[] pov;

        bool[] buttons;


        private string test = null;

        private Joystick joystick;
        private JoystickState state;

        private int numPOVs;
        private int SlidersCount;

        Thread newThread;

        public Controller()
        {

            state = new JoystickState();

            // initialize direct input
            DirectInput dinput = new DirectInput();

            // search for all the devices
            foreach (DeviceInstance device in dinput.GetDevices(DeviceClass.GameController, DeviceEnumerationFlags.AttachedOnly))
            {

                // create the device
                try
                {
                    joystick = new Joystick(dinput, device.InstanceGuid);
                    //joystick.SetCooperativeLevel(this, CooperativeLevel.Exclusive | CooperativeLevel.Foreground);
                    break;
                }
                catch (DirectInputException) { }
            }

            if (joystick == null)
            {
               // MessageBox.Show("There are no attached to the system");
                return;
            }
            else
            {
                //MessageBox.Show("Controller detected!");
            }

            foreach (DeviceObjectInstance deviceObject in joystick.GetObjects())
            {
                if ((deviceObject.ObjectType & ObjectDeviceType.Axis) != 0)
                {
                    joystick.GetObjectPropertiesById((int)deviceObject.ObjectType).SetRange(-1000, 1000);
                    //UpdateControl(deviceObject);
                }
            }

            joystick.Acquire();
        }

        public void ReleaseDevice()
        {
            newThread.Abort();
            if (joystick != null)
            {
                joystick.Unacquire();
                joystick.Dispose();
            }
            joystick = null;

        }

        public bool isConnected()
        {
            return joystick == null ? false : true;
        }
        public void updateStates()
        {

            newThread = new Thread(() =>
            {
                _updateStates();

            });

            newThread.Start();
        }


        private void _print()
        {
            Console.WriteLine(stateX);
            Console.WriteLine(stateY);
            Console.WriteLine(stateZ);
            Console.WriteLine(stateRotationX);
            Console.WriteLine(stateRotationY);
            Console.WriteLine(stateRotationZ);
            Console.WriteLine(slider[0] + " " + slider[1]);
            Console.WriteLine(pov[0] + " " + pov[1] + " " + pov[2]);

            /*
            for (int i = 0; i < buttons.Length; i++)
            {
                Console.Write(buttons[i]);
            }*/

        }
        public int getPOV() {

            return this.pov[0];
        }

        public bool getButton(int i) 
        {
            return this.buttons[i];
        }

        private void _updateStates()
        {

            while (true)
            {

                state = joystick.GetCurrentState();

                stateX = state.X;
                stateY = state.Y;
                stateZ = state.Z;

                stateRotationX = state.RotationX;
                stateRotationY = state.RotationY;
                stateRotationZ = state.RotationZ;

                //0 .. 1
                slider = state.GetSliders();

                //0 ..3
                pov = state.GetPointOfViewControllers(); ;

                buttons = state.GetButtons();
                //MessageBox.Show(buttons[0].ToString());
                /*for (int i = 0; i < buttons.Length; i++)
                {
                    //MessageBox.Show("Im inside a loop");
                    //Console.WriteLine("Running inside loop ");

                    //if one of the buttons was pressed then is set to 1
                    if (buttons[i])
                    {

                        //MessageBox.Show("Button pressed " + i);
                    }
                }*/
                // Console.WriteLine(stateX);
                // Console.WriteLine("Running ... ");
                // _print();
                Thread.Sleep(50);
            }
        }
    }
}
