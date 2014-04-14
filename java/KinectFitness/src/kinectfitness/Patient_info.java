/*
 * To change this template, choose Tools | Templates
 * and open the template in the editor.
 */
package kinectfitness;

import java.io.IOException;
import java.io.ObjectOutputStream;
import java.net.InetAddress;
import java.net.ServerSocket;
import java.net.Socket;
import java.util.Random;
import java.util.logging.Level;
import java.util.logging.Logger;
import java.util.Timer;

/**
 *
 * @author Marcus
 */
public class Patient_info extends Thread {

    private String doctorIP;
    private int index;
    
    public Patient_info(String doctorIP, int index) {
        this.doctorIP = doctorIP;
        this.index = index;
    }

    @Override
    public void run() {
//        Info patient_info = new Info();
//        int counter = 0;
        Socket socket1;
        Socket csharpsocket;
        int portNumber = 5020 + index;
        try {
//            socket1 = new Socket(doctorIP, portNumber);
//            ObjectOutputStream oos = new ObjectOutputStream(socket1.getOutputStream());
            BluetoothServer server = new BluetoothServer();
            server.runTest();
//            while (counter != 1000)
//            {
//                patient_info.heart_rate = randInt(60, 180);
//                patient_info.blood_pressure[0] = randInt(90, 140);
//                patient_info.blood_pressure[1] = randInt(60, 100);
//                patient_info.ECG = randInt(60, 200);
//                patient_info.O2 = randInt(0, 100);
//                Thread.sleep(100);
//                ++counter;
//                System.out.println(patient_info.heart_rate);
//                oos.writeObject(patient_info);
//                oos.reset();

//            }
//            socket1.close();
        } catch (IOException ex) {
            Logger.getLogger(Patient.class.getName()).log(Level.SEVERE, null, ex);
        } catch (InterruptedException ex) {
            Logger.getLogger(Patient_info.class.getName()).log(Level.SEVERE, null, ex);
        }
    }
//    public static int randInt(int min, int max) {
//
//        // Usually this can be a field rather than a method variable
//        Random rand = new Random();
//
//        // nextInt is normally exclusive of the top value,
//        // so add 1 to make it inclusive
//        int randomNum = rand.nextInt((max - min) + 1) + min;
//
//        return randomNum;
//    }
}
