using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.IO;

using Autodesk.Navisworks.Api;
using Autodesk.Navisworks.Api.Plugins;
using ComBridge = Autodesk.Navisworks.Api.ComApi.ComApiBridge;
using COMApi = Autodesk.Navisworks.Api.Interop.ComApi;



namespace GetPrimitive
{
    #region NW Plugin
    [PluginAttribute("Test", "ADSK", DisplayName = "Test")]
    [AddInPluginAttribute(AddInLocation.AddIn)]
    public class Class1 : AddInPlugin
    {
        public override int Execute(params string[] parameters)
        {
            MessageBox.Show("Hello!");

            //StartForm startForm = new StartForm();
            //startForm.Show();

            // get the current selection
            ModelItemCollection oModelColl =
                Autodesk.Navisworks.Api.Application.
                    ActiveDocument.CurrentSelection.SelectedItems;

            //convert to COM selection
            COMApi.InwOpState oState = ComBridge.State;
            COMApi.InwOpSelection oSel =
                    ComBridge.ToInwOpSelection(oModelColl);

            // create the callback object
            CallbackGeomListener callbkListener =
                    new CallbackGeomListener();

            foreach (COMApi.InwOaPath3 path in oSel.Paths())
            {
                foreach (COMApi.InwOaFragment3 frag in path.Fragments())
                {
                    // generate the primitives
                    frag.GenerateSimplePrimitives(
                        COMApi.nwEVertexProperty.eNORMAL,
                                       callbkListener);

                }
            }

            //MessageBox.Show(callbkListener.points);
            //this.SaveData(callbkListener);
            string originPath = @"D:\temp\origin.txt";
            this.SavaData2txt(originPath, callbkListener.coordinate.ToString());

            //Get all the points and save them into a 2-dimensional list
            List<List<string>> points = GetPoints(originPath);

            StringBuilder preData = new StringBuilder();
            foreach (var line in points)
            {
                preData.Append(line[0] + "," + line[1] + "," + line[2] + "\r\n");
            }
            this.SavaData2txt(@"D:\temp\preData.txt", preData.ToString());

            return 0;
        }

        //save data to a txt file
        public void SavaData2txt(string filePath, string data)
        {
            FileStream fst = new FileStream(filePath, FileMode.OpenOrCreate);
            StreamWriter swt = new StreamWriter(fst);

            swt.Write(data);

            swt.Close();
            fst.Close();
        }

        //public List<List<string>> GetPoints(string filePath)
        public List<List<string>> GetPoints(string filePath)
        {
            //Array<string> points = new Array<string>();
            List<List<string>> Opoints = new List<List<string>>();

            FileStream fst = new FileStream(filePath, FileMode.Open);
            StreamReader srd = new StreamReader(fst);

            string line;
            while ((line = srd.ReadLine()) != null)
            {
                if (line != "")
                {
                    string[] temp = line.Split(',');

                    {
                        for (int i = 0; i < 9; i += 3)
                        {

                            List<string> list = new List<string>();
                            for (int j = i; j < i + 3; j++)
                            {
                                list.Add(temp[j]);
                            }
                            Opoints.Add(list);

                        }
                    }
                }
            }

            fst.Close();
            srd.Close();

            //delete the duplicate points in the list
            //http://blog.sina.com.cn/s/blog_6148930e0100odqx.html
            //
            //

            List<List<string>> Npoints = new List<List<string>>();

            Npoints.Add(Opoints[0]);
            int flag = 0;

            for (int i = 0; i < Opoints.Count; i++)
            {
                flag = 0;

                for (int j = 0; (j < Npoints.Count) && (flag != 1); j++)
                {
                    //if ((points[i][0] != Npoints[j][0]) && (points[i][1] != Npoints[j][1]) && (points[i][2] != Npoints[j][2]))
                    if ((Opoints[i][0] == Npoints[j][0]) && (Opoints[i][1] == Npoints[j][1]) && (Opoints[i][2] == Npoints[j][2]))
                    {
                        //if (!(points[i][1] == Npoints[j][1]))
                        //{
                        //    if (!(points[i][2] == Npoints[j][2]))
                        //    {
                        //        Npoints.Add(points[i]);
                        //        k++;
                        //    }
                        //}
                        flag = 1;
                    }
                }

                if (flag == 0)
                {
                    Npoints.Add(Opoints[i]);
                }
            }
            MessageBox.Show("Triangles: " + (Opoints.Count / 3).ToString() + ", Points: " + Npoints.Count);
            return Npoints;
        }

