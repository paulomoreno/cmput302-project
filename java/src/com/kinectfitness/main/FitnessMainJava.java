package com.kinectfitness.main;

import javax.swing.*;

public class FitnessMainJava {

	public static void main(String[] args) {
		createInputDialog();
		
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
