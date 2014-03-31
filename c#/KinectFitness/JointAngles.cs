using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KinectFitness
{

    class JointAngles
    {
        public int leftShoulder;
        public int leftElbow;
        public int leftHip;
        public int leftKnee;
        public int rightShoulder;
        public int rightElbow;
        public int rightHip;
        public int rightKnee;

        public JointAngles(int leftShoulder, int leftElbow, int leftHip, int leftKnee,
            int rightShoulder, int rightElbow, int rightHip, int rightKnee)
        {
            this.leftShoulder = leftShoulder;
            this.leftElbow = leftElbow;
            this.leftHip = leftHip;
            this.leftKnee = leftKnee;
            this.rightShoulder = rightShoulder;
            this.rightElbow = rightElbow;
            this.rightHip = rightHip;
            this.rightKnee = rightKnee;
        }

        public JointAngles(){ }


    }
}
