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
import javax.swing.JFrame;
import javax.swing.JOptionPane;
import javax.swing.JPanel;

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
            // send patients alert to connect their bluetooth devices
            socket1 = new Socket(doctorIP, portNumber);
            ObjectOutputStream oos = new ObjectOutputStream(socket1.getOutputStream());
            
            BluetoothServer server = new BluetoothServer(oos);
            server.runTest();
            //server.connectToPhone();

            socket1.close();
        } catch (IOException ex) {
            Logger.getLogger(Patient.class.getName()).log(Level.SEVERE, null, ex);
        } catch (InterruptedException ex) {
            Logger.getLogger(Patient_info.class.getName()).log(Level.SEVERE, null, ex);
        }
    }

    public void alert(String message) {
        JFrame frame = new JFrame("DialogDemo");
        frame.setDefaultCloseOperation(JFrame.EXIT_ON_CLOSE);

        //Create and set up the content pane.
        JPanel newContentPane = new JPanel();

        newContentPane.setOpaque(true); //content panes must be opaque
        frame.setContentPane(newContentPane);

        //Display the window.
        frame.pack();
        frame.setVisible(true);
    }
}
