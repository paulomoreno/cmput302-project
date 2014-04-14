package kinectfitness;

import java.io.DataOutputStream;
import java.io.IOException;
import java.io.ObjectOutputStream;
import java.net.InetAddress;
import java.net.ServerSocket;
import java.net.Socket;
import java.nio.charset.Charset;
import java.util.Random;
import java.util.Timer;
import java.util.TimerTask;
import java.util.logging.Level;
import java.util.logging.Logger;
import javax.xml.parsers.ParserConfigurationException;
import javax.xml.transform.TransformerConfigurationException;
import javax.xml.transform.TransformerException;
import org.w3c.dom.Element;

public class BluetoothServer {

    public static ObjectOutputStream doctorOutput;
    // need the heart_rate and oxygen_percentage for testing
    Integer heart_rate = 100;
    Integer oxygen_percentage = 90;
    // need these to mock blood pressure readings
    Integer systolic = 140;
    Integer diastolic = 100;

    public BluetoothServer(ObjectOutputStream docout) {
        this.doctorOutput = docout;
    }

    public void connectToPhone() throws IOException {
        // create socket
        int port = 4444;
        ServerSocket serverSocket = new ServerSocket(port);
        System.err.println("Started server on port " + port);

        final Random r = new Random();
        Timer timer = new Timer();

        timer.schedule(
                new TimerTask() {
            public void run() {
                // generates heart rate +- 1 from previous value
                // within range of 90-175
                systolic += r.nextInt(3) - 1;
                if (systolic < 90) {
                    systolic = 90;
                } else if (systolic > 175) {
                    systolic = 175;
                }

                // generates heart rate +- 1 from previous value
                // within range of 50-105
                diastolic += r.nextInt(3) - 1;
                if (diastolic < 50) {
                    diastolic = 50;
                } else if (diastolic > 105) {
                    diastolic = 105;
                }

            }
        }, 0, 5000);

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
                if (heartRate != "") {
                    patient_info.heart_rate = heartRate;
                }
                if (oxiRate != "") {
                    patient_info.O2 = oxiRate;
                }
                patient_info.blood_pressure[0] = String.valueOf(systolic);
                patient_info.blood_pressure[1] = String.valueOf(diastolic);

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

    public void runTest() {
        try {
            // currently when this code is enabled, it lags the video and also until the java application is shut off,
            // the kinect side does not show up... probably need a thread
//            ServerSocket dataserver = new ServerSocket(5001, 0, InetAddress.getByName("localhost"));
//            Socket dataclient = dataserver.accept();
//            
//            System.err.println("Accepted connection from client");
//            DataOutputStream dataOutputStream = new DataOutputStream(dataclient.getOutputStream());

            final Random r = new Random();
            Timer timer = new Timer();

            timer.schedule(
                    new TimerTask() {
                public void run() {
                    //        System.err.println("Accepted connection from client");
                    // within range of 90-175
                    systolic += r.nextInt(3) - 1;
                    if (systolic < 90) {
                        systolic = 90;
                    } else if (systolic > 175) {
                        systolic = 175;
                    }

                    // generates heart rate +- 1 from previous value
                    // within range of 50-105
                    diastolic += r.nextInt(3) - 1;
                    if (diastolic < 50) {
                        diastolic = 50;
                    } else if (diastolic > 105) {
                        diastolic = 105;
                    }

                    // generates heart rate +- 1 from previous value
                    // within range of 50-130
                    heart_rate += r.nextInt(3) - 1;
                    if (heart_rate < 50) {
                        heart_rate = 50;
                    } else if (heart_rate > 130) {
                        heart_rate = 130;
                    }

                    // generates heart rate +- 1 from previous value
                    // within range of 93-99
                    oxygen_percentage += r.nextInt(3) - 1;
                    if (oxygen_percentage < 93) {
                        oxygen_percentage = 93;
                    } else if (oxygen_percentage > 99) {
                        oxygen_percentage = 99;
                    }

                }
            }, 0, 5000);
            while (true) {

                Info patient_info = new Info();
                patient_info.heart_rate = String.valueOf(heart_rate);
                patient_info.O2 = String.valueOf(oxygen_percentage);
                patient_info.blood_pressure[0] = String.valueOf(systolic);
                patient_info.blood_pressure[1] = String.valueOf(diastolic);
                BluetoothServer.doctorOutput.writeObject(patient_info);
                BluetoothServer.doctorOutput.reset();

//                dataOutputStream.writeBytes(String.valueOf(patient_info.heart_rate) + "||" + String.valueOf(patient_info.O2)
//                         + "||" + String.valueOf(patient_info.blood_pressure[0]) + "/" + patient_info.blood_pressure[1] + "\n");
                
            }
            //        dataclient.close();
            //        dataserver.close();
        } catch (IOException ex) {
            Logger.getLogger(BluetoothServer.class.getName()).log(Level.SEVERE, null, ex);
        }



    }
}
