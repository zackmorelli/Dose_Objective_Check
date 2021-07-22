using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.Tables;
using System.Windows.Forms;

namespace PdfReport.Reporting.MigraDoc.Internal
{
    class ConvStats
    {
        public void Add(Section section, ReportData data)
        {
           // section.AddPageBreak();
            var k = section.AddParagraph("Conventional Dose Coverage Statistics", StyleNames.Heading1);
            k.Format.KeepWithNext = true;
            k.Format.Font.Underline = Underline.Single;

            if(data.conventionalstats.Simple == true)
            {
                var k1 = section.AddParagraph("Single target plan with a simple PTV/CTV pair. Target dose coverage reported below.");
                k1.Format.KeepWithNext = true;

                if(data.conventionalstats.Sequential == true)
                {
                    var k2 = section.AddParagraph("Course of sequential plans with a single target and a simple PTV/CTV pair. The target dose coverage is shown below in separate sections for each plan, and plansums.");
                    k1.Format.KeepWithNext = true;
                }
            }
            else if (data.conventionalstats.DosePainted == true)
            {
                var k2 = section.AddParagraph("Dose painted plan. If a dose level target is made up of a simple PTV/CTV pair, the target coverage for that dose level is displayed in one section. If not, there is a section for each structure.");
                k2.Format.KeepWithNext = true;
            }
            else if (data.conventionalstats.Manual == true)
            {
                var k2 = section.AddParagraph("User has manually selected target structures. The target coverage for each structure is shown in its own section.");
                k2.Format.KeepWithNext = true;
            }

            string title = null;

            if (data.conventionalstats.Simple == true)
            {
                if (data.conventionalstats.Sequential == false)
                {
                    TargetStructure PTV = data.conventionalstats.TargetStructures.Where(s => s.StructureNAME.Contains("PTV")).First();
                    List<TargetStructure> CTVlist = data.conventionalstats.TargetStructures.Where(s => s.StructureNAME.Contains("CTV")).ToList();
                    List<TargetStructure> GTVlist = data.conventionalstats.TargetStructures.Where(s => s.StructureNAME.Contains("GTV")).ToList();

                    if(CTVlist.Count > 0 && GTVlist.Count > 0)
                    {
                        TargetStructure CTV = CTVlist.First();
                        TargetStructure GTV = GTVlist.First();
                        title = "Target : " + PTV.StructureNAME + " and " + CTV.StructureNAME + " and " + GTV.StructureNAME;
                        AddTableTitle(section, title);
                        AddDoseListTableSimplewithCTVandGTV(section, data, PTV, CTV, GTV);
                    }
                    else if(CTVlist.Count > 0)
                    {
                        TargetStructure CTV = CTVlist.First();
                        title = "Target : " + PTV.StructureNAME + " and " + CTV.StructureNAME;
                        AddTableTitle(section, title);
                        AddDoseListTableSimplewithCTV(section, data, PTV, CTV);
                    }
                    else if(GTVlist.Count > 0)
                    {
                        TargetStructure GTV = GTVlist.First();
                        title = "Target : " + PTV.StructureNAME + " and " + GTV.StructureNAME;
                        AddTableTitle(section, title);
                        AddDoseListTableSimplewithGTV(section, data, PTV, GTV);
                    }
                }
                else if(data.conventionalstats.Sequential == true)
                {
                    //so we want to iterate this by plan
                    //first use some advanced LINQ to make a list of distinct plans in the target list
                    List<string> plandistinct = data.conventionalstats.TargetStructures.Select(r => r.plan).Distinct().ToList();

                    foreach(string str in plandistinct)
                    {
                        // so this is iterating through all the distinct plans in the target list of a course of sequential plans
                        TargetStructure PTV = data.conventionalstats.TargetStructures.Where(s => s.StructureNAME.Contains("PTV") && s.plan.Equals(str)).First();
                        List<TargetStructure> CTVlist = data.conventionalstats.TargetStructures.Where(s => s.StructureNAME.Contains("CTV") && s.plan.Equals(str)).ToList();
                        List<TargetStructure> GTVlist = data.conventionalstats.TargetStructures.Where(s => s.StructureNAME.Contains("GTV") && s.plan.Equals(str)).ToList();

                        if (CTVlist.Count > 0 && GTVlist.Count > 0)
                        {
                            TargetStructure CTV = CTVlist.First();
                            TargetStructure GTV = GTVlist.First();
                            title = "Plan/Plansum: " + str + "   Target : " + PTV.StructureNAME + " and " + CTV.StructureNAME + " and " + GTV.StructureNAME;
                            AddTableTitle(section, title);
                            AddDoseListTableSimplewithCTVandGTV(section, data, PTV, CTV, GTV);
                        }
                        else if (CTVlist.Count > 0)
                        {
                            TargetStructure CTV = CTVlist.First();
                            title = "Plan/Plansum: " + str + "   Target : " + PTV.StructureNAME + " and " + CTV.StructureNAME;
                            AddTableTitle(section, title);
                            AddDoseListTableSimplewithCTV(section, data, PTV, CTV);
                        }
                        else if (GTVlist.Count > 0)
                        {
                            TargetStructure GTV = GTVlist.First();
                            title = "Plan/Plansum: " + str + "   Target : " + PTV.StructureNAME + " and " + GTV.StructureNAME;
                            AddTableTitle(section, title);
                            AddDoseListTableSimplewithGTV(section, data, PTV, GTV);
                        }
                    }
                }
            }
            else if (data.conventionalstats.DosePainted == true)
            {
                List<TargetStructure> doselevelone = data.conventionalstats.TargetStructures.Where(t => t.DosePaintedTargetNumber == 1).ToList();
                List<TargetStructure> doseleveltwo = data.conventionalstats.TargetStructures.Where(t => t.DosePaintedTargetNumber == 2).ToList();
                List<TargetStructure> doselevelthree = data.conventionalstats.TargetStructures.Where(t => t.DosePaintedTargetNumber == 3).ToList();
                List<TargetStructure> doselevelfour = data.conventionalstats.TargetStructures.Where(t => t.DosePaintedTargetNumber == 4).ToList();

                if(doselevelone.Count > 0)
                {
                    //so at least one structure in the first dose level
                    //need to find out if its a nice PTV/CTV pair or something else
                    //MessageBox.Show("doselevelcount: " + doselevelone.Count + "  " + doselevelone[0].StructureNAME + "  " + doselevelone[1].StructureNAME);
                    if(doselevelone.Count < 4 && doselevelone.All(s => s.StructureNAME.Contains("PTV") || s.StructureNAME.Contains("CTV") || s.StructureNAME.Contains("GTV"))) 
                    {
                        //Nice PTV/CTV pair, so call the simple doselist table
                        TargetStructure PTV = data.conventionalstats.TargetStructures.Where(s => s.StructureNAME.Contains("PTV")).First();
                        List<TargetStructure> CTVlist = data.conventionalstats.TargetStructures.Where(s => s.StructureNAME.Contains("CTV")).ToList();
                        List<TargetStructure> GTVlist = data.conventionalstats.TargetStructures.Where(s => s.StructureNAME.Contains("GTV")).ToList();

                        if (CTVlist.Count > 0 && GTVlist.Count > 0)
                        {
                            TargetStructure CTV = CTVlist.First();
                            TargetStructure GTV = GTVlist.First();
                            title = "Dose Level One - " + doselevelone.First().Dose + " cGy   Target: " + PTV.StructureNAME + " and " + CTV.StructureNAME + " and " + GTV.StructureNAME;
                            AddTableTitle(section, title);
                            AddDoseListTableSimplewithCTVandGTV(section, data, PTV, CTV, GTV);
                        }
                        else if (CTVlist.Count > 0)
                        {
                            TargetStructure CTV = CTVlist.First();
                            title = "Dose Level One - " + doselevelone.First().Dose + " cGy   Target: " + PTV.StructureNAME + " and " + CTV.StructureNAME;
                            AddTableTitle(section, title);
                            AddDoseListTableSimplewithCTV(section, data, PTV, CTV);
                        }
                        else if (GTVlist.Count > 0)
                        {
                            TargetStructure GTV = GTVlist.First();
                            title = "Dose Level One - " + doselevelone.First().Dose + " cGy   Target: " + PTV.StructureNAME + " and " + GTV.StructureNAME;
                            AddTableTitle(section, title);
                            AddDoseListTableSimplewithGTV(section, data, PTV, GTV);
                        }
                    }
                    else
                    {
                        int dosepaintedcount = 0;
                        foreach (TargetStructure ts in doselevelone)
                        {
                            title = "Dose Level One - " + doselevelone.First().Dose + " cGy   Target: " + ts.StructureNAME;
                            AddTableTitle(section, title);
                            AddDoseListTableManual(section, data, dosepaintedcount);
                            dosepaintedcount++;
                        }
                    }
                }
                
                if(doseleveltwo.Count > 0)
                {
                    if (doseleveltwo.Count < 4 && doseleveltwo.All(s => s.StructureNAME.Contains("PTV") || s.StructureNAME.Contains("CTV") || s.StructureNAME.Contains("GTV")))
                    {
                        //Nice PTV/CTV pair, so call the simple doselist table

                        TargetStructure PTV = data.conventionalstats.TargetStructures.Where(s => s.StructureNAME.Contains("PTV")).First();
                        List<TargetStructure> CTVlist = data.conventionalstats.TargetStructures.Where(s => s.StructureNAME.Contains("CTV")).ToList();
                        List<TargetStructure> GTVlist = data.conventionalstats.TargetStructures.Where(s => s.StructureNAME.Contains("GTV")).ToList();

                        if (CTVlist.Count > 0 && GTVlist.Count > 0)
                        {
                            TargetStructure CTV = CTVlist.First();
                            TargetStructure GTV = GTVlist.First();
                            title = "Dose Level Two - " + doseleveltwo.First().Dose + " cGy   Target: " + PTV.StructureNAME + " and " + CTV.StructureNAME + " and " + GTV.StructureNAME;
                            AddTableTitle(section, title);
                            AddDoseListTableSimplewithCTVandGTV(section, data, PTV, CTV, GTV);
                        }
                        else if (CTVlist.Count > 0)
                        {
                            TargetStructure CTV = CTVlist.First();
                            title = "Dose Level Two - " + doseleveltwo.First().Dose + " cGy   Target: " + PTV.StructureNAME + " and " + CTV.StructureNAME;
                            AddTableTitle(section, title);
                            AddDoseListTableSimplewithCTV(section, data, PTV, CTV);
                        }
                        else if (GTVlist.Count > 0)
                        {
                            TargetStructure GTV = GTVlist.First();
                            title = "Dose Level Two - " + doseleveltwo.First().Dose + " cGy   Target: " + PTV.StructureNAME + " and " + GTV.StructureNAME;
                            AddTableTitle(section, title);
                            AddDoseListTableSimplewithGTV(section, data, PTV, GTV);
                        }
                    }
                    else
                    {
                        int dosepaintedcount = 0;
                        foreach (TargetStructure ts in doseleveltwo)
                        {
                            title = "Dose Level Two - " + doseleveltwo.First().Dose + " cGy   Target: " + ts.StructureNAME;
                            AddTableTitle(section, title);
                            AddDoseListTableManual(section, data, dosepaintedcount);
                            dosepaintedcount++;
                        }
                    }
                }

                if (doselevelthree.Count > 0)
                {
                    if (doselevelthree.Count < 4 && doselevelthree.All(s => s.StructureNAME.Contains("PTV") || s.StructureNAME.Contains("CTV") || s.StructureNAME.Contains("GTV")))
                    {
                        //Nice PTV/CTV pair, so call the simple doselist table
                        TargetStructure PTV = data.conventionalstats.TargetStructures.Where(s => s.StructureNAME.Contains("PTV")).First();
                        List<TargetStructure> CTVlist = data.conventionalstats.TargetStructures.Where(s => s.StructureNAME.Contains("CTV")).ToList();
                        List<TargetStructure> GTVlist = data.conventionalstats.TargetStructures.Where(s => s.StructureNAME.Contains("GTV")).ToList();

                        if (CTVlist.Count > 0 && GTVlist.Count > 0)
                        {
                            TargetStructure CTV = CTVlist.First();
                            TargetStructure GTV = GTVlist.First();
                            title = "Dose Level Three - " + doselevelthree.First().Dose + " cGy   Target: " + PTV.StructureNAME + " and " + CTV.StructureNAME + " and " + GTV.StructureNAME;
                            AddTableTitle(section, title);
                            AddDoseListTableSimplewithCTVandGTV(section, data, PTV, CTV, GTV);
                        }
                        else if (CTVlist.Count > 0)
                        {
                            TargetStructure CTV = CTVlist.First();
                            title = "Dose Level Three - " + doselevelthree.First().Dose + " cGy   Target: " + PTV.StructureNAME + " and " + CTV.StructureNAME;
                            AddTableTitle(section, title);
                            AddDoseListTableSimplewithCTV(section, data, PTV, CTV);
                        }
                        else if (GTVlist.Count > 0)
                        {
                            TargetStructure GTV = GTVlist.First();
                            title = "Dose Level Three - " + doselevelthree.First().Dose + " cGy   Target: " + PTV.StructureNAME + " and " + GTV.StructureNAME;
                            AddTableTitle(section, title);
                            AddDoseListTableSimplewithGTV(section, data, PTV, GTV);
                        }
                    }
                    else
                    {
                        int dosepaintedcount = 0;
                        foreach (TargetStructure ts in doselevelthree)
                        {
                            title = "Dose Level Three - " + doselevelthree.First().Dose + " cGy   Target: " + ts.StructureNAME;
                            AddTableTitle(section, title);
                            AddDoseListTableManual(section, data, dosepaintedcount);
                            dosepaintedcount++;
                        }
                    }
                }

                if (doselevelfour.Count > 0)
                {
                    if (doselevelfour.Count < 4 && doselevelfour.All(s => s.StructureNAME.Contains("PTV") || s.StructureNAME.Contains("CTV") || s.StructureNAME.Contains("GTV")))
                    {
                        //Nice PTV/CTV pair, so call the simple doselist table

                        TargetStructure PTV = data.conventionalstats.TargetStructures.Where(s => s.StructureNAME.Contains("PTV")).First();
                        List<TargetStructure> CTVlist = data.conventionalstats.TargetStructures.Where(s => s.StructureNAME.Contains("CTV")).ToList();
                        List<TargetStructure> GTVlist = data.conventionalstats.TargetStructures.Where(s => s.StructureNAME.Contains("GTV")).ToList();

                        if (CTVlist.Count > 0 && GTVlist.Count > 0)
                        {
                            TargetStructure CTV = CTVlist.First();
                            TargetStructure GTV = GTVlist.First();
                            title = "Dose Level Four - " + doselevelfour.First().Dose + " cGy   Target: " + PTV.StructureNAME + " and " + CTV.StructureNAME + " and " + GTV.StructureNAME;
                            AddTableTitle(section, title);
                            AddDoseListTableSimplewithCTVandGTV(section, data, PTV, CTV, GTV);
                        }
                        else if (CTVlist.Count > 0)
                        {
                            TargetStructure CTV = CTVlist.First();
                            title = "Dose Level Four - " + doselevelfour.First().Dose + " cGy   Target: " + PTV.StructureNAME + " and " + CTV.StructureNAME;
                            AddTableTitle(section, title);
                            AddDoseListTableSimplewithCTV(section, data, PTV, CTV);
                        }
                        else if (GTVlist.Count > 0)
                        {
                            TargetStructure GTV = GTVlist.First();
                            title = "Dose Level Four - " + doselevelfour.First().Dose + " cGy   Target: " + PTV.StructureNAME + " and " + GTV.StructureNAME;
                            AddTableTitle(section, title);
                            AddDoseListTableSimplewithGTV(section, data, PTV, GTV);
                        }
                    }
                    else
                    {
                        int dosepaintedcount = 0;
                        foreach (TargetStructure ts in doselevelfour)
                        {
                            title = "Dose Level Four - " + doselevelfour.First().Dose + " cGy   Target: " + ts.StructureNAME;
                            AddTableTitle(section, title);
                            AddDoseListTableManual(section, data, dosepaintedcount);
                            dosepaintedcount++;
                        }
                    }
                }
            }
            else if (data.conventionalstats.Manual == true)
            {
                int manualcount = 0;
                foreach (TargetStructure targetstat in data.conventionalstats.TargetStructures)
                {
                    title = "Plan/Plansum: " + targetstat.plan + "   Target: " + targetstat.StructureNAME;

                    AddTableTitle(section, title);
                    AddDoseListTableManual(section, data, manualcount);
                    manualcount++;
                }
            }
        }

