using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.IO;

namespace RAInteractionTracker
{
    static class FileSystem
    {
        const string filesystemPath = @"C:\RAInteractionFileSystem";
        const string residentPath = filesystemPath + @"\Residents";
        const string allCounterPath = filesystemPath + @"\AllCounter";
        const string reportsPath = filesystemPath + @"\Reports";

        static internal void CheckDir()
        {
            // Checks if main folder exist
            if (Directory.Exists(filesystemPath) == false)
            {
                Directory.CreateDirectory(filesystemPath);
            }

            // Checks if resident folder exist
            if (Directory.Exists(residentPath) == false)
            {
                Directory.CreateDirectory(residentPath);
            }

            // Checks if reports folder exist
            if (Directory.Exists(reportsPath) == false)
            {
                Directory.CreateDirectory(reportsPath);
            }

            // Checks if allcounter file exist
            if (File.Exists(allCounterPath) == false)
            {
                // Creates the new file as a reset
                AddCount(true);
            }

            // Checks for all resident
            if (File.Exists(residentPath + @"\All") == false)
            {
                // Saves the new resident\
                Resident allResident = new Resident("All", "All", "");
                Save(allResident);
            }

            // Checks for jamboard resident
            if (File.Exists(residentPath + @"\Jamboard") == false)
            {
                // Saves the new resident\
                Resident jamResident = new Resident("Jamboard", "Jamboard", "");
                Save(jamResident);
            }
        }

        static public List<Resident> ReadRoster()
        {
            List<Resident> residents = new List<Resident>();

            // Gets all the files in the resident directory
            string[] files = Directory.GetFiles(residentPath);

            foreach (string file in files)
            {
                // For debugging
                //Console.WriteLine(file);

                // Loads in the current course
                Resident currResident = Load(file);

                residents.Add(currResident);
            }

            return residents;
        }

        // Loads a file
        private static Resident Load(string path)
        {
            // Opens the file
            FileStream fileIn = new FileStream(path, FileMode.Open, FileAccess.Read);
            IFormatter formatter = new BinaryFormatter();

            // Reads in the new course information
            Resident newResident = (Resident)formatter.Deserialize(fileIn);
            fileIn.Close();

            return newResident;
        }

        // Creates a new file
        public static void Save(Resident resident)
        {
            // Creates the file stream
            FileStream fileOut = new FileStream(residentPath + "\\" + resident.lastName, FileMode.Create);
            IFormatter formatter = new BinaryFormatter();

            // Serialize the object
            formatter.Serialize(fileOut, resident);
            fileOut.Close();
        }

        public static int ReadCount()
        {
            // Opens the file
            FileStream fileIn = new FileStream(allCounterPath, FileMode.Open, FileAccess.Read);
            IFormatter formatter = new BinaryFormatter();

            // Reads in the new course information
            int count = (int)formatter.Deserialize(fileIn);
            fileIn.Close();

            return count;
        }

        public static void AddCount(bool reset = false)
        {
            int count;
            if (reset == false)
            {
                count = ReadCount();
                count++;
            }
            else
            {
                count = 0;
            }

            // Creates the file stream
            FileStream fileOut = new FileStream(allCounterPath, FileMode.Create);
            IFormatter formatter = new BinaryFormatter();

            // Serialize the object
            formatter.Serialize(fileOut, count);
            fileOut.Close();
        }

