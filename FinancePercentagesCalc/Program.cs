// See https://aka.ms/new-console-template for more information

using FinancePercentagesCalc;

int code = ConsoleUI.Run();

Environment.Exit(code);
// var currentBalance = startAmount;
// Console.WriteLine(compoundFrequency);
// var apy = Math.Pow(1 + interestRate / 365, 365) - 1;
// // Console.WriteLine(apy);
// // Environment.Exit(0);
//
// var periods = new List<Period>();
// for (var month = 1; month <= months; month++)
// {
//     var monthStart = currentBalance;
//     // AI generated ------
//     int daysInMonth = DateTime.DaysInMonth(DateTime.Now.Year, ((DateTime.Now.Month - 1 + month - 1) % 12) + 1);
//     // ---------
//     // var apy = Math.Pow(1 + interestRate / daysInMonth, daysInMonth) - 1;
//     var holdValue = currentBalance + (currentBalance * apy); // just for debugging
//     
//     for (var d = 0; d < daysInMonth; d++)
//     {
//         var dailyInterestInDollars = (currentBalance * interestRate) / 365;
//         currentBalance += dailyInterestInDollars;
//     }
//     var interestForMonth = currentBalance - monthStart;
//     currentBalance += monthlyContribution;
//     
//     periods.Add(
//         new Period
//         {
//             Month = month,
//             StartingBalance = monthStart,
//             EndingBalance = currentBalance,
//             InterestEarned = interestForMonth,
//         });
// }
// double totalInterestEarned = currentBalance - ((monthlyContribution * months) + startAmount);
//
// Console.WriteLine();
// Console.WriteLine($"Ending Balance: {(currentBalance):C2}");
// Console.WriteLine($"Interest Earned: {(totalInterestEarned):C2}");
// Console.WriteLine();
// Console.WriteLine("Would you like to see a complete breakdown? (y/n) ");
//
// if (Console.ReadKey(intercept: true).KeyChar == 'y' || Console.ReadKey(intercept: true).KeyChar == 'Y')
// {
//     ConsoleUI.PrintBreakdownToConsole(periods);
// }


