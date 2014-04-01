using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KinectFitness
{
    class JointSpeeds
    {
        public int leftShoulder;
        public int leftHand;
        public int leftHip;
        public int leftFoot;
        public int leftKnee;
        public int rightShoulder;
        public int rightHand;
        public int rightHip;
        public int rightKnee;
        public int rightFoot;

        public JointSpeeds(int leftShoulder, int leftElbow, int leftHip, int leftKnee, int leftFoot,
            int rightShoulder, int rightElbow, int rightHip, int rightKnee, int rightFoot)
        {
            this.leftShoulder = leftShoulder;
            this.leftHand = leftElbow;
            this.leftHip = leftHip;
            this.leftKnee = leftKnee;
            this.rightShoulder = rightShoulder;
            this.rightHand = rightElbow;
            this.rightHip = rightHip;
            this.rightKnee = rightKnee;
            this.leftFoot = leftFoot;
            this.rightFoot = rightFoot;
        }

        public JointSpeeds() { }


    }
}
