/*
 * To change this template, choose Tools | Templates
 * and open the template in the editor.
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
 *
 * @author Ga Young Kim
 */
public class XMLExporter {

    String label;

    public XMLExporter(String patient_name) throws ParserConfigurationException {
        this.label = patient_name;
    }

    public Document startXML() throws TransformerConfigurationException, TransformerException, ParserConfigurationException {
        DocumentBuilderFactory docFactory = DocumentBuilderFactory.newInstance();
        DocumentBuilder builder = docFactory.newDocumentBuilder();
        Document document = builder.newDocument();

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

        TransformerFactory transformerFactory = TransformerFactory.newInstance();
        Transformer transformer = transformerFactory.newTransformer();

        transformer.setOutputProperty(OutputKeys.INDENT, "yes");
        transformer.setOutputProperty("{http://xml.apache.org/xslt}indent-amount", "2");

        DOMSource source = new DOMSource(document);
//            StreamResult result = new StreamResult(new File(filepath));


        StreamResult result = new StreamResult(System.out);

        transformer.transform(source, result);
        
        return document;

    }

    public Document createDataElement(Document document, Info patient_data) throws TransformerConfigurationException, TransformerException, ParserConfigurationException {
        Element dataElement = document.createElement("data");
        document.appendChild(dataElement);

//        Element timeElement = newdoc.createElement("time");
//        DateFormat dateFormat = new SimpleDateFormat("HH:mm:ss");
//        Date date = new Date();
//        timeElement.appendChild(newdoc.createTextNode(String.valueOf(dateFormat.format(date))));
//        dataElement.appendChild(timeElement);
//
//        Element heartRateElement = newdoc.createElement("hr");
//        heartRateElement.appendChild(newdoc.createTextNode(patient_data.heart_rate));
//        dataElement.appendChild(heartRateElement);
//
//        Element oxygenElement = newdoc.createElement("o2");
//        oxygenElement.appendChild(newdoc.createTextNode(patient_data.O2));
//        dataElement.appendChild(oxygenElement);
//
//        Element bpElement = newdoc.createElement("bp");
//        String bpValue = patient_data.blood_pressure[0] + "/" + patient_data.blood_pressure[1];
//        bpElement.appendChild(newdoc.createTextNode(bpValue));
//
//        Element memoElement = newdoc.createElement("memo");
//        memoElement.appendChild(newdoc.createTextNode(patient_data.memo));
//        dataElement.appendChild(memoElement);

        TransformerFactory transformerFactory = TransformerFactory.newInstance();
        Transformer transformer = transformerFactory.newTransformer();

        transformer.setOutputProperty(OutputKeys.INDENT, "yes");
        transformer.setOutputProperty("{http://xml.apache.org/xslt}indent-amount", "2");

        DOMSource source = new DOMSource(document);
//            StreamResult result = new StreamResult(new File(filepath));


        StreamResult result = new StreamResult(System.out);

        transformer.transform(source, result);

        return document;
    }

    public void exportXML(Document document) {
        try {
            Element exercise = (Element) document.getElementsByTagName("exerciseData").item(0);
            Document newdoc = exercise.getOwnerDocument();

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

            DOMSource source = new DOMSource(newdoc);
//            StreamResult result = new StreamResult(new File(filepath));


            StreamResult result = new StreamResult(System.out);

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
