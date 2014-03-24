/*
 * This file is part of VLCJ.
 *
 * VLCJ is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * VLCJ is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with VLCJ.  If not, see <http://www.gnu.org/licenses/>.
 *
 * Copyright 2009, 2010, 2011, 2012, 2013, 2014 Caprica Software Limited.
 */
package kinectfitness;

import java.awt.BorderLayout;
import java.awt.Canvas;
import java.awt.Color;
import java.awt.GridLayout;

import javax.swing.ImageIcon;
import javax.swing.JFrame;
import javax.swing.JLabel;
import javax.swing.JOptionPane;
import javax.swing.JPanel;
import javax.swing.SwingUtilities;
import javax.swing.border.EmptyBorder;

import uk.co.caprica.vlcj.player.MediaPlayerFactory;
import uk.co.caprica.vlcj.player.embedded.EmbeddedMediaPlayer;
import uk.co.caprica.vlcj.player.embedded.videosurface.CanvasVideoSurface;
import uk.co.caprica.vlcj.runtime.RuntimeUtil;
import uk.co.caprica.vlcj.test.VlcjTest;
import com.sun.jna.NativeLibrary;
import java.util.logging.Level;
import java.util.logging.Logger;
import uk.co.caprica.vlcj.player.headless.HeadlessMediaPlayer;

/**
 * An example showing a possible two-way video chat application.
 * <p>
 * The local media source is expected to be an MRL for a local video capture
 * device. This is displayed locally and streamed via RTP.
 * <p>
 * Examples of video capture device MRLs are:
 * <ul>
 * <li>v4l2:///dev/video0</li>
 * <li>dshow://</li>
 * </ul>
 * <p>
 * The remote media source is expected to be an incoming RTP stream.
 * <p>
 * A remote client would use the inverse MRLs.
 * <p>
 * Any MRL will do for local media, i.e. you do not have to use a video capture
 * device, so you can test with regular media files if you do not have a video
 * capture device.
 * <p>
 * You are not required to use RTP in your own application of course, you can
 * stream however you like.
 * <p>
 * You should also be able to adapt this to use IPv6 addresses.
 * <p>
 * If you want to capture audio, you should a media option similar to the
 * following (on Linux at least):
 *
 * <pre>
 *   :input-slave=alsa://hw:0,0
 * </pre>
 */
public class Doctor extends VlcjTest {

    private String remoteIP;
    
    private final MediaPlayerFactory mediaPlayerFactoryOut;
    private final MediaPlayerFactory mediaPlayerFactoryIn;
    private final HeadlessMediaPlayer localMediaPlayer;
    private final EmbeddedMediaPlayer remoteMediaPlayer;

    private final JFrame frame;
    private final JPanel contentPane;
    private final Canvas remoteCanvas;

    private final JLabel lbl_connection_status;
    
    private final CanvasVideoSurface remoteVideoSurface;

    private final JPanel pnl_info;
    private final JLabel lbl_hr_title;
    private final JLabel lbl_bp_title;
    private final JLabel lbl_o2;
    private final JLabel lbl_o2_title;
    private final JLabel lbl_heart_rate;
    private final JLabel lbl_blood_pressure;

    private final Color bkg_color = Color.white;

    public void startDoctor() throws Exception {
        NativeLibrary.addSearchPath("libvlc", "./");
        //NativeLibrary.addSearchPath("libvlc", "libvlc");
        //NativeLibrary.addSearchPath("libvlccore", "./");
        setLookAndFeel();

        SwingUtilities.invokeLater(new Runnable() {
            @Override
            public void run() {
                try {
                    new Doctor().start();
                } catch (InterruptedException ex) {
                    Logger.getLogger(Doctor.class.getName()).log(Level.SEVERE, null, ex);
                }
            }
        });
    }

    public Doctor() {
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
        
        lbl_connection_status = new JLabel();
        lbl_connection_status.setFont(new java.awt.Font("Helvetica", 1, 16) );
        lbl_connection_status.setText(" Waiting for connection...");
        
        remoteVideoSurface = mediaPlayerFactoryIn.newVideoSurface(remoteCanvas);
        remoteMediaPlayer.setVideoSurface(remoteVideoSurface);

        contentPane.add(lbl_connection_status, BorderLayout.PAGE_START);
        contentPane.add(remoteCanvas, BorderLayout.CENTER);

        frame = new JFrame("Doctor View");
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

    public void updateInfo(Info patientInfo) {
        this.lbl_heart_rate.setText(patientInfo.heart_rate + " bpm");
        //this.lbl_blood_pressure.setText(patientInfo.blood_pressure[0] + "/" + patientInfo.blood_pressure[1]);
        this.lbl_o2.setText(patientInfo.O2 + "%");
    }
    
    public void updateStatus(String status){
        this.lbl_connection_status.setText(status);
    }
    
    private void start() throws InterruptedException {
        //this.remoteIP = "142.244.215.246:5555";
        //send();//send video
        frame.setVisible(true);
                
        Doctor_info pi = new Doctor_info(this);
        pi.start();//get information from patient
               
    }
    
    public void startStreaming(String remoteIP){       
        //System.out.println("IP = " + remoteIP);
        this.remoteIP = remoteIP + ":5555";
         
        send();
        receive();//receive video
    }
    
    private void send() {
        String mrl = !RuntimeUtil.isWindows() ? "v4l2:///dev/video0" : "dshow://";
        if(mrl.length() > 0) {
            String streamTo = this.remoteIP;
            
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