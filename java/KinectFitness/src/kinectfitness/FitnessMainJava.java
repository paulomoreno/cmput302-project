package kinectfitness;

import java.io.File;
import java.io.IOException;
import java.net.URISyntaxException;
import java.net.URL;
import javax.swing.*;

public class FitnessMainJava {

	public static void main(String[] args) throws URISyntaxException, IOException {
            //createInputDialog();
                
            // getting path to KinectFitness.exe file
            ClassLoader classLoader = Thread.currentThread().getContextClassLoader();
            URL url = classLoader.getResource("");
            File file = new File(url.toURI());
            String[] filepathinfo = file.getAbsolutePath().split("\\\\");
            
            String exeFilePath = ""+filepathinfo[0];
            
            for(int i = 1; i < filepathinfo.length-4; i++)
            {
                exeFilePath += "/"+filepathinfo[i];
            }
            
            exeFilePath += "/c#/KinectFitness/bin/Release/KinectFitness.exe";
            Process process=Runtime.getRuntime().exec(exeFilePath);
		
		// TODO Auto-generated method stub
		// 1. ask them for password --> to branch off to doctor vs patient
		// 2a. if patient, ask for their age
		// 3a. popup with ip address, instruction to add this number to android app
		// 3b. instruction to put heart monitor & V02 and starting the android phone
		// 3c. if continue is pressed, execute C# application
		// 2b. if doctor, display panels?
	}
	
	private static void createInputDialog()
	{
		JFrame dialogWindow = new JFrame("Kinect Fitness");
                dialogWindow.setDefaultCloseOperation(JFrame.EXIT_ON_CLOSE);
		
		JPanel panel = new JPanel();
		
		JLabel label1 = new JLabel("<html>Welcome to Kinect Fitness!<br><br></html>", SwingConstants.CENTER);
		JLabel label2 = new JLabel("<html><p>Please input the given password</p><p> below to access the program.</p></html>", SwingConstants.CENTER);
		JTextField textField = new JTextField(20);

		panel.add(label1);
		panel.add(label2);
		panel.add(textField);
		
		dialogWindow.getContentPane().add(panel);

		dialogWindow.pack(); // sets size relative to content
		dialogWindow.setSize(300, 200);
		dialogWindow.setLocationRelativeTo(null);
		dialogWindow.setVisible(true);
		
	}

}
