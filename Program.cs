using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using System.Net.Mail;

namespace RAInteractionTracker
{
    enum Screens { ViewAInteraction, ViewResidents, AddAInteraction, DeleteAInteraction, PrintReport, AddResidents, EndProg };
    enum numResidents { none, all, one };

    class Program
    {
        static public ConsoleColor currColor;
        public const int WindowWidth = 119;
        public const int WindowHeight = 29;
        static private bool endProg = false;
        static private Screens userChoice;
        static private List<Resident> residents;
        static private Resident currResident;
        static private numResidents numR;
        static private int currHeight, currCol;

        static void Main(string[] args)
        {
            // Checks if the directories exit
            FileSystem.CheckDir();

            // Loads in file
            residents = FileSystem.ReadRoster();

            // Test if its an auto run
            // Usage needs to be RAInteractionTracter m <recipent email>
            if ( args.Length == 2 && args[0].ToLower().Equals("m"))
            {
                // Make a new mailer
                Mailer mailer = new Mailer("STMP-SERVER", "BOT-EMAIL", "PASSWORD");

                // Makes a new mail
                mailer.NewMail("RECIPENT-MAIL", "SUBJECT", "BODY");

                // runs the report, returns the path
                string reportPath = FileSystem.PrintReport(residents, false);

                // Adds the file
                mailer.AddAttachement(reportPath);

                // Sends the email
                mailer.Send();

                return;
            }

            // Sets up the main menu
            BranchingScreen mainMenu = new BranchingScreen("Main Menu");
            currColor = ConsoleColor.DarkMagenta;
            mainMenu.SetChoices(new List<string> { "View an Interaction", "View Residents", "Add an Interaction", "Delete an Interaction", "Print Report", "Add Residents", "End Program" });
            mainMenu.SetLogic(new List<Screens> { Screens.ViewAInteraction, Screens.ViewResidents, Screens.AddAInteraction, Screens.DeleteAInteraction, Screens.PrintReport, Screens.AddResidents, Screens.EndProg });

            while (endProg == false)
            {
                Console.Clear();
                userChoice = (Screens)mainMenu.RunScreen();

                switch (userChoice)
                {
                    case Screens.ViewAInteraction:

                        Console.Clear();
                        Draw.Border("View A Interaction", 4, ConsoleColor.DarkGreen);
                        numR = GetResident(ref currResident);

                        const int descLeftStart = 23;
                        if (numR == numResidents.one)
                        {
                            Console.Clear();
                            Draw.Border("View A Interaction for " + currResident.firstName, 5, ConsoleColor.DarkGreen);
                            Console.SetCursorPosition(2, 4);
                            Console.Write("Date:");
                            Console.SetCursorPosition(descLeftStart, 4);
                            Console.Write("Description:");

                            // Starts to print all description
                            currHeight = 7;
                            foreach (Resident.Interaction interaction in currResident.interactions)
                            {
                                Console.SetCursorPosition(2, currHeight);
                                Console.Write(interaction.date.ToShortDateString());
                                Console.SetCursorPosition(descLeftStart, currHeight);
                                Console.Write(interaction.description);
                                currHeight++;
                            }

                            Console.SetCursorPosition(3, WindowHeight - 2);
                            Console.Write("Press any key to continue");
                            Console.ReadKey();
                        }
                        else if (numR == numResidents.all)
                        {
                            Console.Clear();
                            Draw.Border("View A Interaction for All", 5, ConsoleColor.DarkGreen);
                            Console.SetCursorPosition(2, 4);
                            Console.Write("Date:");
                            Console.SetCursorPosition(descLeftStart, 4);
                            Console.Write("Description:");

                            // Finds the all resident
                            foreach (Resident resident in residents)
                            {
                                if (resident.firstName.Equals("All"))
                                {
                                    currHeight = 7;
                                    foreach (Resident.Interaction interaction in resident.interactions)
                                    {
                                        Console.SetCursorPosition(2, currHeight);
                                        Console.Write(interaction.date.ToShortDateString());
                                        Console.SetCursorPosition(descLeftStart, currHeight);
                                        Console.Write(interaction.description);
                                        currHeight++;
                                    }
                                }
                            }

                            Console.SetCursorPosition(3, WindowHeight - 2);
                            Console.Write("Press any key to continue");
                            Console.ReadKey();
                        }

                        break;
                    case Screens.ViewResidents:

                        // Prints it all to the screen
                        Console.Clear();
                        Draw.Border("View Residents Screen", 4, ConsoleColor.Blue);
                        currHeight = 6;
                        currCol = 0;
                        foreach (Resident resident in residents)
                        {
                            // Prints the name
                            Console.Write(resident.firstName + " " + resident.lastName);

                            // Moves the cursor
                            if (currHeight == WindowHeight - 4)
                            {
                                // Moves to the next col
                                currHeight = 5;
                                currCol++;
                            }

                            currHeight++;
                            Console.SetCursorPosition(currCol * 20 + 2, currHeight);
                        }

                        Console.SetCursorPosition(3, WindowHeight - 2);
                        Console.Write("Press any key to continue");
                        Console.ReadKey();

                        break;
                    case Screens.AddAInteraction:

                        Console.Clear();
                        Draw.Border("Add A Interaction", 4, ConsoleColor.Blue);
                        numR = GetResident(ref currResident);

                        if (numR == numResidents.one || numR == numResidents.all)
                        {
                            Console.Clear();
                            Resident.Interaction newInteraction;
                            if (numR == numResidents.one)
                                Draw.Border("Add A Interaction for " + currResident.firstName, 4, ConsoleColor.Blue);
                            else
                                Draw.Border("Add A Interaction for all", 4, ConsoleColor.Blue);

                            // Gets and scans for a date
                            Console.Write("Enter in date (mm-dd): ");
                            string userInput = Console.ReadLine();

                            // Stores it in the date
                            userInput += "-2020"; // puts in current year
                            DateTime newDate;
                            bool success = DateTime.TryParseExact(userInput, "MM-dd-yyyy", 
                                CultureInfo.CurrentCulture, DateTimeStyles.None, out newDate);

                            // Checks for error
                            while (success == false)
                            {
                                Console.SetCursorPosition(2, 6);
                                Console.Write("Incorrect format, try again (mm-dd): ");
                                Console.Write(new string(' ', 50));
                                Console.SetCursorPosition(39, 6);

                                userInput = Console.ReadLine();

                                // Stores it in the date
                                userInput += "-2020"; // puts in current year
                                success = DateTime.TryParseExact(userInput, "MM-dd-yyyy",
                                    CultureInfo.CurrentCulture, DateTimeStyles.None, out newDate);
                            }

                            // Stores the data
                            newInteraction.date = newDate;

                            // Gets and scans for the description
                            Console.SetCursorPosition(2, 8);
                            Console.Write("Write description (enter a blank line for end):");
                            Console.SetCursorPosition(2, 9);
                            string currLine = Console.ReadLine();

                            // Checks for blank line entered
                            while (currLine.Equals(""))
                            {
                                Console.SetCursorPosition(2, 8);
                                Console.Write("Description must not be empty (enter a blank line for end):");
                                Console.SetCursorPosition(2, 9);
                                currLine = Console.ReadLine();
                            }

                            currHeight = 10;
                            userInput = currLine;
                            while (currLine != "")
                            {
                                Console.SetCursorPosition(2, currHeight);
                                currHeight++;

                                // Gets the next line
                                currLine = Console.ReadLine();
                                userInput += " " + currLine;
                            }

                            // Stores the data, adds it
                            newInteraction.description = userInput;

                            if (numR == numResidents.one)
                            {
                                currResident.interactions.Add(newInteraction);

                                // Saves
                                FileSystem.Save(currResident);
                            }
                            else
                            {
                                // Adds to the count
                                FileSystem.AddCount();

                                // Adds the interaction for each resident
                                foreach (Resident resident in residents)
                                {
                                    if (resident.firstName.Equals("Jamboard") == false)
                                    {
                                        resident.interactions.Add(newInteraction);
                                        FileSystem.Save(resident);
                                    }
                                }
                            }
                        }

                        break;

                    case Screens.DeleteAInteraction:
                        Console.WriteLine("FIXME - DELETEAINTERACTION");
                        break;

                    case Screens.PrintReport:
                        FileSystem.PrintReport(residents);
                        break;

                    case Screens.AddResidents:

                        // Creates screen
                        Console.Clear();
                        Draw.Border("View Residents Screen", 4, ConsoleColor.Cyan);

                        try
                        {

                            // Gets excel path
                            Console.Write("Type in path to excel file: ");
                            string path = Console.ReadLine();

                            // Gets starting and ending rows
                            Console.SetCursorPosition(2, 8);
                            Console.Write("Enter starting row: ");
                            int startRow = Convert.ToInt16(Console.ReadLine());
                            Console.SetCursorPosition(2, 10);
                            Console.Write("Enter ending row: ");
                            int endRow = Convert.ToInt16(Console.ReadLine());

                            // Gets first name col and last name col
                            Console.SetCursorPosition(2, 12);
                            Console.Write("Enter first name col: ");
                            char firstChar = Convert.ToChar(Console.ReadLine());
                            Console.SetCursorPosition(2, 14);
                            Console.Write("Enter last name col: ");
                            char lastChar = Convert.ToChar(Console.ReadLine());

                            // Creates all of the residents
                            residents = FileSystem.AddResidents(path, startRow, endRow, firstChar, lastChar);

                        }
                        catch(Exception exp)
                        {
                            // Prints error msg
                            Console.SetCursorPosition(2, WindowHeight - 2);
                            Console.Write("Something went wrong, try again");

                            // For debugging
                            Console.Write(exp.ToString());
                        }

                        break;

                    case Screens.EndProg:
                        return; // exits
                }
            }
        }

