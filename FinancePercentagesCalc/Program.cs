using System.ComponentModel;
using FinancePercentagesCalc;

MenuOption? GetMenuSelection()
{
    Console.Clear();
    Console.WriteLine("Finance Percentages Calculator");
    Console.WriteLine("------------------------------");
    foreach (MenuOption option in Enum.GetValues(typeof(MenuOption)))
    {
        if (option == MenuOption.Quit)
            Console.WriteLine("Q. " + GetEnumDescription(option));
        else
            Console.WriteLine($"{(int)option}. {GetEnumDescription(option)}");
    }
    Console.WriteLine();
    Console.Write("Select an option: ");

    var key = Console.ReadKey(true).Key;
    switch (key)
    {
        case ConsoleKey.D1:
        case ConsoleKey.NumPad1:
            return MenuOption.GeneralCalculator;
        case ConsoleKey.D2:
        case ConsoleKey.NumPad2:
            return MenuOption.SavingsCalculator;
        case ConsoleKey.Q:
            return MenuOption.Quit;
        default:
            return null;
    }
}

bool running = true;
while (running)
{
    var selection = GetMenuSelection();
    switch (selection)
    {
        case MenuOption.SavingsCalculator:
            var calcUI = new CalcUI();
            calcUI.Run();
            break;
        case MenuOption.Quit:
            running = false;
            break;
        default:
            Console.WriteLine("Invalid selection. Press any key to try again...");
            Console.ReadKey();
            break;
    }
}

Console.WriteLine();
Console.WriteLine("Goodbye.");
Console.WriteLine();

string GetEnumDescription(MenuOption option)
{
    var fi = option.GetType().GetField(option.ToString());
    var attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);
    return attributes.Length > 0 ? attributes[0].Description : option.ToString();
}

enum MenuOption
{
    [Description("General Calculator")]
    GeneralCalculator = 1,
    [Description("Savings Calculator")]
    SavingsCalculator = 2,
    [Description("Quit")]
    Quit = 3
}