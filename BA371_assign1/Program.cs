using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BA371_assign1 {
    class Program {

        //struct that holds all the data for each period 
        public struct Period {
            public List<DataPoint> Values;
        }

        //struct that represents a single point with captial and interest rate
        public struct DataPoint {
            public float capital;
            public float interest;
        }

        static void Main() {
            List<Period> compoundGraph = CalculateGraph(); //calls a method that calculates all of the values for the compound graph
            if(WriteFile(compoundGraph)) { //Calls WriteFile which returns true if successful and false if it fails. 
                Console.WriteLine("Success!");
            }
            else {
                Console.WriteLine("The was an problem writing to the file");
            }
            ExitProgram();
        }

        //Gets user input
        static List<Period> CalculateGraph() {
            float capital = GetUserInput("Starting Captial $" , 0 , 1000);
            float lowInterest = GetUserInput("Low Interest Rate %" , 0 , 10);
            float highInterest = GetUserInput("High Interet Rate %" , lowInterest , 10);
            int intervals = (int)GetUserInput("How many intervals" , 1 , 25); //inputs are automatically rounded down
            int periods = (int)GetUserInput("how many periods per inverval" , 1 , 25); //inputs are automatically rounded down

            List<Period> compoundGraph = new List<Period>();
            float interestStep = Calc_Interval(lowInterest , highInterest , intervals);  //how much the interest incraments between intervals
            //for each Period
            for(int count = 1; count <= periods; count++) {
                //initializes a new period
                Period period = new Period();
                period.Values = new List<DataPoint>();
                float interest = lowInterest;

                //for each interest interval
                for(int interestCount = 0; interestCount <= intervals; interestCount++) {
                    DataPoint dataPoint = new DataPoint();
                    dataPoint.capital = GetAmount(capital , interest , count); //calculates capital at the given point
                    dataPoint.interest = (float)Math.Round(interest , 2);
                    period.Values.Add(dataPoint); //add an interval to the period
                    interest += interestStep;
                }

                compoundGraph.Add(period); //add the period to the arraylist
            }
            return compoundGraph;
        }

        //Makes three attemps to get valid input from the user. Exits the application after the third failed attempt
        static float GetUserInput(string text , float minValue , float maxValue) {
            int attempt = 0;
            do {
                Console.Write(text + " [" + minValue + " - " + maxValue + "]: ");
                string userInput = Console.ReadLine();
                //if the input is numeric and is between the min and max values
                if(float.TryParse(userInput , out float returnValue) && returnValue <= maxValue && returnValue >= minValue) {
                    return returnValue;
                }
                else {
                    Console.WriteLine("Bad Input: Expected input range from: " + minValue + " to " + maxValue);
                }
                attempt++;
                Console.WriteLine("Attempt: " + attempt + " of 3");
            } while(attempt < 3);
            ExitProgram();  //exit program after three bad attempts
            return 0;
        }

        //returns the amount for the given period and interest rate
        static float GetAmount(float startingCaptial , float interestRate , int period) {
            return startingCaptial * ((float)Math.Pow(1 + (interestRate / 100) , period));
        }

        //calculates the difference between each interest inverval
        static float Calc_Interval(float lowInterest , float highInterest , int intervals) {
            return (highInterest - lowInterest) / intervals;
        }

        //Write The Output file From Scratch
        static bool WriteFile(List<Period> compoundGraph) {
            string timestamp = string.Format("{0:yyyy-MM-dd_hh-mm-ss-tt}" , DateTime.Now);
            try {
                Console.WriteLine("Writing to file C:\\temp\\" + timestamp + ".html");
                StreamWriter Writer = new StreamWriter("C:\\temp\\" + timestamp + ".html");

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
                Writer.WriteLine(html);
                Writer.Close();
            }
            catch(Exception e) {
                //prints out the exact error message to help with troubleshooting
                Console.WriteLine(e.Message.ToString());
                return false;
            }
            return true;
        }

        //converts the Saved Data from memory into a table format that javascript can read
        static string AppendData(List<Period> compoundGraph) {

            //sets up labels
            string dataString = "['Period'";
            foreach(DataPoint data in compoundGraph[0].Values) {
                dataString += ",'" + data.interest + "%'";
            }
            dataString += "],\n";

            //adds all the values 
            for(int index = 0; index < compoundGraph.Count; index++) {
                dataString += "['" + (index + 1) + "'";
                foreach(DataPoint data in compoundGraph[index].Values) {
                    dataString += ", " + data.capital;
                }
                dataString += "],\n";
            }
            return dataString;
        }

        //exits the program
        static void ExitProgram() {
            Console.WriteLine("\npress any key to exit ...");
            Console.ReadKey();
            Environment.Exit(1);
        }
    }
}
