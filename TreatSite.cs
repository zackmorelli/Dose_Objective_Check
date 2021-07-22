using System;
using System.Collections.Generic;


/*
    Dose Objective Check - Treat Site Class
    
    Description:
    This is an internal helper class of the Dose Objective Check program. This defines the TreatSite class, which is used to manage the various treatment sites the program uses.

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


    Release 3.2 - 6/8/2021

*/



namespace DoseObjectiveCheck
{
    //treatment site class used to make a list of treatment sites
    //the treatment site list is used when the user is prompted to choose the treatment site of the selected Plan
    public class TreatSite : IEquatable<TreatSite>       
    {                                                     
        public string Name { get; set; }

        public string DisplayName { get; set; }

        public int Id { get; set; }

        public override string ToString()
        {
            return DisplayName;
        }
        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            TreatSite objAsTreatSite = obj as TreatSite;
            if (objAsTreatSite == null) return false;
            else return Equals(objAsTreatSite);
        }
        public override int GetHashCode()
        {
            return Id;
        }
        public bool Equals(TreatSite other)
        {
            if (other == null) return false;
            return (this.Id.Equals(other.Id));
        }

        //treatment site list for Conventional plans
        public static List<TreatSite> MakelistConv()           
        {
            List<TreatSite> treatsite = new List<TreatSite>();

            treatsite.Add(new TreatSite() { DisplayName = "Abdomen", Name = "Abdomen", Id = 1 });
            treatsite.Add(new TreatSite() { DisplayName = "Brain", Name = "Brain", Id = 2 });
            treatsite.Add(new TreatSite() { DisplayName = "Brain Hypofx", Name = "BrainHypofx", Id = 3 });
            treatsite.Add(new TreatSite() { DisplayName = "Breast 23+fx", Name = "Breast23+fx", Id = 4 });
            treatsite.Add(new TreatSite() { DisplayName = "Breast Hypofx", Name = "BreastHypofx", Id = 5 });
            treatsite.Add(new TreatSite() { DisplayName = "Breast+regional_LN 23+fx", Name = "Breast+regional_LN 23+fx", Id = 6 });
            treatsite.Add(new TreatSite() { DisplayName = "Breast+regional_LN Hypofx", Name = "Breast+regional_LN Hypofx", Id = 7 });
            treatsite.Add(new TreatSite() { DisplayName = "Hypofx 5fx", Name = "Hypofx 5fx", Id = 8 });
            treatsite.Add(new TreatSite() { DisplayName = "Hypofx 10fx", Name = "Hypofx 10fx", Id = 9 });
            treatsite.Add(new TreatSite() { DisplayName = "Hypofx 15fx", Name = "Hypofx 15fx", Id = 10 });
            treatsite.Add(new TreatSite() { DisplayName = "Esophagus", Name = "Esophagus", Id = 11 });
            treatsite.Add(new TreatSite() { DisplayName = "Pelvis - GYN", Name = "Pelvis - GYN", Id = 12 });
            treatsite.Add(new TreatSite() { DisplayName = "Head & Neck", Name = "Head&Neck", Id = 13 });
            treatsite.Add(new TreatSite() { DisplayName = "Lung", Name = "Lung", Id = 14 });
            treatsite.Add(new TreatSite() { DisplayName = "Lung Hypofx", Name = "LungHypofx", Id = 15 });
            treatsite.Add(new TreatSite() { DisplayName = "Pelvis - Rectal", Name = "Pelvis - Rectal", Id = 16 });
            treatsite.Add(new TreatSite() { DisplayName = "Pelvis - Anal", Name = "Pelvis - Anal", Id = 17 });
            treatsite.Add(new TreatSite() { DisplayName = "Pelvis (Other)", Name = "Pelvis(Other)", Id = 18 });
            treatsite.Add(new TreatSite() { DisplayName = "Pelvis EBRT 25fx + HDR", Name = "PelivsEBRT25fx+HDR", Id = 19 });
            treatsite.Add(new TreatSite() { DisplayName = "Pelvis EBRT 15fx + HDR", Name = "PelivsEBRT15fx+HDR", Id = 20 });
            treatsite.Add(new TreatSite() { DisplayName = "Prostate", Name = "Prostate", Id = 21 });
            treatsite.Add(new TreatSite() { DisplayName = "Prostate Bed", Name = "ProstateBed", Id = 22 });
            treatsite.Add(new TreatSite() { DisplayName = "Prostate Hypo 20fx", Name = "ProstateHypo20fx", Id = 23 });
            treatsite.Add(new TreatSite() { DisplayName = "Prostate Hypo 28fx", Name = "ProstateHypo28fx", Id = 24 });
            treatsite.Add(new TreatSite() { DisplayName = "Thorax (Other)", Name = "Thorax(Other)", Id = 25 });
            treatsite.Add(new TreatSite() { DisplayName = "Sarcoma", Name = "Sarcoma", Id = 26 });
            treatsite.Add(new TreatSite() { DisplayName = "Prostate NRG Prot Arm 1", Name = "ProstateNRGProtArm1", Id = 27 });

            return treatsite;
        }

