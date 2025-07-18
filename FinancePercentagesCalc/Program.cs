// See https://aka.ms/new-console-template for more information

Console.WriteLine("------------ Savings Calculator ------------");
Console.WriteLine("A console application that calculates and displays the growth of a savings account with monthly contributions and daily-compounded interest");
Console.WriteLine();

var inputStartingBalance = Calculator.PromptForInput("Starting Balance (in us dollars): $");
var inputMonths = Calculator.PromptForInput("Months: ");
var inputInterestRate = Calculator.PromptForInput("Annual rate of return (percentage): ");
var inputMonthlyContribution = Calculator.PromptForInput("Monthly Contribution: $");

double.TryParse(inputStartingBalance, out var startAmount);
int.TryParse(inputMonths, out var months);

double.TryParse(inputInterestRate, out var interestRate);
interestRate /= 100;

double.TryParse(inputMonthlyContribution, out var monthlyContribution);
var currentBalance = startAmount;



var monthlyBreakdown = new List<Period>();
for (var month = 1; month <= months; month++)
{
    var monthStart = currentBalance;
    // AI generated ------
    int daysInMonth = DateTime.DaysInMonth(DateTime.Now.Year, ((DateTime.Now.Month - 1 + month - 1) % 12) + 1);
    // ---------
    var apy = Math.Pow(1 + interestRate / daysInMonth, daysInMonth) - 1;
    var holdValue = currentBalance + (currentBalance * apy); // just for debugging
    
    for (var d = 0; d < daysInMonth; d++)
    {
        var dailyInterestInDollars = (currentBalance * interestRate) / 365;
        currentBalance += dailyInterestInDollars;
    }
    var interestForMonth = currentBalance - monthStart;
    currentBalance += monthlyContribution;
    
    monthlyBreakdown.Add(
        new Period
        {
            Month = month,
            StartingBalance = monthStart,
            EndingBalance = currentBalance,
            InterestEarned = interestForMonth,
        });
}
double totalInterestEarned = currentBalance - ((monthlyContribution * months) + startAmount);

Console.WriteLine();
Console.WriteLine($"Ending Balance: {(currentBalance):C2}");
Console.WriteLine($"Interest Earned: {(totalInterestEarned):C2}");
Console.WriteLine();
Console.WriteLine("Would you like to see a complete breakdown? (y/n) ");

if (Console.ReadKey(intercept: true).KeyChar == 'y' || Console.ReadKey(intercept: true).KeyChar == 'Y')
{
    Console.WriteLine();
    Console.WriteLine("Month      Starting Balance      Interest Earned      Ending Balance");
    Console.WriteLine("====================================================================");

    foreach (var p in monthlyBreakdown)
    {
        var period = p.Month.ToString().PadRight(11);
        var start = p.StartingBalance.ToString("C2").PadLeft(16);
        // var start = p.StartingBalance.ToString("C2").PadRight(22);
        var interestEarned = p.InterestEarned.ToString("C2").PadLeft(21);
        var end = p.EndingBalance.ToString("C2").PadLeft(18);

        Console.WriteLine($"{period}{start}{interestEarned}{end}");
    }
}

internal struct Period
{
    public int Month;
    public double StartingBalance;
    public double EndingBalance;
    public double InterestEarned;
}

internal class Calculator
{
    public static string PromptForInput(string prompt)
    {
        Console.Write(prompt);
        string? input = Console.ReadLine();
        return string.IsNullOrEmpty(input) ? "0" : input;
    }
}