        static internal numResidents GetResident(ref Resident resident)
        {
            string userInput;

            // Saves current position and asks and scan
            int left = Console.CursorLeft;
            int top = Console.CursorTop;
            Console.SetCursorPosition(left, top + 1);
            Console.Write("Type all for all or exit to exit");
            Console.SetCursorPosition(left, top);
            Console.Write("Enter the resident's name: ");
            userInput = Console.ReadLine();

            // Test if they put exit
            if (userInput.ToLower().Equals("exit"))
                return numResidents.none;

            // Test if they put all
            if (userInput.ToLower().Equals("all"))
                return numResidents.all;

            // Test if it is a valid resident
            bool found = false;
            foreach( Resident r in residents)
            {
                if (r.firstName.ToLower().Equals(userInput.ToLower()))
                {
                    // Marks as found and exits
                    found = true;
                    resident = r;
                    break;
                }
            }

            while (found == false)
            {
                Console.SetCursorPosition(left, top);
                Console.Write("Resident not found, try again: ");
                Console.Write(new string(' ', 50));
                Console.SetCursorPosition(left + 31, top);
                userInput = Console.ReadLine();

                // Test if they put exit
                if (userInput.ToLower().Equals("exit"))
                    return numResidents.none;

                // Test if they put all
                if (userInput.ToLower().Equals("all"))
                    return numResidents.all;

                // Test if it is a valid resident
                found = false;
                foreach (Resident r in residents)
                {
                    if (r.firstName.ToLower().Equals(userInput.ToLower()))
                    {
                        // Marks as found and exits
                        found = true;
                        resident = r;
                        break;
                    }
                }
            }

            return numResidents.one;
        }
    }
}
