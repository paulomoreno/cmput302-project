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
import javax.swing.JLabel;
import javax.swing.JPanel;
import javax.swing.SwingUtilities;
import javax.swing.border.EmptyBorder;

import uk.co.caprica.vlcj.player.MediaPlayerFactory;
import uk.co.caprica.vlcj.player.embedded.EmbeddedMediaPlayer;
import uk.co.caprica.vlcj.player.embedded.videosurface.CanvasVideoSurface;
import uk.co.caprica.vlcj.runtime.RuntimeUtil;
import uk.co.caprica.vlcj.test.VlcjTest;
import com.sun.jna.NativeLibrary;
import static java.awt.Component.LEFT_ALIGNMENT;
import static java.awt.Component.TOP_ALIGNMENT;
import java.awt.Dimension;
import java.awt.Image;
import java.awt.event.ActionEvent;
import java.util.logging.Level;
import java.util.logging.Logger;
import javax.swing.AbstractAction;
import javax.swing.BorderFactory;
import javax.swing.JButton;
import javax.swing.JOptionPane;
import javax.swing.border.Border;
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
class ChangeModeAction extends AbstractAction {

    private int current_mode;
    private final int index;
    private final DoctorViewFrame window;
    private final JButton btn_change_mode;
    private final ImageIcon contract_icon;
    private final ImageIcon expand_icon;

    public ChangeModeAction(int current_mode, DoctorViewFrame window, int index, JButton btn_change_mode, ImageIcon expand_icon, ImageIcon contract_icon) {
        super();

        this.current_mode = current_mode;
        this.window = window;
        this.index = index;
        this.btn_change_mode = btn_change_mode;
        this.expand_icon = expand_icon;
        this.contract_icon = contract_icon;
    }

    public void setCurrentMode(int new_current_mode) {
        this.current_mode = new_current_mode;
    }

    @Override
    public void actionPerformed(ActionEvent ae) {
        if (this.current_mode == Doctor.ALL_PATIENTS) {
            this.setCurrentMode(Doctor.ONE_PATIENT);

            this.window.changeLayoutToOne(this.index);
            this.btn_change_mode.setIcon(contract_icon);

        } else if (this.current_mode == Doctor.ONE_PATIENT) {
            this.setCurrentMode(Doctor.ALL_PATIENTS);

            this.window.changeLayoutToAll();
            this.btn_change_mode.setIcon(expand_icon);

        }
        //throw new UnsupportedOperationException("Not supported yet."); //To change body of generated methods, choose Tools | Templates.
    }
}

class ChangeMuteAction extends AbstractAction {

    private boolean mute;
    private final int index;
    private final Doctor doctor;
    private final DoctorViewFrame window;

    public ChangeMuteAction(boolean mute, Doctor doctor, int index, DoctorViewFrame window) {
        super();

        this.doctor = doctor;
        this.window = window;
        this.index = index;
    }

    @Override
    public void actionPerformed(ActionEvent ae) {
        this.window.change_mute(index, this.doctor.isMute());
        //throw new UnsupportedOperationException("Not supported yet."); //To change body of generated methods, choose Tools | Templates.
    }
}

public class Doctor extends VlcjTest {

    private int age = 40;
    private int maximum;
    private int max_target;
    private int min_target;
    private boolean isMute;
    private float max_target_percentage = 0.85f;
    private float min_target_percentage = 0.5f;
    
    private ImageIcon mute_icon;
    private ImageIcon unmute_icon;

    public static final int ALL_PATIENTS = 0;
    public static final int ONE_PATIENT = 1;

    private String remoteIP;
    private final String localPort;
    private final int localInfoPort;
    private final int index;
    private final DoctorViewFrame window;

    private final MediaPlayerFactory mediaPlayerFactoryOut;
    private final MediaPlayerFactory mediaPlayerFactoryIn;
    private final HeadlessMediaPlayer localMediaPlayer;
    private final EmbeddedMediaPlayer remoteMediaPlayer;

    //private final JInternalFrame frame;
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

    private final JPanel pnl_bottom;

    private final JPanel pnl_btn;
    private final JButton btn_change_mode;
    private final JButton btn_mute;

    private Color bkg_color = Color.WHITE; //new Color(150, 180, 255);
    private Color text_color = Color.BLACK;

    public Doctor startDoctor() throws Exception {
        NativeLibrary.addSearchPath("libvlc", "./");
        NativeLibrary.addSearchPath("libvlccore", "libvlccore");
        setLookAndFeel();

        final Doctor doc = new Doctor(localPort, localInfoPort, index, window);

        SwingUtilities.invokeLater(new Runnable() {
            @Override
            public void run() {
                try {
                    doc.start();
                } catch (InterruptedException ex) {
                    Logger.getLogger(Doctor.class.getName()).log(Level.SEVERE, null, ex);
                }
            }
        });

        return doc;
    }

    public JPanel getContent() {
        return this.contentPane;
    }
    
