using System;
using System.Diagnostics;
using System.Linq;

RandomAnswers answers = new();

int next = Random.Shared.Next(0, answers.Count - 1);
string response = args.Length is 0
    ? "You must ask a question to be provided an answer."
    : $"The answer to your question: \"{args[0]}\" is '{answers[next]}' on ({Environment.MachineName})";

Console.WriteLine(response);
Debug.WriteLine(response);
