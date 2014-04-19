package kinectfitness;

import java.io.IOException;
import java.io.ObjectOutputStream;
import java.net.ServerSocket;
import java.net.Socket;
import java.util.Random;
import java.util.Timer;
import java.util.TimerTask;
import java.util.logging.Level;
import java.util.logging.Logger;

/**
 * This class is used to set up a socket connection with the 
 * Samsung Galaxy S4 phone with the Sensor Read application.
 * This class creates a socket server that listens at port 4444
 * of localhost and once the client (the phone) connects to the
 * server, then the data is sent from the phone (which receives
 * directly from the Bluetooth devices) and is sent to the doctor
 * application.  The methods of this class is called from the
 * Patient_info class.
 * 
 * @author Ga Young Kim
 */
public class BluetoothServer {

    // the stream for sending the data to the doctor
    public static ObjectOutputStream doctorOutput;
    // need the heart_rate and oxygen_percentage for testing
    Integer heart_rate = 100;
    Integer oxygen_percentage = 90;
    // need these to mock blood pressure readings
    Integer systolic = 140;
    Integer diastolic = 100;

    /**
     * The main constructor for the BluetoothServer class.
     * @param docout       the OutputStream where the data is sent to the
     *                     doctor's side of the application.
     */
    public BluetoothServer(ObjectOutputStream docout) {
        this.doctorOutput = docout;
    }

    /**
     * The method is used to connect to the phone as explained in the 
     * class description above.  The socket server at port 4444 waits for
     * the connection by the client to start the data transfer.
     * @throws IOException 
     */
    public void connectToPhone() throws IOException {
        // create socket
        int port = 4444;
        ServerSocket serverSocket = new ServerSocket(port);

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

            }

            // close IO streams, then socket
            in.close();
            clientSocket.close();
        }
    }

    /**
     * This method is just used to only retrieve the relevant value
     * from the heart rate monitor data.  The Sensor Read android application
     * on the phone sends the data in form of HR XX so need to get the
     * value XX.
     * @param hrInput   the data string received from the phone starting with HR
     * @return          the heart rate value
     */
    private static String getHeartRate(String hrInput) {
        String processedString = hrInput.substring(3);
        return processedString;
    }

    /**
     * This method is used to retrieve the blood oxygen percentage from 
     * the oximeter reading.  The Sensor Read application send the data 
     * in the form of OX AA BB.  To get the correct oxygen percentage,
     * one has to get the AA value.
     * @param oxInput   the data string received from the phone starting with OX 
     * @return          the blood oxygen percentage value
     */
    private static String getOxiPercent(String oxInput) {
        String processedString = oxInput.substring(3, 6);
        return processedString;
    }

    /**
     * This function is used to randomly generate values.  It
     * is used to mock blood pressure data and also in runTest to
     * simulate the data being received for testing purposes without
     * the phone.
     * @param min           min value of the random number
     * @param max           max value of the random number
     * @return              randomly generated number within the min and max range
     */
    public static String randInt(int min, int max) {

        // Usually this can be a field rather than a method variable
        Random rand = new Random();

        // nextInt is normally exclusive of the top value,
        // so add 1 to make it inclusive
        int randomNum = rand.nextInt((max - min) + 1) + min;

        return String.valueOf(randomNum);
    }

    /**
     * This method is called by the Patient_info class when the 
     * phone and the Bluetooth devices were not available for testing. It
     * simulates the data sent from the phone to test the application.
     */
    public void runTest() {
        try {
            final Random r = new Random();
            Timer timer = new Timer();

            timer.schedule(
                    new TimerTask() {
                public void run() {
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
                // send mock data to the doctor
                Info patient_info = new Info();
                patient_info.heart_rate = String.valueOf(heart_rate);
                patient_info.O2 = String.valueOf(oxygen_percentage);
                patient_info.blood_pressure[0] = String.valueOf(systolic);
                patient_info.blood_pressure[1] = String.valueOf(diastolic);
                BluetoothServer.doctorOutput.writeObject(patient_info);
                BluetoothServer.doctorOutput.reset(); 
            }
        } catch (IOException ex) {
            Logger.getLogger(BluetoothServer.class.getName()).log(Level.SEVERE, null, ex);
        }



    }
}