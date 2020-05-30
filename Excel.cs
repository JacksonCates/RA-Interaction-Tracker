using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Microsoft.Office.Interop.Excel;
using _Excel = Microsoft.Office.Interop.Excel;

namespace RAInteractionTracker
{
    class Excel
    {
        string path = "";
        _Application excel = new _Excel.Application();
        Workbook wb;
        Worksheet ws;
        private object misValue = System.Reflection.Missing.Value;

        // Used if the excel file already exist
        public Excel(string path)
        {
            this.path = path;

            // Checks if the file exist, if so get the information. If not create file
            if (File.Exists(path))
            {
                // Opens the book and opens the first sheet
                wb = excel.Workbooks.Open(path);
                OpenSheet(1);
            }
            else
            {
                // Creates the new file                
                excel.Visible = false;
                wb = excel.Workbooks.Add(misValue);

                // Saves and closes
                wb.SaveAs(Path.GetFullPath(path));
            }
        }

        ~Excel()
        {
            Close();
        }

        // NOTE: SHEETS START AT 1
        public void OpenSheet(int Sheet)
        {
            ws = wb.Worksheets[Sheet];
        }

        public void OpenSheet(string name)
        {
            // Finds the index for the sheet
            int index = -1;

            // Find the sheet
            for (int i = 1; i <= wb.Worksheets.Count; i++)
            {
                if (wb.Worksheets[i].Name.Equals(name))
                {
                    index = i;
                }
            }

            // Sees if the sheet is found or not
            if (index == -1)
            {
                // Sends error msg
                Console.WriteLine("ERROR: NO SHEET WITH NAME " + name);
                Console.ReadKey();
            }
            else
            {
                ws = wb.Worksheets[index];
            }
        }

        public void AddSheet(string name)
        {
            // Appends the worksheet to the end
            ws = wb.Sheets.Add(After: wb.Sheets[wb.Sheets.Count]);

            RenameCurrentSheet(name);
        }

        public void RenameCurrentSheet(string name)
        {
            // Test if a worksheet is selected
            if (ws == null)
            {
                // Sends error msg
                Console.WriteLine("ERROR: NO SHEET SELECTED");
                Console.ReadKey();
            }
            else
            {
                // Renames the ws
                ws.Name = name;
            }
        }

        public void AlignCol(int col, string align)
        {
            align = align.ToLower();

            // Test if a worksheet is selected
            if (ws == null)
            {
                // Sends error msg
                Console.WriteLine("ERROR: NO SHEET SELECTED");
                Console.ReadKey();
            }
            // Test if a correct option was choosen
            else if (align.Equals("left") == false && align.Equals("right") == false && align.Equals("center") == false)
            {
                // Sends error msg
                Console.WriteLine("ERROR: WRONG ALIGNMENT");
                Console.ReadKey();
            }
            else
            {
                // Changes the alignment
                if (align.Equals("left"))
                    ws.Columns[col].HorizontalAlignment = _Excel.XlHAlign.xlHAlignLeft;
                else if (align.Equals("right"))
                    ws.Columns[col].HorizontalAlignment = _Excel.XlHAlign.xlHAlignRight;
                else if (align.Equals("center"))
                    ws.Columns[col].HorizontalAlignment = _Excel.XlHAlign.xlHAlignCenter;
            }
        }


        public void SetColWidth(int col, double width)
        {
            // Test if a worksheet is selected
            if (ws == null)
            {
                // Sends error msg
                Console.WriteLine("ERROR: NO SHEET SELECTED");
                Console.ReadKey();
            }
            // Test if a correct option was choosen
            else if (width < 0)
            {
                // Sends error msg
                Console.WriteLine("ERROR: WRONG ALIGNMENT");
                Console.ReadKey();
            }
            else
            {
                ws.Columns[col].ColumnWidth = width;
            }
        }

        public string ReadCell(int row, int col)
        {
            // Converts the row and col to its respective numbers
            int i = row;
            int j = col;

            // Checks to see if the space is empty
            if (ws.Cells[i, j].Value2 != null)
            {
                return ws.Cells[i, j].Value2.ToString();
            }
            else
            {
                return "";
            }
        }

        public string ReadCell(int row, char col)
        {
            // Converts the row and col to its respective numbers
            int i = row;
            int j = Convert.ToInt16(Char.ToUpper(col) - 'A') + 1;

            // Checks to see if the space is empty
            if (ws.Cells[i, j].Value2 != null)
            {
                return ws.Cells[i, j].Value2;
            }
            else
            {
                return "";
            }
        }

        public void WriteCell(int row, char col, string data)
        {
            // Converts the row and col to its respective numbers
            int i = row;
            int j = Convert.ToInt16(Char.ToUpper(col) - 'A') + 1;

            // Writes it to the cell
            ws.Cells[i, j].Value2 = data;
        }

        public void WriteCell(int row, int col, string data)
        {
            // Converts the row and col to its respective numbers
            int i = row;
            int j = col;

            // Writes it to the cell
            ws.Cells[i, j].Value2 = data;
        }

        public void WriteCellFormula(int row, int col, string formula)
        {
            int i = row;
            int j = col;

            ws.Cells[i, j].Value = formula;
        }

        public void WriteCol(int startRow, char col, string[] datas)
        {
            // Converts the row and col to its respective numbers
            int i = startRow;
            int j = Convert.ToInt16(Char.ToUpper(col) - 'A') + 1;

            foreach (string data in datas)
            {
                ws.Cells[i, j].Value2 = data;
                i++;
            }
        }

        public void WriteCol(int startRow, int col, string[] datas)
        {
            // Converts the row and col to its respective numbers
            int i = startRow;
            int j = col;

            foreach (string data in datas)
            {
                ws.Cells[i, j].Value2 = data;
                i++;
            }
        }

        public int ReplaceAtCol(int startRow, int col, string oldData, string newData)
        {
            // Finds the row
            int i;
            for (i = startRow; ReadCell(i, col).Equals(oldData) == false; i++) ;

            // Replaces
            WriteCell(i, col, newData);

            // Returns the row incase they need to know
            return i;
        }

        public void AppendAtCol(int startRow, int col, string data)
        {
            // Finds the row index
            int i;
            for (i = startRow; ReadCell(i, col).Equals("") == false; i++) ;
            WriteCell(i, col, data);
        }

        // Removes the row and shifts all of the info below upwards
        public void RemoveRowAndShift(int startRow, int startCol, int endCol, string oldDataID)
        {
            // Finds the index where to delete and where it ends
            int deleteRow = -1;
            int endRow;
            int i;
            for (i = startRow; ReadCell(i, startCol).Equals("") == false; i++)
            {
                string currCell = ReadCell(i, startCol);

                if (currCell.Equals(oldDataID))
                {
                    deleteRow = i;
                }
            }
            endRow = i - 1; // Where the row ends

            // Test, if unassign post error
            if (deleteRow == -1)
            {
                Console.WriteLine("ERROR: DeleteRow not found");
            }

            // Saves all of the data in a 2D matrix
            for (i = deleteRow + 1; i <= endRow; i++)
            {
                // Take the current row and move it down the the previous row
                for (int j = startCol; j <= endCol; j++)
                {
                    WriteCell(i - 1, j, ReadCell(i, j));
                }
            }

            // Deletes the last row
            for (i = startCol; i <= endCol; i++)
            {
                WriteCell(endRow, i, "");
            }
        }

        public void Close()
        {
            // Saves
            wb.Save();
            wb.Close();
            excel.Quit();
        }
    }
}
