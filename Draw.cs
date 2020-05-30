using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RAInteractionTracker
{
    class Draw
    {
        private static int savedCursorLeft;
        private static int savedCursorTop;
        private static ConsoleColor savedColor;
        private static char blank = ' ';

        private static char ulCorner = '╔';
        private static char llCorner = '╚';
        private static char urCorner = '╗';
        private static char lrCorner = '╝';
        private static char vertical = '║';
        private static char horizontal = '═';

        public static void Rectangle(
            int xLoc, int yLoc,
            int width, int height,
            ConsoleColor color)
        {
            SaveInfo(color);

            // Draws the rectangle
            // Draws the corners
            Write(xLoc, yLoc, ulCorner);
            Write(xLoc, yLoc + height, llCorner);
            Write(xLoc + width, yLoc, urCorner);
            Write(xLoc + width, yLoc + height, lrCorner);

            // Draws the sides
            // Horizontal
            for (int i = xLoc + 1; i < xLoc + width; ++i)
            {
                Write(i, yLoc, horizontal);
                Write(i, yLoc + height, horizontal);
            }
            // Vertical
            for (int i = yLoc + 1; i < yLoc + height; ++i)
            {
                Write(xLoc, i, vertical);
                Write(xLoc + width, i, vertical);
            }

            RestoreInfo();
        }

        public static void HorLine(
            int xLoc, int yLoc, int Length,
            ConsoleColor color)
        {
            SaveInfo(color);

            for (int i = xLoc; i <= xLoc + Length; ++i)
            {
                Write(i, yLoc, horizontal);
            }

            RestoreInfo();
        }


        public static void VerLine(
            int xLoc, int yLoc, int Length,
            ConsoleColor color)
        {
            SaveInfo(color);

            for (int i = yLoc; i <= yLoc + Length; ++i)
            {
                Write(xLoc, i, vertical);
            }

            RestoreInfo();
        }

        // For internal use
        private static void HorLine(
            int xLoc, int yLoc, int Length)
        {
            for (int i = xLoc; i <= xLoc + Length; ++i)
            {
                Write(i, yLoc, horizontal);
            }
        }
        private static void VerLine(int xLoc, int yLoc, int Length)
        {
            for (int i = yLoc; i <= yLoc + Length; ++i)
            {
                Write(xLoc, i, vertical);
            }
        }
        private static void VerSolidLine(int xLoc, int yLoc, int Length)
        {
            for (int i = yLoc; i <= yLoc + Length; ++i)
            {
                Write(xLoc, i, blank);
            }
        }
        private static void Rectangle(
            int xLoc, int yLoc,
            int width, int height)
        {
            // Draws the rectangle
            // Draws the corners
            Write(xLoc, yLoc, ulCorner);
            Write(xLoc, yLoc + height, llCorner);
            Write(xLoc + width, yLoc, urCorner);
            Write(xLoc + width, yLoc + height, lrCorner);

            // Draws the sides
            // Horizontal
            for (int i = xLoc + 1; i < xLoc + width; ++i)
            {
                Write(i, yLoc, horizontal);
                Write(i, yLoc + height, horizontal);
            }
            // Vertical
            for (int i = yLoc + 1; i < yLoc + height; ++i)
            {
                Write(xLoc, i, vertical);
                Write(xLoc + width, i, vertical);
            }
        }
        private static void Graph(int xLoc, int yLoc, int height, int width, ConsoleColor color)
        {
            SaveInfo(color);

            Write(xLoc, yLoc, llCorner);
            VerLine(xLoc, yLoc - 1 - height, height);
            HorLine(xLoc + 1, yLoc, width);

            RestoreInfo();
        }


        public static void GraphWithData(int xLoc, int yLoc, double[] data,
            int height, double max, double min, List<Tuple<double, string>> labelData,
            ConsoleColor axisColor, ConsoleColor barColor)
        {
            // first index of labelData is a precentage, the nexxt is the string to write

            Graph(xLoc, yLoc, height, data.Length, axisColor);

            // Preps the colors
            SaveInfo(ConsoleColor.White);
            ConsoleColor savedBackgroundColor = Console.BackgroundColor;
            Console.BackgroundColor = barColor;

            // Draws each bar
            for (int i = xLoc + 1; i <= xLoc + data.Length; i++)
            {
                // Finds the height of the bar
                int barHeight = Convert.ToInt16(height * ((data[i - xLoc - 1] - min) / (max - min)));

                // Draws the bar
                VerSolidLine(i, yLoc - 1 - barHeight, barHeight);

            }

            Console.BackgroundColor = savedBackgroundColor;
            RestoreInfo();

            // Labels the max a min
            Console.SetCursorPosition(xLoc - 7, yLoc - height - 1);
            Console.Write("{0:0.00}%", max);
            Console.SetCursorPosition(xLoc - 7, yLoc - 1);
            Console.Write("{0:0.00}%", min);

            // Prints the label data
            for (int i = 0; i < labelData.Count(); ++i)
            {
                // Finds the height
                int labelHeight = Convert.ToInt16(height * ((labelData[i].Item1 - min) / (max - min)));

                // Writes the info
                Console.SetCursorPosition(xLoc - 2, yLoc - 1 - labelHeight);
                Console.Write(labelData[i].Item2);
            }
        }

        public static void Border(string title, int titleHeight, ConsoleColor BorderColor)
        {
            // Prints title
            Console.SetCursorPosition(2, 2);
            Console.Write(title);

            SaveInfo(BorderColor);

            Rectangle(0, 0, Program.WindowWidth, Program.WindowHeight);

            // Draws line seperator
            Write(0, titleHeight, llCorner);
            Write(Program.WindowWidth, titleHeight, lrCorner);
            HorLine(1, titleHeight, Program.WindowWidth - 2);

            RestoreInfo();
            Console.SetCursorPosition(2, titleHeight + 2);
        }


        public static void Table(int xLoc, int yLoc,
            int height, string[] headers, int[] extraSpace, ConsoleColor color)
        {
            SaveInfo(color);

            // Draws left boarder
            VerLine(xLoc, yLoc, height);

            // Draws each col
            int totalSpaces = xLoc;
            for (int i = 0; i < headers.Length; ++i)
            {
                int spaceBetweenCols = 4 + headers[i].Length + extraSpace[i] * 2;

                // Prints label
                Console.SetCursorPosition(totalSpaces + 3 + extraSpace[i], yLoc);
                Console.Write(headers[i]);

                // Updates totalSpaces
                totalSpaces += spaceBetweenCols + 1;

                // Draws the left border
                VerLine(totalSpaces, yLoc, height);
            }

            RestoreInfo();
        }


        public static void TableWithData(int xLoc, int yLoc,
            int height, string[] headers, ConsoleColor color,
            string[,] data, bool ContainsTotal = false, bool ExtraSpace = true,
            Action<string, int, string> colorFormat = null)
        {
            // Boolean contains total is if we need to put the total at the bottom

            // Figures out the space needed
            int[] spaceNeeded = new int[50];
            for (int i = 0; i < headers.Length; ++i)
            {
                // Finds the max size of the col
                int maxLength = data[0, i].Length;
                for (int j = 0; j < data.GetLength(0); ++j)
                {
                    if (maxLength < data[j, i].Length)
                    {
                        maxLength = data[j, i].Length;
                    }
                }

                // Stores that information into the array
                // If the title is large enough, dont need space!
                if (headers[i].Length + 2 > maxLength)
                {
                    spaceNeeded[i] = 0;
                }
                else
                {
                    spaceNeeded[i] = maxLength - headers[i].Length - 2;
                }
            }

            // Draws the table
            Table(xLoc, yLoc, height, headers, spaceNeeded, color);

            // Fills the table with data
            int xPrint = xLoc; // x location to print
            int yPrint = yLoc + 2; // y location to print
            for (int i = 0; i < data.GetLength(1); ++i)
            {
                // Finds the sapce in the col
                int space = headers[i].Length + 5 + spaceNeeded[i] * 2;

                int origX = xPrint;

                // Prints all the data in the col
                for (int j = 0; j < data.GetLength(0); ++j)
                {
                    // Finds where to print in the xLoc
                    xPrint += (space + 1) / 2;
                    xPrint -= data[j, i].Length / 2;

                    // Finds cursor position, sets it to the bottom if it is the total info
                    if (ContainsTotal && j + 1 == data.GetLength(0))
                        Console.SetCursorPosition(xPrint, yLoc + height);
                    else
                        Console.SetCursorPosition(xPrint, yPrint);

                    if (colorFormat == null)
                    {
                        Console.Write(data[j, i]);
                    }
                    else
                    {
                        colorFormat(data[j, i], i, data[j, 0]);
                        Console.Write(data[j, i]);
                        RestoreInfo();
                    }

                    // Resets xPrint
                    xPrint = origX;

                    if (ExtraSpace)
                        yPrint += 2;
                    else
                        yPrint++;
                }

                // Goes to the nex col
                xPrint = origX + space;
                yPrint = yLoc + 2;
            }
        }

        private static void Write(int xLoc, int yLoc, char ch)
        {
            Console.SetCursorPosition(xLoc, yLoc);
            Console.Write(ch);
        }

        private static void SaveInfo(ConsoleColor color)
        {
            savedCursorLeft = Console.CursorLeft;
            savedCursorTop = Console.CursorTop;

            // Saves the cursor color
            savedColor = Console.ForegroundColor;
            Console.ForegroundColor = color;
        }

        private static void RestoreInfo()
        {
            // Restores the cursor and background values
            Console.CursorTop = savedCursorTop;
            Console.CursorLeft = savedCursorLeft;
            Console.ForegroundColor = savedColor;

        }
    }
}
