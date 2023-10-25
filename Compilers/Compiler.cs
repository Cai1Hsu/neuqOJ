using System.Diagnostics;
using System.Reflection;
using neuqOJ.Runners;

namespace neuqOJ.Compilers;

public abstract class Compiler
{
    // Actually we should use a list
    public required List<string> FileExtensions;
    public abstract bool Supported { get; }
    public required List<string> AdditionalCompilerFlag;
    public required Runner runner;

    public abstract bool Compile(FileInfo SourceCode);

    public abstract string GetCompiled();

    public abstract bool IsSuitable(string extension);

    public static Compiler SelectSuitableCompiler(string extension)
    {
        var compilerType = typeof(Compiler);
        var subclasses = Assembly.GetExecutingAssembly().GetTypes()
            .Where(type => type.IsSubclassOf(compilerType) && !type.IsAbstract);

        foreach (var subclass in subclasses)
        {
            var suitableMethod = subclass.GetMethod("IsSuitable");

            // this should never happen
            Debug.Assert(suitableMethod != null);

            var instense = Activator.CreateInstance(subclass);

            var isSuitable = (bool)suitableMethod.Invoke(instense, new object[] { extension })!;

            if (isSuitable)
                return (Compiler)instense!;
        }

        throw new NoAvaliableCompilerFoundException();
    }
}

public class NoAvaliableCompilerFoundException : Exception
{
    
}