    public void fullScreen(){
        contentPane.setSize(1000, 700);
        
        contentPane.setAlignmentX(LEFT_ALIGNMENT);
        contentPane.setAlignmentY(TOP_ALIGNMENT);
    }

    public Doctor(String port, int infoPort, int index, DoctorViewFrame window) {
        this.localPort = port;
        this.localInfoPort = infoPort;
        this.index = index;
        this.window = window;
        this.calculateRates();

        String[] VLC_ARGS = {"--vout", "dummy", "--aout", "dummy",};
        mediaPlayerFactoryOut = new MediaPlayerFactory(VLC_ARGS);
        mediaPlayerFactoryIn = new MediaPlayerFactory();
        localMediaPlayer = mediaPlayerFactoryOut.newHeadlessMediaPlayer();
        remoteMediaPlayer = mediaPlayerFactoryIn.newEmbeddedMediaPlayer();

        contentPane = new RoundedCornerPanel();
        contentPane.setBackground(bkg_color);
        contentPane.setLayout(new BorderLayout(0, 0));

        remoteCanvas = new Canvas();
        remoteCanvas.setBackground(Color.BLACK);
        remoteCanvas.setSize(640, 360);

        lbl_connection_status = new JLabel();
        lbl_connection_status.setFont(new java.awt.Font("Helvetica", 1, 14));
        lbl_connection_status.setBorder(BorderFactory.createEmptyBorder(3, 3, 3, 3));
        lbl_connection_status.setText(" Patient " + Integer.toString(this.index+1) + ": Waiting for connection on port " + localInfoPort);

        remoteVideoSurface = mediaPlayerFactoryIn.newVideoSurface(remoteCanvas);
        remoteMediaPlayer.setVideoSurface(remoteVideoSurface);
        
        contentPane.add(lbl_connection_status, BorderLayout.PAGE_START);
        contentPane.add(remoteCanvas, BorderLayout.CENTER);

        //frame = new JInternalFrame("Doctor View");
        //frame.setIconImage(new ImageIcon(getClass().getResource("/icons/vlcj-logo.png")).getImage());
        //frame.setContentPane(contentPane);
        //frame.setDefaultCloseOperation(JFrame.EXIT_ON_CLOSE);
        //frame.pack();
        pnl_info = new JPanel();
        pnl_info.setBackground(bkg_color);
        pnl_info.setBorder(new EmptyBorder(16, 16, 16, 16));
        pnl_info.setLayout(new GridLayout(6, 2));

        pnl_bottom = new JPanel();
        pnl_bottom.setLayout(new BorderLayout(0, 0));

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

        lbl_heart_rate.setText("Not available");

        lbl_blood_pressure.setText("Not available");

        lbl_o2.setText("Not available");

        pnl_info.add(lbl_hr_title);
        pnl_info.add(lbl_heart_rate);
        pnl_info.add(lbl_bp_title);
        pnl_info.add(lbl_blood_pressure);
        pnl_info.add(lbl_o2_title);
        pnl_info.add(lbl_o2);

        pnl_btn = new JPanel();

        btn_change_mode = new JButton();
        ImageIcon expand_icon = new ImageIcon("expand.png");
        ImageIcon contract_icon = new ImageIcon("contract.png");

        Image img = expand_icon.getImage();
        Image newimg = img.getScaledInstance(30, 30, java.awt.Image.SCALE_SMOOTH);
        expand_icon = new ImageIcon(newimg);

        img = contract_icon.getImage();
        newimg = img.getScaledInstance(30, 30, java.awt.Image.SCALE_SMOOTH);
        contract_icon = new ImageIcon(newimg);

        ChangeModeAction btn_action = new ChangeModeAction(Doctor.ALL_PATIENTS, window, index, btn_change_mode, expand_icon, contract_icon);
        btn_change_mode.setAction(btn_action);
        btn_change_mode.setIcon(expand_icon);
        btn_change_mode.setSize(30, 30);
        btn_change_mode.setOpaque(false);
        btn_change_mode.setContentAreaFilled(false);
        btn_change_mode.setBorderPainted(false);

        btn_mute = new JButton();
        mute_icon = new ImageIcon("mute.png");
        unmute_icon = new ImageIcon("unmute.png");

        Image img2 = mute_icon.getImage();
        Image newimg2 = img2.getScaledInstance(30, 30, java.awt.Image.SCALE_SMOOTH);
        mute_icon = new ImageIcon(newimg2);

        img2 = unmute_icon.getImage();
        newimg2 = img2.getScaledInstance(30, 30, java.awt.Image.SCALE_SMOOTH);
        unmute_icon = new ImageIcon(newimg2);

        ChangeMuteAction btn_mute_action = new ChangeMuteAction(true, this, index, this.window);
        btn_mute.setAction(btn_mute_action);
        btn_mute.setIcon(mute_icon);
        btn_mute.setSize(30, 30);
        btn_mute.setOpaque(false);
        btn_mute.setContentAreaFilled(false);
        btn_mute.setBorderPainted(false);

        pnl_btn.add(btn_mute);
        pnl_btn.add(btn_change_mode);
        pnl_btn.setBackground(bkg_color);

        pnl_bottom.add(pnl_info, BorderLayout.CENTER);
        pnl_bottom.add(pnl_btn, BorderLayout.EAST);

        pnl_bottom.setOpaque(false);
        pnl_info.setOpaque(false);
        pnl_btn.setOpaque(false);
        
        contentPane.add(pnl_bottom, BorderLayout.SOUTH);
    }