        public static string PrintReport(List<Resident> residents, bool openReport = true)
        {
            DateTime currDate = DateTime.Now.Date;
            int allCount;
            int totalInteractions;
            int uniqueInteractions;
            int numParticipate;
            List<Resident> participants = new List<Resident>();
            List<Resident> nonparticipants = new List<Resident>();
            int residentCount;

            // Finds all and jamboard resident and sorts
            Resident allResident = new Resident("", "", "");
            Resident jamboardResident = new Resident("", "", "");
            foreach (Resident resident in residents)
            {
                if (resident.firstName.Equals("All"))
                    allResident = resident;

                else if (resident.firstName.Equals("Jamboard"))
                    jamboardResident = resident;

            }
            allResident.sortInteractions();
            jamboardResident.sortInteractions();
            allCount = allResident.interactions.Count();

            // Creates a txt file
            using (StreamWriter outFile = File.CreateText(reportsPath + @"\Interaction Report " + currDate.Month.ToString() + "-" + currDate.Day.ToString() + ".txt"))
            {
                // Header
                outFile.WriteLine(new string('-', 100));
                outFile.WriteLine(" Interaction Report for " + currDate.ToShortDateString());
                outFile.WriteLine(new string('-', 100));

                // Overview
                outFile.WriteLine(" Overview:" + Environment.NewLine);

                // Calculates the total number of interactions
                totalInteractions = 0;
                uniqueInteractions = 0;
                numParticipate = 0;
                foreach (Resident resident in residents)
                {
                    // Since jamboard doesnt include all
                    if (resident.firstName.Equals("Jamboard") == false)
                    {
                        totalInteractions += resident.interactions.Count();
                        uniqueInteractions += resident.interactions.Count() - allCount;
                    }
                    else
                    {
                        totalInteractions += resident.interactions.Count();
                    }

                    /* For debugging
                    Console.Clear();
                    Console.WriteLine(resident.firstName);
                    foreach (Resident.Interaction interaction in resident.interactions)
                        Console.WriteLine("     " + interaction.date.ToShortDateString() + " - " + interaction.description);
                    Console.WriteLine();
                    Console.WriteLine(totalInteractions);
                    Console.WriteLine(uniqueInteractions);
                    Console.WriteLine(resident.interactions.Count());
                    Console.ReadKey();*/


                    // Adds to the counter
                    if (resident.interactions.Count() - allCount != 0 && resident.firstName.Equals("Jamboard") == false)
                    {
                        numParticipate++;
                        participants.Add(resident);
                    }
                    else if (resident.interactions.Count() - allCount == 0 && resident.firstName.Equals("Jamboard") == false && resident.firstName.Equals("All") == false)
                        nonparticipants.Add(resident);
                }

                // Adjust for the jamboard and all resident
                totalInteractions -= allCount;
                residentCount = residents.Count() - 2;

                // prints
                outFile.WriteLine(" Total number of interactions: " + totalInteractions.ToString());
                outFile.WriteLine(" Total number of mass interactions: " + allCount.ToString());
                outFile.WriteLine(" Total number of one-on-one interactions: " + uniqueInteractions.ToString());
                outFile.WriteLine(" Total number of Jamboard interactions: " + (jamboardResident.interactions.Count()).ToString());
                outFile.WriteLine(" Total number of unique interactions: " + (uniqueInteractions + allCount + jamboardResident.interactions.Count()));
                outFile.WriteLine();
                outFile.WriteLine(" Total number of residents: " + residentCount);
                outFile.WriteLine(" Number of residents with one-on-one interactions: " + numParticipate.ToString());
                outFile.WriteLine(" Percentage of resident particpation: {0:0.00}%", (100 * (double)numParticipate / residentCount));
                outFile.WriteLine();
                outFile.WriteLine(" List of residents with one-on-one interactions:");
                foreach (Resident resident in participants)
                    outFile.WriteLine("     " + resident.firstName + " " + resident.lastName);
                outFile.WriteLine();
                outFile.WriteLine(" List of residents with no one-on-one interactions:");
                foreach (Resident resident in nonparticipants)
                    outFile.WriteLine("     " + resident.firstName + " " + resident.lastName);
                outFile.WriteLine();

                // mass interactions
                outFile.WriteLine(new string('-', 100));
                outFile.WriteLine(" Mass Interactions:" + Environment.NewLine);
                outFile.WriteLine(" Total number of mass interactions: " + allCount.ToString());
                outFile.WriteLine();
                outFile.WriteLine(" List of mass interactions:");
                foreach (Resident.Interaction interaction in allResident.interactions)
                    outFile.WriteLine("     " + interaction.date.ToShortDateString() + " - " + interaction.description);
                outFile.WriteLine();
                
                // Jamboard interactions
                outFile.WriteLine(new string('-', 100));
                outFile.WriteLine(" Jamboard Interactions:" + Environment.NewLine);
                outFile.WriteLine(" Number of jamboard interactions: " + (jamboardResident.interactions.Count()).ToString());
                outFile.WriteLine();
                outFile.WriteLine(" List of Jamboard interactions:"); 
                foreach (Resident.Interaction interaction in jamboardResident.interactions)
                    outFile.WriteLine("     " + interaction.date.ToShortDateString() + " - " + interaction.description);
                outFile.WriteLine();

                // Individual Interactions
                outFile.WriteLine(new string('-', 100));
                outFile.WriteLine(" One-on-one Interactions:" + Environment.NewLine);
                outFile.WriteLine(" Number of residents with one-on-one interactions: " + numParticipate.ToString());
                outFile.WriteLine();
                outFile.WriteLine(" List of unique interactions with each resident: ");
                outFile.WriteLine();
                foreach (Resident resident in participants)
                {
                    outFile.WriteLine(" --" + resident.firstName + " " + resident.lastName + "--");
                    outFile.WriteLine();

                    // Removes all of the all interactions
                    resident.sortInteractions();
                    List<Resident.Interaction> listOfUniqueInteractions = resident.interactions;
                    foreach (Resident.Interaction interaction in allResident.interactions)
                    {
                        listOfUniqueInteractions.Remove(interaction);
                    }

                    // Then we can print
                    foreach (Resident.Interaction interaction in listOfUniqueInteractions)
                        outFile.WriteLine("     " + interaction.date.ToShortDateString() + " - " + interaction.description);
                    outFile.WriteLine();
                }

                // Definitions
                outFile.WriteLine(new string('-', 100));
                outFile.WriteLine(" Definitions:" + Environment.NewLine);
                outFile.WriteLine(" Mass interactions - An interaction made to all residents at one (like a email to all my residents).");
                outFile.WriteLine(" One-on-one interactions - An interaction made just to one resident (like a resident participating in a game, or asking a question). Indicates participation.");
                outFile.WriteLine(" Unique interactions - Total count of mass interactions, Jamboard interactions, and one-on-one interactions.");
                outFile.WriteLine(" Jamboard interactions - Since you cannot track who does what on Jamboard, I decided to make this one catagory.");
            }

            // Opens the file
            if (openReport)
                System.Diagnostics.Process.Start(reportsPath + @"\Interaction Report " + currDate.Month.ToString() + "-" + currDate.Day.ToString() + ".txt");

            return reportsPath + @"\Interaction Report " + currDate.Month.ToString() + "-" + currDate.Day.ToString() + ".txt";
        }

        static internal List<Resident> AddResidents(string path, int startRow, int endRow, char firstChar, char lastChar)
        {
            List<Resident> residents = new List<Resident>();

            // Opens the excel file
            Excel excel = new Excel(path);

            // Reads through all of the rows
            for (int i = startRow; i <= endRow; i++)
            {
                // Builds the resident
                string firstName = excel.ReadCell(i, firstChar);
                string lastName = excel.ReadCell(i, lastChar);
                Resident resident = new Resident(firstName, lastName, "");

                // adds to the list and save
                residents.Add(resident);
                Save(resident);
            }

            // Closes excel file
            excel.Close();

            return residents;
        }
    }
}
