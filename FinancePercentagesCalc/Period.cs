namespace FinancePercentagesCalc;

public readonly struct Period
{
    public int Month { get; init; }
    public double StartingBalance { get; init; }
    public double EndingBalance { get; init; }
    public double InterestEarned { get; init; }
    public double Contribution { get; init; }

    public string GetMonthText()
    {
        return Month.ToString();
    }
}