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

public class FitnessMainJava {

    public static void main(String[] args) throws URISyntaxException, IOException {
        NativeLibrary.addSearchPath("libvlc", "./");
        NativeLibrary.addSearchPath("libvlccore", "libvlccore");
        createInputDialog();
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
                                    FitnessMainJava.startKinectApp();
                                    Patient patient = new Patient();
                                    patient.Patient("142.244.153.10", index);
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
        final Process process = Runtime.getRuntime().exec(exeFilePath);

    }
}