        private void AddTableTitle(Section section, string title)
        {
            var p = section.AddParagraph(title, StyleNames.Heading2);
            p.Format.KeepWithNext = true;
        }

        private void AddDoseListTableManual(Section section, ReportData data, int count)
        {
            var table = section.AddTable();

            FormatTable(table);
            AddColumnsAndHeaders(table);
            AddRows(table, data, count);
            AddLastRowBorder(table);
            // AlternateRowShading(table);
        }

        private void AddDoseListTableSimplewithCTV(Section section, ReportData data, TargetStructure PTV, TargetStructure CTV)
        {
            var table = section.AddTable();

            FormatTable(table);
            AddColumnsAndHeaders(table);
            AddRowsSimplewithCTV(table, data, PTV, CTV);
            AddLastRowBorder(table);
            // AlternateRowShading(table);
        }

        private void AddDoseListTableSimplewithGTV(Section section, ReportData data, TargetStructure PTV, TargetStructure GTV)
        {
            var table = section.AddTable();

            FormatTable(table);
            AddColumnsAndHeaders(table);
            AddRowsSimplewithGTV(table, data, PTV, GTV);
            AddLastRowBorder(table);
            // AlternateRowShading(table);
        }

        private void AddDoseListTableSimplewithCTVandGTV(Section section, ReportData data, TargetStructure PTV, TargetStructure CTV, TargetStructure GTV)
        {
            var table = section.AddTable();

            FormatTable(table);
            AddColumnsAndHeaders(table);
            AddRowsSimplewithCTVandGTV(table, data, PTV, CTV, GTV);
            AddLastRowBorder(table);
            // AlternateRowShading(table);
        }

