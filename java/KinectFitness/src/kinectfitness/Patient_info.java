/*
 * To change this template, choose Tools | Templates
 * and open the template in the editor.
 */
package kinectfitness;

import java.io.IOException;
import java.io.ObjectOutputStream;
import java.net.Socket;
import java.util.logging.Level;
import java.util.logging.Logger;

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
        Socket socket1;
        int portNumber = 5020 + index;
        try {
            socket1 = new Socket(doctorIP, portNumber);
            ObjectOutputStream oos = new ObjectOutputStream(socket1.getOutputStream());
            BluetoothServer server = new BluetoothServer(oos);
            //server.connectToPhone();
            server.runTest();
            //socket1.close();
        } catch (IOException ex) {
            Logger.getLogger(Patient.class.getName()).log(Level.SEVERE, null, ex);
//        } catch (InterruptedException ex) {
//            Logger.getLogger(Patient_info.class.getName()).log(Level.SEVERE, null, ex);
        } 
    }
}
