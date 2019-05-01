using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp5
{
    class Program
    {
        static void Main(string[] args)
        {

            Portfolio();

        }
        static void Portfolio()
        {

            // ask user for risk-free rate that will be used for all kinds of assets.
            Console.WriteLine("What is the risk-free rate? ");
            double risk_free = double.Parse(Console.ReadLine());

            //Equity part of portfolio
            // ask for the number of equities the user have
            Console.WriteLine("How many equities do you have? ");
            int num_equity = int.Parse(Console.ReadLine());
            Equity[] your_equity = new Equity[num_equity];
            // use a loop to construct corresponding number of equities
            for (int i = 0; i < num_equity; i++)
            {
                Console.WriteLine($"Please tell us about your equity no. {i + 1}: ");
                your_equity[i] = new Equity(risk_free);
            }
            // calculate the valuation and expected return for 1, 5, 10, 20 years of all equities 
            // note that expected returns are absolute values instead of relative values
            double equity_valuation = 0;
            double[] equity_return = new double[4];
            for (int i = 0; i < num_equity; i++)
            {
                equity_valuation += your_equity[i].valuation;
                for (int j = 0; j < 4; j++)
                {
                    equity_return[j] += your_equity[i].expected_return[j];
                }
            }
            Console.WriteLine("The valuation and expected return of your equities are: ");
            Console.WriteLine(equity_valuation);
            for (int i = 0; i < 4; i++)
            {
                Console.WriteLine(equity_return[i]);

            }

            // Same procedure with Equity (see above)
            //Bond 
            Console.WriteLine("How many bonds do you have? ");
            int num_bond = int.Parse(Console.ReadLine());
            Bond[] your_bond = new Bond[num_bond];

            for (int i = 0; i < num_bond; i++)
            {
                Console.WriteLine($"Please tell us about your bond no. {i + 1}: ");
                your_bond[i] = new Bond(risk_free);
            }
            double bond_valuation = 0;
            double[] bond_return = new double[4];
            for (int i = 0; i < num_bond; i++)
            {
                bond_valuation += your_bond[i].PV;
                for (int j = 0; j < 4; j++)
                {
                    bond_return[j] += your_bond[i].expected_returns[j];
                }
            }
            Console.WriteLine("The valuation and expected return of your bonds are: ");
            Console.WriteLine(bond_valuation);
            for (int i = 0; i < 4; i++)
            {
                Console.WriteLine(bond_return[i]);

            }

            // Same procedure with Equity (see above)
            //CDs
            Console.WriteLine("How many CDs do you have? ");
            int num_CD = int.Parse(Console.ReadLine());
            CDs[] your_CDs = new CDs[num_CD];

            for (int i = 0; i < num_CD; i++)
            {
                Console.WriteLine($"Please tell us about your CD no. {i + 1}: ");
                your_CDs[i] = new CDs(risk_free);
            }
            double CDs_valuation = 0;
            double[] CDs_return = new double[4];
            for (int i = 0; i < num_CD; i++)
            {
                CDs_valuation += your_CDs[i].present_value_CD;
                for (int j = 0; j < 4; j++)
                {
                    CDs_return[j] += your_CDs[i].expected_return[j];
                }
            }
            Console.WriteLine("The valuation and expected return of your CDs are: ");
            Console.WriteLine(CDs_valuation);
            for (int i = 0; i < 4; i++)
            {
                Console.WriteLine(CDs_return[i]);

            }

            //House 
            Console.WriteLine("How many houses do you have? ");
            int num_house = int.Parse(Console.ReadLine());
            House[] your_house = new House[num_house];

            for (int i = 0; i < num_house; i++)
            {
                Console.WriteLine($"Please tell us about your house no. {i + 1}: ");
                your_house[i] = new House(risk_free);
            }
            double house_valuation = 0;

            for (int i = 0; i < num_house; i++)
            {
                house_valuation += your_house[i].valuation;
                
            }
            Console.WriteLine("The valuation of your houses are: ");
            Console.WriteLine(house_valuation);
            

            double total = equity_valuation + bond_valuation + CDs_valuation + house_valuation;
            string equity_perc = (equity_valuation / total).ToString("p1");
            string bond_perc = (bond_valuation / total).ToString("p1");
            string CDs_perc = (CDs_valuation / total).ToString("p1");
            string house_perc = (house_valuation / total).ToString("p1");
            Console.WriteLine("The breakdown of the portfolio per asset is: ");
            Console.WriteLine($"Equity: {equity_perc}.");
            Console.WriteLine($"Bond: {bond_perc}.");
            Console.WriteLine($"CDs: {CDs_perc}.");
            Console.WriteLine($"House: {house_perc}.");





        }
    }

    // each class have to have a constructor, an attribute that stores the valuation of this asset, 
    // and an attribute that stores the expected return of this asset as an array with length of 4
    class Bond
    {
        // variables that will be used
        private double coupon_rate { get; set; }
        private double coupon_frenquency { get; set; }
        private int face_value { get; set; }

        private double time_period { get; set; }
        private DateTime start_time { get; set; }
        private int j = 0;
        public double[] expected_returns = new double[4];
        public double PV { get; set; }
        // constructor, very important, must-have
        public Bond(double rf) // use rf from main as input 
        {
            getinfo(); // ask information needed
            PV = pv(rf); //use user-defined method to calculate present value of this bond
            expected_returns = ERs(rf); //use user-defined method to calculate expected return of this bond
        }


        public void getinfo()
        {
            Console.WriteLine("what is the coupon rate? (e.g. 0.05 for 5%)");
            coupon_rate = double.Parse(Console.ReadLine());
            Console.WriteLine("what is the coupon frenquency?(e.g. 0.5 for every half year)");
            coupon_frenquency = double.Parse(Console.ReadLine());
            Console.WriteLine("what is the face value?");
            face_value = int.Parse(Console.ReadLine());


            Console.WriteLine("what is the maturity in years?(e.g. 0.5 for six months)");
            time_period = double.Parse(Console.ReadLine());
            Console.WriteLine("what is the start date?(e.g. 20180505)");
            start_time = DateTime.ParseExact(Console.ReadLine(), "yyyyMMdd", System.Globalization.CultureInfo.CurrentCulture);
        }


        public double pv(double interest_rate)
        {
            int couponflow = (int)(time_period / coupon_frenquency);
            double[] cf = new double[couponflow];
            DateTime[] couponday = new DateTime[couponflow];
            couponday[0] = start_time;

            for (int i = 0; i < couponflow - 1; i++)
            {
                couponday[i + 1] = couponday[i].AddMonths((int)(12 * coupon_frenquency));
                cf[i] = coupon_rate * coupon_frenquency * face_value;
            }
            cf[couponflow - 1] += face_value;
            double[] effcf = new double[couponflow - j];
            DateTime[] effcd = new DateTime[couponflow - j];
            for (int i = 0; i < couponflow - j; i++)
            {
                effcf[i] = cf[i + j];
                effcd[i] = couponday[i + j];
            }

            for (int i = 0; i < couponflow - j; i++)
            {
                PV += effcf[i] * Math.Exp(-interest_rate * effcd[i].Subtract(DateTime.Now).Days / 365);
            }
            return PV;

        }
        public double[] ERs(double interest_rate)
        {
            int couponflow = (int)(time_period / coupon_frenquency);
            double[] cf = new double[couponflow];
            DateTime[] couponday = new DateTime[couponflow];
            couponday[0] = start_time.AddMonths((int)(12 * coupon_frenquency));

            for (int i = 0; i < couponflow - 1; i++)
            {
                couponday[i + 1] = couponday[i].AddMonths((int)(12 * coupon_frenquency));
                cf[i] = coupon_rate * coupon_frenquency * face_value;
            }
            cf[couponflow - 1] = face_value + coupon_rate * coupon_frenquency * face_value;
            while (couponday[j].Subtract(DateTime.Now).Days < 0)
            {
                j++;
            }

            double[] effcf = new double[couponflow - j];
            DateTime[] effcd = new DateTime[couponflow - j];

            for (int i = 0; i < couponflow - j; i++)
            {
                effcf[i] = cf[i + j];

                effcd[i] = couponday[i + j];

            }


            for (int i = 0; i < couponflow - j; i++)
            {
                expected_returns[0] += effcf[i] * Math.Exp(-interest_rate * effcd[i].Subtract(DateTime.Now.AddYears(1)).Days / 365);
                expected_returns[1] += effcf[i] * Math.Exp(-interest_rate * effcd[i].Subtract(DateTime.Now.AddYears(5)).Days / 365);
                expected_returns[2] += effcf[i] * Math.Exp(-interest_rate * effcd[i].Subtract(DateTime.Now.AddYears(10)).Days / 365);
                expected_returns[3] += effcf[i] * Math.Exp(-interest_rate * effcd[i].Subtract(DateTime.Now.AddYears(20)).Days / 365);
            }
            for (int i = 0; i < 4; i++)
            {
                expected_returns[i] -= PV;
            }
            return expected_returns;
        }
    }

    class Equity
    {
        private string ID { get; set; }
        public double price { get; set; }
        public double valuation { get; set; }
        public int number_of_shares { get; set; }
        public double dividend_yield { get; set; }
        public double vol { get; set; }
        public double[] expected_return = new double[4];
        public double[,] spath = new double[300, 7300];


        public Equity(double rf)
        {

            Console.WriteLine("How much does this stock worth now?");
            price = double.Parse(Console.ReadLine());
            Console.WriteLine("How many shares do you have?");
            number_of_shares = int.Parse(Console.ReadLine());

            Console.WriteLine("What is the dividend yield?");
            dividend_yield = double.Parse(Console.ReadLine());
            Console.WriteLine("What is the volatility?");
            vol = double.Parse(Console.ReadLine());

            valuation = calc_valuation(price, number_of_shares);
            spath = MCprices(price, rf, dividend_yield, vol);

            expected_return = ERs(spath, rf, dividend_yield, number_of_shares, price);

        }

        public double calc_valuation(double price, int number_of_shares)
        {
            valuation = price * number_of_shares;
            return valuation;
        }

        public double[,] MCprices(double price, double rf, double dividend_yield, double vol)
        {
            double[,] spath = new double[300, 7300];

            for (int i = 0; i < 300; i++)
            {
                spath[i, 0] = valuation;
                for (int j = 1; j < 7300; j++)
                {
                    spath[i, j] = spath[i, j - 1] * Math.Exp((rf - dividend_yield - 0.5 * vol * vol) / 365 + vol * Rand(0, 1) / Math.Sqrt(365));
                }
            }
            return spath;
        }

        public double[] ERs(double[,] spath, double rf, double dividend_yield, int number_of_shares, double price)
        {
            double[] expected_prices = new double[20];
            for (int j = 0; j < 20; j++)
            {
                double sum = 0;
                for (int i = 0; i < 300; i++)
                {
                    sum += spath[i, (1 + j) * 365 - 1];
                }
                expected_prices[j] = sum / 300;
            }

            double[] expected_return = new double[4];
            expected_return[0] = expected_prices[0] * (1 + dividend_yield);
            expected_return[1] = expected_prices[4];
            for (int i = 0; i < 5; i++)
            {
                expected_return[1] += expected_prices[i] * dividend_yield * Math.Exp((4 - i) * rf);
            }
            expected_return[2] = expected_prices[9];
            for (int i = 0; i < 9; i++)
            {
                expected_return[2] += expected_prices[i] * dividend_yield * Math.Exp((9 - i) * rf);
            }
            expected_return[3] = expected_prices[19];
            for (int i = 0; i < 19; i++)
            {
                expected_return[3] += expected_prices[i] * dividend_yield * Math.Exp((19 - i) * rf);
            }

            for (int i = 0; i < 4; i++)
            {
                expected_return[i] = expected_return[i] - price * number_of_shares;
            }
            return expected_return;

        }
        private static int GetRandomSeed()
        {
            byte[] bytes = new byte[4];
            System.Security.Cryptography.RNGCryptoServiceProvider rng = new System.Security.Cryptography.RNGCryptoServiceProvider();
            rng.GetBytes(bytes);
            return BitConverter.ToInt32(bytes, 0);
        }


        public static double Rand(double u, double d)
        {
            double u1, u2, z, x;
            //Random ram = new Random();
            if (d <= 0)
            {
                return u;
            }
            u1 = (new Random(GetRandomSeed())).NextDouble();
            u2 = (new Random(GetRandomSeed())).NextDouble();
            z = Math.Sqrt(-2 * Math.Log(u1)) * Math.Sin(2 * Math.PI * u2);
            x = u + d * z;

            return x;
        }

    }

    class CDs
    {
        public int face_value { get; set; }
        public double interest_rate { get; set; }
        public double maturity { get; set; }
        public DateTime end_time { get; set; }


        public double present_value_CD { get; set; }
        public double[] CDs_fv = new double[4];
        public double[] expected_return = new double[4];

        public CDs(double rf)
        {

            user_info();
            present_value_CD = calc_CDs(rf);
            CDs_fv = calc_fv(rf);
            expected_return = calc_return(rf);

        }
        public void user_info()
        {
            Console.WriteLine("what is the face value");
            face_value = int.Parse(Console.ReadLine());
            Console.WriteLine("what is the interest rate?(e.g. 0.05 for 5%)");
            interest_rate = double.Parse(Console.ReadLine());
            Console.WriteLine("what is the maturity in years?(e.g. 0.5 for six months)");
            maturity = double.Parse(Console.ReadLine());
            Console.WriteLine("what is the end date?(e.g. 20200505)");
            end_time = DateTime.ParseExact(Console.ReadLine(), "yyyyMMdd", System.Globalization.CultureInfo.CurrentCulture);
        }



        public double calc_CDs(double rf)
        {
            double CDs = 0;
            if (maturity < 1)
            {
                CDs = face_value * (1 + interest_rate * maturity);
            }
            else
            {
                CDs = face_value * Math.Pow((1 + interest_rate), maturity);
            }

            int days = 0;
            days = end_time.Subtract(DateTime.Now).Days;


            double pv = 0;
            pv = CDs * Math.Exp(-rf * days / 365);
            //Console.WriteLine("the present value is {0}", pv);

            return pv;
        }

        public double[] calc_fv(double rf)
        {

            CDs_fv[0] = face_value * Math.Pow((1 + interest_rate), maturity) * Math.Exp(-rf * end_time.Subtract(DateTime.Now.AddYears(1)).Days / 365);
            CDs_fv[1] = face_value * Math.Pow((1 + interest_rate), maturity) * Math.Exp(-rf * end_time.Subtract(DateTime.Now.AddYears(5)).Days / 365);
            CDs_fv[2] = face_value * Math.Pow((1 + interest_rate), maturity) * Math.Exp(-rf * end_time.Subtract(DateTime.Now.AddYears(10)).Days / 365);
            CDs_fv[3] = face_value * Math.Pow((1 + interest_rate), maturity) * Math.Exp(-rf * end_time.Subtract(DateTime.Now.AddYears(20)).Days / 365);
            return CDs_fv;
        }

        public double[] calc_return(double rf)
        {
            for (int i = 0; i < 4; i++)
            {
                expected_return[i] = CDs_fv[i] - calc_CDs(rf);
            }

            return expected_return;
        }

    }

    class House
    {
        
        private double rent_per_month { get; set; }
        public bool rent { get; set; }
        
        private double return_rate { get; set; }
        private double expense { get; set; }
        private double other_expense { get; set; }

        public double valuation { get; set; } 
        public House(double rf)
        {
            Console.WriteLine("Is there any revenue from this house?");
            Console.WriteLine("i.e. is this house renting instead of living?");
            rent = bool.Parse(Console.ReadLine());

            if (rent)
            {
                valuation = present_value_rent(rf);
            }
            else
            {
                valuation = present_value_live();
            }

        }
        public double present_value_rent(double rf)
        {
            Console.WriteLine("Please enter the monthly rent or revenue of your house: ");
            rent_per_month = double.Parse(Console.ReadLine());
            Console.WriteLine("Please enter the average return rate of this house: ");
            return_rate = double.Parse(Console.ReadLine());
            double valuation_rent = rent_per_month / (return_rate - rf);
            return valuation_rent;
        }
        public double present_value_live()
        {
            Console.WriteLine("Please enter the expense of purchasing / building this house: ");
            expense = double.Parse(Console.ReadLine());
            Console.WriteLine("Please enter the other expense of acquiring your house: ");
            other_expense = double.Parse(Console.ReadLine());

            return expense + other_expense;

        }
    }
}