        private static void FormatTable(Table table)
        {
            table.LeftPadding = 0;
            table.TopPadding = Size.TableCellPadding;
            table.RightPadding = 0;
            table.BottomPadding = Size.TableCellPadding;
            table.Format.LeftIndent = Size.TableCellPadding;
            table.Format.RightIndent = Size.TableCellPadding;
        }

        private void AddColumnsAndHeaders(Table table)
        {
            var width = Size.GetWidth(table.Section);
            table.AddColumn(width * 0.5);
            table.AddColumn(width * 0.5);
        }

        private static void AddRowsSimplewithCTV(Table table, ReportData data, TargetStructure PTV, TargetStructure CTV)
        {
            string roundedPTVXXRX = null;

            var row0 = table.AddRow();
            row0.VerticalAlignment = VerticalAlignment.Center;
            row0.Cells[0].AddParagraph("Dose (cGy)");
            row0.Cells[1].AddParagraph(Convert.ToString(PTV.Dose));

            var row1 = table.AddRow();
            row1.VerticalAlignment = VerticalAlignment.Center;
            row1.Cells[0].AddParagraph("Target Volume (cc)");
            string roundedptvvol = Convert.ToString(Math.Round(PTV.vol, 2, MidpointRounding.AwayFromZero));
            row1.Cells[1].AddParagraph(roundedptvvol);

            //Second row. Global Max Point Dose FOR THE PLAN OF THIS TARGET.
            double globalmaxpercentofRX = (PTV.GlobalMaxPointDose / PTV.Dose) * 100.0;
            var row2 = table.AddRow();
            row2.VerticalAlignment = VerticalAlignment.Center;
            row2.Cells[0].AddParagraph("Global Max Point Dose");
            row2.Cells[1].AddParagraph(Math.Round(PTV.GlobalMaxPointDose, 2, MidpointRounding.AwayFromZero) + " cGy.\nThis is " + Math.Round(globalmaxpercentofRX, 2, MidpointRounding.AwayFromZero) + "% of the dose for this target.");

            // third row
            var row3 = table.AddRow();
            row3.VerticalAlignment = VerticalAlignment.Center;
            row3.Cells[0].AddParagraph("CTV V" + data.conventionalstats.CTVRXcoverage + "% RX\nShould be greater than or equal to " + data.conventionalstats.CTVVolcoverage + "%");
            roundedPTVXXRX = Convert.ToString(Math.Round(CTV.CTVXXRX0, 2, MidpointRounding.AwayFromZero));
            row3.Cells[1].AddParagraph(roundedPTVXXRX);
            if (Convert.ToDouble(roundedPTVXXRX) >= data.conventionalstats.CTVVolcoverage)
            {
                row3.Cells[1].Shading.Color = Color.FromRgb(0, 255, 0);
            }
            else
            {
                row3.Cells[1].Shading.Color = Color.FromRgb(255, 0, 0);
            }

            // fouth row
            var row4 = table.AddRow();
            row4.VerticalAlignment = VerticalAlignment.Center;
            row4.Cells[0].AddParagraph("PTV V" + data.conventionalstats.PTV1RXcoverage + "% RX\nShould be greater than or equal to " + data.conventionalstats.PTV1Volcoverage + "%");
            roundedPTVXXRX = Convert.ToString(Math.Round(PTV.PTVXXRX1, 2, MidpointRounding.AwayFromZero));
            row4.Cells[1].AddParagraph(roundedPTVXXRX);
            //MessageBox.Show("rounedePTVXXRX1: " + roundedPTVXXRX);
            if (Convert.ToDouble(roundedPTVXXRX) >= data.conventionalstats.PTV1Volcoverage)
            {
                row4.Cells[1].Shading.Color = Color.FromRgb(0, 255, 0);
            }
            else
            {
                row4.Cells[1].Shading.Color = Color.FromRgb(255, 0, 0);
            }

            // fifth row
            var row5 = table.AddRow();
            row5.VerticalAlignment = VerticalAlignment.Center;
            row5.Cells[0].AddParagraph("PTV V" + data.conventionalstats.PTV2RXcoverage + "% RX\nShould be greater than or equal to " + data.conventionalstats.PTV2Volcoverage + "%");
            roundedPTVXXRX = Convert.ToString(Math.Round(PTV.PTVXXRX2, 2, MidpointRounding.AwayFromZero));
            row5.Cells[1].AddParagraph(roundedPTVXXRX);

            if (Convert.ToDouble(roundedPTVXXRX) >= data.conventionalstats.PTV2Volcoverage)
            {
                row5.Cells[1].Shading.Color = Color.FromRgb(0, 255, 0);
            }
            else
            {
                row5.Cells[1].Shading.Color = Color.FromRgb(255, 0, 0);
            }

            // sixth row
            var row6 = table.AddRow();
            row6.VerticalAlignment = VerticalAlignment.Center;
            row6.Cells[0].AddParagraph("PTV V" + data.conventionalstats.PTV3RXcoverage + "% RX\nShould be less than or equal to " + data.conventionalstats.PTV3Volcoverage + "%");
            roundedPTVXXRX = Convert.ToString(Math.Round(PTV.PTVXXRX3, 2, MidpointRounding.AwayFromZero));
            row6.Cells[1].AddParagraph(roundedPTVXXRX);

            if (Convert.ToDouble(roundedPTVXXRX) <= data.conventionalstats.PTV3Volcoverage)
            {
                row6.Cells[1].Shading.Color = Color.FromRgb(0, 255, 0);
            }
            else
            {
                row6.Cells[1].Shading.Color = Color.FromRgb(255, 0, 0);
            }
        }

