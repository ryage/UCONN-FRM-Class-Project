using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CC
{
    class Program
    {
        static void Main(string[] args)
        {
            double[,] temp = new double[4, 4];
            double[,] array = { { 4, 2, -1 }, { -1, -2, 5 }, { 5, 7, -3 } };
            lu(array);

        }

        static double[,] pivot(double[,] m)
        {
            int n = m.GetLength(0);
            double[,] im = new double[n, n];
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    im[i, j] = 0;
                }
                for (int q = 0; q < n; q++)
                {
                    if (i == q)
                    {
                        im[i, q] = 1;
                    }
                }
            }

            for (int i = 0; i < n; i++)
            {
                double mx = m[i, i];
                int row = i;
                for (int j = i; j < n; j++)
                {
                    if (m[j, i] > mx)
                    {
                        mx = m[j, i];
                        row = j;
                    }




                    if (i != row)
                    {
                        for (int k = 0; k < n; k++)
                        {
                            double tmp = im[i, k];
                            im[i, k] = im[row, k];
                            im[row, k] = tmp;
                        }
                    }
                }

            }
            return im;
        }

        static void lu(double[,] a)
        {
            int n = a.GetLength(0);
            double[,] l = new double[n, n];
            double[,] u = new double[n, n];
            double[,] p = pivot(a);
            double[,] a2 = new double[n, n];
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    for (int k = 0; k < n; k++)
                    {
                        a2[i, j] += p[i, k] * a[k, j];
                    }
                }
            }

            for (int j = 0; j < n; j++)
            {
                l[j, j] = 1;
                for (int i = 0; i < j + 1; i++)
                {
                    double s1 = 0;
                    for (int k = 0; k < i; k++)
                        s1 += u[k, j] * l[i, k];
                    u[i, j] = a2[i, j] - s1;

                }
                for (int i = j; i < n; i++)
                {
                    double s2 = 0;
                    for (int k = 0; k < j; k++)
                    {
                        s2 += u[k, j] * l[j, k];
                    }
                    l[i, j] = (a2[i, j] - s2) / u[j, j];
                }

            }

            for (int i = 0; i < p.GetLength(0); i++)
            {
                for (int j = 0; j < p.GetLength(1); j++)
                {
                    Console.Write(p[i, j] + " ");
                }
                Console.WriteLine();
            }

            for (int i = 0; i < l.GetLength(0); i++)
            {
                for (int j = 0; j < l.GetLength(1); j++)
                {
                    Console.Write(l[i, j] + " ");
                }
                Console.WriteLine();
            }


            for (int i = 0; i < u.GetLength(0); i++)
            {
                for (int j = 0; j < u.GetLength(1); j++)
                {
                    Console.Write(u[i, j] + " ");
                }
                Console.WriteLine();
            }

        }
    }
}
