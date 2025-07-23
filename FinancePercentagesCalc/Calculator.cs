namespace FinancePercentagesCalc;

public static class Calculator
{
    public static double CalculateAPY(int numberOfPeriods, double interestRate)
    {
        return Math.Pow(1 + interestRate / numberOfPeriods, numberOfPeriods) - 1;
    }

    public static void CalculateAPR()
    {
    }

    public static double CalculateAPYPercentage(CompoundFrequency compoundFrequency, int ageOfAccountInMonths, double interestRate)
    {
        var apy = compoundFrequency switch
        {
            CompoundFrequency.Yearly => CalculateAPY(ageOfAccountInMonths / 12, interestRate),
            CompoundFrequency.Monthly => CalculateAPY(ageOfAccountInMonths, interestRate),
            _ => CalculateAPY(365, interestRate)
        };
        var apyPercentage = Math.Round(apy * 100, 4);
        return apyPercentage;
    }
}
