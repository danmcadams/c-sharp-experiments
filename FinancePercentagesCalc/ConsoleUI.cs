using System.ComponentModel;
using System.Text.Json;

namespace FinancePercentagesCalc;

internal enum MenuOptions
{
    [Description("APY Calculator")] APYCalculator,
    [Description("Savings Calculator")] SavingsCalculator,
    [Description("View Previous Savings Calculations")] SavingsBreakdowns,
    [Description("Exit")] Exit,
}

internal class SavingsBreakdown
{
    public int NumMonths { get; init; }
    public double APY { get; init; }
    public double StartAmount { get; init; }
    public double Contributions { get; init; }
    public double InterestEarned { get; init; }
    public double FinalBalance { get; init; }
    public CompoundFrequency CompoundFrequency { get; init; }
    public List<Period> Periods { get; init; } = [];
    public int RunNumber { get; init; }
    public DateTime CalculationTime { get; init; }
}

public class ConsoleUI
{
    private List<SavingsBreakdown> SavingsBreakdowns { get; init; } = [];

    private static readonly char[] YesOptions = ['Y', 'y'];

    public void Run()
    {
        while (true)
        {
            ShowMenuHeader();
            PrintMenuOptions();
            Break();

            // make selection
            var key = PromptForKeyPress("Please make a selection");
            if (!int.TryParse(key.ToString(), out var selection) || !IsValidSelection(selection))
                continue;

            if (MenuOptions.Exit == (MenuOptions)selection - 1) return;

            var action = (MenuOptions)(selection - 1) switch
            {
                MenuOptions.APYCalculator => (Action)APYCalculator,
                MenuOptions.SavingsCalculator => (Action)SavingsCalculator,
                MenuOptions.SavingsBreakdowns => (Action)PrintBreakdowns,
                _ => (Action)delegate { },
            };
            action();
        }
    }

    private static bool IsValidSelection(int selection)
    {
        return selection >= 1 && selection <= Enum.GetValues<MenuOptions>().Length;
    }

    private static void PrintMenuOptions()
    {
        foreach (var option in Enum.GetValues<MenuOptions>())
        {
            Console.WriteLine($"[{(int)option + 1}] {GetMenuOptionLabel(option)}");
        }
    }

    private static void ShowMenuHeader()
    {
        Clear();
        Console.ForegroundColor = ConsoleColor.DarkGreen;
        Console.WriteLine("----------------------");
        Console.WriteLine("$$ Money Calculator $$");
        Console.WriteLine("----------------------");
        Console.ResetColor();
    }

    private void PrintBreakdowns()
    {
        Clear();
        foreach (var breakdown in SavingsBreakdowns)
        {
            PrintSavingsBreakdown(breakdown);
        }
        PromptForInput("Press any key to continue", "");
    }
    
    private static void PrintSavingsBreakdown(SavingsBreakdown breakdown)
    {
        Console.WriteLine($"==== Savings Breakdown #{breakdown.RunNumber} ({breakdown.CalculationTime:g}) ====");
        Console.WriteLine($"APY {breakdown.APY}%");
        Console.WriteLine($"Months: {breakdown.NumMonths}");
        Console.WriteLine($"Compound Frequency: {breakdown.CompoundFrequency}");
        Console.WriteLine($"Starting Balance: {breakdown.StartAmount:C2}");
        Console.WriteLine($"Contributions: {breakdown.Contributions:C2}");
        Console.WriteLine($"Interest Earned: {breakdown.InterestEarned:C2}");
        Console.WriteLine($"Final Balance: {breakdown.FinalBalance:C2}");
        Console.WriteLine();

        const string header = "Month | Start Balance | Contribution | Interest Earned | End Balance";
        Console.WriteLine(header);
        Console.WriteLine(new string('-', header.Length));

        int row = 0;
        foreach (var period in breakdown.Periods)
        {
            if (row % 2 == 1)
                Console.BackgroundColor = ConsoleColor.DarkGray;
            Console.WriteLine(
                $"{period.Month,5} | {period.StartingBalance,13:C2} | {period.Contribution,12:C2} | {period.InterestEarned,15:C2} | {period.EndingBalance,11:C2}");
            Console.ResetColor();
            row++;
        }
        Console.WriteLine();
    }
    
    private static void APYCalculator()
    {
        Clear();
        while (true)
        {
            Console.WriteLine("------------ APY Calculator ------------");

            var interestRate = PromptForInput<double>("Annual rate of return (percentage): ", 0.0);
            var ageOfAccountInMonths = PromptForInput<int>("Age of account in months (default 12): ", 12);
            var inputCompoundFrequency =
                PromptForInput<int>("Compound Frequency [1=Daily (default), 2=Monthly, 3=Yearly]: ", 1);
            var compoundFrequency = (CompoundFrequency)inputCompoundFrequency;

            interestRate /= 100;
            var apyPercentage =
                Calculator.CalculateAPYPercentage(compoundFrequency, ageOfAccountInMonths, interestRate);

            Break();
            Console.WriteLine($"APY: {apyPercentage}%");
            Console.WriteLine(
                $"A {interestRate * 100}% interest rate, compounded {compoundFrequency.ToString().ToLower()} for {ageOfAccountInMonths} months, yields a return of {apyPercentage}%");

            var shouldReRun = AskYesNoQuestion("Run Again? (y/n)");
            if (!shouldReRun) return;

            Console.WriteLine("\n");
        }
    }

