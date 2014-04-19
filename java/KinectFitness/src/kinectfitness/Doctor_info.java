/*
 * To change this template, choose Tools | Templates
 * and open the template in the editor.
 */
package kinectfitness;

import java.io.IOException;
import java.io.ObjectInputStream;
import java.net.ServerSocket;
import java.net.Socket;
import java.util.Timer;
import java.util.TimerTask;
import java.util.logging.Level;
import java.util.logging.Logger;
import javax.swing.JLabel;
import javax.swing.JPanel;
import javax.xml.parsers.ParserConfigurationException;
import javax.xml.transform.TransformerConfigurationException;
import javax.xml.transform.TransformerException;
import org.w3c.dom.Element;

/**
 * This class sets and gets information from the patient. 
 * It receives the patient IP and then use it to establish the connection
 * 
 * @author Marcus
 */
class Doctor_info extends Thread {

    Doctor doctor;
    boolean foundIP = false;
    int localPort = 5020;
    String remoteIP = "";
    
    // return True if an IP was found 
    public boolean foundIP() {
        return this.foundIP;
    }
    
    
    // set the LocalPort
    public void setLocalPort(int port) {
        this.localPort = port;
    }

    // get the remote IP, it should be used to make the connection
    public String getRemoteIP() {
        return this.remoteIP;
    }

    public Doctor_info(Doctor doctor) {
        this.doctor = doctor;
    }

    @Override
    public void run() {
        Info patient_info;
        ServerSocket servSocket;
        Socket fromClientSocket;
        XMLExporter exporter = null;

        int cTosPortNumber = this.localPort;
        try {
            
            // Create sockets and then waits for connection
            System.out.println("Info Port: " + cTosPortNumber);
            servSocket = new ServerSocket(cTosPortNumber);
            fromClientSocket = servSocket.accept();
            
            // Create new info class. The class is responsible for storing the patient info 
            ObjectInputStream ois = new ObjectInputStream(fromClientSocket.getInputStream());
            Info info = new Info();
            
            
            // According to the patient number a specific port is reserved
            String patientId = "";
            switch (cTosPortNumber) {
                case 5020:
                    patientId = "patient1";
                    break;
                case 5021:
                    patientId = "patient2";
                    break;
                case 5022:
                    patientId = "patient3";
                    break;
                case 5023:
                    patientId = "patient4";
                    break;
                case 5024:
                    patientId = "patient5";
                    break;
                case 5025:
                    patientId = "patient6";
                    break;
                case 5026:
                    patientId = "patient7";
                    break;
                case 5027:
                    patientId = "patient8";
                    break;

            }

            exporter = new XMLExporter(patientId);
            Element exercise = exporter.startXML();

            int counter = 0;
            
            
            while ((patient_info = (Info) ois.readObject()) != null) {
                
                // If the connection was established and an IP found then gets the remoteIP and 
                //starts the video streaming 
                if (!foundIP) {
                    foundIP = true;
                    remoteIP = fromClientSocket.getRemoteSocketAddress().toString().replaceAll("(/|(:[0-9]+))", "");
                    //System.out.println("PI: " + remoteIP);
                    doctor.startStreaming(remoteIP);
                    doctor.updateStatus("Connected to: " + remoteIP);
                }
                doctor.updateInfo(patient_info);

                // replace with timer function if can 
                //(without making exercise, patient_into and exporter into final variables)
                if(counter == 1000)
                {
                   counter = 0;
                   exporter.createDataElement(exercise, patient_info); 
                }
                counter++;
            }

            fromClientSocket.close();
        } catch (IOException | ClassNotFoundException ex) {
            // for debugging
//            Logger.getLogger(Doctor.class.getName()).log(Level.SEVERE, null, ex);
            // message for the user instead of error message
            if (exporter != null) {
                exporter.exportXML();
            }
            System.out.println("Your patient has left the session.");
        } catch (ParserConfigurationException ex) {
            Logger.getLogger(Doctor_info.class.getName()).log(Level.SEVERE, null, ex);
        } catch (TransformerConfigurationException ex) {
            Logger.getLogger(Doctor_info.class.getName()).log(Level.SEVERE, null, ex);
        } catch (TransformerException ex) {
            Logger.getLogger(Doctor_info.class.getName()).log(Level.SEVERE, null, ex);
        }
    }
}
