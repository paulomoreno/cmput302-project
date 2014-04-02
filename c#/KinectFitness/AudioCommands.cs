using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Speech.Recognition;
using System.Windows;
using System.Threading;

/*How to use by examples
 Example 1
 AudioCommands myCommands = new AudioCommands("quit", "options", "tutorial", "play", "record");//instantiate an AudioCommands object with the possible commands
 myCommands.setFunction("play", Button_Play);//tell AudioCommands what to do when the speech "play" is recognized. The second parameter is a function
 myCommands.setFunction("record", Button_Record);
 myCommands.setFunction("options", Button_Options);

In this example, after one of the words is recognized, the AudioCommands stops recognizing speeches. It is set to recognize only once.
 Example 2
 AudioCommands myCommands = new AudioCommands(false, 0.8, "quit", "options", "tutorial", "play", "record");//instantiate an AudioCommands object with the possible commands
 myCommands.setFunction("play", Button_Play);//tell AudioCommands what to do when the speech "play" is recognized. The second parameter is a function
 myCommands.setFunction("record", Button_Record);
 myCommands.setFunction("options", Button_Options);
Like the example above, but the speech recognizer will keep listening to commands indefinitely(false parameter, the default is true). The 0.8 is a number that varies between 0 and 1. The higher, more precise is the speech recognition. The default is 0.9(values higher than this makes it difficult to recognize valid commands)
 
*/

namespace KinectFitness
{
    public class AudioCommands
    {
        private SpeechRecognitionEngine recognizer;//Speech Recognition
        private double confidence;//varies between 0 and 1. The higher, more precise is the speech recognition. The default is 0.9(values higher than this makes it difficult to recognize valid commands)
        private Thread RecThread;//the thread that will run speech recognition
        private Boolean RecognizeOnce;//flag that indicates if it should recognize only once ore not

        private struct cmnFunct//struct that stores the command and the function associated with it
        {
            public string command;
            public Action<object, RoutedEventArgs> myfunc;
        }

        private cmnFunct[] cmnFuncts;//array of commands and associated functions

        public AudioCommands(params string[] commands)
            : this(true, 0.9, commands)
        {
        }

        public AudioCommands(Boolean RecognizeOnce, double confidence, params string[] commands)
        {
            this.RecognizeOnce = RecognizeOnce;
            this.confidence = confidence;
            // Create a new SpeechRecognitionEngine instance.
            this.recognizer = new SpeechRecognitionEngine();


            int length = commands.Length;//store the number of commands
            string[] cm = new string[length];//Create new string to use in Choices, based on the commands string array passed by parameter
            this.cmnFuncts = new cmnFunct[length];//Instantiate array of commands and functions
            for (int i = 0; i < length; ++i)
            {
                cm[i] = commands[i];
                this.cmnFuncts[i].command = cm[i];
            }

            //Create a simple grammar that recognizes some words and/or statements
            Choices commandChoices = new Choices(cm);

            // Create a GrammarBuilder object and append the Choices object.
            GrammarBuilder gb = new GrammarBuilder();
            gb.Append(commandChoices);

            // Create the Grammar instance and load it into the speech recognition engine.
            Grammar g = new Grammar(gb);
            this.recognizer.LoadGrammar(g);

            // Assign input to the recognizer.
            this.recognizer.SetInputToDefaultAudioDevice();

            // Register a handler for the SpeechRecognized event.
            this.recognizer.SpeechRecognized += sre_SpeechRecognized;

            // Initialize Recognizer thread
            this.RecThread = new Thread(new ThreadStart(RecThreadFunction));
            this.RecThread.Start();
        }

        //set the function of the command. What is supposed to be done when the command is detected.
        public void setFunction(string command, Action<object, RoutedEventArgs> func)
        {
            int i = 0;
            while (i < cmnFuncts.Count() && cmnFuncts[i].command != command )
                ++i;

            if (i < cmnFuncts.Count())
            {
                cmnFuncts[i].myfunc = func;
            }

        }

        public void setConfidence(double confidence)//set confidence
        {
            this.confidence = confidence;
        }

        public double getConfidence()//get confidence
        {
            return this.confidence;
        }

        // Create a simple handler for the SpeechRecognized event.
        private void sre_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {

            if (e.Result.Confidence >= this.confidence)//make confidence check
            {
                int i = 0;
                while (cmnFuncts[i].command != e.Result.Text)//find spoken command
                    ++i;

                Application.Current.Dispatcher.Invoke((Action)(() =>
                {
                    cmnFuncts[i].myfunc.Invoke(sender, new RoutedEventArgs());//execute function associated with command
                }));

                if (this.RecognizeOnce == true)//if it's supposed to recognize only once
                {
                    StopSpeechRecognition();//stop recognition
                }

            }
        }

        private void RecThreadFunction()
        {
            //This function is on a separate thread (RecThread). This will loop the recognizer receive call
            while (true)
            {
                try
                {
                    this.recognizer.Recognize();
                }
                catch (Exception ex)
                {
                    //Handle errors. Most errors are caused from the recognizer not recognizing speech
                    Console.WriteLine(ex.Message);
                }
            }
        }

        public void StopSpeechRecognition()
        {
            this.RecThread.Abort();
            this.RecThread = null;
            this.recognizer.UnloadAllGrammars();
            this.recognizer.Dispose();
            this.cmnFuncts = null;
        }

    }
}
