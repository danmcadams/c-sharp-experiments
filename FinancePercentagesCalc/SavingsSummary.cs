namespace FinancePercentagesCalc;

internal class SavingsSummary
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