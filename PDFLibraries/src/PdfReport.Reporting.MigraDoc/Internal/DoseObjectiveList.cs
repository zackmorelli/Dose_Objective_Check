﻿using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.Tables;
using System;
using System.Collections.Generic;


namespace PdfReport.Reporting.MigraDoc.Internal
{
    internal class DoseObjectiveList
    {
        public void Add(Section section, List<ROI.ROI> PROI, Plan plan, Patient patient)
        {
           // AddHeading(section, PROI);
            
            AddDoseObjectiveList(section, PROI, plan, patient);
        }


      /*
        private void AddHeading(Section section, List<ROI.ROI> PROI)
        {
            section.AddParagraph(PROI.Id, StyleNames.Heading1);
            section.AddParagraph($"Image {PROI.Image.Id} " +
                                 $"taken {PROI.Image.CreationTime:g}");
        }
    */


        private void AddDoseObjectiveList(Section section, List<ROI.ROI> PROI, Plan plan, Patient patient)
        {
           // string str = "DOSE OBJECTIVE REPORT FOR ,'S PLAN";

          //  str.Insert(26, patient.LastName);
          //  str.Insert(28, patient.FirstName);
          //  str.Insert(36, plan.Id);

            AddTableTitle(section, "DOSE OBJECTIVE REPORT - (Total Prescribed Dose: " + plan.TotalPrescribedDose.ToString() + " cGy )");
            AddDoseListTable(section, PROI);
        }


        private void AddTableTitle(Section section, string title)
        {
            var p = section.AddParagraph(title, StyleNames.Heading2);
            p.Format.KeepWithNext = true;
        }

        private void AddDoseListTable(Section section, List<ROI.ROI> PROI)
        {
            var table = section.AddTable();

            FormatTable(table);
            AddColumnsAndHeaders(table);
            AddStructureRows(table, PROI);

            AddLastRowBorder(table);
            AlternateRowShading(table);
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
            table.AddColumn(width * 0.34);
            table.AddColumn(width * 0.13);
            table.AddColumn(width * 0.13);
            table.AddColumn(width * 0.13);
            table.AddColumn(width * 0.13);
            table.AddColumn(width * 0.13);

            var headerRow = table.AddRow();
            headerRow.Borders.Bottom.Width = 1;

            AddHeader(headerRow.Cells[0], "Name");
            AddHeader(headerRow.Cells[1], "Hard Dose Limit (cGy)");
            AddHeader(headerRow.Cells[2], "Goal Dose Limit(cGy)");
            AddHeader(headerRow.Cells[3], "Eclipse Estimated Dose (cGy)");
            AddHeader(headerRow.Cells[4], "Status");
            AddHeader(headerRow.Cells[5], "Structure Volume [cc]");
          
        }

        private void AddHeader(Cell cell, string header)
        {
            var p = cell.AddParagraph(header);
            p.Style = CustomStyles.ColumnHeader;
        }

        private void AddStructureRows(Table table, List<ROI.ROI> PROI)
        {
            foreach(ROI.ROI aroi in PROI)
            {
                var row = table.AddRow();
                row.VerticalAlignment = VerticalAlignment.Center;

                if (aroi.actvol > 0.5)
                {

                    if (aroi.strict == "[record]")
                    {

                        row.Cells[0].AddParagraph(aroi.ROIName);
                        row.Cells[1].AddParagraph("NA");
                        row.Cells[2].AddParagraph("NA");
                        row.Cells[3].AddParagraph(Math.Round(aroi.actvol, 2, MidpointRounding.AwayFromZero).ToString() + " cc");
                        row.Cells[4].AddParagraph(aroi.status);
                        row.Cells[5].AddParagraph(Math.Round(aroi.structvol, 2, MidpointRounding.AwayFromZero).ToString());

                    }
                    else
                    {

                        row.Cells[0].AddParagraph(aroi.ROIName);
                        row.Cells[1].AddParagraph(Math.Round(aroi.limvol, 2, MidpointRounding.AwayFromZero).ToString() + " cc");
                        row.Cells[2].AddParagraph(Math.Round(aroi.goalvol, 2, MidpointRounding.AwayFromZero).ToString() + " cc");
                        row.Cells[3].AddParagraph(Math.Round(aroi.actvol, 2, MidpointRounding.AwayFromZero).ToString() + " cc");                  
                        row.Cells[4].AddParagraph(aroi.status);
                        row.Cells[5].AddParagraph(Math.Round(aroi.structvol, 2, MidpointRounding.AwayFromZero).ToString());

                    }
                }
                else if (aroi.strict == "[record]")
                {

                    row.Cells[0].AddParagraph(aroi.ROIName);
                    row.Cells[1].AddParagraph("NA");
                    row.Cells[2].AddParagraph(aroi.goal);
                    row.Cells[3].AddParagraph(Math.Round(aroi.actdose, 0, MidpointRounding.AwayFromZero).ToString());
                    row.Cells[4].AddParagraph(aroi.status);
                    row.Cells[5].AddParagraph(Math.Round(aroi.structvol, 2, MidpointRounding.AwayFromZero).ToString());

                }
                else
                {

                    row.Cells[0].AddParagraph(aroi.ROIName);
                    row.Cells[1].AddParagraph(aroi.limdose.ToString());
                    row.Cells[2].AddParagraph(aroi.goal);
                    row.Cells[3].AddParagraph(Math.Round(aroi.actdose, 0, MidpointRounding.AwayFromZero).ToString());
                    row.Cells[4].AddParagraph(aroi.status);
                    row.Cells[5].AddParagraph(Math.Round(aroi.structvol, 2, MidpointRounding.AwayFromZero).ToString());

                }

            }
        }


        private void AddLastRowBorder(Table table)
        {
            var lastRow = table.Rows[table.Rows.Count - 1];
            lastRow.Borders.Bottom.Width = 2;
        }

        private void AlternateRowShading(Table table)
        {
            // Start at i = 1 to skip column headers
            for (var i = 1; i < table.Rows.Count; i++)
            {
                if (i % 2 == 0)  // Even rows
                {
                    table.Rows[i].Shading.Color = Color.FromRgb(216, 216, 216);
                }
            }
        }
    }
}