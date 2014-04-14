package kinectfitness;

import java.io.DataOutputStream;
import java.io.IOException;
import java.io.ObjectOutputStream;
import java.net.InetAddress;
import java.net.ServerSocket;
import java.net.Socket;
import java.nio.charset.Charset;
import java.util.Random;

public class BluetoothServer {

    public static ObjectOutputStream doctorOutput;

    public BluetoothServer(ObjectOutputStream docout) {
        this.doctorOutput = docout;
    }

    public static void connectToPhone() throws IOException {
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
            In in = new In(clientSocket);

            // waits for data and reads it in until connection dies
            // readLine() blocks until the server receives a new line from client
            String s = "";
            while ((s = in.readLine()) != null) {
                String heartRate = "";
                String oxiRate = "";
                if (s.substring(0, 2).equals("HR")) {
                    heartRate = getHeartRate(s);
                } else if (s.substring(0, 2).equals("OX")) {
                    oxiRate = getOxiPercent(s);
                }

                // Now this prints out just the number
                System.err.println("Heart Rate: " + heartRate);
                System.err.println("Oximeter Rate: " + oxiRate);

                // send the received information to the doctor
                Info patient_info = new Info();
                patient_info.heart_rate = heartRate;
                patient_info.O2 = oxiRate;
                BluetoothServer.doctorOutput.writeObject(patient_info);
                BluetoothServer.doctorOutput.reset();

                // send the received information to the C# application
//                ServerSocket dataserver = new ServerSocket(5000, 0, InetAddress.getByName("localhost"));
//                System.err.println("Started server on port 5000");
//
//                while (true) {
//                    Socket dataclient = dataserver.accept();
//                    System.err.println("Accepted connection from client");
//                }
            }

            // close IO streams, then socket
            System.err.println("Closing connection with client");
            in.close();
            clientSocket.close();
        }
    }

    private static String getHeartRate(String hrInput) {
        String processedString = hrInput.substring(3);
        return processedString;
    }

    private static String getOxiPercent(String oxInput) {
        String processedString = oxInput.substring(3, 6);
        return processedString;
    }

    public static String randInt(int min, int max) {

        // Usually this can be a field rather than a method variable
        Random rand = new Random();

        // nextInt is normally exclusive of the top value,
        // so add 1 to make it inclusive
        int randomNum = rand.nextInt((max - min) + 1) + min;

        return String.valueOf(randomNum);
    }

    public void runTest() throws IOException {


//        ServerSocket dataserver = new ServerSocket(5000, 0, InetAddress.getByName("localhost"));
//        System.err.println("Started server on port 5000");
//        Socket dataclient = dataserver.accept();
//        System.err.println("Accepted connection from client");
//        
        int counter = 0;
        while (true) {

            String heartRate = randInt(130, 150);
            String oxiRate = randInt(50, 100);

            Info patient_info = new Info();
            patient_info.heart_rate = heartRate;
            patient_info.O2 = oxiRate;
            BluetoothServer.doctorOutput.writeObject(patient_info);
            BluetoothServer.doctorOutput.reset();


//            ObjectOutputStream oos = new ObjectOutputStream(dataclient.getOutputStream());
//            String outputString = heartRate.trim() + ", " + oxiRate.trim();
//            byte[] b = outputString.getBytes(Charset.forName("UTF-8"));
//            
//            oos.write(b);
//            oos.reset();
//            
            counter ++;

//            while (true) {
//                Socket dataclient = dataserver.accept();
//                System.err.println("Accepted connection from client");
//                ObjectOutputStream oos = new ObjectOutputStream(dataclient.getOutputStream());
//                oos.writeBytes(heartRate + ", " + oxiRate + "\n");
//                oos.reset();
//            }
        }
//        dataclient.close();
//        dataserver.close();
        

    }
}
