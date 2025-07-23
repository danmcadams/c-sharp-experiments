using System.ComponentModel;

namespace FinancePercentagesCalc;

internal enum MenuOptions
{
    [Description("APY Calculator")]
    APYCalculator,
    [Description("Savings Calculator")]
    SavingsCalculator,
    [Description("Another Option")]
    AnotherOption,
}

public class ConsoleUI
{
    private static void Clear()
    {
        Console.Clear();
    }
    
    private static string GetMenuOptionLabel(MenuOptions option)
    {
        var type = typeof(MenuOptions);
        var memInfo = type.GetMember(option.ToString());
        var attributes = memInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);
        return attributes.Length > 0 ? ((DescriptionAttribute)attributes[0]).Description : option.ToString();
    }
    
    public static int Run()
    {
        var showMenu = true;
        
        while (showMenu)
        {
            Clear();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("----------------------");
            Console.WriteLine("$$ Money Calculator $$");
            Console.WriteLine("----------------------");
            Console.ResetColor();
            Console.WriteLine();
            
            // set Exit option
            var numOptions = Enum.GetValues<MenuOptions>().Length;
            var exitOption = numOptions + 1;
            
            // print options
            foreach (MenuOptions option in Enum.GetValues<MenuOptions>())
            {
                Console.WriteLine($"[{(int) option + 1}] {GetMenuOptionLabel(option)}");
            }
            Console.WriteLine($"[{(exitOption)}] Exit");

            PrintBreak();
            
            
            var key = PromptForKeyPress("Please make a selection ");
            if (!int.TryParse(key.ToString(), out int selection) || selection < 1 || selection > exitOption)
                continue;
            
            if (selection == exitOption)
            {
                showMenu = false;
                continue;
            }

            var action = (MenuOptions)(selection - 1) switch
            {
                MenuOptions.APYCalculator => (Func<int>)APYCalculator,
                MenuOptions.SavingsCalculator => (Func<int>)SavingsCalculator,
                _ => () => 0
            };
            Clear();
            action();
        }

        return 0;
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
            var apyPercentage = Math.Round(apy * 100, 4);

            Console.BackgroundColor = ConsoleColor.Green;
            Console.ForegroundColor = ConsoleColor.Black;
            Console.WriteLine(apyPercentage + "%");
            Console.ResetColor();

            var key = PromptForKeyPress("Run Again? (y/n) ");
            if (!(new[] { 'Y', 'y' }.Contains(key))) return -1;

            Console.WriteLine("\n");
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
        var apyPercentage = Math.Round(apy * 100, 4);

        Console.BackgroundColor = ConsoleColor.Green;
        Console.ForegroundColor = ConsoleColor.Black;
        Console.WriteLine(apyPercentage + "%");
        Console.ResetColor();
        
        PromptForKeyPress("Press any key to exit");
        // no errors 
        return -1;
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

    private static void PrintBreak(int numBreaks = 1)
    {
        for (var i = 0; i < numBreaks; i++)
        {
            Console.WriteLine();
        }
    }
}