        private static void AddRowsSimplewithGTV(Table table, ReportData data, TargetStructure PTV, TargetStructure GTV)
        {
            string roundedPTVXXRX = null;

            var row0 = table.AddRow();
            row0.VerticalAlignment = VerticalAlignment.Center;
            row0.Cells[0].AddParagraph("Dose (cGy)");
            row0.Cells[1].AddParagraph(Convert.ToString(PTV.Dose));

            var row1 = table.AddRow();
            row1.VerticalAlignment = VerticalAlignment.Center;
            row1.Cells[0].AddParagraph("Target Volume (cc)");
            string roundedptvvol = Convert.ToString(Math.Round(PTV.vol, 2, MidpointRounding.AwayFromZero));
            row1.Cells[1].AddParagraph(roundedptvvol);

            //Second row. Global Max Point Dose FOR THE PLAN OF THIS TARGET.
            double globalmaxpercentofRX = (PTV.GlobalMaxPointDose / PTV.Dose) * 100.0;
            var row2 = table.AddRow();
            row2.VerticalAlignment = VerticalAlignment.Center;
            row2.Cells[0].AddParagraph("Global Max Point Dose");
            row2.Cells[1].AddParagraph(Math.Round(PTV.GlobalMaxPointDose, 2, MidpointRounding.AwayFromZero) + " cGy.\nThis is " + Math.Round(globalmaxpercentofRX, 2, MidpointRounding.AwayFromZero) + "% of the dose for this target.");

            // third row
            var row3 = table.AddRow();
            row3.VerticalAlignment = VerticalAlignment.Center;
            row3.Cells[0].AddParagraph("GTV V" + data.conventionalstats.CTVRXcoverage + "% RX\nShould be greater than or equal to " + data.conventionalstats.CTVVolcoverage + "%");
            roundedPTVXXRX = Convert.ToString(Math.Round(GTV.CTVXXRX0, 2, MidpointRounding.AwayFromZero));
            row3.Cells[1].AddParagraph(roundedPTVXXRX);
            if (Convert.ToDouble(roundedPTVXXRX) >= data.conventionalstats.CTVVolcoverage)
            {
                row3.Cells[1].Shading.Color = Color.FromRgb(0, 255, 0);
            }
            else
            {
                row3.Cells[1].Shading.Color = Color.FromRgb(255, 0, 0);
            }

            // fouth row
            var row4 = table.AddRow();
            row4.VerticalAlignment = VerticalAlignment.Center;
            row4.Cells[0].AddParagraph("PTV V" + data.conventionalstats.PTV1RXcoverage + "% RX\nShould be greater than or equal to " + data.conventionalstats.PTV1Volcoverage + "%");
            roundedPTVXXRX = Convert.ToString(Math.Round(PTV.PTVXXRX1, 2, MidpointRounding.AwayFromZero));
            row4.Cells[1].AddParagraph(roundedPTVXXRX);
            //MessageBox.Show("rounedePTVXXRX1: " + roundedPTVXXRX);
            if (Convert.ToDouble(roundedPTVXXRX) >= data.conventionalstats.PTV1Volcoverage)
            {
                row4.Cells[1].Shading.Color = Color.FromRgb(0, 255, 0);
            }
            else
            {
                row4.Cells[1].Shading.Color = Color.FromRgb(255, 0, 0);
            }

            // fifth row
            var row5 = table.AddRow();
            row5.VerticalAlignment = VerticalAlignment.Center;
            row5.Cells[0].AddParagraph("PTV V" + data.conventionalstats.PTV2RXcoverage + "% RX\nShould be greater than or equal to " + data.conventionalstats.PTV2Volcoverage + "%");
            roundedPTVXXRX = Convert.ToString(Math.Round(PTV.PTVXXRX2, 2, MidpointRounding.AwayFromZero));
            row5.Cells[1].AddParagraph(roundedPTVXXRX);

            if (Convert.ToDouble(roundedPTVXXRX) >= data.conventionalstats.PTV2Volcoverage)
            {
                row5.Cells[1].Shading.Color = Color.FromRgb(0, 255, 0);
            }
            else
            {
                row5.Cells[1].Shading.Color = Color.FromRgb(255, 0, 0);
            }

            // sixth row
            var row6 = table.AddRow();
            row6.VerticalAlignment = VerticalAlignment.Center;
            row6.Cells[0].AddParagraph("PTV V" + data.conventionalstats.PTV3RXcoverage + "% RX\nShould be less than or equal to " + data.conventionalstats.PTV3Volcoverage + "%");
            roundedPTVXXRX = Convert.ToString(Math.Round(PTV.PTVXXRX3, 2, MidpointRounding.AwayFromZero));
            row6.Cells[1].AddParagraph(roundedPTVXXRX);

            if (Convert.ToDouble(roundedPTVXXRX) <= data.conventionalstats.PTV3Volcoverage)
            {
                row6.Cells[1].Shading.Color = Color.FromRgb(0, 255, 0);
            }
            else
            {
                row6.Cells[1].Shading.Color = Color.FromRgb(255, 0, 0);
            }
        }

