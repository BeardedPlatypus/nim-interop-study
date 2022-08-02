using System.Text;
using function_export.main;

int nameSize = NimSource.get_name_size();
StringBuilder buffer = new(nameSize);
NimSource.get_name(buffer);

Console.WriteLine($"nim says: my name is '{buffer}'");

double requestValue()
{
    Console.WriteLine("Please enter a value:");
    while (true)
    {
        string v = Console.ReadLine().Trim();

        if (double.TryParse(v, out double parsedValue)) return parsedValue;
        Console.WriteLine($"'{v}' is not a valid value, please enter a valid value:");
    }
}

double a = requestValue();
double b = requestValue();


Console.WriteLine($"Calculating '{a}' + '{b}':");
double nimResult = NimSource.add(a, b);
Console.WriteLine($"  nim: {nimResult}");
double cResult = NimSource.cadd(a, b);
Console.WriteLine($"  c: {cResult}");

