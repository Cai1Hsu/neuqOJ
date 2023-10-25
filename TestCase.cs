namespace neuqOJ;

public class TestCase : IDisposable
{
    public string Name;
    public int Score;
    public FileStream InputStream = null!;
    public FileStream OutputStream = null!;

    public bool Loaded => InputStream != null && OutputStream != null; 

    public TestCase(string name, int score)
    {
        this.Name = name;
        this.Score = score;
    }

    public void BeginReadTestCase()
    {
        this.InputStream = File.OpenRead(this.GetInputFilename());
        this.OutputStream = File.OpenRead(this.GetOutputFilename());
    }

    private string GetInputFilename() => Name.Trim() + ".in";
    
    private string GetOutputFilename() => Name.Trim() + ".out";

    public void Dispose()
    {
        if (InputStream != null)
        {
            InputStream.Close();
            InputStream.Dispose();
        }

        if (OutputStream != null)
        {
            OutputStream.Close();
            OutputStream.Dispose();
        }
    }
}