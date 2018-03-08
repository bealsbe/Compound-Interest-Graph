using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Website.Models
{
    public class CompoundGraph
    {
        public float startingCaptial { get; private set; }
        public float lowInterest { get; private set; }
        public float highInterest { get; private set; }
        public int intervals { get; private set; }
        public int periods { get; private set; }

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

        public CompoundGraph(float startingCaptial , float lowInterest , float highInterest , int intervals , int periods)
        {
            this.startingCaptial = startingCaptial;
            this.lowInterest = lowInterest;
            this.highInterest = highInterest;
            this.intervals = intervals;
            this.periods = periods;
        }

        //converts the Saved Data from memory into a table format
        public string AppendData()
        {
            List<Period> graph = CalculateGraph();
            string dataString = "['Period'";
            foreach(DataPoint data in graph[0].Values) //sets up labels
                dataString += ",'" + data.interest + "%'";
            dataString += "],";

            //Nestedloop that goes through all of the values
            for(int index = 0; index < graph.Count; index++)
            {
                dataString += "['" + (index + 1) + "'";
                foreach(DataPoint data in graph[index].Values)
                    dataString += ", " + data.capital;
                dataString += "],";
            }
            return dataString;
        }

        //Function that calculates all of the values for the graph and saves them in a List of Values
        private List<Period> CalculateGraph()
        {
            //how much the interest incraments between the intervals
            float interestStep = (highInterest - lowInterest) / intervals;
            List<Period> graph = new List<Period>();
            //Loops Through all the Periods
            for(int i = 1; i <= periods; i++)
            {
                Period period = new Period();
                period.Values = new List<DataPoint>();
                float interest = lowInterest;  //keeps track of the current interest rate

                //for each interest interval
                for(int j = 0; j <= intervals; j++)
                {
                    DataPoint dataPoint = new DataPoint();
                    dataPoint.capital = startingCaptial * ((float)Math.Pow(1 + (interest / 100) , i));
                    dataPoint.interest = (float)Math.Round(interest , 2);
                    period.Values.Add(dataPoint); //add an interval to the period
                    interest += interestStep; //steps up to the next interest interval
                }
                graph.Add(period); 
            }
            return graph;
        }
    }
}
