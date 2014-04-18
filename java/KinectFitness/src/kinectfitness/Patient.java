/*
 * To change this template, choose Tools | Templates
 * and open the template in the editor.
 */
package kinectfitness;

import java.awt.BorderLayout;
import java.awt.Canvas;
import java.awt.Color;

import javax.swing.JFrame;
import javax.swing.JOptionPane;
import javax.swing.JPanel;
import javax.swing.SwingUtilities;

import uk.co.caprica.vlcj.player.MediaPlayerFactory;
import uk.co.caprica.vlcj.player.embedded.EmbeddedMediaPlayer;
import uk.co.caprica.vlcj.player.embedded.videosurface.CanvasVideoSurface;
import uk.co.caprica.vlcj.runtime.RuntimeUtil;
import uk.co.caprica.vlcj.test.VlcjTest;
import com.sun.jna.NativeLibrary;
import java.awt.Dimension;
import java.awt.Toolkit;
import uk.co.caprica.vlcj.player.headless.HeadlessMediaPlayer;

/**
 *
 * @author Marcus
 */
public class Patient extends VlcjTest {

    private String doctorIP = "127.0.0.1";
    private int patient_index;
    private String doctorVideoPort;
    private final MediaPlayerFactory mediaPlayerFactoryOut;
    private final MediaPlayerFactory mediaPlayerFactoryIn;
    private final HeadlessMediaPlayer localMediaPlayer;
    private final EmbeddedMediaPlayer remoteMediaPlayer;
    private final JFrame frame;
    private final JPanel contentPane;
    private final Canvas remoteCanvas;
    private final CanvasVideoSurface remoteVideoSurface;

    private final Color bkg_color = Color.white;

    public void Patient(String docIP, int patient_index) throws Exception {
        this.patient_index = patient_index;
        this.doctorVideoPort = Integer.toString(5555 + patient_index);
        this.doctorIP = docIP;
    }

    public static void startPatient(final Patient p) throws Exception {

        NativeLibrary.addSearchPath("libvlc", "./");
        //NativeLibrary.addSearchPath("libvlc", "libvlc");
        //NativeLibrary.addSearchPath("libvlccore", "./");
        setLookAndFeel();

        SwingUtilities.invokeLater(new Runnable() {
            @Override
            public void run() {
                p.start();
            }
        });
    }

    public Patient() {
        String[] VLC_ARGS = {"--vout", "dummy", "--aout", "dummy",};
        mediaPlayerFactoryOut = new MediaPlayerFactory(VLC_ARGS);
        mediaPlayerFactoryIn = new MediaPlayerFactory();
        localMediaPlayer = mediaPlayerFactoryOut.newHeadlessMediaPlayer();
        remoteMediaPlayer = mediaPlayerFactoryIn.newEmbeddedMediaPlayer();

        contentPane = new JPanel();
        contentPane.setBackground(bkg_color);
        contentPane.setLayout(new BorderLayout(0, 0));

        remoteCanvas = new Canvas();
        remoteCanvas.setBackground(Color.BLACK);
        remoteCanvas.setSize(320, 200);

        remoteVideoSurface = mediaPlayerFactoryIn.newVideoSurface(remoteCanvas);
        remoteMediaPlayer.setVideoSurface(remoteVideoSurface);

        contentPane.add(remoteCanvas, BorderLayout.CENTER);

        frame = new JFrame("Patient View");
        frame.setUndecorated(true);
        frame.setAlwaysOnTop(true);


        Toolkit kit = Toolkit.getDefaultToolkit();
        Dimension screen = kit.getScreenSize();

        frame.setLocation((int) screen.getWidth() - 320 - 100, 0);

        frame.setContentPane(contentPane);
        frame.setDefaultCloseOperation(JFrame.EXIT_ON_CLOSE);

        frame.pack();
    }

    private void start() {
        send();//send video
        Patient_info pi = new Patient_info(this.doctorIP, patient_index);
        pi.start();//send information from patient
        receive();//receive video
        frame.setVisible(true);
    }

    private void send() {
        String mrl = !RuntimeUtil.isWindows() ? "v4l2:///dev/video0" : "dshow://";
        if (mrl.length() > 0) {
            System.out.println("Before streamTo: " + this.doctorIP);
            String streamTo = this.doctorIP + ":" + doctorVideoPort;

            String[] parts = streamTo.split(":");
            if (parts.length == 2) {
                String host = parts[0];
                int port = Integer.parseInt(parts[1]);

                String[] localOptions = {formatRtpStream(host, port), ":no-sout-rtp-sap", ":no-sout-standard-sap", ":sout-all", ":sout-keep",};

                localMediaPlayer.playMedia(mrl, localOptions);
            } else {
                JOptionPane.showMessageDialog(frame, "You must specify host:port to stream to.", "Error", JOptionPane.ERROR_MESSAGE);
            }
        } else {
            JOptionPane.showMessageDialog(frame, "You must specify source media, e.g. v4l2:///dev/video0 on Linux or dshow:// on Windows.", "Error", JOptionPane.ERROR_MESSAGE);
        }
    }

    private void receive() {
        String streamFrom = "230.0.0.1:5555";
        remoteMediaPlayer.playMedia("rtp://@" + streamFrom, ":sout-mux-caching=100", ":live-caching=100", ":network-caching=100", ":clock-jitter=0");
    }

    private static String formatRtpStream(String serverAddress, int serverPort) {
        StringBuilder sb = new StringBuilder(60);
        sb.append(":sout=#transcode{vcodec=mp2v,vb=512,scale=1,acodec=mpga,ab=128,channels=2,samplerate=44100}:duplicate{dst=display,dst=rtp{dst=");
        sb.append(serverAddress);
        sb.append(",port=");
        sb.append(serverPort);
        sb.append(",mux=ts}}");
        return sb.toString();
    }
}