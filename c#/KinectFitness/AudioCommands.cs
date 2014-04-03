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
 myCommands.setFunction("play", Button_Play);//tell AudioCommands what to do when the speech "play" is recognized. The second parameter is a function with 2 parameters: object and RoutedEventArgs, respectively
 myCommands.setFunction("record", Button_Record);
 myCommands.setFunction("options", Button_Options);

 Example 2
 AudioCommands myCommands = new AudioCommands(0.8, "quit", "options", "tutorial", "play", "record");//instantiate an AudioCommands object with the possible commands
 myCommands.setFunction("play", Button_Play);//tell AudioCommands what to do when the speech "play" is recognized. The second parameter is a function with 2 parameters: object and RoutedEventArgs, respectively
 myCommands.setFunction("record", Button_Record);
 myCommands.setFunction("options", Button_Options);


In both examples, the speech recognizer will keep listening to commands indefinitely. The 0.8 in Example 2 is a number called "confidence" that varies between 0 and 1. The higher, more precise is the speech recognition. The default is 0.9(values higher than this makes it difficult to recognize valid commands)
To force the speech recognizer to stop listening,you must use the static function StopSpeechRecognition, which takes as the only parameter an AudioCommands object.
Example: AudioCommands.StopSpeechRecognition(myAudioCommandsObject);

You can also use the methods public void setConfidence(double confidence)
and public double getConfidence()
to set the confidence and get the current confidence, respectively(though you will probably never use because you can set it on the constructor, as shown in example 2
*/

namespace KinectFitness
{
    public class AudioCommands
    {
        private SpeechRecognitionEngine recognizer;//Speech Recognition
        private double confidence;//varies between 0 and 1. The higher, more precise is the speech recognition. The default is 0.9(values higher than this makes it difficult to recognize valid commands)
        private Thread RecThread;//the thread that will run speech recognition

        private struct cmnFunct//struct that stores the command and the function associated with it
        {
            public string command;
            public Action<object, RoutedEventArgs> myfunc;
        }

        private cmnFunct[] cmnFuncts;//array of commands and associated functions

        public AudioCommands(params string[] commands)
            : this(0.9, commands)
        {
        }

        public AudioCommands(double confidence, params string[] commands)
        {
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
            while (cmnFuncts[i].command != command)
                ++i;

            cmnFuncts[i].myfunc = func;

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

        public static void StopSpeechRecognition(AudioCommands commands)
        {
            commands.RecThread.Abort();
            commands.RecThread = null;
            commands.recognizer.UnloadAllGrammars();
            commands.recognizer.Dispose();
            commands.cmnFuncts = null;
        }

    }
}
