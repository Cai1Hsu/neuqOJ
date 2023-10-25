using System.Diagnostics;
using System.Text.Json.Serialization;
using neuqOJ.Compilers;

namespace neuqOJ;

public class Problem
{
    private const int TIME_LIMIT = 1000;
    private const string FAILEDCOMPILE = "#### FAILED ####";
    private const string SCORES_FILE = "scores.txt";

    public List<TestCase> TestCases;

    public bool Judged = false;

    private int _score = 0;

    public int Score
    {
        get
        {
            if (!Judged)
                throw new InvalidOperationException("Please judge before access score.");

            return _score;
        }
    }

    private DirectoryInfo _directory;

    public Problem(string directory)
    {
        this._directory = new DirectoryInfo(directory);

        if (!Directory.Exists(directory))
            throw new DirectoryNotFoundException($"Directory not found : {directory}");

        var path = Path.Combine(directory, SCORES_FILE);

        if (!File.Exists(path))
            throw new FileNotFoundException($"File not found : {path}");

        TestCases = File.ReadAllLines(path).Select(l =>
        {
            var splited = l.Split(' ');
            return new TestCase(splited[0], int.Parse(splited[1]));
        }).ToList();
    }

    public readonly string[] IgnoredExtensions = { ".in", ".out", ".txt" };

    public ExecutableFile CompileSolution()
    {
        foreach (var file in Directory.GetFiles(_directory.FullName))
        {
            if (IgnoredExtensions.Any(e => file.ToLower().EndsWith(e)))
                continue;

            try
            {
                var compiler = Compiler.SelectSuitableCompiler(file);

                var success = compiler.Compile(new FileInfo(file));

                if (success)
                {
                    return new ExecutableFile(compiler.runner, compiler.GetCompiled());
                }
                else
                {
                    return new ExecutableFile(compiler.runner, FAILEDCOMPILE);
                }
            }
            catch (Exception)
            {
                // ignored
            }
        }

        return new ExecutableFile(null, null);
    }

    /// <summary>
    /// This method judge the solution with testCase and return the time
    /// if the executable runs overtime, we return -1
    /// if the solution runs incorrectly we return -2
    /// </summary>
    /// <param name="exe"></param>
    /// <param name="testCase"></param>
    /// <returns>Cost time in ms</returns>
    /// <exception cref="Exception"></exception>
    private long JudgeTestCase(ExecutableFile exe, TestCase testCase)
    {
        try
        {
            testCase.BeginReadTestCase();
        }
        catch (Exception ex)
        {
            // Failed to load TestCase
            //
            // Currently i dont want to handle it
            throw new Exception(ex.Message);
        }
        
        var input = testCase.InputStream;

        var expectOutputReader = new StreamReader(testCase.OutputStream);

        JudgeResult result = JudgeResult.Unfinished; 
        Stopwatch sw = new Stopwatch();

        sw.Start();
        Process p = exe.runner!.Run(new FileInfo(exe.executable!));
        
        Console.Write("Output:\"");
        
        p.OutputDataReceived += (_, args) =>
        {
            Console.Write(args.Data);

            if (args.Data != expectOutputReader.ReadLine())
            {
                result = JudgeResult.WrongAnswer;
                p.Kill();
            }
        };

        p.BeginOutputReadLine();

        using (var stdin = new StreamWriter(p.StandardInput.BaseStream))
        {
            input.CopyTo(stdin.BaseStream);
            
            stdin.Flush();
        }

        p.WaitForExit(TIME_LIMIT);

        p.WaitForExit();
        
        sw.Stop();
        Console.WriteLine("\"");

        if (result == JudgeResult.TimeLimitExceeded)
            return -1;

        if (result == JudgeResult.WrongAnswer)
            return -2;

        return sw.ElapsedMilliseconds;
    }

    public void BeginJudge(ExecutableFile executableFile)
    {
        foreach (var t in TestCases)
        {
            if (JudgeTestCase(executableFile, t) > -1)
                _score += t.Score;

            t.Dispose();
        }

        Judged = true;
    }

    public void AutoJudge()
    {
        Environment.CurrentDirectory = _directory.FullName;

        var executableFile = CompileSolution();

        if (executableFile == null || executableFile.executable == null)
            throw new NoSupportedSolutionFoundException();

        if (executableFile.executable == FAILEDCOMPILE)
            throw new CompileFailedException();

        if (!File.Exists(executableFile.executable))
            throw new Exception("Unknown exception occurred : executable file not found.");

        if (!executableFile.IsRunnable())
            throw new Exception("Unknowed exception occurred : Failed to run executable file.");

        BeginJudge(executableFile);
    }
}

public class NoSupportedSolutionFoundException : Exception
{

}

public class CompileFailedException : Exception
{

}

public class WrongAnswerException : Exception
{

}

public enum JudgeResult
{
    WrongAnswer,
    TimeLimitExceeded,
    Unfinished
}