        private static void AddRowsSimplewithCTVandGTV(Table table, ReportData data, TargetStructure PTV, TargetStructure CTV, TargetStructure GTV)
        {
            string roundedPTVXXRX = null;

            var row0 = table.AddRow();
            row0.VerticalAlignment = VerticalAlignment.Center;
            row0.Cells[0].AddParagraph("Dose (cGy)");
            row0.Cells[1].AddParagraph(Convert.ToString(PTV.Dose));

            var row1 = table.AddRow();
            row1.VerticalAlignment = VerticalAlignment.Center;
            row1.Cells[0].AddParagraph("Target Volume (cc)");
            string roundedptvvol = Convert.ToString(Math.Round(PTV.vol, 2, MidpointRounding.AwayFromZero));
            row1.Cells[1].AddParagraph(roundedptvvol);

            //Second row. Global Max Point Dose FOR THE PLAN OF THIS TARGET.
            double globalmaxpercentofRX = (PTV.GlobalMaxPointDose / PTV.Dose) * 100.0;
            var row2 = table.AddRow();
            row2.VerticalAlignment = VerticalAlignment.Center;
            row2.Cells[0].AddParagraph("Global Max Point Dose");
            row2.Cells[1].AddParagraph(Math.Round(PTV.GlobalMaxPointDose, 2, MidpointRounding.AwayFromZero) + " cGy.\nThis is " + Math.Round(globalmaxpercentofRX, 2, MidpointRounding.AwayFromZero) + "% of the dose for this target.");

            // third row
            var row3 = table.AddRow();
            row3.VerticalAlignment = VerticalAlignment.Center;
            row3.Cells[0].AddParagraph("CTV V" + data.conventionalstats.CTVRXcoverage + "% RX\nShould be greater than or equal to " + data.conventionalstats.CTVVolcoverage + "%");
            roundedPTVXXRX = Convert.ToString(Math.Round(CTV.CTVXXRX0, 2, MidpointRounding.AwayFromZero));
            row3.Cells[1].AddParagraph(roundedPTVXXRX);
            if (Convert.ToDouble(roundedPTVXXRX) >= data.conventionalstats.CTVVolcoverage)
            {
                row3.Cells[1].Shading.Color = Color.FromRgb(0, 255, 0);
            }
            else
            {
                row3.Cells[1].Shading.Color = Color.FromRgb(255, 0, 0);
            }

            // fourth row
            var row4 = table.AddRow();
            row3.VerticalAlignment = VerticalAlignment.Center;
            row3.Cells[0].AddParagraph("GTV V" + data.conventionalstats.CTVRXcoverage + "% RX\nShould be greater than or equal to " + data.conventionalstats.CTVVolcoverage + "%");
            roundedPTVXXRX = Convert.ToString(Math.Round(GTV.CTVXXRX0, 2, MidpointRounding.AwayFromZero));
            row3.Cells[1].AddParagraph(roundedPTVXXRX);
            if (Convert.ToDouble(roundedPTVXXRX) >= data.conventionalstats.CTVVolcoverage)
            {
                row3.Cells[1].Shading.Color = Color.FromRgb(0, 255, 0);
            }
            else
            {
                row3.Cells[1].Shading.Color = Color.FromRgb(255, 0, 0);
            }

            // fifth row
            var row5 = table.AddRow();
            row5.VerticalAlignment = VerticalAlignment.Center;
            row5.Cells[0].AddParagraph("PTV V" + data.conventionalstats.PTV1RXcoverage + "% RX\nShould be greater than or equal to " + data.conventionalstats.PTV1Volcoverage + "%");
            roundedPTVXXRX = Convert.ToString(Math.Round(PTV.PTVXXRX1, 2, MidpointRounding.AwayFromZero));
            row5.Cells[1].AddParagraph(roundedPTVXXRX);
            //MessageBox.Show("rounedePTVXXRX1: " + roundedPTVXXRX);
            if (Convert.ToDouble(roundedPTVXXRX) >= data.conventionalstats.PTV1Volcoverage)
            {
                row5.Cells[1].Shading.Color = Color.FromRgb(0, 255, 0);
            }
            else
            {
                row5.Cells[1].Shading.Color = Color.FromRgb(255, 0, 0);
            }

            // sixth row
            var row6 = table.AddRow();
            row6.VerticalAlignment = VerticalAlignment.Center;
            row6.Cells[0].AddParagraph("PTV V" + data.conventionalstats.PTV2RXcoverage + "% RX\nShould be greater than or equal to " + data.conventionalstats.PTV2Volcoverage + "%");
            roundedPTVXXRX = Convert.ToString(Math.Round(PTV.PTVXXRX2, 2, MidpointRounding.AwayFromZero));
            row6.Cells[1].AddParagraph(roundedPTVXXRX);

            if (Convert.ToDouble(roundedPTVXXRX) >= data.conventionalstats.PTV2Volcoverage)
            {
                row6.Cells[1].Shading.Color = Color.FromRgb(0, 255, 0);
            }
            else
            {
                row6.Cells[1].Shading.Color = Color.FromRgb(255, 0, 0);
            }

            // seventh row
            var row7 = table.AddRow();
            row7.VerticalAlignment = VerticalAlignment.Center;
            row7.Cells[0].AddParagraph("PTV V" + data.conventionalstats.PTV3RXcoverage + "% RX\nShould be less than or equal to " + data.conventionalstats.PTV3Volcoverage + "%");
            roundedPTVXXRX = Convert.ToString(Math.Round(PTV.PTVXXRX3, 2, MidpointRounding.AwayFromZero));
            row7.Cells[1].AddParagraph(roundedPTVXXRX);

            if (Convert.ToDouble(roundedPTVXXRX) <= data.conventionalstats.PTV3Volcoverage)
            {
                row7.Cells[1].Shading.Color = Color.FromRgb(0, 255, 0);
            }
            else
            {
                row7.Cells[1].Shading.Color = Color.FromRgb(255, 0, 0);
            }
        }


