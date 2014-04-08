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

    public Patient_info(String doctorIP) {
        this.doctorIP = doctorIP;
    }

    @Override
    public void run() {

        Socket socket1;
        int portNumber = 5020;
        try {
            //socket1 = new Socket(doctorIP, portNumber);
            //ObjectOutputStream oos = new ObjectOutputStream(socket1.getOutputStream());
            BluetoothServer server = new BluetoothServer();
            server.runTest();

            //socket1.close();
        } catch (IOException ex) {
            Logger.getLogger(Patient.class.getName()).log(Level.SEVERE, null, ex);
//        } catch (InterruptedException ex) {
//            Logger.getLogger(Patient_info.class.getName()).log(Level.SEVERE, null, ex);
        }
    }
}
