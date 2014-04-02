using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KinectFitness
{
    class JointSpeeds
    {
        public double leftShoulder;
        public double leftHand;
        public double leftHip;
        public double leftFoot;
        public double leftKnee;
        public double rightShoulder;
        public double rightHand;
        public double rightHip;
        public double rightKnee;
        public double rightFoot;

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
