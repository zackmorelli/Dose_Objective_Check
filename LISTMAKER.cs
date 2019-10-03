using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ROI;
using System.Windows.Forms;

namespace Auto_Report_Script
{
    public class LISTMAKER
    {


        public static List<ROI.ROI> Listmaker(string Ttype, string Tsite, string [] Si)
        {

            List<ROI.ROI> ROIE = new List<ROI.ROI>();                 // OUTPUT - list of actual ROIs for the treatment site given by user
            List<ROI.ROI> ROIL = new List<ROI.ROI>();                 // initial list of all ROI.ROI objects pulled from txt file

            int linecount = 0;                     // line counter
            string line;                           // stores each line as it is read in
            string temp = null;
            string[] words;                         // array of strings used to store all the strings made when each line is divided into individual words using Split
            int j = 0;
            int k = 0;

            string tname = null;                    // temporary variables used to build each ROI.ROI
            int tid = 0;
            string tlimit = null;
            string tlimval = null;                    // needs to be a string because of a few weird cases
            string tstrict = null;
            string tlimunit = null;
            string tstatus = null;
            string tgoal = null;
            string tstruct = null;


            if (Ttype == "Conventional" | Ttype == "Both")
            {
                string path = @"\\Wvvrnimbp01ss\va_data$\filedata\ProgramData\Vision\PublishedScripts\ConventionalROIList.txt";
               // Console.WriteLine("TriggerConv1");

                if(File.Exists(path))
                {
                   // Console.WriteLine("TriggerConv2");
                    using (StreamReader Lread = File.OpenText(path))   //opens file and puts it into a stream
                    {
                       // Console.WriteLine("TriggerConv3");

                        while((line = Lread.ReadLine()) != "END")    //each loop pulls a line from the file until no lines left
                        {

                            // tempoarary variables cleared at beggining of each new line loop
                            tname = null;
                            tid = 0;
                            tlimit = null;
                            tlimval = null;
                            tstrict = null;
                            tlimunit = null;
                            tstatus = null;
                            tgoal = null;
                            tstruct = null;
                            string[] ttreatsite = new string[20] { "element", "element", "element", "element", "element", "element", "element", "element", "element", "element", "element", "element", "element", "element", "element", "element", "element", "element", "element", "element" };
                            words = null;


                            words = line.Split(',');        // convenientley splits the line into multiple strings, stored in a string array, using space as a delimiter
                                                            // Console.WriteLine("words size: {0}", words.Length);
                            tid = linecount;
                            tname = words[0];
                            tstruct = words[1];
                            tlimit = words[2];
                            tstrict = words[3];

                            if (tstrict == "[record]")
                            {
                                tlimval = "0.0";        // this is done just to prevent type conversion errors later on

                                tlimunit = words[4];
                                tgoal = words[5];

                                for (int i = 0; (7 + i) <= (words.Length); i++)
                                {
                                    temp = words[6 + i];

                                    ttreatsite[i] = temp;

                                }


                            }
                            else
                            {
                                tlimval = words[4];
                                tlimunit = words[5];
                                tgoal = words[6];

                                for (int i = 0; (8 + i) <= (words.Length); i++)
                                {
                                    temp = words[7 + i];

                                    ttreatsite[i] = temp;

                                }

                            }

                            ROIL.Add(new ROI.ROI { ROIName = tname, Rstruct = tstruct, ROIId = tid, limit = tlimit, limval = tlimval, strict = tstrict, limunit = tlimunit, status = tstatus, goal = tgoal, treatsite = ttreatsite});

                          //  Console.WriteLine("Linecount is: {0}", linecount);

                           // Thread.Sleep(3000);
                           //  Console.WriteLine("TriggeraftROIL");

                            linecount++;
                            
                        } // ends loop that pulls lines


                       // Console.WriteLine("TriggerConvend"); 

                        
                    }   // ends open file

                }  // ends if file exists
                else
                {
                    MessageBox.Show("\n\n File not found!");

                }


            }
            else if (Ttype == "SRS/SBRT" | Ttype == "Both" )
            {

                string path = @"\\Wvvrnimbp01ss\va_data$\filedata\ProgramData\Vision\PublishedScripts\SRSROIList.txt";

                if (File.Exists(path))
                {
                    using (StreamReader Lread = File.OpenText(path))   //opens file and puts it into a stream
                    {

                        while ((line = Lread.ReadLine()) != "END")    //each loop pulls a line from the file until no lines left
                        {

                             // Console.WriteLine("TriggerConv4");
                            // tempoarary variables cleared at beggining of each new line loop
                            tname = null;
                            tid = 0;
                            tlimit = null;
                            tlimval = null;
                            tstrict = null;
                            tlimunit = null;
                            tstatus = null;
                            tgoal = null;
                            string[] ttreatsite = new string[20] { "element", "element", "element", "element", "element", "element", "element", "element", "element", "element", "element", "element", "element", "element", "element", "element", "element", "element", "element", "element" };
                            words = null;
                            tstruct = null;

                            words = line.Split(',');            // convenientley splits the line into multiple strings, stored in a string array, using comma as a delimiter
                                                                // Console.WriteLine("words size: {0}", words.Length);
                            tid = linecount;
                            tname = words[0];
                            tstruct = words[1];
                            tlimit = words[2];
                            tstrict = words[3];

                            if (tstrict == "[record]")
                            {
                                tlimval = "0.0";             // this is done to prevent type conversion errors later on

                                tlimunit = words[4];
                                tgoal = words[5];

                                for (int i = 0; (7 + i) <= (words.Length); i++)
                                {
                                    temp = words[6 + i];

                                    ttreatsite[i] = temp;
                                }
                            }
                            else
                            {
                                tlimval = words[4];
                                tlimunit = words[5];
                                tgoal = words[6];

                                for (int i = 0; (8 + i) <= (words.Length); i++)
                                {
                                    temp = words[7 + i];

                                    ttreatsite[i] = temp;
                                }
                            }

                            ROIL.Add(new ROI.ROI { ROIName = tname, Rstruct = tstruct, ROIId = tid, limit = tlimit, limval = tlimval, strict = tstrict, limunit = tlimunit, status = tstatus, goal = tgoal, treatsite = ttreatsite });


                            //  Console.WriteLine("Linecount is: {0}", linecount);
                            // Thread.Sleep(3000);
                            //  Console.WriteLine("TriggeraftROIL");
                            linecount++;

                        } // ends loop that pulls lines
                       // Console.WriteLine("TriggerSRS");
                    }   // ends open file
                }  // ends if file exists
                else
                {
                    MessageBox.Show("\n\n File not found!");
                }
            }  // ends if Ttype loop
            foreach (ROI.ROI roi in ROIL )       // iterate through all the ROI.ROI elements in the ROIL list
            {
              // Console.WriteLine("ROIL size is: {0}", ROIL.Count);
              //  Console.WriteLine("ROIL index: {0}", j);
              //  Console.WriteLine("roi name is: {0}", roi.ROIName);
                j++;

                foreach (string str in roi.treatsite)      // iterates through all the treatsite elements of a specific ROI.ROI element in the ROIL list 
                {
                  //  Console.WriteLine("This treatsite size is: {0}", roi.treatsite.Length);
                  //  Console.WriteLine("Treatsite index: {0}", k);
                   // Console.WriteLine("treatsite {0} is: {1}", k, str);
                    k++;

                    if (Tsite != null)
                    {
                        if (str == Tsite)
                        {
                            // Console.WriteLine("TrigBef____ROIEadd");
                            ROIE.Add(new ROI.ROI { ROIName = roi.ROIName, Rstruct = roi.Rstruct, ROIId = roi.ROIId, limit = roi.limit, limval = roi.limval, strict = roi.strict, limunit = roi.limunit, goal = roi.goal });
                            //  Console.WriteLine("TrigAft____ROIEadd");
                            //  Thread.Sleep(4000);
                        }
                    }
                    else if (Tsite == null)
                    {
                        foreach (string foo in Si)
                        {
                            if (str == foo)
                            {
                                ROIE.Add(new ROI.ROI { ROIName = roi.ROIName, Rstruct = roi.Rstruct, ROIId = roi.ROIId, limit = roi.limit, limval = roi.limval, strict = roi.strict, limunit = roi.limunit, goal = roi.goal });
                            }
                        }
                    }
                }
                k = 0;
            } 

         return ROIE;

        }  // ends main


    }


}
