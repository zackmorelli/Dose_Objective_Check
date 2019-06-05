using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Auto_Report_Script
{
    public class ROIcriteriacheck
    {
   








        public static List<ROI> Main(string Ttype, string Tsite)
        {
            List<ROI> roi = new List<ROI>();
            string i = null;
            int counter = 0;
            string line;

          
            if (Ttype == "Conventional")
            {
                System.IO.StreamReader file = new System.IO.StreamReader(@"B:\filedata\ProgramData\Vision\PublishedScripts\ConventionalROI.txt");

                while ((line = file.ReadLine()) != null)       //StreamReader.Readline should read an entire line from the text file
                {                                               //then we need to break up each line in the file into the tab delimited strings, these indiviudaul strings are the data we want
                    counter = 0;

                    System.Console.WriteLine(line);



                    counter++;
                }

                Console.WriteLine("Enter k to continue: ");
                i = Console.ReadLine();

            }
            else if (Ttype == "SRS/SBRT" )
            {
                System.IO.StreamReader file = new System.IO.StreamReader(@"B:\filedata\ProgramData\Vision\PublishedScripts\SRSROI.txt");

                while ((line = file.ReadLine()) != null)
                {
                    counter = 0;

                    System.Console.WriteLine(line);


                    counter++;
                }

                Console.WriteLine("Enter k to continue: ");
                i = Console.ReadLine();



            }









            // thing that reads through ROI criteria from ASCII file 

            // add ROIs for given treatment site to list













            // one by one, loop through list and pull that specific DVH data from Eclipse, compare


            //ouput list with pass/fail parameters

            return roi;


        }








    }


}
