package kinectfitness;

import java.io.Serializable;

/**
 * This class represents the different data
 * collected from the Bluetooth devices to monitor the 
 * patients vital signs.
 * 
 * @author Marcus
 */
class Info implements Serializable {
     String heart_rate;
     String[] blood_pressure = new String[2];
     //int ECG;
     String O2;
     String memo;
}
