package com.kinectfitness.main;

import java.io.BufferedReader;
import java.io.IOException;
import java.io.InputStream;
import java.io.InputStreamReader;
import java.net.ServerSocket;
import java.net.Socket;

public class BluetoothServer {
	
	public static void connectToPhone() throws IOException
	{
		// create socket
        int port = 4444;
        ServerSocket serverSocket = new ServerSocket(port);
        System.err.println("Started server on port " + port);

        // repeatedly wait for connections, and process
        while (true) {
            // a "blocking" call which waits until a connection is requested
            Socket clientSocket = serverSocket.accept();
            System.err.println("Accepted connection from client");

            // open up IO streams
            In  in  = new In (clientSocket);

            // waits for data and reads it in until connection dies
            // readLine() blocks until the server receives a new line from client
            String s = "";
            while ((s = in.readLine()) != null) {
            	String heartRate = "";
            	String oxiRate = "";
            	if(s.substring(0,2).equals("HR"))
            	{
            		heartRate = getHeartRate(s);
            	}
            	else if(s.substring(0,2).equals("OX"))
            	{
            		oxiRate = getOxiPercent(s);
            	}
            	
            	// Now this prints out just the number
            	System.err.println("Heart Rate: "+ heartRate);
            	System.err.println("Oximeter Rate: "+ oxiRate);
            }

            // close IO streams, then socket
            System.err.println("Closing connection with client");
            in.close();
            clientSocket.close();
        }
	}
	
	private static String getHeartRate(String hrInput)
    {
    	String processedString = hrInput.substring(3);
    	return processedString;
    }
	
	private static String getOxiPercent(String oxInput)
    {
		String processedString = oxInput.substring(3,5);
    	return processedString;
    }
    
}

