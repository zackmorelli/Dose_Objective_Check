using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media.Media3D;
using System.Text;
using System.Threading.Tasks;
using g3;

using VMS.TPS.Common.Model.API;
using VMS.TPS.Common.Model.Types;

namespace DoseObjectiveCheck
{
    static class SRSTargetOverlapCheck
    {

        //So this figures out if SRS Targets are too close to each other for normal dosimetric analysis
        // The user is alerted with a pop-up and a warning is included in the PDF
        public static string DistanceCheck(List<Structure> targlist)
        {
            string output = null;
            List<string> targnames = new List<string>();
            DMesh3 TargMesh = new DMesh3();
            List<DMesh3> meshlist = new List<DMesh3>();
            List<Index3i> ptl = new List<Index3i>();
            Index3i pt = new Index3i();
            int ptmcount = 0;
            int rem = 0;

            double XP;
            double YP;
            double ZP;
            Vector3d Vect;
            List<Vector3d> VectList = new List<Vector3d>();

            foreach (Structure S in targlist)
            {
                ptl.Clear();
                VectList.Clear();
                ptmcount = 0;
                rem = 0;
                MeshGeometry3D MSMesh = S.MeshGeometry;

                foreach (Point3D p in MSMesh.Positions)
                {
                    XP = p.X;
                    YP = p.Y;
                    ZP = p.Z;
                    Vect = new Vector3d(XP, YP, ZP);
                    VectList.Add(Vect);
                }

                foreach (int ptm in MSMesh.TriangleIndices)
                {
                    ptmcount++;
                    Math.DivRem(ptmcount, 3, out rem);

                    if (rem == 2)
                    {
                        pt.a = ptm;
                    }
                    else if (rem == 1)
                    {
                        pt.b = ptm;
                    }
                    else if (rem == 0)
                    {
                        pt.c = ptm;
                        ptl.Add(pt);
                    }
                }

                TargMesh = new DMesh3(MeshComponents.VertexNormals);
                for (int i = 0; i < VectList.Count; i++)
                {
                    TargMesh.AppendVertex(new NewVertexInfo(VectList[i]));
                }

                for (int i = 0; i < ptl.Count; i++)
                {
                    TargMesh.AppendTriangle(ptl[i]);
                }

                IOWriteResult result51 = StandardMeshWriter.WriteFile(@"C:\Users\ztm00\Desktop\STL Files\CollisionCheck\Temp\SRS_Target " + S.Id + ".stl", new List<WriteMesh>() { new WriteMesh(TargMesh) }, WriteOptions.Defaults);

                DMesh3 TargMeshCopy = new DMesh3(TargMesh);

                Remesher CY = new Remesher(TargMesh);
                MeshConstraintUtil.PreserveBoundaryLoops(CY);
                CY.PreventNormalFlips = true;
                CY.SetTargetEdgeLength(2.0);
                CY.SmoothSpeedT = 0.7;
                CY.SetProjectionTarget(MeshProjectionTarget.Auto(TargMeshCopy));
                CY.ProjectionMode = Remesher.TargetProjectionMode.Inline;

                for (int k = 0; k < 8; k++)
                {
                    CY.BasicRemeshPass();
                }

                gs.MeshAutoRepair repair = new gs.MeshAutoRepair(TargMesh);
                repair.RemoveMode = gs.MeshAutoRepair.RemoveModes.None;
                repair.Apply();

                meshlist.Add(TargMesh);
                targnames.Add(S.Id);
                IOWriteResult result56 = StandardMeshWriter.WriteFile(@"C:\Users\ztm00\Desktop\STL Files\CollisionCheck\Temp\SRS_Target " + S.Id + "_Remeshed+repaired.stl", new List<WriteMesh>() { new WriteMesh(TargMesh) }, WriteOptions.Defaults);
            }

            DMeshAABBTree3 JSpatial;
            DMeshAABBTree3 KSpatial;
            Index2i nearesttris = new Index2i();
            double intersectdistance = 0.0;
            DistTriangle3Triangle3 TriDist;
            double distance = 5000;

            for (int J = 0; J < meshlist.Count; J++)
            {
                JSpatial = new DMeshAABBTree3(meshlist[J]);
                JSpatial.Build();
                for (int K = 0; K < meshlist.Count; K++)
                {
                    if(J == K)
                    {
                        continue;
                    }
                    KSpatial = new DMeshAABBTree3(meshlist[K]);
                    KSpatial.Build();
                    nearesttris = JSpatial.FindNearestTriangles(KSpatial, null, out intersectdistance);
                    System.Windows.Forms.MessageBox.Show("intersectdistance: " + intersectdistance);
                    if (nearesttris.a == DMesh3.InvalidID || nearesttris.b == DMesh3.InvalidID)
                    {
                        System.Windows.Forms.MessageBox.Show("Invalid triangle indices");
                    }
                    else
                    {
                        intersectdistance = Math.Round(intersectdistance, 2, MidpointRounding.AwayFromZero);
                        if (intersectdistance <= 50.0)
                        {
                            output += "The target structures " + targnames[J] + " and " + targnames[K] + " are only " + intersectdistance + " mm away from each other.\nUse caution when analyzing dose objectives for these targets.\n\n";
                        }
                    }
                }
            }


            return output;
        }


    }
}
