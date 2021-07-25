using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.Tables;
using System.Windows.Forms;


namespace DoseObjectiveCheck
{
    class SRSstats
    {
        public void Add(Section section, ReportData data)
        {
            //section.AddPageBreak();
            var k = section.AddParagraph("SRS/SBRT Dose Coverage Statistics", StyleNames.Heading1);
            DoseObjective BodyMaxDose = data.PROI.Where(ob => ob.DoseObjectiveName.Equals("Body Max Point Dose (Global Max)")).First();
            double maxcoursedose = data.srsstats.Targets.Max(pt => pt.Dose);  // awesome use of LINQ
            double percentofhighestdose = (BodyMaxDose.actdose / maxcoursedose) * 100.0;
            var k1 = section.AddParagraph("Each target (PTV) will be shown in its own section.\nGlobal Max Point Dose: " + Math.Round(BodyMaxDose.actdose, 2, MidpointRounding.AwayFromZero) + " cGy. This is " + Math.Round(percentofhighestdose, 2, MidpointRounding.AwayFromZero) + "% of the highest target dose in this plan.");
            k.Format.KeepWithNext = true;
            k1.Format.KeepWithNext = true;
            k.Format.Font.Underline = Underline.Single;

            int count = 0;
            foreach (SRSTargetstats targetstat in data.srsstats.Targets)
            {
                if(targetstat.DistanceWarning == false)
                {
                    AddTableTitle(section, targetstat.TargetNAME);
                }
                else if(targetstat.DistanceWarning == true)
                {
                    AddTableTitle(section, (targetstat.TargetNAME + " Warning: This target is close to other targets, so conformity and dose fall-off values are approximate"));
                }
                
                AddDoseListTable(section, data, count);
                count++;
            }
        }

        private void AddTableTitle(Section section, string title)
        {
            var p = section.AddParagraph(title, StyleNames.Heading2);
            p.Format.KeepWithNext = true;
        }

