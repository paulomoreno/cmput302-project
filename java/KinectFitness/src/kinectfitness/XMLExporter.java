/*
 * This class is responsible for the XML export system of the doctor side
 * application.
 */
package kinectfitness;

import java.io.File;
import java.net.URISyntaxException;
import java.net.URL;
import java.text.DateFormat;
import java.text.SimpleDateFormat;
import java.util.Date;
import java.util.logging.Level;
import java.util.logging.Logger;
import javax.xml.parsers.DocumentBuilder;
import javax.xml.parsers.DocumentBuilderFactory;
import javax.xml.parsers.ParserConfigurationException;
import javax.xml.transform.OutputKeys;
import javax.xml.transform.Transformer;
import javax.xml.transform.TransformerConfigurationException;
import javax.xml.transform.TransformerException;
import javax.xml.transform.TransformerFactory;
import javax.xml.transform.dom.DOMSource;
import javax.xml.transform.stream.StreamResult;

import org.w3c.dom.Document;
import org.w3c.dom.Element;

/**
 * This class is called by the Doctor_info class to allow the data received
 * from the patient to be exported as an XML file.  The structure of the XML is:
 * <KinectFitness>
 *       <patient>
 *           <label>patient1</label>
 *           <date>2014-04-10 19:16:40</date>
 *       </patient>
 *       <exerciseData>
 *           <data>
 *               <time>19:16:40</time>
 *               <hr>100</hr>
 *               <o2>93</o2>
 *               <bp>140/100</bp>
 *               <memo/>
 *           </data>
 *           (the data tag is generated with the values every 1/3 of a second)
 *      </exerciseData>
 *  </KinectFitness>
 * The XML file is generated with the name "label-of-patient-timestamp.xml" and 
 * within the PatientData directory in the root java project directory.
 * 
 * @author Ga Young Kim
 */
public class XMLExporter {

    // label associated with the patient (patient1, patient2, ..., patient8)
    String label;
    // DOMDocument object
    Document document;

    /**
     * Main constructor of the class which starts the DOMDocument to create the
     * XML file.
     * @param patient_name      label associated with the patient 
     *                          (patient1, patient2, ..., patient8)
     * @throws ParserConfigurationException 
     */
    public XMLExporter(String patient_name) throws ParserConfigurationException {
        this.label = patient_name;
        DocumentBuilderFactory docFactory = DocumentBuilderFactory.newInstance();
        DocumentBuilder builder = docFactory.newDocumentBuilder();
        Document doc = builder.newDocument();
        this.document = doc;
    }

    /**
     * The method is used to start generating the XML file.  It creates the 
     * KinectFitness, Patient elements (with all of its children elements) and 
     * creates the empty exerciseData element.
     * @return                  exerciseData DOMelement
     * @throws TransformerConfigurationException
     * @throws TransformerException
     * @throws ParserConfigurationException 
     */
    public Element startXML() throws TransformerConfigurationException, TransformerException, ParserConfigurationException {

        Element rootElement = document.createElement("KinectFitness");
        document.appendChild(rootElement);

        Element patientElement = document.createElement("patient");
        rootElement.appendChild(patientElement);

        Element labelElement = document.createElement("label");
        labelElement.appendChild(document.createTextNode(label));
        patientElement.appendChild(labelElement);

        Element dateElement = document.createElement("date");
        DateFormat dateFormat = new SimpleDateFormat("yyyy-MM-dd HH:mm:ss");
        Date date = new Date();
        dateElement.appendChild(document.createTextNode(String.valueOf(dateFormat.format(date))));
        patientElement.appendChild(dateElement);

        Element exerciseDataElement = document.createElement("exerciseData");
        rootElement.appendChild(exerciseDataElement);
        return exerciseDataElement;

    }

    /**
     * This method is used to create the data DOMElements that are the children
     * nodes of the exerciseData element.  This data DOMElement is generated then
     * appended to the exerciseData element every 1/3 of a second.
     * @param exercise                  the exerciseData DOMElement
     * @param patient_data              the patient data received
     * @throws TransformerConfigurationException
     * @throws TransformerException
     * @throws ParserConfigurationException 
     */
    public void createDataElement(Element exercise, Info patient_data) throws TransformerConfigurationException, TransformerException, ParserConfigurationException {
        Element dataElement = document.createElement("data");
        exercise.appendChild(dataElement);

        Element timeElement = document.createElement("time");
        DateFormat dateFormat = new SimpleDateFormat("HH:mm:ss");
        Date date = new Date();
        timeElement.appendChild(document.createTextNode(String.valueOf(dateFormat.format(date))));
        dataElement.appendChild(timeElement);

        Element heartRateElement = document.createElement("hr");
        heartRateElement.appendChild(document.createTextNode(patient_data.heart_rate));
        dataElement.appendChild(heartRateElement);

        Element oxygenElement = document.createElement("o2");
        oxygenElement.appendChild(document.createTextNode(patient_data.O2));
        dataElement.appendChild(oxygenElement);

        Element bpElement = document.createElement("bp");
        String bpValue = patient_data.blood_pressure[0] + "/" + patient_data.blood_pressure[1];
        bpElement.appendChild(document.createTextNode(bpValue));
        dataElement.appendChild(bpElement);
    }

    /**
     * This method is used to export the finished XML file at the end of the
     * session.
     */
    public void exportXML() {
        try {
            ClassLoader classLoader = Thread.currentThread().getContextClassLoader();
            URL url = classLoader.getResource("");
            File file = new File(url.toURI());
            String[] filepathinfo = file.getAbsolutePath().split("\\\\");

            String projectPath = "" + filepathinfo[0];

            for (int i = 1; i < filepathinfo.length - 4; i++) {
                projectPath += "/" + filepathinfo[i];
            }

            String path = projectPath + "/java/PatientData";

            File parentDirectory = new File(path);
            parentDirectory.mkdirs();

            DateFormat dateFormat = new SimpleDateFormat("yyyyMMdd-HHmmss");
            Date date = new Date();
            String currentDate = String.valueOf(dateFormat.format(date));
            String filepath = path + "/" + label + "-" + currentDate + ".xml";

            TransformerFactory transformerFactory = TransformerFactory.newInstance();
            Transformer transformer = transformerFactory.newTransformer();

            transformer.setOutputProperty(OutputKeys.INDENT, "yes");
            transformer.setOutputProperty("{http://xml.apache.org/xslt}indent-amount", "2");

            DOMSource source = new DOMSource(document);
            StreamResult result = new StreamResult(new File(filepath));

            transformer.transform(source, result);

        } catch (URISyntaxException ex) {
            Logger.getLogger(XMLExporter.class.getName()).log(Level.SEVERE, null, ex);
        } catch (TransformerConfigurationException ex) {
            Logger.getLogger(XMLExporter.class.getName()).log(Level.SEVERE, null, ex);
        } catch (TransformerException ex) {
            Logger.getLogger(XMLExporter.class.getName()).log(Level.SEVERE, null, ex);
        }
    }
}
