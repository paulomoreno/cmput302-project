package kinectfitness;

import com.sun.jna.NativeLibrary;
import java.awt.event.ActionEvent;
import java.awt.event.ActionListener;
import java.io.File;
import java.io.IOException;
import java.net.URISyntaxException;
import java.net.URL;
import java.util.logging.Level;
import java.util.logging.Logger;
import javax.swing.*;

public class FitnessMainJava {

    public static void main(String[] args) throws URISyntaxException, IOException {
        NativeLibrary.addSearchPath("libvlc", "./");
        NativeLibrary.addSearchPath("libvlccore", "libvlccore");
        createInputDialog();




        // TODO Auto-generated method stub
        // 1. ask them for password --> to branch off to doctor vs patient
        // 2a. if patient, ask for their age
        // 3a. popup with ip address, instruction to add this number to android app
        // 3b. instruction to put heart monitor & V02 and starting the android phone
        // 3c. if continue is pressed, execute C# application
        // 2b. if doctor, display panels?
    }

    private static void createInputDialog() {
        final JFrame dialogWindow = new JFrame("Kinect Fitness");
        dialogWindow.setDefaultCloseOperation(JFrame.EXIT_ON_CLOSE);

        JPanel panel = new JPanel();

        JLabel label1 = new JLabel("<html>Welcome to Kinect Fitness!<br><br></html>", SwingConstants.CENTER);
        JLabel label2 = new JLabel("<html><p>Please input the given password</p><p> below to access the program.</p></html>", SwingConstants.CENTER);
        final JTextField textField = new JTextField(20);

        JButton button = new JButton("Enter");
        button.addActionListener(new ActionListener() {
            @Override
            public void actionPerformed(ActionEvent e) {
                String value = textField.getText();

                switch (value) {
                    case "patient1":
                        try {
                            FitnessMainJava.startKinectApp();
                            
                            // ----------------------------------------------
                            // April 7
                            // currently set for testing without 
                            // bluetooth --> uncomment code in patient.start()
                            // ----------------------------------------------
                            Patient patient = new Patient();
                            patient.Patient("142.244.151.88");
                            Patient.startPatient(patient);
                            

                            dialogWindow.dispose();
                        } catch (IOException ex) {
                            Logger.getLogger(FitnessMainJava.class.getName()).log(Level.SEVERE, null, ex);
                        } catch (URISyntaxException ex) {
                            Logger.getLogger(FitnessMainJava.class.getName()).log(Level.SEVERE, null, ex);
                        } catch (Exception ex) {
                            Logger.getLogger(FitnessMainJava.class.getName()).log(Level.SEVERE, null, ex);
                        }
                        break;
                    case "doctor":
                        DoctorViewFrame window = new DoctorViewFrame();            
                        
                        window.sendInfo(182, 2);
                        window.sendInfo(150, 2);
                        dialogWindow.dispose();
                        
                        break;
                    default:
                        break;
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

    // getting path to KinectFitness.exe file
    private static void startKinectApp() throws IOException, URISyntaxException {
        ClassLoader classLoader = Thread.currentThread().getContextClassLoader();
        URL url = classLoader.getResource("");
        File file = new File(url.toURI());
        String[] filepathinfo = file.getAbsolutePath().split("\\\\");

        String exeFilePath = "" + filepathinfo[0];

        for (int i = 1; i < filepathinfo.length - 4; i++) {
            exeFilePath += "/" + filepathinfo[i];
        }

        exeFilePath += "/c#/KinectFitness/bin/x64/Release/KinectFitness.exe";
        Process process = Runtime.getRuntime().exec(exeFilePath);
    }
}
