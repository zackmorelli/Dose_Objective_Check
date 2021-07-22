using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;


/*
    Dose Objective Check - LISTMAKER

    Description:
    This is an internal helper class of the Dose Objective Check program which contains a function that is used to read in the dose objectives of the Lahey Hospital Radiation Oncology Department, which are maintained in specifically formatted text files.
    There are two text documents, one for Conventional plans, one for SRS/SBRT plans. The file paths are hardcoded for their locations on the Lahey RadOnc department servers.

    This program is expressely written as a plug-in script for use with Varian's Eclipse Treatment Planning System, and requires Varian's API files to run properly.
    This program runs on .NET Framework 4.6.1. It also uses MigraDoc and PDFSharp for the PDF generation, commonly available libraries which can be found on NuGet

    Copyright (C) 2021 Zackary Thomas Ricci Morelli
    
    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <https://www.gnu.org/licenses/>.

    I can be contacted at: zackmorelli@gmail.com

*/


namespace DoseObjectiveCheck
{
    public class LISTMAKER
    {

        public static List<DoseObjective> Listmaker(string Ttype, string Tsite, string [] Si, string laterality, string PMRN)
        {

            List<DoseObjective> ROIE = new List<DoseObjective>();                 // OUTPUT - list of actual ROIs for the treatment site given by user
            List<DoseObjective> ROIL = new List<DoseObjective>();                 // initial list of all ROI.ROI objects pulled from txt file

            int linecount = 0;                     // line counter
            string line;                           // stores each line as it is read in
            string temp = null;
            string[] words;                      // array of strings used to store all the strings made when each line is divided into individual words using Split
            string[] header;          
            int j = 0;
            int k = 0;

            string tname = null;                    // temporary variables used to build each ROI.ROI
            int tid = 0;
            string tlimit = null;
            string tlimval = null;                   // needs to be a string because of a few weird cases
            string tstrict = null;
            string tlimunit = null;
            string tstatus = null;
            string tgoal = null;
            string tstruct = null;
            string site = null;
            string approv = null;
            DialogResult diagR = new DialogResult();

            //check to see if a custom list from the PCTPN exists for this patient

            // string path = @"\\shceclipseimg\VA_DATA$\ProgramData\Vision\PublishedScripts\ConventionalROIList.txt";    // Winchester
            // string path = @"\\wvvrnimbp01ss\va_data$\filedata\ProgramData\Vision\PublishedScripts\ConventionalROIList.txt";   // V13.7 Lahey
            // string path =  @"\\wvariafssp01\VA_DATA$\ProgramData\Vision\PatientSpecificDoseObjectiveLists"; V16.1 Lahey

            if (File.Exists(@"\\wvariafssp01ss\VA_DATA$\ProgramData\Vision\PatientSpecificDoseObjectiveLists\" + PMRN + ".txt"))
            {
                 string path = @"\\wvariafssp01ss\VA_DATA$\ProgramData\Vision\PatientSpecificDoseObjectiveLists\" + PMRN + ".txt";   // Lahey
                // string path = @"\\shceclipseimg\VA_DATA$\ProgramData\Vision\PatientSpecificObjectiveLists\" + PMRN + ".txt";    // Winchester

                StreamReader ss = File.OpenText(path);
                line = ss.ReadLine();
                header = line.Split(',');
                site = header[1];
                approv = header[2];

                ss.Close();

                if (approv.Contains("Approved"))
                {
                    // not approved
                    diagR = MessageBox.Show("This patient has a custom dose objective list for the treatment site " + site + " generated from the Physician Clinical Treatment Planning Note, however it has NOT been approved.\n Would you like to proceed using this objective list? If you click 'No' the program will continue with a default objective list as normal.", "Custom Objective List - " + PMRN, MessageBoxButtons.YesNo);
                }
                else
                {
                    diagR = MessageBox.Show("This patient has a custom dose objective list for the treatment site " + site + " generated from the Physician Clinical Treatment Planning Note approved by " + approv + ".\n Would you like to proceed using this objective list? If you click 'No' the program will continue with a default objective list as normal.", "Custom Objective List - " + PMRN, MessageBoxButtons.YesNo);
                }

                if (diagR == DialogResult.Yes)
                {

                    try
                    {
                        using (StreamReader Lread = File.OpenText(path))   //opens file and puts it into a stream
                        {
                            // Console.WriteLine("TriggerConv3");
                            Lread.ReadLine();  //read in the first line to skip the header (only for the custom lists) 

                            while ((line = Lread.ReadLine()) != "END")    //each loop pulls a line from the file until no lines left
                            {
                                //MessageBox.Show("read line.");
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
                                string[] ttreatsite = new string[5] {"element", "element", "element", "element", "element"};
                                words = null;

                                //MessageBox.Show("line: " + line);

                                words = line.Split(',');        // convenientley splits the line into multiple strings, stored in a string array, using space as a delimiter
                                                                // Console.WriteLine("words size: {0}", words.Length);
                                //This is to accomodate the docs entering "NA" in the structure name to get rid of objectives they don't want in the table in the PTCPN. Beacuse the PTCPN macro will stop if a row is blank
                                if(words[0] == "NA" || words[0] == "Na" || words[0] == "na" || words[0] == "n/a" || words[0] == "N/a" || words[0] == "N/A")
                                {
                                    continue;
                                }

                               // MessageBox.Show("Words size: " + words.Count());

                                // need to artificially make the "Name", ourselves, didn't put this in the PCTPN list

                                tid = linecount;
                               // MessageBox.Show("trig 1.");
                                tname = words[0] + " " + words[1] + " " + words[2] + " " + words[3] + " " + words[4];
                               // MessageBox.Show("trig 2.");
                                tstruct = words[0];
                                tlimit = words[1];
                                tstrict = words[2];

                                if (tstrict == "[record]")
                                {
                                    tlimval = "0.0";        // this is done just to prevent type conversion errors later on

                                    tlimunit = words[4];
                                    tgoal = words[5];                               
                                }
                                else
                                {
                                    tlimval = words[3];
                                    tlimunit = words[4];
                                    tgoal = words[5];
                                }

                                ttreatsite[0] = "MD custom";
                                ttreatsite[1] = site;
                                ROIL.Add(new DoseObjective { DoseObjectiveName = tname, Rstruct = tstruct, DoseObjectiveID = tid, limit = tlimit, limval = tlimval, strict = tstrict, limunit = tlimunit, status = tstatus, goal = tgoal, treatsite = ttreatsite, applystatus = true });

                                //  Console.WriteLine("Linecount is: {0}", linecount);

                                // Thread.Sleep(3000);
                                //  Console.WriteLine("TriggeraftROIL");

                                linecount++;

                            } // ends loop that pulls lines
                              // Console.WriteLine("TriggerConvend"); 
                        }   // ends open file

                        MessageBox.Show("End of custom dose objective file read-in. " + linecount + " objectives added.");
                        //===================================================================================================================================================

                        foreach (DoseObjective DO in ROIL)       // iterate through all the ROI.ROI elements in the ROIL list
                        {
                            //MessageBox.Show("ROIL Name is: " + roi.ROIName);
                           // MessageBox.Show("ROIL Struct is: " + roi.Rstruct);
                           // MessageBox.Show("ROIL Id is: " + roi.ROIId);

                            j++;

                            // MessageBox.Show("Tsite: " + Tsite);
                            //  MessageBox.Show("ROI Name " + roi.ROIName);

                            // snippet to deal with breast plan laterality is below. There are a few dose objectives which are laterality dependent, that is what this is for.

                            // if Right, then Lung_R is Ipsilateral and Lung_L is Contralateral
                            // if Left, then Lung_L is Ipsilateral and Lung_R is Contralateral

                            if (DO.DoseObjectiveName.Contains("Breast") || DO.DoseObjectiveName.Contains("breast"))
                            {
                                if (laterality == "Right")
                                {
                                    if (DO.DoseObjectiveName.Contains("Lung_L") & DO.DoseObjectiveName.Contains("Ipsilateral"))
                                    {
                                        continue;    // continue means nothing gets added to the ROIE list at the end...
                                    }
                                    else if (DO.DoseObjectiveName.Contains("Lung_R") & DO.DoseObjectiveName.Contains("Contralateral"))
                                    {
                                        continue;
                                    }
                                }
                                else if (laterality == "Left")
                                {
                                    if (DO.DoseObjectiveName.Contains("Lung_R") & DO.DoseObjectiveName.Contains("Ipsilateral"))
                                    {
                                        continue;
                                    }
                                    else if (DO.DoseObjectiveName.Contains("Lung_L") & DO.DoseObjectiveName.Contains("Contralateral"))
                                    {
                                        continue;
                                    }
                                }
                            }

                            // MessageBox.Show("Roi name befor list add: " + roi.ROIName);
                            ROIE.Add(new DoseObjective { DoseObjectiveName = DO.DoseObjectiveName, Rstruct = DO.Rstruct, DoseObjectiveID = DO.DoseObjectiveID, limit = DO.limit, limval = DO.limval, strict = DO.strict, limunit = DO.limunit, goal = DO.goal, status = DO.status, treatsite = DO.treatsite, applystatus = true});
                            //  Console.WriteLine("TrigAft____ROIEadd");  
                        }                                
                        //MessageBox.Show("End List finalization.");
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show(e.StackTrace + "\n\n\n" + e.Source + "\n\n\n" + e.Message);
                    }
                }
                else if (diagR == DialogResult.No)
                {
                    // run as normal, no custom dose objective list present
                    ROIE = ListmakerNorm(Ttype, Tsite, Si, laterality, ROIL, ROIE);
                }
            }
            else
            {
                //run as normal, no custom dose objective list present
                ROIE = ListmakerNorm(Ttype, Tsite, Si, laterality, ROIL, ROIE);
            }
                
         return ROIE;

        }  // ends Listmaker method

        //==============================================================================================================================================================================================================

        public static List<DoseObjective> ListmakerNorm(string Ttype, string Tsite, string[] Si, string laterality, List<DoseObjective> ROIL, List<DoseObjective> ROIE)
        {
            //List<ROI.ROI> ROIE = new List<ROI.ROI>();                 // OUTPUT - list of actual ROIs for the treatment site given by user
            //List<ROI.ROI> ROIL = new List<ROI.ROI>();                 // initial list of all ROI.ROI objects pulled from txt file

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
                // string path = @"\\shceclipseimg\VA_DATA$\ProgramData\Vision\PublishedScripts\ConventionalROIList.txt";    // Winchester
                // string path = @"\\wvvrnimbp01ss\va_data$\filedata\ProgramData\Vision\PublishedScripts\ConventionalROIList.txt";   // V13.7 Lahey
                 string path = @"\\wvariafssp01ss\VA_DATA$\ProgramData\Vision\PublishedScripts\ConventionalROIList.txt";    // V16.1 Lahey
                // Console.WriteLine("TriggerConv1");

                if (File.Exists(path))
                {
                    // Console.WriteLine("TriggerConv2");
                    using (StreamReader Lread = File.OpenText(path))   //opens file and puts it into a stream
                    {
                        // Console.WriteLine("TriggerConv3");

                        while ((line = Lread.ReadLine()) != "END")    //each loop pulls a line from the file until no lines left
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
                            string[] ttreatsite = new string[30] { "element", "element", "element", "element", "element", "element", "element", "element", "element", "element", "element", "element", "element", "element", "element", "element", "element", "element", "element", "element", "element", "element", "element", "element", "element", "element", "element", "element", "element", "element" };
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

                            ROIL.Add(new DoseObjective { DoseObjectiveName = tname, Rstruct = tstruct, DoseObjectiveID = tid, limit = tlimit, limval = tlimval, strict = tstrict, limunit = tlimunit, status = tstatus, goal = tgoal, treatsite = ttreatsite, applystatus = true });

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
            else if (Ttype == "SRS/SBRT" | Ttype == "Both")
            {
                // string path = @"\\shceclipseimg\VA_DATA$\ProgramData\Vision\PublishedScripts\SRSROIList.txt";   //Winchester
                // string path = @"\\wvvrnimbp01ss\va_data$\filedata\ProgramData\Vision\PublishedScripts\SRSROIList.txt";     // V13.7 Lahey
                string path = @"\\wvariafssp01ss\VA_DATA$\ProgramData\Vision\PublishedScripts\SRSROIList.txt";    // V16.1 Lahey

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
                            string[] ttreatsite = new string[30] { "element", "element", "element", "element", "element", "element", "element", "element", "element", "element", "element", "element", "element", "element", "element", "element", "element", "element", "element", "element", "element", "element", "element", "element", "element", "element", "element", "element", "element", "element" };
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

                            ROIL.Add(new DoseObjective { DoseObjectiveName = tname, Rstruct = tstruct, DoseObjectiveID = tid, limit = tlimit, limval = tlimval, strict = tstrict, limunit = tlimunit, status = tstatus, goal = tgoal, treatsite = ttreatsite, applystatus = true });

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

            //======================================================================================================================================================
            //This loop deals with parsing through treatment sites that have laterality.

            foreach (DoseObjective DO in ROIL)       // iterate through all the ROI.ROI elements in the ROIL list
            {
                // Console.WriteLine("ROIL size is: {0}", ROIL.Count);
                //  Console.WriteLine("ROIL index: {0}", j);
                //  Console.WriteLine("roi name is: {0}", roi.ROIName);
                j++;

                foreach (string str in DO.treatsite)      // iterates through all the treatsite elements of a specific ROI.ROI element in the ROIL list 
                {
                    //  Console.WriteLine("This treatsite size is: {0}", roi.treatsite.Length);
                    //  Console.WriteLine("Treatsite index: {0}", k);
                    // Console.WriteLine("treatsite {0} is: {1}", k, str);
                    k++;

                    if (Tsite != null)
                    {

                        if (str == Tsite)
                        {

                            // MessageBox.Show("Tsite: " + Tsite);
                            //  MessageBox.Show("ROI Name " + roi.ROIName);

                            // snippet to deal with breast plan laterality is below

                            // if Right, then Lung_R is Ipsilateral and Lung_L is Contralateral
                            // if Left, then Lung_L is Ipsilateral and Lung_R is Contralateral


                            if (Tsite == "Breast 23+fx" || Tsite == "Breast Hypofx" || Tsite == "Breast+regional_LN 23+fx" || Tsite == "Breast+regional_LN Hypofx")
                            {
                                if (laterality == "Right")
                                {
                                    if (DO.DoseObjectiveName.Contains("Lung_L") & DO.DoseObjectiveName.Contains("Ipsilateral"))
                                    {
                                        continue;    // continue means nothing gets added to the ROIE list at the end...
                                    }
                                    else if (DO.DoseObjectiveName.Contains("Lung_R") & DO.DoseObjectiveName.Contains("Contralateral"))
                                    {
                                        continue;
                                    }
                                }
                                else if (laterality == "Left")
                                {
                                    if (DO.DoseObjectiveName.Contains("Lung_R") & DO.DoseObjectiveName.Contains("Ipsilateral"))
                                    {
                                        continue;
                                    }
                                    else if (DO.DoseObjectiveName.Contains("Lung_L") & DO.DoseObjectiveName.Contains("Contralateral"))
                                    {
                                        continue;
                                    }
                                }
                            }

                            // MessageBox.Show("Roi name befor list add: " + roi.ROIName);
                            ROIE.Add(new DoseObjective { DoseObjectiveName = DO.DoseObjectiveName, Rstruct = DO.Rstruct, DoseObjectiveID = DO.DoseObjectiveID, limit = DO.limit, limval = DO.limval, strict = DO.strict, limunit = DO.limunit, status = DO.status, treatsite = DO.treatsite, goal = DO.goal, applystatus = true });
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
                                if (foo == "Breast 23+fx" || foo == "Breast Hypofx" || foo == "Breast+regional_LN 23+fx" || foo == "Breast+regional_LN Hypofx")
                                {
                                    if (laterality == "Right")
                                    {
                                        if (DO.DoseObjectiveName.Contains("Lung_L") & DO.DoseObjectiveName.Contains("Ipsilateral"))
                                        {
                                            continue;
                                        }
                                        else if (DO.DoseObjectiveName.Contains("Lung_R") & DO.DoseObjectiveName.Contains("Contralateral"))
                                        {
                                            continue;
                                        }
                                    }
                                    else if (laterality == "Left")
                                    {
                                        if (DO.DoseObjectiveName.Contains("Lung_R") & DO.DoseObjectiveName.Contains("Ipsilateral"))
                                        {
                                            continue;
                                        }
                                        else if (DO.DoseObjectiveName.Contains("Lung_L") & DO.DoseObjectiveName.Contains("Contralateral"))
                                        {
                                            continue;
                                        }
                                    }
                                }

                                // MessageBox.Show("Add SI");
                                ROIE.Add(new DoseObjective { DoseObjectiveName = DO.DoseObjectiveName, Rstruct = DO.Rstruct, DoseObjectiveID = DO.DoseObjectiveID, limit = DO.limit, limval = DO.limval, strict = DO.strict, limunit = DO.limunit, status = DO.status, treatsite = DO.treatsite, goal = DO.goal, applystatus = true });
                            }
                        }
                    }
                }
                k = 0;
            }

            return ROIE;
        }

    } // ends listmaker class

}
