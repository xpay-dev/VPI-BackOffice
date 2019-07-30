Motivation:
I always wanted to find a good way to read Excel files. Thanks to Iterative design pattern and 
to dynamics now I can do that within just 3 line of code 

Sample Application on how to use the ExcelReader.dll

Create a Cosole Application and copy and past following 

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExcelReader;

namespace TestExcelReader
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Console.WriteLine("Start");
                ProcessFile();
                Console.WriteLine(Environment.NewLine);
                Console.WriteLine("Press any key to exit");
            }
            catch(Exception excp)
            {
                Console.BackgroundColor = ConsoleColor.Red;
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine(excp.ToString());
            }
            Console.ReadKey();
        }

        private static void ProcessFile()
        {
            string fileName = Environment.CurrentDirectory.Replace("\\bin\\Release", "\\ExcelFile\\SampleExcelFileFirstRowHeader.xlsx");

            //https://www.nuget.org/profiles/AnthonyDesa

            using (dynamic rFile = new ExcelFileReader<dynamic>(fileName))
            {
                Console.WriteLine("SheetCount={0}:", rFile.SheetCount());
                Console.WriteLine("HeaderNames={0}", string.Join(",", ((IList<string>)rFile.HeaderNames()).ToArray()));
                Console.WriteLine("SheetNames={0}", string.Join(",", ((IList<string>)rFile.SheetNames()).ToArray()));
                Console.WriteLine(Environment.NewLine);

                Console.WriteLine("ColumnCount={0}:", rFile.ColumnCount());
                Console.WriteLine("RowCount={0}", rFile.RowCount());
                foreach (var line in rFile)
                    Console.WriteLine("Processing ID={0}, URL={1}, Display Intercept={2}", line.ID, line.URL, line.Get("Display Intercept"));

                rFile.Sheet(2);
                Console.WriteLine(Environment.NewLine);
                Console.WriteLine("ColumnCount={0}:", rFile.ColumnCount());
                Console.WriteLine("RowCount={0}", rFile.RowCount());
                foreach (var line in rFile)
                    Console.WriteLine("Processing ID={0}, URL={1}, Display Intercept={2}", line.ID, line.URL, line.Get("Display Intercept"));
            }
        }
    }
}


Line One - Create an instance of ReadExcelFileEnumerable variable and pass file name as a parameter
Line Two - Loop through the dynamic variable like any other collection which implements IEnumerable, IEnumerator
Line Three - Read the data and write to a console.

Create a new Folder in your console application let's name the new folder as "ExcelFile" 
Create a new excel empty file in "ExcelFile" folder and name it as SampleExcelFileFirstRowHeader.xlsx

Copy following content in the excel file Sheet1 
ID	URL							Display Intercept
1	www.Sheet1.hotmail.com		TRUE
2	www.Sheet1.yahoo.com		FALSE
3	www.Sheet1.google.com		FALSE
4	www.Sheet1.microsoft.com	FALSE
5	www.Sheet1.amazon.com		TRUE

Copy following content in the excel file Sheet2 
ID	URL							Display Intercept
1	www.Sheet2.hotmail.com		TRUE
2	www.Sheet2.yahoo.com		FALSE
3	www.Sheet2.google.com		FALSE
4	www.Sheet2.microsoft.com	FALSE
5	www.Sheet2.amazon.com		TRUE
6	www.Sheet2.amazon2.com		TRUE

Save and close the excel file

Run the console application (In Release Mode) and you will see the output displayed in the Console.

Please Note:
property (ID, URL,Display Intercept) of the dynamic variable should match to the header name in excel file i.e. line.ID, line.URL etc
First Row in excel file is considered as header. 

Code for ExcelReader can be viewed at http://anthonydesa.blogspot.ca/2015/05/reading-excel-file-in-c-using-just-3.html



