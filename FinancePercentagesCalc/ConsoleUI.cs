using System.ComponentModel;

namespace FinancePercentagesCalc;

internal enum MenuOptions
{
    [Description("APY Calculator")] APYCalculator,
    // [Description("APR Calculator")] APRCalculator,
    [Description("Savings Calculator")] SavingsCalculator,
    AnotherSelection,
    [Description("And Another")] AndAnother,
    [Description("Exit")] Exit,
}
public class ConsoleUI
{
    public static void Run()
    {
        while (true)
        {
            ShowMenuHeader();
            PrintMenuOptions();
            Break();

            // make selection
            var key = PromptForKeyPress("Please make a selection ");
            if (!int.TryParse(key.ToString(), out int selection) || !IsValidSelection(selection))
                continue;

            if (MenuOptions.Exit == (MenuOptions)selection - 1) return;

            var action = (MenuOptions)(selection - 1) switch
            {
                MenuOptions.APYCalculator => (Action)APYCalculator,
                MenuOptions.SavingsCalculator => (Action)SavingsCalculator,
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

            var shouldReRun = AskYesNoQuestion("Run Again? (y/n) ");
            if (!shouldReRun) return;

            Console.WriteLine("\n");
        }
    }

    private static void SavingsCalculator()
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
                PromptForInput<int>("Compound Frequency [1=Daily (default), 2=Monthly, 3=Yearly]: ", 1);
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
                // AI generated ------
                var daysInMonth =
                    DateTime.DaysInMonth(DateTime.Now.Year, ((DateTime.Now.Month - 1 + month - 1) % 12) + 1);

                for (var d = 0; d < daysInMonth; d++)
                {
                    var dailyInterestInDollars = (currentBalance * interestRate) / 365;
                    currentBalance += dailyInterestInDollars;
                }
                // if interestForMonth is calculated after currentBalance, make sure to subject monthlyContribution
                currentBalance += monthlyContribution;
                totalContributions += monthlyContribution;

                periods.Add(
                    new Period
                    {
                        Month = month,
                        StartingBalance = monthStart,
                        EndingBalance = currentBalance,
                        InterestEarned = currentBalance - monthlyContribution - monthStart,
                    });
            }

            double totalInterestEarned = currentBalance - ((monthlyContribution * ageOfAccountInMonths) + startAmount);

            Console.WriteLine();
            // Console.WriteLine($"Compound Frequency: {compoundFrequency}");
            Console.WriteLine($"Actual APR: {Math.Round(interestRate * 100, 6)}%");
            Console.WriteLine($"Ending Balance: {(currentBalance):C2}");
            Console.WriteLine($"Interest Earned: {(totalInterestEarned):C2}");

            var shouldPrintBreakdown = PromptForBreakdown();
            if (shouldPrintBreakdown)
            {
                Break();
                PrintBreakdownToConsole(periods);
                Break();
                Console.WriteLine($"Starting Balance: {startAmount:C2}");
                Console.WriteLine($"Contributions: {totalContributions:C2}");
                Console.WriteLine($"Interest Earned: {totalInterestEarned:C2}");
                // Console.ForegroundColor = ConsoleColor.Green;
                // Console.BackgroundColor = ConsoleColor.Black;
                Console.WriteLine($"Ending Balance: {currentBalance:C2}");
                Console.ResetColor();
            }

            Break();
            var shouldReRun = AskYesNoQuestion("Run Again? (y/n) ");
            if (!shouldReRun) return;

            Console.WriteLine("\n");
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
        return new[] { 'Y', 'y' }.Contains(key);
    }

    private static bool PromptForBreakdown()
    {
        Break();
        return AskYesNoQuestion("Would you like to see a complete breakdown? (y/n) ");
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
        Console.WriteLine("Month      Starting Balance      Interest Earned      Ending Balance");
        Console.WriteLine("====================================================================");

        var row = 0;
        foreach (var p in periods)
        {
            row++;
            if (row % 2 == 0) Console.BackgroundColor = ConsoleColor.DarkGray;
            
            var period = p.Month.ToString().PadRight(11);
            var start = p.StartingBalance.ToString("C2").PadLeft(16);
            var interestEarned = p.InterestEarned.ToString("C2").PadLeft(21);
            var end = p.EndingBalance.ToString("C2").PadLeft(20);

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
