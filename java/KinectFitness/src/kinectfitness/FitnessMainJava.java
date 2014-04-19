package kinectfitness;

import com.sun.jna.NativeLibrary;
import java.awt.event.ActionEvent;
import java.awt.event.ActionListener;
import java.io.BufferedReader;
import java.io.File;
import java.io.IOException;
import java.io.InputStreamReader;
import java.net.ServerSocket;
import java.net.Socket;
import java.net.URISyntaxException;
import java.net.URL;
import java.util.logging.Level;
import java.util.logging.Logger;
import javax.swing.*;

/**
 * This is the main class of the Kinect Fitness application.
 * It creates the UI for the user to input the password to access 
 * the different side of the application.  If patient1, patient2, patient3, patient4,
 * patient5, patient6, patient7 or patient8 is entered by the user then the
 * application launches the patient application by calling the methods in the
 * Patient class.  Also, for the patient side application, the Kinect application is
 * launched at the same time as well.
 * If the password entered is doctor, then the application launches the
 * method in the Doctor class.
 * 
 * @author Ga Young Kim
 */
public class FitnessMainJava {

    /**
     * The main method of the application.  It calls the method to create
     * the main dialog window for the user to input the password and also loads
     * the dlls for the VLCJ library.
     * 
     * @param args
     * @throws URISyntaxException
     * @throws IOException 
     */
    public static void main(String[] args) throws URISyntaxException, IOException {
        
        // Paths to use VLCj library
        NativeLibrary.addSearchPath("libvlc", "./");
        NativeLibrary.addSearchPath("libvlccore", "libvlccore");
        createInputDialog();
    }

    /**
     * This method creates the main UI for the user to input the password
     * to identify themselves as either the patient or the doctor.  Also, 
     * depending on which password for the patient they associated themselves with,
     * the index number if different (from 0 to 7). This index number corresponds
     * to the location where the patient video will be displayed in the doctor
     * side application and which port would be used from 5020 to 5027 to establish
     * the TCP connection.
     */
    private static void createInputDialog() {
        // create the main UI
        final JFrame dialogWindow = new JFrame("Kinect Fitness");
        dialogWindow.setDefaultCloseOperation(JFrame.EXIT_ON_CLOSE);

        JPanel panel = new JPanel();

        JLabel label1 = new JLabel("<html>Welcome to Kinect Fitness!<br><br></html>", SwingConstants.CENTER);
        JLabel label2 = new JLabel("<html><p>Please input the given password</p><p> below to access the program.</p></html>", SwingConstants.CENTER);
        final JTextField textField = new JTextField(20);

        JButton button = new JButton("Enter");
        button.addActionListener(new ActionListener() {
            // If the password is patients, then depending on the password,
            // set a different index.  If it's for the doctor then the
            // doctor application is launched
            @Override
            public void actionPerformed(ActionEvent e) {
                String value = textField.getText();

                int patientindex = 10;
                switch (value) {
                    case "patient1":
                        patientindex = 0;
                        break;
                    case "patient2":
                        patientindex = 1;
                        break;
                    case "patient3":
                        patientindex = 2;
                        break;
                    case "patient4":
                        patientindex = 3;
                        break;
                    case "patient5":
                        patientindex = 4;
                        break;
                    case "patient6":
                        patientindex = 5;
                        break;
                    case "patient7":
                        patientindex = 6;
                        break;
                    case "patient8":
                        patientindex = 7;
                        break;
                    case "doctor":
                        // launch the doctor application
                        DoctorViewFrame window = new DoctorViewFrame();

                        window.sendInfo(182, 2);
                        window.sendInfo(150, 2);
                        dialogWindow.dispose();

                        break;
                    default:
                        patientindex = 0;
                        break;
                }

                final int index = patientindex;
                // doctor was chosen
                if (index < 10) {
                    try {
                        dialogWindow.dispose();

                        new Thread() {
                            public void run() {
                                try {
                                    // launch the Kinect application
                                    FitnessMainJava.startKinectApp();
                                    // launch the video and audio chat for the 
                                    // patient and also establish a connection
                                    // with the doctor application 
                                    Patient patient = new Patient();
                                    patient.Patient("10.0.1.4", index);
                                    Patient.startPatient(patient);
                                } catch (IOException ex) {
                                    System.out.println("IOException");
                                    Logger.getLogger(FitnessMainJava.class.getName()).log(Level.SEVERE, null, ex);
                                } catch (URISyntaxException ex) {
                                    System.out.println("URISyntax");
                                    Logger.getLogger(FitnessMainJava.class.getName()).log(Level.SEVERE, null, ex);
                                } catch (Exception ex) {
                                    System.out.println("Exception: " + ex);
                                    Logger.getLogger(FitnessMainJava.class.getName()).log(Level.SEVERE, null, ex);
                                }
                            }
                        }.start();
                    } catch (Exception ex) {
                        Logger.getLogger(FitnessMainJava.class.getName()).log(Level.SEVERE, null, ex);
                    }
                }
            }
        });

        panel.add(label1);
        panel.add(label2);
        panel.add(textField);
        panel.add(button);

        dialogWindow.getContentPane().add(panel);

        dialogWindow.pack(); // sets size relative to content
        dialogWindow.setSize(300, 200);
        dialogWindow.setLocationRelativeTo(null);
        dialogWindow.setVisible(true);

    }

    /**
     * This method is used to launch the Kinect application.
     * @throws IOException
     * @throws URISyntaxException 
     */
    private static void startKinectApp() throws IOException, URISyntaxException {
        // get the path to the root project directory
        ClassLoader classLoader = Thread.currentThread().getContextClassLoader();
        URL url = classLoader.getResource("");
        File file = new File(url.toURI());
        String[] filepathinfo = file.getAbsolutePath().split("\\\\");

        String exeFilePath = "" + filepathinfo[0];

        for (int i = 1; i < filepathinfo.length - 4; i++) {
            exeFilePath += "/" + filepathinfo[i];
        }

        exeFilePath += "/c#/KinectFitness/bin/x64/Release/KinectFitness.exe";
        final Process process = Runtime.getRuntime().exec(exeFilePath);

    }
}
