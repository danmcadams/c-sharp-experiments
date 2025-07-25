namespace FinancePercentagesCalc;

public static class Calculator
{
    public static double CalculateAPY(double interestRate, int numberOfPeriods)
    {
        return Math.Pow(1 + interestRate / numberOfPeriods, numberOfPeriods) - 1;
    }

    public static double CalculateAPR(double apy, int compoundingPeriodsPerYear)
    {
        return compoundingPeriodsPerYear * (Math.Pow(1 + apy, 1.0 / compoundingPeriodsPerYear)-1);
    }

    public static double CalculateAPYPercentage(CompoundFrequency compoundFrequency, int ageOfAccountInMonths, double interestRate)
    {
        var apy = compoundFrequency switch
        {
            CompoundFrequency.Yearly => CalculateAPY(interestRate,ageOfAccountInMonths / 12),
            CompoundFrequency.Monthly => CalculateAPY(interestRate, ageOfAccountInMonths),
            _ => CalculateAPY(interestRate, 365)
        };
        var apyPercentage = Math.Round(apy * 100, 4);
        return apyPercentage;
    }
}
