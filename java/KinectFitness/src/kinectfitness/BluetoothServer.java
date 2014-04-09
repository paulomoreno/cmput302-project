package kinectfitness;

import java.io.DataOutputStream;
import java.io.IOException;
import java.io.ObjectOutputStream;
import java.net.InetAddress;
import java.net.ServerSocket;
import java.net.Socket;
import java.net.UnknownHostException;
import java.nio.charset.Charset;
import java.util.Random;
import java.util.Timer;
import java.util.TimerTask;
import java.util.concurrent.Executors;
import java.util.concurrent.ScheduledExecutorService;
import java.util.concurrent.TimeUnit;
import java.util.logging.Level;
import java.util.logging.Logger;

public class BluetoothServer {

    public static ObjectOutputStream doctorOutput;
    // need the heart_rate and oxygen_percentage for testing
    Integer heart_rate = 100;
    Integer oxygen_percentage = 90;
    // need these to mock blood pressure readings
    Integer systolic = 140;
    Integer diastolic = 100;

    // needed this constructor for testing purposes
    public BluetoothServer() {
    }

    public BluetoothServer(ObjectOutputStream docout) {
        this.doctorOutput = docout;
    }

    public void connectToPhone() throws IOException, InterruptedException {
        // create socket
        new Thread() {
            public void run() {
                int port = 4444;
                ServerSocket serverSocket;
                try {
                    serverSocket = new ServerSocket(port);
                    // DEBUG
                    System.err.println("Started server on port " + port);

                    // open socket to send the data to C# application
                    Socket destinationSocket = new Socket("127.0.0.1", 5000);

                    DataOutputStream dataOutputStream = new DataOutputStream(destinationSocket.getOutputStream());
                    // DEBUG
                    System.err.println("Started server on port 5000");


                    // repeatedly wait for connections, and process
                    while (true) {
                        // a "blocking" call which waits until a connection is requested
                        Socket clientSocket = serverSocket.accept();
                        // DEBUG
                        System.err.println("Accepted connection from client");

                        // open up IO streams
                        In in = new In(clientSocket);

                        // waits for data and reads it in until connection dies
                        // readLine() blocks until the server receives a new line from client
                        String s = "";
                        String heartRate = "";
                        String oxiRate = "";
                        while ((s = in.readLine()) != null) {
                            if (s.substring(0, 2).equals("HR")) {
                                if (getHeartRate(s) != null) {
                                    heartRate = getHeartRate(s);
                                }
                            } else if (s.substring(0, 2).equals("OX")) {
                                if (getOxiPercent(s) != null) {
                                    oxiRate = getOxiPercent(s);
                                }
                            }

                            // DEBUG
                            // Now this prints out just the number
                            System.err.println("Heart Rate: " + heartRate);
                            System.err.println("Oximeter Rate: " + oxiRate);

                            // send the received information to the doctor
                            final Info patient_info = new Info();
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

                            // send patient heart rate data to the C# Kinect application
                            dataOutputStream.writeBytes(patient_info.heart_rate + "\n");

                        }
                        System.err.println("Closing connection with client");
                        in.close();
                        clientSocket.close();
                    }

                } catch (IOException ex) {
                    Logger.getLogger(BluetoothServer.class.getName()).log(Level.SEVERE, null, ex);
                }


            }
        }.start();

        Thread.sleep(5000);

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

    public void runTest() throws IOException, InterruptedException {
        new Thread() {
            public void run() {
//                try {
                    //Open socket to send data to kinect application
                    //Socket destinationSocket = new Socket("127.0.0.1", 7003);
                    //DataOutputStream dataOutputStream = new DataOutputStream(destinationSocket.getOutputStream());

                    //Integer i = 0;
                    while (true) {

                        final Info patient_info = new Info();

                        patient_info.heart_rate = String.valueOf(heart_rate);
                        patient_info.O2 = String.valueOf(oxygen_percentage);
                        patient_info.blood_pressure[0] = String.valueOf(systolic);
                        patient_info.blood_pressure[1] = String.valueOf(diastolic);

                        // send data to doctor
                        //BluetoothServer.doctorOutput.writeObject(patient_info);
                        //BluetoothServer.doctorOutput.reset();

                        // send data to C# application
                        //dataOutputStream.writeBytes(String.valueOf(patient_info.heart_rate) + "||" + String.valueOf(patient_info.O2)
                               // + "||" + String.valueOf(patient_info.blood_pressure[0]) + "/" + patient_info.blood_pressure[1] + "\n");

//                        System.out.println("patient info");
//                        System.out.println("heart rate: " + patient_info.heart_rate);
//                        System.out.println("oximeter rate: " + patient_info.O2);
//                        System.out.println("blood pressure: " + patient_info.blood_pressure[0] + "/" + patient_info.blood_pressure[1]);
                        //i++;
                    }
//                } catch (UnknownHostException e) {
//                    e.printStackTrace();
//                } catch (IOException e) {
//                    e.printStackTrace();
                }
//            }
        }.start();

        //Thread.sleep(5000);
        // "randomly" generate values every 5 seconds
        // --> somewhat intelligent generation where it
        // only takes +- 1 in a range
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

    }
}