        #region old SaveData
        public int SaveData(CallbackGeomListener callbkListener)
        {
            string filePath;
            filePath = @"D:\temp\qq.txt";

            ////如果文件txt存在就打开，不存在就新建 .append 是追加写
            //if (File.Exists(path))
            //{

            //}

            FileStream fst = new FileStream(filePath, FileMode.Create);
            //写数据到txt
            StreamWriter swt = new StreamWriter(fst);
            //写入
            swt.WriteLine(callbkListener.coordinate);
            //swt.WriteLine(callbkListener.points.Length.ToString());

            swt.Close();
            fst.Close();

            //Form1 form = new Form1();
            Form1 form = new Form1(filePath);
            form.Show();


            //Get all the points and save them into a 2-dimensional list
            List<List<string>> points = GetPoints(filePath);

            //delete the duplicate points in the list
            //http://blog.sina.com.cn/s/blog_6148930e0100odqx.html

            List<List<string>> Npoints = new List<List<string>>();
            //for (int i = 0; i < points.Count; i++)
            //{
            //    for (int j = i+1; j < points.Count; j++)
            //    {
            //        if (points[i][0] == points[j][0])
            //        {
            //            if (points[i][1] == points[j][1])
            //            {
            //                if (points[i][2] == points[j][2])
            //                {

            //                }
            //            }
            //        }
            //    }
            //}
            Npoints.Add(points[0]);
            int flag = 0;

            for (int i = 0; i < points.Count; i++)
            {
                flag = 0;

                for (int j = 0; (j < Npoints.Count) && (flag != 1); j++)
                {
                    //if ((points[i][0] != Npoints[j][0]) && (points[i][1] != Npoints[j][1]) && (points[i][2] != Npoints[j][2]))
                    if ((points[i][0] == Npoints[j][0]) && (points[i][1] == Npoints[j][1]) && (points[i][2] == Npoints[j][2]))
                    {
                        //if (!(points[i][1] == Npoints[j][1]))
                        //{
                        //    if (!(points[i][2] == Npoints[j][2]))
                        //    {
                        //        Npoints.Add(points[i]);
                        //        k++;
                        //    }
                        //}
                        flag = 1;
                    }
                }

                if (flag == 0)
                {
                    Npoints.Add(points[i]);
                }
            }
            MessageBox.Show("points:" + points.Count.ToString() + "Npoints:" + Npoints.Count);
            return 0;
        }
        #endregion
    }
    #endregion


#region InwSimplePrimitivesCB Class
        public class CallbackGeomListener : COMApi.InwSimplePrimitivesCB
    {
        
        public StringBuilder coordinate = new StringBuilder();
        //public float[,] points = new float[10000, 3];


        public void Line(COMApi.InwSimpleVertex v1,
            COMApi.InwSimpleVertex v2)
        {
            // do your work
        }

        public void Point(COMApi.InwSimpleVertex v1)
        {
            // do your work
            MessageBox.Show("1");
        }

        public void SnapPoint(COMApi.InwSimpleVertex v1)
        {
            // do your work
        }

        //int i = 0;
        public void Triangle(COMApi.InwSimpleVertex v1,
                COMApi.InwSimpleVertex v2,
                COMApi.InwSimpleVertex v3)
        {
            // do your work

            Array a1 = (Array)(object)v1.coord;
            Array a2 = (Array)(object)v2.coord;
            Array a3 = (Array)(object)v3.coord;
            Array color = (Array)(object)v1.color;
            Array normal = (Array)(object)v1.normal;
            

            float X1 = (float)(a1.GetValue(1));
            float Y1 = (float)(a1.GetValue(2));
            float Z1 = (float)(a1.GetValue(3));

            float X2 = (float)(a2.GetValue(1));
            float Y2 = (float)(a2.GetValue(2));
            float Z2 = (float)(a2.GetValue(3));

            float X3 = (float)(a3.GetValue(1));
            float Y3 = (float)(a3.GetValue(2));
            float Z3 = (float)(a3.GetValue(3));
            //MessageBox.Show("4: " + X + ", " + Y + ", " + Z);
            //this.points.Add("4: " + X + ", " + Y + ", " + Z);

            float X4 = (float)(normal.GetValue(1));
            float Y4 = (float)(normal.GetValue(2));
            float Z4 = (float)(normal.GetValue(3));

            coordinate.Append(X1.ToString() + "," + Y1.ToString() + "," + Z1.ToString() + ",");
            coordinate.Append(X2.ToString() + "," + Y2.ToString() + "," + Z2.ToString() + ",");
            coordinate.Append(X3.ToString() + "," + Y3.ToString() + "," + Z3.ToString() + ",");
            coordinate.Append(X4.ToString() + "," + Y4.ToString() + "," + Z4.ToString() + "\r\n");

            //points[i, 0] = X1;
            //points[i, 1] = Y1;
            //points[i, 2] = Z1;
            //i++;
            //points[i, 0] = X2;
            //points[i, 1] = Y2;
            //points[i, 2] = Z2;
            //i++;
            //points[i, 0] = X3;
            //points[i, 1] = Y3;
            //points[i, 2] = Z3;
            //i++;
        }
    }

#endregion


}



 