        //treatment site list for SRS plans
        public static List<TreatSite> MakelistSRS()           
        {
            List<TreatSite> treatsite = new List<TreatSite>();

            treatsite.Add(new TreatSite() { DisplayName = "Single fraction", Name = "Singlefraction", Id = 1 });
            treatsite.Add(new TreatSite() { DisplayName = "3 fraction", Name = "3fraction", Id = 2 });
            treatsite.Add(new TreatSite() { DisplayName = "4 fraction", Name = "4fraction", Id = 3 });
            treatsite.Add(new TreatSite() { DisplayName = "5 fraction", Name = "5fraction", Id = 4 });
            treatsite.Add(new TreatSite() { DisplayName = "6 fraction", Name = "6fraction", Id = 5 });
            treatsite.Add(new TreatSite() { DisplayName = "8 fraction", Name = "8fraction", Id = 6 });
            treatsite.Add(new TreatSite() { DisplayName = "10 fraction", Name = "10fraction", Id = 7 });
            treatsite.Add(new TreatSite() { DisplayName = "Liver", Name = "Liver", Id = 8 });
            treatsite.Add(new TreatSite() { DisplayName = "Lung 4 fraction", Name = "Lung4fraction", Id = 9 });
            treatsite.Add(new TreatSite() { DisplayName = "Lung 5 fraction", Name = "Lung5fraction", Id = 10 });
            treatsite.Add(new TreatSite() { DisplayName = "Lung 8 fraction", Name = "Lung8fraction", Id = 11 });
            treatsite.Add(new TreatSite() { DisplayName = "Oligomets 1 fraction", Name = "Oligomets1fraction", Id = 12 });
            treatsite.Add(new TreatSite() { DisplayName = "Oligomets 3 fractions", Name = "Oligomets3fractions", Id = 13 });
            treatsite.Add(new TreatSite() { DisplayName = "Oligomets 5 fractions", Name = "Oligomets5fractions", Id = 14 });
            treatsite.Add(new TreatSite() { DisplayName = "Pancreas", Name = "Pancreas", Id = 15 });
            treatsite.Add(new TreatSite() { DisplayName = "SRS Cranial 1 fraction", Name = "SRSCranial1fraction", Id = 16 });
            treatsite.Add(new TreatSite() { DisplayName = "SRS Cranial 3 fraction", Name = "SRSCranial3fraction", Id = 17 });
            treatsite.Add(new TreatSite() { DisplayName = "SRS Cranial 5 fraction", Name = "SRSCranial5fraction", Id = 18 });
            treatsite.Add(new TreatSite() { DisplayName = "SRS Cranial AVM", Name = "SRSCranialAVM", Id = 19 });
            treatsite.Add(new TreatSite() { DisplayName = "SRS Cranial Trigeminal Neuralgia", Name = "SRSCranialTrigeminalNeuralgia", Id = 20 });
            treatsite.Add(new TreatSite() { DisplayName = "Prostate NRG Prot Arm 2", Name = "ProstateNRGProtArm2", Id = 21 });

            return treatsite;
        }

