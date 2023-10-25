using neuqOJ.Runners;

namespace neuqOJ;

public class ExecutableFile
{
    public Runner? runner;

    public string? executable;

    public ExecutableFile(Runner? runner, string? executable)
    {
        this.runner = runner;
        this.executable = executable;
    }

    public bool IsRunnable() => runner != null && executable != null;
}