        private static void AddRows(Table table, ReportData data, int count)
        {
            try
            {
                string roundedPTVXXRX = null;

                var row0 = table.AddRow();
                row0.VerticalAlignment = VerticalAlignment.Center;
                row0.Cells[0].AddParagraph("Dose (cGy)");
                row0.Cells[1].AddParagraph(Convert.ToString(data.conventionalstats.TargetStructures[count].Dose));

                // first row
                var row1 = table.AddRow();
                row1.VerticalAlignment = VerticalAlignment.Center;
                row1.Cells[0].AddParagraph("Target Volume (cc)");
                string roundedptvvol = Convert.ToString(Math.Round(data.conventionalstats.TargetStructures[count].vol, 2, MidpointRounding.AwayFromZero));
                row1.Cells[1].AddParagraph(roundedptvvol);

                //Second row. Global Max Point Dose FOR THE PLAN OF THIS TARGET.
                double globalmaxpercentofRX = (data.conventionalstats.TargetStructures[count].GlobalMaxPointDose / data.conventionalstats.TargetStructures[count].Dose) * 100.0;
                var row2 = table.AddRow();
                row2.VerticalAlignment = VerticalAlignment.Center;
                row2.Cells[0].AddParagraph("Global Max Point Dose");
                row2.Cells[1].AddParagraph(Math.Round(data.conventionalstats.TargetStructures[count].GlobalMaxPointDose, 2, MidpointRounding.AwayFromZero) + " cGy.\nThis is " + Math.Round(globalmaxpercentofRX, 2, MidpointRounding.AwayFromZero) + "% of the dose for this target.");


                // third row
                var row3 = table.AddRow();
                row3.VerticalAlignment = VerticalAlignment.Center;
                row3.Cells[0].AddParagraph("CTV/GTV V" + data.conventionalstats.CTVRXcoverage + "% RX\nShould be greater than or equal to " + data.conventionalstats.CTVVolcoverage + "%");
                if (data.conventionalstats.TargetStructures[count].StructureNAME.Contains("PTV"))
                {
                    row3.Cells[1].AddParagraph("CTV/GTV Coverage objective is not applicable to PTV");
                }
                else
                {
                    roundedPTVXXRX = Convert.ToString(Math.Round(data.conventionalstats.TargetStructures[count].CTVXXRX0, 2, MidpointRounding.AwayFromZero));
                    row3.Cells[1].AddParagraph(roundedPTVXXRX);

                    if (Convert.ToDouble(roundedPTVXXRX) >= data.conventionalstats.CTVVolcoverage)
                    {
                        row3.Cells[1].Shading.Color = Color.FromRgb(0, 255, 0);
                    }
                    else
                    {
                        row3.Cells[1].Shading.Color = Color.FromRgb(255, 0, 0);
                    }
                }



                // fouth row
                var row4 = table.AddRow();
                row4.VerticalAlignment = VerticalAlignment.Center;
                row4.Cells[0].AddParagraph("PTV V" + data.conventionalstats.PTV1RXcoverage + "% RX\nShould be greater than or equal to " + data.conventionalstats.PTV1Volcoverage + "%");
                if (data.conventionalstats.TargetStructures[count].StructureNAME.Contains("CTV") || data.conventionalstats.TargetStructures[count].StructureNAME.Contains("GTV"))
                {
                    row4.Cells[1].AddParagraph("PTV Coverage objective is not applicable to CTV/GTV");
                }
                else
                {
                    roundedPTVXXRX = Convert.ToString(Math.Round(data.conventionalstats.TargetStructures[count].PTVXXRX1, 2, MidpointRounding.AwayFromZero));
                    row4.Cells[1].AddParagraph(roundedPTVXXRX);
                    //MessageBox.Show("rounedePTVXXRX1: " + roundedPTVXXRX);

                    if (Convert.ToDouble(roundedPTVXXRX) >= data.conventionalstats.PTV1Volcoverage)
                    {
                        row4.Cells[1].Shading.Color = Color.FromRgb(0, 255, 0);
                    }
                    else
                    {
                        row4.Cells[1].Shading.Color = Color.FromRgb(255, 0, 0);
                    }
                }
 

                // fifth row
                var row5 = table.AddRow();
                row5.VerticalAlignment = VerticalAlignment.Center;
                row5.Cells[0].AddParagraph("PTV V" + data.conventionalstats.PTV2RXcoverage + "% RX\nShould be greater than or equal to " + data.conventionalstats.PTV2Volcoverage + "%");
                if (data.conventionalstats.TargetStructures[count].StructureNAME.Contains("CTV") || data.conventionalstats.TargetStructures[count].StructureNAME.Contains("GTV"))
                {
                    row5.Cells[1].AddParagraph("PTV Coverage objective is not applicable to CTV/GTV");
                }
                else
                {
                    roundedPTVXXRX = Convert.ToString(Math.Round(data.conventionalstats.TargetStructures[count].PTVXXRX2, 2, MidpointRounding.AwayFromZero));
                    row5.Cells[1].AddParagraph(roundedPTVXXRX);

                    if (Convert.ToDouble(roundedPTVXXRX) >= data.conventionalstats.PTV2Volcoverage)
                    {
                        row5.Cells[1].Shading.Color = Color.FromRgb(0, 255, 0);
                    }
                    else
                    {
                        row5.Cells[1].Shading.Color = Color.FromRgb(255, 0, 0);
                    }
                }


                // sixth row
                var row6 = table.AddRow();
                row6.VerticalAlignment = VerticalAlignment.Center;
                row6.Cells[0].AddParagraph("PTV V" + data.conventionalstats.PTV3RXcoverage + "% RX\nShould be less than or equal to " + data.conventionalstats.PTV3Volcoverage + "%");
                if (data.conventionalstats.TargetStructures[count].StructureNAME.Contains("CTV") || data.conventionalstats.TargetStructures[count].StructureNAME.Contains("GTV"))
                {
                    row5.Cells[1].AddParagraph("PTV Coverage objective is not applicable to CTV/GTV");
                }
                else
                {
                    roundedPTVXXRX = Convert.ToString(Math.Round(data.conventionalstats.TargetStructures[count].PTVXXRX3, 2, MidpointRounding.AwayFromZero));
                    row6.Cells[1].AddParagraph(roundedPTVXXRX);

                    if (Convert.ToDouble(roundedPTVXXRX) <= data.conventionalstats.PTV3Volcoverage)
                    {
                        row6.Cells[1].Shading.Color = Color.FromRgb(0, 255, 0);
                    }
                    else
                    {
                        row6.Cells[1].Shading.Color = Color.FromRgb(255, 0, 0);
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("Conventional Stats Add Rows error: " + e.ToString() + "\n\n" + e.StackTrace + "\n\n" + e.Source);
            }

        }


        private void AddLastRowBorder(Table table)
        {
            var lastRow = table.Rows[table.Rows.Count - 1];
            lastRow.Borders.Bottom.Width = 2;
        }



















    }
}
