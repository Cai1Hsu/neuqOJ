using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using neuqOJ.Runners;
using neuqOJ.Utils;

namespace neuqOJ.Compilers;

public class GppCompiler : Compiler
{
    private static string executableFile = "a.out";
    
    // Currently we assume we have g++
    public override bool Supported { get; } = true;
    public List<string> CompilerFlag = new() { "-o", Path.Combine(Environment.CurrentDirectory, executableFile) };
    
    private const string COMPILER = "g++";

    [SetsRequiredMembers]
    public GppCompiler()
    {
        runner = new CppRunner(executableFile);
        FileExtensions = new(){".cpp"};
        AdditionalCompilerFlag = new List<string>();
    }

    public override bool Compile(FileInfo SourceCode)
    {
        CompilerFlag.Insert(0, SourceCode.FullName.Trim());
        
        var args = CompilerFlag;
        
        var p = Process.Start(COMPILER, CompilerFlag);

        p.WaitForExit();
        
        if (p.ExitCode != 0)
        {
            throw new OperationCanceledException($"Failed to compile source code");
        }

        return true;
    }

    public override string GetCompiled() => executableFile;

    /// <summary>
    /// Used for selecting correctly compilers
    /// </summary>
    /// <returns></returns>
    public override bool IsSuitable(string extension) => FileExtensions.Any(e => extension.ToLower().Trim().EndsWith(e));
}