using System;
using System.IO;
using System.Data;
using System.Text;
using System.Collections.Generic;
using System.Linq;







namespace ConsoleApp3
{

    class linear
    {
        private double Scale(double averX,double averY, double[] arrayX, double[] arrayY)
        {
            double scale = 0;
            if (arrayX.Length == arrayY.Length)
            {
                double Molecular = 0;
                double Denominator = 0;
                for(int i=0;i<arrayX.Length;i++)
                {
                    Molecular += (arrayX[i] - averX) * (arrayY[i] - averY);
                    Denominator += Math.Pow((arrayX[i] - averX), 2);
                }
                scale = Molecular / Denominator;

            }
            return scale;
        }

        private double Offset(double scale, double averX, double averY)
        {
            double offset = 0;
            offset = averY - scale * averX;
            return offset;
        }

        public double[] LinearResult(double[] arrayX, double[] arrayY)
        {
            double[] result = { 0, 0 };
            if (arrayX.Length == arrayY.Length)
            {
                double averX = arrayX.Average();
                double averY = arrayY.Average();
                result[0] = Scale(averX, averY, arrayX, arrayY);
                result[1] = Offset(result[0], averX, averY);

            }
            return result;
        }
    }



    class program
    {


        static void Main(string[] args)
        {
            linear LSA = new linear();
            Console.WriteLine("please input the file location:(e.g. E:\\data.csv)");
            using (var reader = new StreamReader(Console.ReadLine ()))
            {
                List<string> listDate = new List<string>();
                List<string> listS1 = new List<string>();
                List<string> listS2 = new List<string>();
                List<string> listS3 = new List<string>();
                List<string> listS4 = new List<string>();
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var values = line.Split(',');

                    listDate.Add(values[0]);
                    listS1.Add(values[1]);
                    listS2.Add(values[2]);
                    listS3.Add(values[3]);
                    listS4.Add(values[4]);

                }

                string[] S1 = listS1.ToArray();
                string[] S2 = listS2.ToArray();
                string[] S3 = listS3.ToArray();
                string[] S4 = listS4.ToArray();

                do
                {
                    Console.WriteLine("Please choose the stock to create price model:1 for {0}, 2 for {1}, 3 for {2}, 4 for {3}",S1[0],S2[0],S3[0],S4[0]);
                
                    int userchoice = int.Parse(Console.ReadLine());
                    string[] date = listDate.ToArray();
                    int n = date.Length - 1;

                    double[] arrayXt = new double[n];
                    for (int i = 0; i < n; i++)
                    {
                        arrayXt[i] = i + 1;
                    }
                    double[] SPrice = new double[n];
                    double[] SLogPrice = new double[n];
                    double[] coe_l = new double[2];
                    double[] coe_e = new double[2];

                    switch (userchoice)
                    {
                        case 1:
                            //string[] S1 = listS1.ToArray();
                            int n1 = S1.Length - 1;
                            for (int i = 0; i < n1; i++)
                            {
                                SPrice[i] = Convert.ToDouble(S1[i + 1]);
                            }

                            for (int i = 0; i < n1; i++)
                            {
                                SLogPrice[i] = Math.Log(SPrice[i]);
                            }
                            break;
                        case 2:
                            //string[] S2 = listS2.ToArray();
                            int n2 = S2.Length - 1;
                            for (int i = 0; i < n2; i++)
                            {
                                SPrice[i] = Convert.ToDouble(S2[i + 1]);
                            }

                            for (int i = 0; i < n2; i++)
                            {
                                SLogPrice[i] = Math.Log(SPrice[i]);
                            }
                            break;
                        case 3:
                            //string[] S3 = listS3.ToArray();
                            int n3 = S3.Length - 1;
                            for (int i = 0; i < n3; i++)
                            {
                                SPrice[i] = Convert.ToDouble(S3[i + 1]);
                            }

                            for (int i = 0; i < n3; i++)
                            {
                                SLogPrice[i] = Math.Log(SPrice[i]);
                            }
                            break;
                        case 4:
                            //string[] S4 = listS4.ToArray();
                            int n4 = S4.Length - 1;
                            for (int i = 0; i < n4; i++)
                            {
                                SPrice[i] = Convert.ToDouble(S4[i + 1]);
                            }

                            for (int i = 0; i < n4; i++)
                            {
                                SLogPrice[i] = Math.Log(SPrice[i]);
                            }
                            break;

                    }

                    coe_l = LSA.LinearResult(arrayXt, SPrice);
                    coe_e = LSA.LinearResult(arrayXt, SLogPrice);

                    Console.WriteLine("The equation for linear model is:y={0}x+{1}", coe_l[0].ToString("f4"), coe_l[1].ToString("f4"));
                    Console.WriteLine("The equation for exp model is:y={0}e^({1}x)", Math.Exp(coe_e[1]).ToString("f4"), coe_e[0].ToString("f4"));
                    Console.WriteLine("Input 1 to continue, or anything else to end:");

                } while (Console.ReadLine() == "1");
                

            }
            

        }

    }
}