namespace FinancePercentagesCalc;

public class Calculator
{
    public static double CalculateAPY(int numberOfPeriods, double interestRate)
    {
        return Math.Pow(1 + interestRate / numberOfPeriods, numberOfPeriods) - 1;
    }
}
