namespace TestDrivenTestingFramework;

class ConsoleReporter
{
    private readonly IEnumerable<TestResult> _results;

    public ConsoleReporter(IEnumerable<TestResult> results)
    {
        _results = results;
    }

    public void Report()
    {
        foreach(var result in _results)
        {
            if (result.Status == TestStatus.Failed)
            {
                Console.ForegroundColor = ConsoleColor.Red;
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.DarkGreen;
            }
            Console.WriteLine($"{result.Scope}.{result.Name}: {result.Status}");
            if (result.Status == TestStatus.Failed)
            {
                if (result.Error is AggregateException ae)
                {
                    foreach (var error in ae.InnerExceptions)
                    {
                        Console.WriteLine(error);
                    }
                }
                else
                {
                    Console.WriteLine(result.Error);
                }
            }
            Console.ResetColor();
        }
    }
}