using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;

namespace ConsoleApplication1
{

    class Coordinate
    {
        public int x;//坐标x
        public int y;//坐标y
    }

    class Program
    {
        static void Main(string[] args)
        {
            string strLine, strLine1;//strLine读excel.txt，strLine1读层位cw
            int index = 0;//计数器
            int aflag = 0, bflag = 0;
            string jh = "", str = "";
            string cw;
            bool isWater = false;//判断是否水井
            bool preWater = false;//存放前一口井是不是水井的判断
            bool preTwo = false;
            bool isTwo = false;//判断是否二类油层
            bool isThree = false;//判断是否三类油层
            bool flag = false;//跳出if判断
            int x = 0, y = 0, k = 0;//坐标轴
            Dictionary<string, int> dic = new Dictionary<string, int>();
            Coordinate coordinate = new Coordinate();
            try
            {
                FileStream aFile = new FileStream(@"C:\Users\Sun\Desktop\data\0307data\excel150.txt", FileMode.Open);
                StreamReader sr = new StreamReader(aFile);
                FileStream bFile = new FileStream(@"C:\Users\Sun\Desktop\data\0307data\cw.txt", FileMode.Open);
                StreamReader sr1 = new StreamReader(bFile);
                FileStream wFile = new FileStream(@"C:\Users\Sun\Desktop\data\0307data\skzl\150-2&3.txt", FileMode.Append);
                StreamWriter sw = new StreamWriter(wFile);
                //    //char nextChar = Convert.ToChar(sr.Read());

                //Read data in line by line 一行一行的读取
                strLine1 = sr1.ReadLine();
                while (strLine1 != null)
                {
                    string[] c = Regex.Split(strLine1, @"\s+");
                    dic.Add(c[0], Convert.ToInt32(c[1]));
                    strLine1 = sr1.ReadLine();
                }

                strLine = sr.ReadLine();//跳过第一行
                strLine = sr.ReadLine();
                
                index = 0;
                while (strLine != null)
                {
                    string[] nums = Regex.Split(strLine, @"\s+");
                    cw = nums[3];
                    if (nums[2].Equals("W"))
                    {
                        isWater = true;
                    }
                    else
                        isWater = false;
                    if (nums[15].Equals("B"))
                    {
                        isThree = true;
                    }
                    else
                        isThree = false;
                    if (nums[17].Equals("A"))
                    {
                        isTwo = true;
                    }
                    else
                        isTwo = false;
                    if (!isWater)
                    {
                        //if (isThree && (!isTwo))
                        //if (isThree || (isTwo))//油井2&3
                        //{

                            if (jh.Equals(nums[0].Substring(3, nums[0].Length - 3)))
                            {
                                str = jh;
                                flag = true;
                                bflag = 0;
                            }
                            else
                            {
                                if (bflag == 1)
                                {
                                    str = jh;
                                    bflag = 0;
                                }
                                jh = nums[0].Substring(3, nums[0].Length - 3);
                                coordinate = fileReader(@"C:\Users\Sun\Desktop\data\0307data\skwgyj.txt", jh, isWater);
                                x = coordinate.x;
                                y = coordinate.y;
                                flag = false;
                                bflag++;
                                index = 0;
                                k = 0;
                            }
                            if (!flag && (str.Length != 0))
                            {
                            
                                sw.WriteLine();
                                if (aflag == 1)
                                {
                                    sw.WriteLine("XFLOW-MODEL '" + str + "A_ij' FULLY-MIXED");
                                }
                                else if(aflag == 2)
                                    sw.WriteLine("XFLOW-MODEL '" + str + "B_ij' FULLY-MIXED");
                                else if(aflag == 3)
                                    sw.WriteLine("XFLOW-MODEL '" + str + "' FULLY-MIXED");

                                sw.WriteLine();
                            }
                            if ((jh.Length != 0))
                            {
                                //result = 0;
                                foreach (KeyValuePair<string, int> a in dic)
                                {
                                    if (cw == a.Key)
                                    {
                                        if (k == a.Value)
                                        {
                                            break;
                                        }
                                        else
                                        {
                                            k = a.Value;

                                            if (index == 0)
                                            {
                                                sw.WriteLine("**$");
                                                if (isWater)
                                                {
                                                    if (isTwo)
                                                    {
                                                        sw.WriteLine("WELL  '" + jh + "A_ij' ATTACHTO '42'");
                                                        aflag = 1;
                                                    }
                                                    else
                                                    {
                                                        sw.WriteLine("WELL  '" + jh + "B_ij' ATTACHTO '42'");
                                                        aflag = 2;
                                                    }
                                                    //sw.WriteLine("WELL  '" + jh + "_ij' ATTACHTO '42'");
                                                }
                                                else
                                                {
                                                    sw.WriteLine("WELL  '" + jh + "' ATTACHTO '42'");
                                                    aflag = 3;
                                                }
                                                sw.WriteLine("**$          rad  geofac  wfrac  skin");
                                                sw.WriteLine("GEOMETRY  K  0.05  0.37  1.  0.");
                                                if (isWater)
                                                {
                                                    if (isTwo)
                                                    {
                                                        sw.WriteLine("PERF  GEOA  '" + jh + "A_ij'");
                                                    }
                                                    else
                                                        sw.WriteLine("PERF  GEOA  '" + jh + "B_ij'");
                                                    //sw.WriteLine("PERF  GEOA  '" + jh + "_ij'");
                                                }
                                                else
                                                    sw.WriteLine("PERF  GEOA  '" + jh + "'");
                                                sw.WriteLine("**$ UBA       ff  Status  Connection  ");
                                                sw.WriteLine("    " + x + " " + y + " " + k + "    " + "1.  OPEN    FLOW-TO  'SURFACE'  REFLAYER");
                                            }
                                            else
                                            {
                                                sw.WriteLine("    " + x + " " + y + " " + k + "  " + "  1.  OPEN    FLOW-TO  " + index);
                                            }
                                            index++;



                                        }
                                        break;
                                    }
                                }
                            //}
                        }
                    }
                    strLine = sr.ReadLine();

                }
                sw.WriteLine();
                if (aflag == 1)
                {
                    sw.WriteLine("XFLOW-MODEL '" + str + "A_ij' FULLY-MIXED");
                }
                else if (aflag == 2)
                    sw.WriteLine("XFLOW-MODEL '" + str + "B_ij' FULLY-MIXED");
                else if (aflag == 3)
                    sw.WriteLine("XFLOW-MODEL '" + str + "' FULLY-MIXED");
                sw.WriteLine();
                sr.Close();
                sw.Close();
            }
            catch (IOException ex)
            {
                Console.WriteLine("An IOException has been thrown!");
                Console.WriteLine(ex.ToString());
                Console.ReadLine();
                return;
            }
        }


