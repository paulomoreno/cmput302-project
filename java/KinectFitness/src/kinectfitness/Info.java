/*
 * To change this template, choose Tools | Templates
 * and open the template in the editor.
 */
package kinectfitness;

import java.io.Serializable;

/**
 *
 * @author Marcus
 */
class Info implements Serializable {
     String heart_rate;
     String[] blood_pressure = new String[2];
     //int ECG;
     String O2;
}
