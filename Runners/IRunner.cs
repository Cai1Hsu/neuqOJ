using System.Diagnostics;
using neuqOJ.Compilers;

namespace neuqOJ.Runners;

public abstract class Runner
{
    private string _executable;

    public Runner(string executable)
    {
        this._executable = executable;
    }
        
    public abstract Process Run(FileInfo executable);
}