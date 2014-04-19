/*
 * The purpose of this class is to send the data received and processed
 * from the BluetoothServer class to the doctor's side of the application
 */
package kinectfitness;

import java.io.IOException;
import java.io.ObjectOutputStream;
import java.net.Socket;
import java.util.logging.Level;
import java.util.logging.Logger;

/**
 * This class establishes socket connection with the doctor to send the
 * Bluetooth data to the doctor's side of the application.  Depending on the
 * patient label or password they have entered in FitnessMain class UI, the
 * port number is different.  The port number ranges from 5020 to 5027 depending
 * on the password typed from the FitnessMain class.
 * 
 * @author Marcus
 */
public class Patient_info extends Thread {

    // IP address of the doctor
    private String doctorIP;
    // the index associated with the patient (0 to 7)
    private int index;
    
    /**
     * The main constructor of the class
     * @param doctorIP          IP address of the doctor
     * @param index             index number associated with the patient (0-7)
     */
    public Patient_info(String doctorIP, int index) {
        this.doctorIP = doctorIP;
        this.index = index;
    }

    /**
     * This main method is an inherited method from the Thread class and
     * it runs the instance of the Patient_info class as a new thread. 
     * It establishes the connection to the doctor and to the phone by calling
     * the method from the BluetoothServer class.
     */
    @Override
    public void run() {
        Socket socket1;
        int portNumber = 5020 + index;
        try {
            socket1 = new Socket(doctorIP, portNumber);
            ObjectOutputStream oos = new ObjectOutputStream(socket1.getOutputStream());
            BluetoothServer server = new BluetoothServer(oos);
            server.connectToPhone();
            // for debugging
            //server.runTest();
            socket1.close();
        } catch (IOException ex) {
            Logger.getLogger(Patient.class.getName()).log(Level.SEVERE, null, ex);
        } 
    }
}