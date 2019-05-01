using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Project3
{
    class Program
    {
        static void Main(string[] args)
        {
            double[,,] stock = new double[4, 200, 756];
            stock = Read_and_Calc();

            double[] temp = Quantile(0.01, stock, 1);
            for (int i = 0; i < temp.Length; i++)
            {
                Console.WriteLine(temp[i]);
            }
            double var = ValueAtRisk(temp);
            Console.WriteLine(var);
            double es = ES(temp);
            Console.WriteLine(es);

            //string[] option_price = Option(stock);
            //Console.WriteLine("Simulated call option with strike of S0 are priced at: ");
            //Console.WriteLine("ATVI\tNFLX\tPFE\tTSLA");
            //for (int i = 0; i < 4; i++)
            //{
            //    Console.Write(option_price[i] + "\t");
            //}

        }

        static string[] Average_Return(double[,,] mc)
        {
            double[] ret = new double[] { 0.0, 0.0, 0.0, 0.0 };
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 200; j++)
                {
                    ret[i] = ret[i] + Math.Log(mc[i, j, 755] / mc[i, j, 0]);
                }
                ret[i] = ret[i] / 600;
            }

            string[] result = ret.Select(x => x.ToString("p2")).ToArray();
            return result;
        }

        static double[,,] Read_and_Calc()
        {

            List<double> ATVI = new List<double>();
            List<double> NFLX = new List<double>();
            List<double> PFE = new List<double>();
            List<double> TSLA = new List<double>();

            FileStream fileStream = File.OpenRead("D:/DATA.csv");
            StreamReader reader = new StreamReader(fileStream);

            while (!reader.EndOfStream)
            {
                string line = reader.ReadLine();
                var values = line.Split(',');

                ATVI.Add(double.Parse(values[1]));
                NFLX.Add(double.Parse(values[2]));
                PFE.Add(double.Parse(values[3]));
                TSLA.Add(double.Parse(values[4]));
            }

            reader.Close();

            double[] A_array = ATVI.ToArray();
            double[] N_array = NFLX.ToArray();
            double[] P_array = PFE.ToArray();
            double[] T_array = TSLA.ToArray();

            //mu
            double[] stock_mean = new double[4];
            stock_mean[0] = Math.Log(ATVI[ATVI.Count - 1] / ATVI[0]) / 3.0;
            stock_mean[1] = Math.Log(NFLX[NFLX.Count - 1] / NFLX[0]) / 3.0;
            stock_mean[2] = Math.Log(PFE[PFE.Count - 1] / PFE[0]) / 3.0;
            stock_mean[3] = Math.Log(TSLA[TSLA.Count - 1] / TSLA[0]) / 3.0;

            //sigma
            List<double> a_return = new List<double>();
            List<double> n_return = new List<double>();
            List<double> p_return = new List<double>();
            List<double> t_return = new List<double>();

            for (int i = 0; i < ATVI.Count - 1; i++)
            {
                a_return.Add(Math.Log(ATVI[i + 1] / ATVI[i]));
                n_return.Add(Math.Log(NFLX[i + 1] / NFLX[i]));
                p_return.Add(Math.Log(PFE[i + 1] / PFE[i]));
                t_return.Add(Math.Log(TSLA[i + 1] / TSLA[i]));
            }
            double[] stock_std = new double[4];
            stock_std[0] = a_return.StandardDeviation() * Math.Sqrt(252);
            stock_std[1] = n_return.StandardDeviation() * Math.Sqrt(252);
            stock_std[2] = p_return.StandardDeviation() * Math.Sqrt(252);
            stock_std[3] = t_return.StandardDeviation() * Math.Sqrt(252);



            //S0
            double[] s0 = new double[4];
            s0[0] = A_array[0];
            s0[1] = N_array[0];
            s0[2] = P_array[0];
            s0[3] = T_array[0];

            //calculate the drift
            double[] nu = new double[4];
            for (int i = 0; i < 4; i++)
            {
                nu[i] = stock_mean[i] - stock_std[i] * stock_std[i] / 2;
            }

            //coefficient matrix
            double[,] coeff = new double[4, 4];
            for (int i = 0; i < 4; i++)
            {
                coeff[i, i] = 1;

            }
            coeff[0, 1] = Calc_Coeff(A_array, N_array);
            coeff[1, 0] = coeff[0, 1];

            coeff[0, 2] = Calc_Coeff(A_array, P_array);
            coeff[2, 0] = coeff[0, 2];

            coeff[0, 3] = Calc_Coeff(A_array, T_array);
            coeff[3, 0] = coeff[0, 3];

            coeff[1, 2] = Calc_Coeff(P_array, N_array);
            coeff[2, 1] = coeff[1, 2];

            coeff[1, 3] = Calc_Coeff(T_array, N_array);
            coeff[3, 1] = coeff[1, 3];

            coeff[2, 3] = Calc_Coeff(P_array, T_array);
            coeff[3, 2] = coeff[2, 3];

            //cholesky decomposition
            double[,] chole = new double[4, 4];
            chole = Cholesky(coeff);

            double[,,] rnd = RandomPath(4, 200, 755);

            double[,,] rnd_mc = new double[4, 200, 755];
            for (int t = 0; t < 200; t++)
            {
                for (int i = 0; i < 4; i++)
                {
                    for (int j = 0; j < 755; j++)
                    {
                        rnd_mc[i, t, j] = 0;
                        for (int k = 0; k < 4; k++)
                        {
                            rnd_mc[i, t, j] += chole[i, k] * rnd[k, t, j];
                        }

                    }
                }
            }

            double[,,] stock_mc = new double[4, 200, 756];

            double dt = 0.00396825397;

            for (int i = 0; i < 200; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    stock_mc[j, i, 0] = s0[j];
                    for (int k = 1; k < 756; k++)
                    {
                        stock_mc[j, i, k] = stock_mc[j, i, k - 1] * Math.Exp(nu[j] *
                            dt + stock_std[j] * Math.Sqrt(dt) * rnd_mc[j, i, k - 1]);

                    }
                }
            }


            return stock_mc;
        }

        static string[] Option(double[,,] stock_path)
        {
            double rf = 0.04;
            double[] strike = new double[] { 37.66, 114.38, 28.64, 240.00 };
            double[] payoff = new double[] { 0.0, 0.0, 0.0, 0.0 };
            double[] price = new double[] { 0.0, 0.0, 0.0, 0.0 };
            for (int i = 0; i < 4; i++)
            {
                double[] one_trial = new double[200];
                for (int j = 0; j < 200; j++)
                {
                    payoff[i] += Math.Max(0, stock_path[i, j, 755] - strike[i]) / 200;
                }
                price[i] = payoff[i] * Math.Exp(-rf * 3);

            }
            string[] result = price.Select(x => x.ToString("c")).ToArray();
            return result;

        }

        //method to calculate coefficient of two arrays
        static double Calc_Coeff(double[] values1, double[] values2)
        {
            //make sure two arrays have same length
            if (values1.Length != values2.Length)
                throw new ArgumentException("Values must be the same length");

            //calculate average of two arrays using user defined function
            var avg1 = values1.Average();
            var avg2 = values2.Average();
            //calculate covariance by using zip and anonymous function
            var cov = values1.Zip(values2, (x1, y1) => (x1 - avg1) * (y1 - avg2)).Sum();
            //variance or std of two arrays
            var sumSqr1 = values1.Sum(x => Math.Pow((x - avg1), 2.0));
            var sumSqr2 = values2.Sum(y => Math.Pow((y - avg2), 2.0));

            var coeff = cov / Math.Sqrt(sumSqr1 * sumSqr2);

            return coeff;
        }
        //method to cholesky decomposition a matrix
        static double[,] Cholesky(double[,] matrix_A)
        {
            int n = (int)Math.Sqrt(matrix_A.Length);
            double[,] matrix_R = new double[n, n];
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j <= i; j++)
                {
                    if (i == j)
                    {
                        double sum = 0;
                        for (int k = 0; k < j; k++)
                        {
                            sum += matrix_R[j, k] * matrix_R[j, k];
                        }
                        matrix_R[j, j] = Math.Sqrt(matrix_A[j, j] - sum);
                    }
                    else
                    {
                        double sum = 0;
                        for (int k = 0; k < j; k++)
                        {
                            sum += matrix_R[i, k] * matrix_R[j, k];
                        }
                        matrix_R[i, j] = 1.0 / matrix_R[j, j] * (matrix_A[i, j] - sum);
                    }
                }
            }
            return matrix_R;
        }

        static double[,,] RandomPath(int stock_num, int path_num, int time_step)
        {

            double a = 22695477.0;
            double c = 1.0;
            double m = Math.Pow(2, 32);
            double seed = 100.0;

            double[,,] random_path = new double[stock_num, path_num, time_step];
            for (int k = 0; k < stock_num; k++)
            {

                for (int i = 0; i < path_num; i++)
                {

                    for (int j = 0; j < time_step; j++)
                    {
                        seed = (a * seed + c) % m;
                        double u1 = 1.0 - seed / m;
                        seed = (a * seed + c) % m;
                        double u2 = 1.0 - seed / m;
                        double rand_std = Math.Sqrt(-2.0 * Math.Log(u1)) *
                            Math.Sin(2.0 * Math.PI * u2);
                        random_path[k, i, j] = rand_std;
                    }
                }
            }

            return random_path;
        }

        static double[] Quantile(double quant, double[,,] mc, int stock)
        {
            double[] lastprice = new double[mc.GetLength(1)];
            for (int i = 0; i < mc.GetLength(1); i++)
            {
                lastprice[i] = mc[stock - 1, i, mc.GetLength(1) - 1];
            }
            
             

            //sort
            
            
            //ascending
            Array.Sort(lastprice);
            

            
            int num = (int)(200 * quant);
            double[] selected = new double[num];
            for (int j = 0; j < num; j++)
            {
                selected[j] = lastprice[j];
            }
            for (int i = 0; i < num; i++)
            {
                selected[i] = selected[i] - mc[stock - 1, 0, 0];
            }
            
            return selected;

        }
        static double ValueAtRisk(double[] qt)
        {
            double VaR = new double { };
            VaR = qt[qt.Length-1];
            return VaR;
        }
        static double ES(double[] qt)
        {
            double mean = new double { };
            for (int i = 0; i < qt.Length; i++)
            {
                mean += qt[i];
            }
            mean = mean / qt.Length;
            return mean;

        }

    }
    public static class MyListExtensions
    {
        public static double Mean(this List<double> values)
        {
            return values.Count == 0 ? 0 : values.Mean(0, values.Count);
        }

        public static double Mean(this List<double> values, int start, int end)
        {
            double s = 0;

            for (int i = start; i < end; i++)
            {
                s += values[i];
            }

            return s / (end - start);
        }

        public static double Variance(this List<double> values)
        {
            return values.Variance(values.Mean(), 0, values.Count);
        }

        public static double Variance(this List<double> values, double mean)
        {
            return values.Variance(mean, 0, values.Count);
        }

        public static double Variance(this List<double> values, double mean, int start, int end)
        {
            double variance = 0;

            for (int i = start; i < end; i++)
            {
                variance += Math.Pow((values[i] - mean), 2);
            }

            int n = end - start;
            if (start > 0) n -= 1;

            return variance / (n);
        }

        public static double StandardDeviation(this List<double> values)
        {
            return values.Count == 0 ? 0 : values.StandardDeviation(0, values.Count);
        }

        public static double StandardDeviation(this List<double> values, int start, int end)
        {
            double mean = values.Mean(start, end);
            double variance = values.Variance(mean, start, end);

            return Math.Sqrt(variance);
        }
    }


}