        private void AddDoseListTable(Section section, ReportData data, int count)
        {
            var table = section.AddTable();

            FormatTable(table);
            AddColumnsAndHeaders(table);
            AddRows(table, data, count);
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

        private static void AddRows (Table table, ReportData data, int count)
        {
            try
            {
                var row0 = table.AddRow();
                row0.VerticalAlignment = VerticalAlignment.Center;
                row0.Cells[0].AddParagraph("Dose (cGy)");
                row0.Cells[1].AddParagraph(Convert.ToString(data.srsstats.Targets[count].Dose));

                var row1 = table.AddRow();
                row1.VerticalAlignment = VerticalAlignment.Center;
                row1.Cells[0].AddParagraph("PTV Volume (cc)");
                string roundedptvvol = Convert.ToString(Math.Round(data.srsstats.Targets[count].Targetvol, 2, MidpointRounding.AwayFromZero));
                row1.Cells[1].AddParagraph(roundedptvvol);

                // second row
                var row2 = table.AddRow();
                row2.VerticalAlignment = VerticalAlignment.Center;
                row2.Cells[0].AddParagraph("Body V50% (cc)");
                string roundedB50 = Convert.ToString(Math.Round(data.srsstats.Targets[count].BodyV50, 2, MidpointRounding.AwayFromZero));
                row2.Cells[1].AddParagraph(roundedB50);

                // third row
                var row3 = table.AddRow();
                row3.VerticalAlignment = VerticalAlignment.Center;
                row3.Cells[0].AddParagraph("Body V100% (cc)");
                string roundedB100 = Convert.ToString(Math.Round(data.srsstats.Targets[count].BodyV100, 2, MidpointRounding.AwayFromZero));
                row3.Cells[1].AddParagraph(roundedB100);

                // fourth row
                var row4 = table.AddRow();
                row4.VerticalAlignment = VerticalAlignment.Center;
                row4.Cells[0].AddParagraph("PTV V" + data.srsstats.CoverageReqRX + "% RX\nShould be greater than or equal to " + data.srsstats.CoverageReqVol + "%");
                string roundedPTVXXRX = Convert.ToString(Math.Round(data.srsstats.Targets[count].PTVXXRX, 2, MidpointRounding.AwayFromZero));
                row4.Cells[1].AddParagraph(roundedPTVXXRX);

                if (Convert.ToDouble(roundedPTVXXRX) >= data.srsstats.CoverageReqVol)
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
                row5.Cells[0].AddParagraph("Conformity Index\nIdeally less than or equal to 1.2\nLimit is 1.5");
                double CI = (data.srsstats.Targets[count].BodyV100) / (data.srsstats.Targets[count].Targetvol);
                double roundedCI = Math.Round(CI, 2, MidpointRounding.AwayFromZero);
                row5.Cells[1].AddParagraph(Convert.ToString(roundedCI));

                if (CI <= 1.2)
                {
                    row5.Cells[1].Shading.Color = Color.FromRgb(0, 255, 0);
                }
                else if(CI <= 1.5)
                {
                    row5.Cells[1].Shading.Color = Color.FromRgb(255, 255, 0);
                }
                else if(CI > 1.5)
                {
                    row5.Cells[1].Shading.Color = Color.FromRgb(255, 0, 0);
                }

                // sixth row
                var row6 = table.AddRow();
                row6.VerticalAlignment = VerticalAlignment.Center;
                row6.Cells[0].AddParagraph("Gradient Index");
                double GI = (data.srsstats.Targets[count].BodyV50) / (data.srsstats.Targets[count].BodyV100);
                double roundedGI = Math.Round(GI, 2, MidpointRounding.AwayFromZero);
                row6.Cells[1].AddParagraph(Convert.ToString(roundedGI));

                // seventh row

                // this is a function modeling the R50% limit, which is dependent on PTV volume
                double R50limitextreme = -0.743 * Math.Log(data.srsstats.Targets[count].Targetvol) + 7.7335;   //extreme limit
                double R50limit = -0.668 * Math.Log(data.srsstats.Targets[count].Targetvol) + 6.4269;    //ideal limit
                var row7 = table.AddRow();
                row7.VerticalAlignment = VerticalAlignment.Center;
                row7.Cells[0].AddParagraph("R50%\nIdeally less than or equal to " + Math.Round(R50limit, 2, MidpointRounding.AwayFromZero) + ".\nLimit is " + Math.Round(R50limitextreme, 2, MidpointRounding.AwayFromZero) + ".");
                double R50 = (data.srsstats.Targets[count].BodyV50) / (data.srsstats.Targets[count].Targetvol);
                double roundedR50 = Math.Round(R50, 2, MidpointRounding.AwayFromZero);
                row7.Cells[1].AddParagraph(Convert.ToString(roundedR50));

                if (Convert.ToDouble(roundedR50) <= R50limit)
                {
                    row7.Cells[1].Shading.Color = Color.FromRgb(0, 255, 0);
                }
                else if (Convert.ToDouble(roundedR50) <= R50limitextreme)
                {
                    row7.Cells[1].Shading.Color = Color.FromRgb(255, 255, 0);
                }
                else if (Convert.ToDouble(roundedR50) > R50limitextreme)
                {
                    row7.Cells[1].Shading.Color = Color.FromRgb(255, 0, 0);
                }


                // 8th row. First, need to check if the Body-PTV_20 structure actually exists 
                if (data.srsstats.Targets[count].BodyPTV_20_Dose == -5000)
                {
                    var row8 = table.AddRow();
                    row8.VerticalAlignment = VerticalAlignment.Center;
                    row8.Cells[0].AddParagraph("Body-PTV_20");
                    row8.Cells[1].AddParagraph("Not present in this plan.");
                }
                else
                {
                    double BodyPTV_20normper = -5000;
                    double BodyPTV_20extper = -5000;
                    double BodyPTV_20Actualnorm = -5000;
                    double BodyPTV_20Actualext = -5000;
                    if (data.srsstats.Targets[count].Targetvol <= 13.2)
                    {
                        // the normal limit is 50% and the extreme is 58%
                        BodyPTV_20normper = 0.50;
                        BodyPTV_20extper = 0.58;
                    }
                    else
                    {
                        // this is a function modeling the Body-PTV_20, max dose to 0.03cc limit, which is dependent on PTV volume
                        BodyPTV_20normper = 0.317 * Math.Pow(data.srsstats.Targets[count].Targetvol, 0.1728);   //ideal limit
                        BodyPTV_20extper = 0.3371 * Math.Pow(data.srsstats.Targets[count].Targetvol, 0.2079);   //extreme limit
                    }

                    BodyPTV_20Actualnorm = data.srsstats.Targets[count].Dose * BodyPTV_20normper;
                    BodyPTV_20Actualext = data.srsstats.Targets[count].Dose * BodyPTV_20extper;

                    var row8 = table.AddRow();
                    row8.VerticalAlignment = VerticalAlignment.Center;
                    row8.Cells[0].AddParagraph("Body-PTV_20, D2cc\nIdeally less than or equal to " + Math.Round(BodyPTV_20Actualnorm, 2, MidpointRounding.AwayFromZero) + " cGy.\nLimit is " + Math.Round(BodyPTV_20Actualext, 2, MidpointRounding.AwayFromZero) + ".");
                    double roundedBodyPTV20 = Math.Round(data.srsstats.Targets[count].BodyPTV_20_Dose, 2, MidpointRounding.AwayFromZero);
                    row8.Cells[1].AddParagraph(Convert.ToString(roundedBodyPTV20));

                    if (Convert.ToDouble(roundedBodyPTV20) <= BodyPTV_20Actualnorm)
                    {
                        row8.Cells[1].Shading.Color = Color.FromRgb(0, 255, 0);
                    }
                    else if (Convert.ToDouble(roundedBodyPTV20) <= BodyPTV_20Actualext)
                    {
                        row8.Cells[1].Shading.Color = Color.FromRgb(255, 255, 0);
                    }
                    else if (Convert.ToDouble(roundedBodyPTV20) > BodyPTV_20Actualext)
                    {
                        row8.Cells[1].Shading.Color = Color.FromRgb(255, 0, 0);
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("SRS Stats Add Rows error: " + e.ToString() + "\n\n" + e.StackTrace + "\n\n" + e.Source);
            }
        }

        private void AddLastRowBorder(Table table)
        {
            var lastRow = table.Rows[table.Rows.Count - 1];
            lastRow.Borders.Bottom.Width = 2;
        }












    }






}
