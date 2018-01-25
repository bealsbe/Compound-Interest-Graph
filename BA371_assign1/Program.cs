using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BA371_assign1
{
    class Program
    {

        //struct that holds all the data for each period 
        public struct Period
        {
            public List<DataPoint> Values;
        }

        //struct that represents a single point with captial and interest rate
        public struct DataPoint
        {
            public float capital;
            public float interest;
        }

        static void Main()
        {
            //calls a method that calculates all of the values for the compound graph and stores them in an arraylist
            List<Period> compoundGraph = CalculateGraph();

            //Attemps to write the HTML File
            try {
                WriteFile(compoundGraph);
                Console.WriteLine("Success!");
            }
            catch (Exception e) {
                //prints out the exact error message
                Console.WriteLine(e.Message.ToString());
                Console.WriteLine("\nThe was an problem writing to the file");
            }

            ExitProgram();
        }

        //Function that calculates all of the values for the graph and saves them in a List of Values
        static List<Period> CalculateGraph()
        {
            //Initial Values
            float capital = GetUserInput("Starting Captial $", 0, 1000);
            float lowInterest = GetUserInput("Low Interest Rate %", 0, 10);
            float highInterest = GetUserInput("High Interet Rate %", lowInterest, 10);
            int intervals = (int)GetUserInput("How many intervals", 1, 25); //inputs are automatically rounded down
            int periods = (int)GetUserInput("how many periods per inverval", 1, 25); //inputs are automatically rounded down

            //how much the interest incraments between the intervals
            float interestStep = (highInterest - lowInterest) / intervals;

            //an Arraylist of the calculated Interest
            List<Period> compoundGraph = new List<Period>();

            //for each Period
            for (int count = 1; count <= periods; count++) {
                //initializes a new period
                Period period = new Period();
                period.Values = new List<DataPoint>();

                //keeps track of the current interest rate
                float interest = lowInterest;

                //for each interest interval
                for (int interestCount = 0; interestCount <= intervals; interestCount++) {
                    DataPoint dataPoint = new DataPoint();
                    dataPoint.capital = capital * ((float)Math.Pow(1 + (interest / 100), count));
                    dataPoint.interest = (float)Math.Round(interest, 2);
                    period.Values.Add(dataPoint); //add an interval to the period
                    interest += interestStep; //steps up to the next interest interval
                }
                compoundGraph.Add(period); //add the period to the arraylist
            }
            return compoundGraph;
        }

        //Makes three attemps to get valid input from the user. Exits the application after the third failed attempt      
        static float GetUserInput(string text, float minValue, float maxValue)
        {
            //Number of Input Attempts
            int attempt = 0;

            do {
                Console.Write(text + " [" + minValue + " - " + maxValue + "]: ");
                string userInput = Console.ReadLine();

                //if the input is numeric and is between the min and max values
                if (float.TryParse(userInput, out float returnValue) && returnValue <= maxValue && returnValue >= minValue)
                    return returnValue; //returns the user input and exists the function       
                
                Console.WriteLine("Invalid Input: Expected input range from: " + minValue + " to " + maxValue);
                attempt++;
                Console.WriteLine("Attempt: " + attempt + " of 3");
            } while (attempt < 3);

            ExitProgram();  //exit program after three bad attempts
            return 0;
        }

        //Writes the output HTML file.  Returns true if successful and false if it fails
        static void WriteFile(List<Period> compoundGraph)
        {
            //String for the dynmically created html file
            string html =
                "<html>\n" +
                    "<head>\n" +
                         "<script type =\"text/javascript\" src=\"https://www.google.com/jsapi\"></script>\n" +
                         "<script type =\"text/javascript\">\n" +
                             "google.load(\"visualization\", \"1\", {packages:[\"linechart\"]});\n" +
                             "google.setOnLoadCallback(drawChart);\n" +
                              "function drawChart(){\n" +
                                "var data = google.visualization.arrayToDataTable([\n" +
                                AppendData(compoundGraph) + //calls the Appenddata function which converts the saved list of values into a javascript table
                               "]);\n" +
                                "var chart = new google.visualization.LineChart(document.getElementById('chart_div'));\n" +
                                "chart.draw(data, { width: 1000, height: 500, legend: 'bottom', title: 'Compound Interest'})\n" +
                                "}\n" +
                              "</script>\n" +
                        "</head>\n" +
                         "<body>\n" +
                            "<div id =\"chart_div\"></div>\n" +
                         "</body>\n" +
                "</html>\n";

            //Creates a timestamp as a unique file name
            string timestamp = string.Format("{0:yyyy-MM-dd_hh-mm-ss-tt}", DateTime.Now);
            Console.WriteLine("Writing to file C:\\CompoundGraph\\" + timestamp + ".html");

            //Creates a C:\CompoundGraph folder if one does not exist on the System
            if (!Directory.Exists("C:\\CompoundGraph"))
                Directory.CreateDirectory("C:\\CompoundGraph");

            //Creates a streamwriter and writes the saved string into the file
            StreamWriter Writer = new StreamWriter("C:\\CompoundGraph\\" + timestamp + ".html");
            Writer.WriteLine(html);
            Writer.Close();
        }

        //converts the Saved Data from memory into a table format that javascript can read
        static string AppendData(List<Period> compoundGraph)
        {
            //string that is created based off the calcuated values
            string dataString = "['Period'";

            //sets up labels
            foreach (DataPoint data in compoundGraph[0].Values) {
                dataString += ",'" + data.interest + "%'";
            }
            dataString += "],\n";

            //Nestedloop that goes through all of the values
            for (int index = 0; index < compoundGraph.Count; index++) {
                dataString += "['" + (index + 1) + "'";
                foreach (DataPoint data in compoundGraph[index].Values) {
                    dataString += ", " + data.capital;
                }
                dataString += "],\n";
            }
            return dataString;
        }

        //exits the program
        static void ExitProgram()
        {
            Console.WriteLine("\npress any key to exit ...");
            Console.ReadKey();
            Environment.Exit(0);
        }
    }
}
