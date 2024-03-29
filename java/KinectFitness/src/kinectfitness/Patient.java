/*
 * To change this template, choose Tools | Templates
 * and open the template in the editor.
 */
package kinectfitness;

import java.awt.BorderLayout;
import java.awt.Canvas;
import java.awt.Color;
import java.awt.GridLayout;

import javax.swing.Box;
import javax.swing.BoxLayout;
import javax.swing.ImageIcon;
import javax.swing.JFrame;
import javax.swing.JLabel;
import javax.swing.JOptionPane;
import javax.swing.JPanel;
import javax.swing.SwingUtilities;
import javax.swing.border.EmptyBorder;
import javax.swing.border.TitledBorder;

import uk.co.caprica.vlcj.player.MediaPlayerFactory;
import uk.co.caprica.vlcj.player.embedded.EmbeddedMediaPlayer;
import uk.co.caprica.vlcj.player.embedded.videosurface.CanvasVideoSurface;
import uk.co.caprica.vlcj.runtime.RuntimeUtil;
import uk.co.caprica.vlcj.test.VlcjTest;
import com.sun.jna.NativeLibrary;
import java.awt.Font;
import uk.co.caprica.vlcj.player.headless.HeadlessMediaPlayer;

/**
 *
 * @author Marcus
 */
public class Patient extends VlcjTest {

    private String doctorIP = "127.0.0.1";
    
    private final MediaPlayerFactory mediaPlayerFactoryOut;
    private final MediaPlayerFactory mediaPlayerFactoryIn;
    private final HeadlessMediaPlayer localMediaPlayer;
    private final EmbeddedMediaPlayer remoteMediaPlayer;

    private final JFrame frame;
    private final JPanel contentPane;
    private final Canvas remoteCanvas;


    private final CanvasVideoSurface remoteVideoSurface;

    private final JPanel pnl_info;
    private final JLabel lbl_hr_title;
    private final JLabel lbl_bp_title;
    private final JLabel lbl_o2;
    private final JLabel lbl_o2_title;
    private final JLabel lbl_heart_rate;
    private final JLabel lbl_blood_pressure;

    private final Color bkg_color = Color.white;

    public void Patient(String docIP) throws Exception {
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
        String[] VLC_ARGS = {"--vout","dummy","--aout","dummy",};
        mediaPlayerFactoryOut = new MediaPlayerFactory(VLC_ARGS);
        mediaPlayerFactoryIn = new MediaPlayerFactory();
        localMediaPlayer = mediaPlayerFactoryOut.newHeadlessMediaPlayer();
        remoteMediaPlayer = mediaPlayerFactoryIn.newEmbeddedMediaPlayer();

        contentPane = new JPanel();
        contentPane.setBackground(bkg_color);
        contentPane.setLayout(new BorderLayout(0, 0));

        remoteCanvas = new Canvas();
        remoteCanvas.setBackground(Color.BLACK);
        remoteCanvas.setSize(640, 360);
 
        remoteVideoSurface = mediaPlayerFactoryIn.newVideoSurface(remoteCanvas);
        remoteMediaPlayer.setVideoSurface(remoteVideoSurface);

        contentPane.add(remoteCanvas, BorderLayout.CENTER);

        frame = new JFrame("Patient View");
        frame.setIconImage(new ImageIcon(getClass().getResource("/icons/vlcj-logo.png")).getImage());
        frame.setContentPane(contentPane);
        frame.setDefaultCloseOperation(JFrame.EXIT_ON_CLOSE);
        frame.pack();

        pnl_info = new JPanel();
        pnl_info.setBackground(bkg_color);
        pnl_info.setBorder(new EmptyBorder(16, 16, 16, 16));
        pnl_info.setLayout(new GridLayout(12, 1));

        lbl_hr_title = new javax.swing.JLabel();
        lbl_bp_title = new javax.swing.JLabel();
        lbl_o2_title = new javax.swing.JLabel();
        lbl_heart_rate = new javax.swing.JLabel();
        lbl_blood_pressure = new javax.swing.JLabel();
        lbl_o2 = new javax.swing.JLabel();

        lbl_hr_title.setFont(new java.awt.Font("Tahoma", 1, 14)); // NOI18N
        lbl_hr_title.setText("Heart Rate");

        lbl_bp_title.setFont(new java.awt.Font("Tahoma", 1, 14)); // NOI18N
        lbl_bp_title.setText("Blood Pressure");

        lbl_o2_title.setFont(new java.awt.Font("Tahoma", 1, 14)); // NOI18N
        lbl_o2_title.setText("O2");

        lbl_heart_rate.setText("120 / bpm");

        lbl_blood_pressure.setText("120 / 80");

        lbl_o2.setText("76%");

        pnl_info.add(lbl_hr_title);
        pnl_info.add(lbl_heart_rate);
        pnl_info.add(lbl_bp_title);
        pnl_info.add(lbl_blood_pressure);
        pnl_info.add(lbl_o2_title);
        pnl_info.add(lbl_o2);

        contentPane.add(pnl_info, BorderLayout.LINE_END);
    }
    
    private void start() {
        send();//send video
        Patient_info pi = new Patient_info(this.doctorIP);
        pi.start();//send information from patient
        receive();//receive video
        frame.setVisible(true);
    }
    
    private void send() {
        String mrl = !RuntimeUtil.isWindows() ? "v4l2:///dev/video0" : "dshow://";
        if(mrl.length() > 0) {
            System.out.println("Before streamTo: "+this.doctorIP);
            String streamTo = this.doctorIP+":5555";
            
            String[] parts = streamTo.split(":");
            if(parts.length == 2) {
                String host = parts[0];
                int port = Integer.parseInt(parts[1]);
                
                String[] localOptions = {formatRtpStream(host, port), ":no-sout-rtp-sap", ":no-sout-standard-sap", ":sout-all", ":sout-keep",};
                
                localMediaPlayer.playMedia(mrl, localOptions);
            }
            else {
                JOptionPane.showMessageDialog(frame, "You must specify host:port to stream to.", "Error", JOptionPane.ERROR_MESSAGE);
            }
        }
        else {
            JOptionPane.showMessageDialog(frame, "You must specify source media, e.g. v4l2:///dev/video0 on Linux or dshow:// on Windows.", "Error", JOptionPane.ERROR_MESSAGE);
        }
    }
    
    private void receive() {
        String streamFrom = "230.0.0.1:5555";
        remoteMediaPlayer.playMedia("rtp://@" + streamFrom,":sout-mux-caching=100", ":live-caching=100",":network-caching=100", ":clock-jitter=0");
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