        private static Coordinate fileReader(string path, string jh, bool isWater)
        {
            Coordinate c = new Coordinate();
            string strLine;
            string str;
            string trim;
            if (isWater)
            {
                str = "'" + jh + "_ij'";
            }
            else 
                str = "'" + jh + "'";
            bool flag = false;//跳出循环
            try
            {
                FileStream aFile = new FileStream(path, FileMode.Open);
                StreamReader sr = new StreamReader(aFile);
                strLine = sr.ReadLine();
                while (strLine != null)
                {
                    
                    string[] nums = Regex.Split(strLine, @"\s+");
                    for(int i = 0; i < nums.Count(); i++ )
                    {
                        if (str == nums[i])
                        {
                            for (int j = 0; j < 2; j++)
                            {
                                strLine = sr.ReadLine();
                            }
                            trim = strLine.Trim();
                            string[] tempA = Regex.Split(trim, @"\s+");
                            c.x = Convert.ToInt32(tempA[0]);
                            c.y = Convert.ToInt32(tempA[1]);
                            flag = true;
                            break;
                        }
                    }
                    if (flag)
                    {
                        break;
                    }
                    strLine = sr.ReadLine();
                }
                sr.Close();
            }
            catch (IOException ex)
            {
                Console.WriteLine("An IOException has been thrown!");
                Console.WriteLine(ex.ToString());
                Console.ReadLine();
            }
            return c;
        }

        
    }

}
