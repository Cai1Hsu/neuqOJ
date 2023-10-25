// this file is for debug only
// and it presents a simple way of using this project
// 
// The whole project is licensed under MIT Licence
using neuqOJ;

if (args.Length < 1)
    throw new ArgumentNullException("Please pass the path where the testcase is");

if (!Directory.Exists(args.First()))
    throw new DirectoryNotFoundException();

Problem problem = new Problem(args.First());

Console.WriteLine("Compling and judging...");
problem.AutoJudge();

int score = problem.Score;
if (score == 100)
    Console.ForegroundColor = ConsoleColor.Green;
else if (score == 0)
    Console.ForegroundColor = ConsoleColor.Red;
else
    Console.ForegroundColor = ConsoleColor.Yellow;

Console.WriteLine($"Scores : {score}");

Console.ResetColor();