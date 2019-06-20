using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Auto_Report_Script
{
    public class LISTMAKER
    {


        public static List<ROI> Listmaker(string Ttype, string Tsite)
        {

            List<ROI> ROIE = new List<ROI>();                 // OUTPUT - list of actual ROIs for the treatment site given by user
            List<ROI> ROIL = new List<ROI>();                 // initial list of all ROI objects pulled from txt file

            int linecount = 0;                     // line counter
            string line;                           // stores each line as it is read in
            string temp = null;
            string[] words;                         // array of strings used to store all the strings made when each line is divided into individual words using Split
            int j = 0;
            int k = 0;

            string tname = null;                    // temporary variables used to build each ROI
            int tid = 0;
            string tlimit = null;
            int tlimval = 0;
            string tstrict = null;
            string tlimunit = null;
            string tstatus = null;
            string tgoal = null;
            string tstruct = null;
          //  string[] ttreatsite = new string[20] {"element", "element", "element", "element", "element", "element", "element", "element", "element", "element", "element", "element", "element", "element", "element", "element", "element", "element", "element", "element"};






            if (Ttype == "Conventional")
            {
                string path = @"\\Wvvrntboxt01g1\va_data$\filedata\ProgramData\Vision\PublishedScripts\testf.txt";
              //  Console.WriteLine("TriggerConv1");

                if(File.Exists(path))
                {
                  //  Console.WriteLine("TriggerConv2");
                    using (StreamReader Lread = File.OpenText(path))   //opens file and puts it into a stream
                    {
                      //  Console.WriteLine("TriggerConv3");

                        while((line = Lread.ReadLine()) != "END")    //each loop pulls a line from the file until no lines left
                        {

                          //  Console.WriteLine("TriggerConv4");
                            // tempoarary variables cleared at beggining of each new line loop
                            tname = null;
                            tid = 0;
                            tlimit = null;
                            tlimval = 0;
                            tstrict = null;
                            tlimunit = null;
                            tstatus = null;
                            tgoal = null;
                            tstruct = null;
                            string[] ttreatsite = new string[20] { "element", "element", "element", "element", "element", "element", "element", "element", "element", "element", "element", "element", "element", "element", "element", "element", "element", "element", "element", "element" };
                            words = null;


                            words = line.Split(' ');   // convenientley splits the line into multiple strings, stored in a string array, using space as a delimiter
                           // Console.WriteLine("words size: {0}", words.Length);
                            tid = linecount;
                            tname = words[0];
                            tstruct = words[1];
                            tlimit = words[2];
                            tstrict = words[3];

                            if(tstrict == "[record]")
                            {
                                tlimval = 100000;  // set "record" limits which don't have a value to a high number so they always pass/don't cause issues
                            }
                            else
                            {
                                tlimval = Convert.ToInt32(words[4]);
                            }
                            tlimunit = words[5];
                            tgoal = words[6];

                                // words.size - 6 = number of treat site elements to add to treatsite list

                                // so 6 + treatsite( 1,2,3 ...) are elements representing treatsites
                            for(int i = 0; (8 + i) <= (words.Length); i++)
                            {
                              //  Console.WriteLine("****TRIGGER****");
                               // Thread.Sleep(4000);

                                temp = words[7 + i];
                              //  Console.WriteLine("TriggerConv5");
                              //  Console.WriteLine("TEMP IS: {0}", temp);

                                ttreatsite[i] = temp;

                              //  Console.WriteLine("TRIGGERCONV6");
                            }

                          //  Console.WriteLine("TriggerbefROIL");
                          //  Console.WriteLine("treatsite {0} list size: {1}", linecount, ttreatsite.Length);
                           //  Console.WriteLine(roi.ToString());
                           // Thread.Sleep(4000);

                            ROIL.Add(new ROI { ROIName = tname, Rstruct = tstruct, ROIId = tid, limit = tlimit, limval = tlimval, strict = tstrict, limunit = tlimunit, status = tstatus, goal = tgoal, treatsite = ttreatsite});

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
                    Console.WriteLine("\n\n File not found!");

                }


            }
            else if (Ttype == "SRS/SBRT" )
            {

                string path = @"\\Wvvrntboxt01g1\va_data$\filedata\ProgramData\Vision\PublishedScripts\testf.txt";

                if (File.Exists(path))
                {
                    using (StreamReader Lread = File.OpenText(path))   //opens file and puts it into a stream
                    {

                        while ((line = Lread.ReadLine()) != "END")    //each loop pulls a line from the file until no lines left
                        {

                            //  Console.WriteLine("TriggerConv4");
                            // tempoarary variables cleared at beggining of each new line loop
                            tname = null;
                            tid = 0;
                            tlimit = null;
                            tlimval = 0;
                            tstrict = null;
                            tlimunit = null;
                            tstatus = null;
                            tgoal = null;
                            string[] ttreatsite = new string[20] { "element", "element", "element", "element", "element", "element", "element", "element", "element", "element", "element", "element", "element", "element", "element", "element", "element", "element", "element", "element" };
                            words = null;
                            tstruct = null;

                            words = line.Split(' ');   // convenientley splits the line into multiple strings, stored in a string array, using space as a delimiter
                                                       // Console.WriteLine("words size: {0}", words.Length);
                            tid = linecount;
                            tname = words[0];
                            tstruct = words[1];
                            tlimit = words[2];
                            tstrict = words[3];

                            if (tstrict == "[record]")
                            {
                                tlimval = 100000;  // set "record" limits which don't have a value to a high number so they always pass/don't cause issues
                            }
                            else
                            {
                                tlimval = Convert.ToInt32(words[4]);
                            }
                            tlimunit = words[5];
                            tgoal = words[6];

                            // words.size - 6 = number of treat site elements to add to treatsite list

                            // so 6 + treatsite( 1,2,3 ...) are elements representing treatsites
                            for (int i = 0; (8 + i) <= (words.Length); i++)
                            {
                                //  Console.WriteLine("****TRIGGER****");
                                // Thread.Sleep(4000);

                                temp = words[7 + i];
                                //  Console.WriteLine("TriggerConv5");
                                //  Console.WriteLine("TEMP IS: {0}", temp);

                                ttreatsite[i] = temp;

                                //  Console.WriteLine("TRIGGERCONV6");
                            }

                            //  Console.WriteLine("TriggerbefROIL");
                            //  Console.WriteLine("treatsite {0} list size: {1}", linecount, ttreatsite.Length);
                            //  Console.WriteLine(roi.ToString());
                            // Thread.Sleep(4000);

                            ROIL.Add(new ROI { ROIName = tname, Rstruct = tstruct, ROIId = tid, limit = tlimit, limval = tlimval, strict = tstrict, limunit = tlimunit, status = tstatus, goal = tgoal, treatsite = ttreatsite });


                            //  Console.WriteLine("Linecount is: {0}", linecount);


                            // Thread.Sleep(3000);
                            //  Console.WriteLine("TriggeraftROIL");


                            linecount++;

                        } // ends loop that pulls lines



                      //  Console.WriteLine("TriggerSRS");


                    }   // ends open file


                }  // ends if file exists
                else
                {
                    Console.WriteLine("\n\n File not found!");

                }

            }  // ends if Ttype loop
            
 
 

            foreach (ROI roi in ROIL )       // iterate through all the ROI elements in the ROIL list
            {
              //  Console.WriteLine("ROIL size is: {0}", ROIL.Count);
              //  Console.WriteLine("ROIL index: {0}", j);
              //  Console.WriteLine("roi name is: {0}", roi.ROIName);
                j++;

                foreach (string str in roi.treatsite)      // iterates through all the treatsite elements of a specific ROI element in the ROIL list 
                {
                  //  Console.WriteLine("This treatsite size is: {0}", roi.treatsite.Length);
                  //  Console.WriteLine("Treatsite index: {0}", k);

                   // Console.WriteLine("treatsite {0} is: {1}", k, str);
                    k++;


                    if (str == Tsite)
                    {
                       // Console.WriteLine("TrigBef____ROIEadd");

                        
                        ROIE.Add(new ROI { ROIName = roi.ROIName, Rstruct = roi.Rstruct, ROIId = roi.ROIId, limit = roi.limit, limval = roi.limval, strict = roi.strict, limunit = roi.limunit, status = roi.status, goal = roi.goal });

                      //  Console.WriteLine("TrigAft____ROIEadd");
                      //  Thread.Sleep(4000);
                    }

                }
                k = 0;
            } 

         return ROIE;

        }  // ends main


    }


}
