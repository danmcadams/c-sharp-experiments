// See https://aka.ms/new-console-template for more information
Console.WriteLine("------------ Savings Calculator ------------");
Console.WriteLine("A console application that calculates and displays the growth of a savings account with monthly contributions and daily-compounded interest");
Console.WriteLine();

var startAmount = ConsoleUI.PromptForInput<double>("Starting Balance (in us dollars): $", 0);
var interestRate = ConsoleUI.PromptForInput<double>("Annual rate of return (percentage): ", 0.0);
var monthlyContribution = ConsoleUI.PromptForInput<double>("Monthly Contribution: $", 0);
var inputCompoundFrequency = ConsoleUI.PromptForInput<int>("Compound Frequency [1=Daily (default), 2=Monthly, 3=Yearly]: ", 1);

var compoundFrequency = (CompoundFrequency) inputCompoundFrequency switch
{
    CompoundFrequency.Monthly => ConsoleUI.PromptForInput<int>("Account age in months (default 12): ", 12),
    CompoundFrequency.Yearly => ConsoleUI.PromptForInput<int>("Account age in years (default 1): ", 1),
    _ => 365
};

Console.WriteLine(compoundFrequency);
Environment.Exit(0);
// interestRate /= 100;
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

public enum CompoundFrequency
{
    Daily = 1,
    Monthly = 2,
    Yearly = 3,
}

public readonly struct Period
{
    public int Month { get; init; }
    public double StartingBalance { get; init; }
    public double EndingBalance { get; init; }
    public double InterestEarned { get; init; }

    public string GetMonthText()
    {
        return Month.ToString();
    }
}

public class ConsoleUI
{
    public static T PromptForInput<T>(string prompt, T defaultValue)
    {
        Console.Write(prompt);
        var input = Console.ReadLine();

        if (string.IsNullOrEmpty(input))
        {
            return defaultValue;
        }

        try
        {
            return (T)Convert.ChangeType(input, typeof(T));
        }
        catch
        {
            return defaultValue;
        }
    }

    public static void PrintBreakdownToConsole(List<Period> periods)
    {
        Console.WriteLine();
        Console.WriteLine("Month      Starting Balance      Interest Earned      Ending Balance");
        Console.WriteLine("====================================================================");

        foreach (var p in periods)
        {
            var period = p.Month.ToString().PadRight(11);
            var start = p.StartingBalance.ToString("C2").PadLeft(16);
            // var start = p.StartingBalance.ToString("C2").PadRight(22);
            var interestEarned = p.InterestEarned.ToString("C2").PadLeft(21);
            var end = p.EndingBalance.ToString("C2").PadLeft(18);

            Console.WriteLine($"{period}{start}{interestEarned}{end}");
        }
    }
}
