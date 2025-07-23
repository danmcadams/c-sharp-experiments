namespace FinancePercentagesCalc;

public class ConsoleUI
{
    public static int Run()
    {
        return APYCalculator();
    }

    private static int APYCalculator()
    {
        while (true)
        {
            Console.WriteLine("------------ APY Calculator ------------");

            var interestRate = ConsoleUI.PromptForInput<double>("Annual rate of return (percentage): ", 0.0);
            var ageOfAccount = PromptForInput<int>("Age of account in months (default 12): ", 12);
            var inputCompoundFrequency = ConsoleUI.PromptForInput<int>("Compound Frequency [1=Daily (default), 2=Monthly, 3=Yearly]: ", 1);
            var compoundFrequency = (CompoundFrequency)inputCompoundFrequency;

            interestRate /= 100;
            var apy = compoundFrequency switch
            {
                CompoundFrequency.Yearly => Calculator.CalculateAPY(ageOfAccount / 12, interestRate),
                CompoundFrequency.Monthly => Calculator.CalculateAPY(ageOfAccount, interestRate),
                _ => Calculator.CalculateAPY(365, interestRate)
            };
            var apyPercentage = Math.Round(apy * 100, 2);

            Console.BackgroundColor = ConsoleColor.Green;
            Console.ForegroundColor = ConsoleColor.Black;
            Console.WriteLine(apyPercentage + "%");
            Console.ResetColor();

            var key = PromptForKeyPress("Press almost any key to exit. (press R to run again)");
            if (!(new[] { 'R', 'r' }.Contains(key))) return 0;

            Console.Clear();
        }
    }

    private static int SavingsCalculator()
    {
        Console.WriteLine("------------ Savings Calculator ------------");
        Console.WriteLine("A console application that calculates and displays the growth of a savings account with monthly contributions and daily-compounded interest");
        Console.WriteLine();

        var startAmount = ConsoleUI.PromptForInput<double>("Starting Balance (in us dollars): $", 0);
        var interestRate = ConsoleUI.PromptForInput<double>("Annual rate of return (percentage): ", 0.0);
        var monthlyContribution = ConsoleUI.PromptForInput<double>("Monthly Contribution: $", 0);
        var ageOfAccount = PromptForInput<int>("Age of account in months (default 12): ", 12);
        var inputCompoundFrequency = ConsoleUI.PromptForInput<int>("Compound Frequency [1=Daily (default), 2=Monthly, 3=Yearly]: ", 1);
        var compoundFrequency = (CompoundFrequency)inputCompoundFrequency;

        interestRate /= 100;
        var apy = compoundFrequency switch
        {
            CompoundFrequency.Yearly => Calculator.CalculateAPY(ageOfAccount / 12, interestRate),
            CompoundFrequency.Monthly => Calculator.CalculateAPY(ageOfAccount, interestRate),
            _ => Calculator.CalculateAPY(365, interestRate)
        };
        var apyPercentage = Math.Round(apy * 100, 2);

        Console.BackgroundColor = ConsoleColor.Green;
        Console.ForegroundColor = ConsoleColor.Black;
        Console.WriteLine(apyPercentage + "%");
        Console.ResetColor();

        Console.WriteLine("Press any key to exit.");
        Console.ReadKey();
        // no errors 
        return 0;
    }
    
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

    public static char PromptForKeyPress(string prompt, bool intercept = true)
    {
        Console.Write(prompt);
        return Console.ReadKey(intercept: intercept).KeyChar;
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
