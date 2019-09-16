using System;
using System.Collections.Generic;
using System.Globalization;
using ChoETL;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace RainPrediction
{
    public class Program
    {
        public static DateTime resultDate;
        public static string dateInput;
        public static List<string> DatesSeen = new List<string>();
        public static List<string> RainSeen = new List<string>();
        public static List<string> DupeValues = new List<string>();


        static void Main(string[] args)
        {
            Console.WriteLine("Welcome! This program is to help you find the average rainfall for a specific date for the zip code 27612.");
            DateTime dateEntered = GetDateEntered();
            DateTime dateOnly = dateEntered.Date;
            string displayedDate = dateEntered.ToString("d");
            displayedDate = displayedDate.Substring(0, displayedDate.Length - 4);
            string shorterDisplayedDate = displayedDate.Substring(0, displayedDate.Length - 1);
            Console.WriteLine($"Finding rainfall amounts for {shorterDisplayedDate}");
            PullMatches(displayedDate);
            double averageRainForDate = GetAverage(RainSeen, DupeValues);

            JObject AverageRainfall = new JObject();
            AverageRainfall["For Date"] = shorterDisplayedDate;
            AverageRainfall["Average Rainfall"] = averageRainForDate;

            string json = AverageRainfall.ToString();
            Console.WriteLine(json);
        }


        public static DateTime GetDateEntered()
        {
            string defaultYearValue = "/2019";
            Console.WriteLine("Please type that date now(suggested format: mm/dd");
            string dateInput = Console.ReadLine();
            if (dateInput == "")
            {
                resultDate = DateTime.Today;
                DateTime dateOnly = resultDate.Date;
                Console.WriteLine("No date entered. Searching for today.");
            }
            else
            {
                if (dateInput.Length < 5 && dateInput.Contains(defaultYearValue) == false)
                {
                    dateInput = dateInput + defaultYearValue;
                }
                resultDate = ConvertDate(dateInput);
            }

            return resultDate;
        }
        public static DateTime ConvertDate(string dateInput)
        {
            string[] formats = {"M/d/yyyy",
                            "M/dd/yyyy", "MM/dd/yyyy",
                            "MM/dd/yyyy"};
            CultureInfo provider = CultureInfo.InvariantCulture;


            try
            {
                resultDate = DateTime.ParseExact(dateInput, formats,
                                                new CultureInfo("en-US"),
                                                DateTimeStyles.None);

            }
            catch (FormatException)
            {
                Console.WriteLine("Invalid date format. Please enter a new month and day for your search");
                GetDateEntered();
            }

            return resultDate;
        }
        static void PullMatches(string displayedDate)
        {
            var reader = new ChoCSVReader("27612-precipitation-data.csv").WithFirstLineHeader();
            dynamic rec;

            while ((rec = reader.Read()) != null)
            {
                if (rec.DATE != null && rec.PRCP != null)
                {
                    string dataDate = rec.DATE;
                    dataDate = dataDate.Substring(0, dataDate.Length - 4);
                    if (dataDate == displayedDate)
                    {
                        int count = 0;
                        string date = rec.DATE;
                        string prcp = rec.PRCP;
                        if (DatesSeen.Contains(date))
                        {
                            int combine = DatesSeen.IndexOf(date);
                            string checkAmount = Convert.ToString(combine);
                            if (DupeValues.Count != 0)
                            {
                                int primaryCount = DupeValues.Count;
                                for (int dupeCounter = 0; dupeCounter < DupeValues.Count; dupeCounter++)
                                {
                                    if (DupeValues[dupeCounter].Contains(checkAmount))
                                    {
                                        string dupeString = DupeValues[dupeCounter];
                                        string[] valuesArray = new string[2];
                                        valuesArray = dupeString.Split(",");
                                        int dupeNumber = Convert.ToInt32(valuesArray[1]);
                                        dupeNumber = dupeNumber + 1;
                                        valuesArray[1] = Convert.ToString(dupeNumber);
                                        dupeString = string.Join(",", valuesArray);
                                        DupeValues[dupeCounter] = dupeString;
                                        int finalCount = DupeValues.Count;
                                        if (finalCount > primaryCount)
                                        {
                                            int subtractAmount = finalCount - primaryCount;
                                            DupeValues.RemoveAt(subtractAmount - 1);
                                        }

                                    }
                                    else
                                    {
                                        string dupeString = $"{checkAmount}, 1";
                                        DupeValues.Add(dupeString);

                                    }

                                }

                            }
                            else
                            {
                                string dupeString = $"{checkAmount}, 1";
                                DupeValues.Add(dupeString);
                            }

                            string recordedPrcp = RainSeen[combine];
                            double oldValue = Convert.ToDouble(recordedPrcp);
                            double newValue = Convert.ToDouble(prcp);
                            double rainTotal = oldValue + newValue;
                            string newRain = Convert.ToString(rainTotal);
                            RainSeen[combine] = newRain;
                        }
                        else
                        {
                            DatesSeen.Add(date);
                            RainSeen.Add(prcp);
                        }
                        count = DatesSeen.Count - 1;
                    }
                }
            }
            return;
        }

        public static double GetAverage(List<string> RainSeen, List<string> DupeValues)
        {

            double totalRainTotal = 0;
            if (DupeValues.Count > 0)
            {
                for (int anotherDupeCounter = 0; anotherDupeCounter < DupeValues.Count; anotherDupeCounter++)
                {
                    string dupeString = DupeValues[anotherDupeCounter];
                    string[] valuesArray = new string[2];
                    valuesArray = dupeString.Split(",");
                    int dupeIndex = Convert.ToInt32(valuesArray[0]);
                    double dupeAmount = Convert.ToDouble(valuesArray[1]);
                    double totalRain = Convert.ToDouble(RainSeen[dupeIndex]);
                    double averageRain = totalRain / dupeAmount;
                    RainSeen[dupeIndex] = Convert.ToString(averageRain);
                }
            }
            for (int rainCounter = 0; rainCounter < RainSeen.Count; rainCounter++)
            {
                double rainAmount = Convert.ToDouble(RainSeen[rainCounter]);
                totalRainTotal = totalRainTotal + rainAmount;
            }
            double totalDupes = DupeValues.Count;
            double totalAverageRain = totalRainTotal / totalDupes;
            return totalAverageRain;
        }
    }
}

