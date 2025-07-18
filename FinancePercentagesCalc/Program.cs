// See https://aka.ms/new-console-template for more information

Console.WriteLine("------------ Savings Calculator ------------");
Console.WriteLine("This program will calculate the interest earned on a savings account over a specified number of months.");
Console.WriteLine();

internal class Calculator
{
    public static string PromptForInput(string prompt)
    {
        Console.Write(prompt);
        string? input = Console.ReadLine();
        return string.IsNullOrEmpty(input) ? "0" : input;
    }
}

var inputStartingBalance = Calculator.PromptForInput("Starting Balance (in us dollars): $");
var inputMonths = PromptForInput("Months: ");
var inputInterestRate = PromptForInput("Interest Rate (percentage): ");
var inputMonthlyContribution = PromptForInput("Monthly Contribution: $");

double.TryParse(inputStartingBalance, out var startAmount);
int.TryParse(inputMonths, out var months);

double.TryParse(inputInterestRate, out var interestRate);
interestRate /= 100;

double.TryParse(inputMonthlyContribution, out var monthlyContribution);
var currentValue = startAmount;



var monthlyBreakdown = new List<Period>();
for (var month = 1; month <= months; month++)
{
    var monthStart = currentValue;
    for (var d = 0; d < 30; d++)
    {
        var dailyInterest = (currentValue * interestRate) / 365;
        currentValue += dailyInterest;
    }
    var interestForMonth = currentValue - monthStart;
    currentValue += monthlyContribution;
    
    monthlyBreakdown.Add(
        new Period
        {
            Month = month,
            StartingBalance = monthStart,
            EndingBalance = currentValue,
            InterestEarned = interestForMonth,
        });
}

Console.WriteLine();
double totalInterestEarned = currentValue - ((monthlyContribution * months) + startAmount);
Console.WriteLine($"Ending Balance: {(currentValue).ToString("C2")}");
Console.WriteLine("Interest Earned: " + totalInterestEarned.ToString("C2"));
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