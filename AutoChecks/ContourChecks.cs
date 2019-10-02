using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Reflection;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using VMS.TPS.Common.Model.API;
using VMS.TPS.Common.Model.Types;

namespace AutoChecks
{
    public class CountourChecks
    {

        public static void ContoursInBody(StructureSet structureSet)
        {

            foreach(Structure astruct in structureSet.Structures)
            {
                MessageBox.Show(astruct.Id + " number of separate parts: " + astruct.GetNumberOfSeparateParts());
                if (astruct.GetNumberOfSeparateParts() > 1)
                {

                    MessageBox.Show( astruct.Id + " is not contiguous!");


                }
                



            }












        }















    }
}
