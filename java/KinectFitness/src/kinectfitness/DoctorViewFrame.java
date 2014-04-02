/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
package kinectfitness;

import java.awt.Color;
import java.awt.GridLayout;
import java.util.logging.Level;
import java.util.logging.Logger;
import javax.swing.BorderFactory;
import javax.swing.ImageIcon;
import javax.swing.JFrame;
import javax.swing.JPanel;

/**
 *
 * @author paulomoreno
 */
public class DoctorViewFrame extends JFrame {

    private GridLayout allPatientsLayout;
    private GridLayout OnePatientsLayout;

    private boolean allPatients = true;
    private int onePatientIndex;

    private JPanel contentPane;

    private final Doctor[] doctor = new Doctor[8];

    public DoctorViewFrame() {
        super("HCI Fitness - Doctor view");
        
        allPatientsLayout = new GridLayout(2, 4, 5, 5);
        OnePatientsLayout = new GridLayout(1, 1);

        contentPane = new JPanel();
        contentPane.setBackground(new Color(255, 255,255));
        contentPane.setBorder(BorderFactory.createEmptyBorder(5, 5, 5, 5));
        contentPane.setLayout(allPatientsLayout);

        for (int i = 0; i < 8; i++) {
            try {
                doctor[i] = new Doctor(Integer.toString(5555 + i), 5020 + i, i, this).startDoctor();
                contentPane.add(doctor[i].getContent());

            } catch (Exception ex) {
                Logger.getLogger(FitnessMainJava.class.getName()).log(Level.SEVERE, null, ex);
            }
        }

        this.setIconImage(new ImageIcon(getClass().getResource("/icons/vlcj-logo.png")).getImage());
        this.setExtendedState(JFrame.MAXIMIZED_BOTH);
        this.setDefaultCloseOperation(JFrame.EXIT_ON_CLOSE);
        this.setContentPane(this.contentPane);
        this.pack();
        this.setVisible(true);

    }

    public void changeLayoutToOne(int index) {

        this.onePatientIndex = index;

        for (int i = 0; i < 8; i++) {
            contentPane.remove(doctor[i].getContent());
        }
        contentPane.add(doctor[this.onePatientIndex].getContent());
        contentPane.setLayout(OnePatientsLayout);
        contentPane.validate();
        contentPane.repaint();
    }

    public void changeLayoutToAll() {
        contentPane.remove(doctor[this.onePatientIndex].getContent());

        for (int i = 0; i < 8; i++) {
            contentPane.add(doctor[i].getContent());
        }
        contentPane.setLayout(allPatientsLayout);
        contentPane.validate();
        contentPane.repaint();
    }
    
    public void sendInfo(int heartrate, int index){
        Info info = new Info();
        info.heart_rate = Integer.toString(heartrate);
        info.O2 = "58";
        
        doctor[index].updateInfo(info);
    }

}