        //treatment site list for Both
        public static List<TreatSite> MakelistBoth()           
        {
            List<TreatSite> treatsite = new List<TreatSite>();

            treatsite.Add(new TreatSite() { DisplayName = "Abdomen", Name = "Abdomen", Id = 1 });
            treatsite.Add(new TreatSite() { DisplayName = "Brain", Name = "Brain", Id = 2 });
            treatsite.Add(new TreatSite() { DisplayName = "Brain Hypofx", Name = "BrainHypofx", Id = 3 });
            treatsite.Add(new TreatSite() { DisplayName = "Breast 23+fx", Name = "Breast23+fx", Id = 4 });
            treatsite.Add(new TreatSite() { DisplayName = "Breast Hypofx", Name = "BreastHypofx", Id = 5 });
            treatsite.Add(new TreatSite() { DisplayName = "Breast+regional_LN 23+fx", Name = "Breast+regional_LN 23+fx", Id = 6 });
            treatsite.Add(new TreatSite() { DisplayName = "Breast+regional_LN Hypofx", Name = "Breast+regional_LN Hypofx", Id = 7 });
            treatsite.Add(new TreatSite() { DisplayName = "Hypofx 5fx", Name = "Hypofx 5fx", Id = 8 });
            treatsite.Add(new TreatSite() { DisplayName = "Hypofx 10fx", Name = "Hypofx 10fx", Id = 9 });
            treatsite.Add(new TreatSite() { DisplayName = "Hypofx 15fx", Name = "Hypofx 15fx", Id = 10 });
            treatsite.Add(new TreatSite() { DisplayName = "Esophagus", Name = "Esophagus", Id = 11 });
            treatsite.Add(new TreatSite() { DisplayName = "Pelvis - GYN", Name = "Pelvis - GYN", Id = 12 });
            treatsite.Add(new TreatSite() { DisplayName = "Head & Neck", Name = "Head&Neck", Id = 13 });
            treatsite.Add(new TreatSite() { DisplayName = "Lung", Name = "Lung", Id = 14 });
            treatsite.Add(new TreatSite() { DisplayName = "Lung Hypofx", Name = "LungHypofx", Id = 15 });
            treatsite.Add(new TreatSite() { DisplayName = "Pelvis - Rectal", Name = "Pelvis - Rectal", Id = 16 });
            treatsite.Add(new TreatSite() { DisplayName = "Pelvis - Anal", Name = "Pelvis - Anal", Id = 17 });
            treatsite.Add(new TreatSite() { DisplayName = "Pelvis (Other)", Name = "Pelvis(Other)", Id = 18 });
            treatsite.Add(new TreatSite() { DisplayName = "Pelvis EBRT 25fx + HDR", Name = "PelivsEBRT25fx+HDR", Id = 19 });
            treatsite.Add(new TreatSite() { DisplayName = "Pelvis EBRT 15fx + HDR", Name = "PelivsEBRT15fx+HDR", Id = 20 });
            treatsite.Add(new TreatSite() { DisplayName = "Prostate", Name = "Prostate", Id = 21 });
            treatsite.Add(new TreatSite() { DisplayName = "Prostate Bed", Name = "ProstateBed", Id = 22 });
            treatsite.Add(new TreatSite() { DisplayName = "Prostate Hypo 20fx", Name = "ProstateHypo20fx", Id = 23 });
            treatsite.Add(new TreatSite() { DisplayName = "Prostate Hypo 28fx", Name = "ProstateHypo28fx", Id = 24 });
            treatsite.Add(new TreatSite() { DisplayName = "Thorax (Other)", Name = "Thorax(Other)", Id = 25 });
            treatsite.Add(new TreatSite() { DisplayName = "Sarcoma", Name = "Sarcoma", Id = 26 });
            treatsite.Add(new TreatSite() { DisplayName = "Prostate NRG Prot Arm 1", Name = "ProstateNRGProtArm1", Id = 27 });
            //===============================================================================================================================
            treatsite.Add(new TreatSite() { DisplayName = "Single fraction", Name = "Singlefraction", Id = 28 });
            treatsite.Add(new TreatSite() { DisplayName = "3 fraction", Name = "3fraction", Id = 29 });
            treatsite.Add(new TreatSite() { DisplayName = "4 fraction", Name = "4fraction", Id = 30 });
            treatsite.Add(new TreatSite() { DisplayName = "5 fraction", Name = "5fraction", Id = 31 });
            treatsite.Add(new TreatSite() { DisplayName = "6 fraction", Name = "6fraction", Id = 32 });
            treatsite.Add(new TreatSite() { DisplayName = "8 fraction", Name = "8fraction", Id = 33 });
            treatsite.Add(new TreatSite() { DisplayName = "10 fraction", Name = "10fraction", Id = 34 });
            treatsite.Add(new TreatSite() { DisplayName = "Liver", Name = "Liver", Id = 35 });
            treatsite.Add(new TreatSite() { DisplayName = "Lung 4 fraction", Name = "Lung4fraction", Id = 36 });
            treatsite.Add(new TreatSite() { DisplayName = "Lung 5 fraction", Name = "Lung5fraction", Id = 37 });
            treatsite.Add(new TreatSite() { DisplayName = "Lung 8 fraction", Name = "Lung8fraction", Id = 38 });
            treatsite.Add(new TreatSite() { DisplayName = "Oligomets 1 fraction", Name = "Oligomets1fraction", Id = 39 });
            treatsite.Add(new TreatSite() { DisplayName = "Oligomets 3 fractions", Name = "Oligomets3fractions", Id = 40 });
            treatsite.Add(new TreatSite() { DisplayName = "Oligomets 5 fractions", Name = "Oligomets5fractions", Id = 41 });
            treatsite.Add(new TreatSite() { DisplayName = "Pancreas", Name = "Pancreas", Id = 42 });
            treatsite.Add(new TreatSite() { DisplayName = "SRS Cranial 1 fraction", Name = "SRSCranial1fraction", Id = 43 });
            treatsite.Add(new TreatSite() { DisplayName = "SRS Cranial 3 fraction", Name = "SRSCranial3fraction", Id = 44 });
            treatsite.Add(new TreatSite() { DisplayName = "SRS Cranial 5 fraction", Name = "SRSCranial5fraction", Id = 45 });
            treatsite.Add(new TreatSite() { DisplayName = "SRS Cranial AVM", Name = "SRSCranialAVM", Id = 46 });
            treatsite.Add(new TreatSite() { DisplayName = "SRS Cranial Trigeminal Neuralgia", Name = "SRSCranialTrigeminalNeuralgia", Id = 47 });
            treatsite.Add(new TreatSite() { DisplayName = "Prostate NRG Prot Arm 2", Name = "ProstateNRGProtArm2", Id = 48 });

            return treatsite;
        }

    }



}