    public void updateInfo(Info patientInfo) {
        this.lbl_heart_rate.setText(patientInfo.heart_rate + " bpm");
        //this.lbl_blood_pressure.setText(patientInfo.blood_pressure[0] + "/" + patientInfo.blood_pressure[1]);
        this.lbl_o2.setText(patientInfo.O2 + "%");

        //If not normal, send messages and change the background to red
        if (Integer.parseInt(patientInfo.heart_rate) > this.max_target) {
            String high_low = "high";
            String type = "heartrate";

            this.updateColor(Color.RED, Color.WHITE);

            JOptionPane.showMessageDialog(null,
                    "Patient " + Integer.toString(this.index + 1) + "'s " + type + " is " + high_low + "!",
                    "Warning",
                    JOptionPane.WARNING_MESSAGE);
        } else if (Integer.parseInt(patientInfo.heart_rate) < this.min_target) {
            String high_low = "low";
            String type = "heartrate";

            this.updateColor(Color.RED, Color.WHITE);

            JOptionPane.showMessageDialog(null,
                    "Patient " + Integer.toString(this.index + 1) + "'s " + type + " is " + high_low + "!",
                    "Warning",
                    JOptionPane.WARNING_MESSAGE);            //If normal, make sure background is white
        } else if (this.bkg_color != Color.WHITE) {
            this.updateColor(Color.WHITE, Color.BLACK);

        }
    }
    
    public void setVisible(boolean var){
        this.contentPane.setVisible(var);
    }

    public void calculateRates() {
        this.maximum = 220 - this.age;
        this.max_target = (int) (this.maximum * this.max_target_percentage);
        this.min_target = (int) (this.maximum * this.min_target_percentage);
    }

    private void updateColor(Color bkg_color, Color text_color) {

        this.bkg_color = bkg_color;
        this.text_color = text_color;

        this.pnl_bottom.setBackground(bkg_color);
        this.pnl_info.setBackground(bkg_color);
        this.pnl_btn.setBackground(bkg_color);
        this.contentPane.setBackground(bkg_color);

        lbl_hr_title.setForeground(text_color);
        lbl_bp_title.setForeground(text_color);
        lbl_o2_title.setForeground(text_color);
        lbl_blood_pressure.setForeground(text_color);
        lbl_heart_rate.setForeground(text_color);
        lbl_o2.setForeground(text_color);
        lbl_connection_status.setForeground(text_color);

        this.contentPane.validate();
        this.contentPane.repaint();
    }

    public void updateStatus(String status) {
        this.lbl_connection_status.setText(" Patient " + Integer.toString(this.index+1) + ": " + status);
    }

    public void start() throws InterruptedException {
        //this.remoteIP = "142.244.215.246:5555";
        //send();//send video

        //frame.setVisible(true);
        Doctor_info di = new Doctor_info(this);
        System.out.println("LLLocal info port: " + localInfoPort);
        di.setLocalPort(this.localInfoPort);
        di.start();//get information from patient
        this.mute(true);

    }

    public void startStreaming(String remoteIP) {
        //System.out.println("IP = " + remoteIP);
        this.remoteIP = remoteIP + ":5555";

        send();
        receive();//receive video
    }

    private void send() {
        String mrl = !RuntimeUtil.isWindows() ? "v4l2:///dev/video0" : "dshow://";
        if (mrl.length() > 0) {
            String streamTo = this.remoteIP;

            String[] parts = streamTo.split(":");
            if (parts.length == 2) {
                String host = parts[0];
                int port = Integer.parseInt(parts[1]);

                String[] localOptions = {formatRtpStream(host, port), ":no-sout-rtp-sap", ":no-sout-standard-sap", ":sout-all", ":sout-keep",};

                localMediaPlayer.playMedia(mrl, localOptions);
            } else {
                //JOptionPane.showMessageDialog(frame, "You must specify host:port to stream to.", "Error", JOptionPane.ERROR_MESSAGE);
            }
        } else {
            //JOptionPane.showMessageDialog(frame, "You must specify source media, e.g. v4l2:///dev/video0 on Linux or dshow:// on Windows.", "Error", JOptionPane.ERROR_MESSAGE);
        }
    }

    private void receive() {
        String streamFrom = "230.0.0.1:" + this.localPort;
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
    
    public void mute(boolean mute){
        this.remoteMediaPlayer.mute(mute);
        this.localMediaPlayer.mute(mute);
        this.isMute = mute;
        
        if (mute) {
            this.btn_mute.setIcon(mute_icon);
        }else{
            this.btn_mute.setIcon(unmute_icon);
        }
        
    }
    
    public boolean isMute(){
        return this.isMute;
    }
}
