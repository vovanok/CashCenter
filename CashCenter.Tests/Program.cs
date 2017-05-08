using CashCenter.Check;
using CashCenter.Common;
using CashCenter.IvEnergySales.Check;
using System;
using System.Threading;

namespace CashCenter.Tests
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                CheckPrinter.ShowProperties();

                if (!CheckPrinter.IsReady)
                {
                    Console.WriteLine("ККМ не готов.");
                    Console.Read();
                    return;
                }

                var random = new Random();

                int countChecks = 100;
                for (int i = 0; i < countChecks; i++)
                {
                    try
                    {
                        Logger.Info($"Печать чека {i}");
                        Console.WriteLine($"Печать чека {i}");

                        CheckPrinter.Print(new CustomerCheck(
                            "153510 г.о.Кохма, пл.Октябрьская, 14, тел. 58-56-02",
                            "121212",
                            1212121,
                            "Вася Пупкин",
                            "Электроэнергия",
                            "Пупкина Василиса Антониновна",
                            (decimal)(random.NextDouble() * 1000.0 + 10000.0),
                            "1@gmail.com"));
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        Console.Read();
                    }

                    Thread.Sleep(4000);
                }

                Console.WriteLine("ЗАВЕРШЕНО");
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.InnerException?.Message);
            }

            Console.Read();
        }
    }
}
