using System.Diagnostics;

namespace neuqOJ.Runners;

public class CppRunner : Runner
{
    public override Process Run(FileInfo executable)
        => Process.Start(new ProcessStartInfo(executable.FullName)
        {
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardInput = true,
        })!;

    public CppRunner(string executable) : base(executable)
    {
        
    }
}