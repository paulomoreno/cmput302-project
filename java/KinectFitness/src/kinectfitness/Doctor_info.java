/*
 * To change this template, choose Tools | Templates
 * and open the template in the editor.
 */
package kinectfitness;

import java.io.IOException;
import java.io.ObjectInputStream;
import java.net.ServerSocket;
import java.net.Socket;
import java.util.logging.Level;
import java.util.logging.Logger;
import javax.swing.JLabel;
import javax.swing.JPanel;

/**
 *
 * @author Marcus
 */
class Doctor_info extends Thread {
    Doctor doctor;
    boolean foundIP = false;
    String remoteIP = "";
    
    public boolean foundIP(){
        return this.foundIP;
    }
    
    public String getRemoteIP(){
        return this.remoteIP;
    }
    
    public Doctor_info(Doctor doctor)
    {
        this.doctor = doctor;
    }
    
    @Override
    public void run()
    {
        Info patient_info;
        ServerSocket servSocket;
        Socket fromClientSocket;

        int cTosPortNumber = 5020;
        try {
            servSocket = new ServerSocket(cTosPortNumber);
            fromClientSocket = servSocket.accept();
            ObjectInputStream ois = new ObjectInputStream(fromClientSocket.getInputStream()); 
            while ((patient_info = (Info) ois.readObject()) != null)
            {
                if (!foundIP){
                    foundIP = true;
                    remoteIP = fromClientSocket.getRemoteSocketAddress().toString().replaceAll("(/|(:[0-9]+))", "");
                    //System.out.println("PI: " + remoteIP);
                    doctor.startStreaming(remoteIP);
                    doctor.updateStatus(" Connected to: " + remoteIP);
                }
                doctor.updateInfo(patient_info);
            }
            
            fromClientSocket.close();
        } catch (IOException | ClassNotFoundException ex) {
            Logger.getLogger(Doctor.class.getName()).log(Level.SEVERE, null, ex);
        }
    }
    
}