    private void SavingsCalculator()
    {
        Clear();
        while (true)
        {
            Console.WriteLine("------------ Savings Calculator ------------");

            var startAmount = PromptForInput<double>("Starting Balance (in us dollars): $", 0);
            var monthlyContribution = PromptForInput<double>("Monthly Contribution: $", 0);
            var targetApy = PromptForInput<double>("Target APY (percentage): ", 0.0);
            var ageOfAccountInMonths = PromptForInput<int>("Age of account in months (default 12): ", 12);
            var inputCompoundFrequency =
                PromptForInput("Compound Frequency [1=Daily (default), 2=Monthly, 3=Yearly]: ", 1);
            var compoundFrequency = (CompoundFrequency)inputCompoundFrequency;

            targetApy /= 100;
            var interestRate = compoundFrequency switch
            {
                CompoundFrequency.Yearly => Calculator.CalculateAPR(targetApy, ageOfAccountInMonths / 12),
                CompoundFrequency.Monthly => Calculator.CalculateAPR(targetApy, ageOfAccountInMonths),
                _ => Calculator.CalculateAPR(targetApy, 365)
            };
            
            var currentBalance = startAmount;

            var periods = new List<Period>();
            var totalContributions = 0.0;
            for (var month = 1; month <= ageOfAccountInMonths; month++)
            {
                var monthStart = currentBalance;

                var currentMonth = ((DateTime.Now.Month - 1 + month - 1) % 12) + 1;
                var daysInMonth = DateTime.DaysInMonth(DateTime.Now.Year, currentMonth);

                for (var d = 0; d < daysInMonth; d++)
                {
                    var dailyInterestInDollars = (currentBalance * interestRate) / 365;
                    currentBalance += dailyInterestInDollars;
                }
                // if interestForMonth is calculated after currentBalance, make sure to subtract the monthlyContribution
                currentBalance += monthlyContribution;
                totalContributions += monthlyContribution;

                periods.Add(
                    new Period
                    {
                        Month = month,
                        StartingBalance = monthStart,
                        Contribution = monthlyContribution,
                        EndingBalance = currentBalance,
                        InterestEarned = currentBalance - monthlyContribution - monthStart,
                    });
            }
            var totalInterestEarned = currentBalance - ((monthlyContribution * ageOfAccountInMonths) + startAmount);

            var breakdown = new SavingsBreakdown
            {
                NumMonths = ageOfAccountInMonths,
                APY = targetApy * 100,
                StartAmount = startAmount,
                Contributions = totalContributions,
                InterestEarned = totalInterestEarned,
                FinalBalance = currentBalance,
                CompoundFrequency = compoundFrequency,
                Periods = periods,
                RunNumber = SavingsBreakdowns.Count + 1, 
                CalculationTime = DateTime.Now
            };
            SavingsBreakdowns.Add(breakdown);
            
            Console.WriteLine();
            // Console.WriteLine($"Compound Frequency: {compoundFrequency}");
            Console.WriteLine($"Actual APR: {Math.Round(interestRate * 100, 6)}%");
            Console.WriteLine($"Ending Balance: {(currentBalance):C2}");
            Console.WriteLine($"Interest Earned: {(totalInterestEarned):C2}");

            var shouldPrintBreakdown = PromptForBreakdown();
            if (shouldPrintBreakdown)
            {
                Break(2);
                PrintSavingsBreakdown(breakdown);
            }

            Break();
            var shouldReRun = AskYesNoQuestion("Run Again? (y/n)");
            if (!shouldReRun) return;
            Clear();
        }
    }

    private static string GetMenuOptionLabel(MenuOptions option)
    {
        var type = typeof(MenuOptions);
        var memInfo = type.GetMember(option.ToString());
        var attributes = memInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);
        return attributes.Length > 0 ? ((DescriptionAttribute)attributes[0]).Description : option.ToString();
    }

    private static bool AskYesNoQuestion(string prompt)
    {
        var key = PromptForKeyPress(prompt);
        return YesOptions.Contains(key);
    }

    private static bool PromptForBreakdown()
    {
        Break();
        return AskYesNoQuestion("Would you like to see a complete breakdown? (y/n)");
    }

    private static T PromptForInput<T>(string prompt, T defaultValue)
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

    private static char PromptForKeyPress(string prompt, bool intercept = true)
    {
        Console.Write(prompt);
        return Console.ReadKey(intercept: intercept).KeyChar;
    }

    private static void PrintBreakdownToConsole(List<Period> periods)
    {
        Console.WriteLine();
        const string header = "Month     Starting Balance     Interest Earned     Ending Balance";
        Console.WriteLine(header);
        Console.WriteLine(new string('=', header.Length));

        var row = 0;
        foreach (var p in periods)
        {
            row++;
            if (row % 2 == 0) Console.BackgroundColor = ConsoleColor.DarkGray;
            
            var period = p.Month.ToString().PadLeft(5);
            var start = p.StartingBalance.ToString("C2").PadLeft(21);
            var interestEarned = p.InterestEarned.ToString("C2").PadLeft(20);
            var end = p.EndingBalance.ToString("C2").PadLeft(19);

            Console.WriteLine($"{period}{start}{interestEarned}{end}");
            Console.ResetColor();
        }
    }

    private static void Clear()
    {
        Console.Clear();
    }

    private static void Break(int numBreaks = 1)
    {
        for (var i = 0; i < numBreaks; i++)
        {
            Console.WriteLine();
        }
    }
}
