using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BA371_assign1 {
    class Program {

        public struct Period {
            public List<DataPoint> Values;
        }

        public struct DataPoint {
            public float capital;
            public float interest;
        }

        static void Main(string[] args) {
            List<Period> compoundGraph = new List<Period>();
            float capital = GetUserInput("Starting Captial $",0,1000);
            float lowInterest = GetUserInput("Low Interest Rate %",0,10);
            float highInterest = GetUserInput("High Interet Rate %",(int) lowInterest,10);
            int intervals = (int) GetUserInput("How many intervals",1,25); //rounded down
            int periods = (int) GetUserInput("how many periods per inverval",1,25); //rounded down
            float interestStep = Calc_Interval(lowInterest,highInterest,intervals);      //how much the interest incraments between intervals

            //for each Period
            for(int count = 1; count <= periods; count++) {
                //initializes a new period
                Period period = new Period();
                period.Values = new List<DataPoint>();
                float interest = lowInterest;
                for(int interestCount = 0; interestCount <= intervals; interestCount++) {
                    DataPoint dataPoint = new DataPoint();
                    dataPoint.capital = GetAmount(capital,interest,count); //calculates capital at the given point
                    dataPoint.interest = (float) Math.Round(interest,2);
                    period.Values.Add(dataPoint); //add an interval to the period
                    interest += interestStep;
                    Console.WriteLine(interestCount);
                }

                compoundGraph.Add(period); //add the period to the arraylist
            }
            WriteFile(compoundGraph);
            ExitProgram();
        }

        //Makes three attemps to get valid input from the user. Exits the application after the third failed attempt
        static float GetUserInput(string text,float minValue,float maxValue) {
            int attempt = 0;
            do {
                Console.Write(text + " [" + minValue + " - " + maxValue + "]: ");
                string userInput = Console.ReadLine();
                //if the input is numeric and is between the min and max values
                if(float.TryParse(userInput,out float returnValue) && returnValue <= maxValue && returnValue >= minValue) {
                    return returnValue;
                }
                else {
                    Console.WriteLine("Bad Input: Expected input range from: " + minValue + " to " + maxValue);
                }
                attempt++;
                Console.WriteLine("Attempt: " + attempt + " of 3");
            } while(attempt < 3);
            ExitProgram();  //exit Program after three bad attempts
            return 0;
        }

        //returns the amount for the given period and interest rate
        static float GetAmount(float startingCaptial,float interestRate,int period) {
            return startingCaptial * ((float)Math.Pow(1 + (interestRate / 100),period));
        }

        //calculates the difference between each interest inverval
        static float Calc_Interval(float lowInterest,float highInterest,int intervals) {
            return (highInterest - lowInterest) / intervals;
        }

        //Write The Output file From Scratch
        static void WriteFile(List<Period> compoundGraph) {
            if(File.Exists("C:\\temp\\output.html")) {
                File.Delete("C:\\temp\\output.html");
            }
            File.Create("C:\\temp\\output.html").Close();
            Console.WriteLine("Writing to file C:\\temp\\output.html");
            StreamWriter Writer = new StreamWriter("C:\\temp\\output.html");
            string dataString = AppendData(compoundGraph);

            //(HTML Text) + dataString + (HTML text)
            string html = "<html> <head> <script type=\"text/javascript\" src=\"https://www.google.com/jsapi\"></script> <script type=\"text/javascript\"> google.load(\"visualization\", \"1\", {packages:[\"linechart\"]}); google.setOnLoadCallback(drawChart); function drawChart() { var data = google.visualization.arrayToDataTable([" + dataString + " ]); var chart = new google.visualization.LineChart(document.getElementById('chart_div')); chart.draw(data, { width: 1000, height: 500, legend: 'bottom', title: 'Compound Interest'})   } </script> </head> <body> <div id =\"chart_div\"></div> </body> </html>";
            Writer.WriteLine(html);
            Writer.Close();
            Console.WriteLine("Success!");
        }

        //converts the Saved Data from memory into a string format that javascript can read
        static string AppendData(List<Period> compoundGraph) {
            //sets up labels
            string dataString = "['Period'";
            foreach(DataPoint data in compoundGraph[0].Values) {
                dataString += ",'" + data.interest + "%'";
            }
            dataString += "],";
            //adds each all the values 
            for(int index = 0; index < compoundGraph.Count; index++) {
                dataString += "['" + (index + 1) + "'";
                foreach(DataPoint data in compoundGraph[index].Values) {
                    dataString += ", " + data.capital;
                }
                dataString += "],";
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
