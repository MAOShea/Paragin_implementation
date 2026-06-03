using Paragin.Shared.Models;
using Paragin.Shared.Parsing;

namespace Paragin.Shared.Services;

public static class StudentValidator
{
    public static IReadOnlyList<int> GetFlaggedQuestions(IReadOnlyList<string> rawScores, IReadOnlyList<double> maxima)
    {
        var flagged = new List<int>();

        for (var i = 0; i < maxima.Count; i++)
        {
            var raw = i < rawScores.Count ? rawScores[i] : string.Empty;
            if (string.IsNullOrWhiteSpace(raw))
                continue;

            if (!ScoreParser.TryParseScore(raw, out var score))
            {
                flagged.Add(i + 1);
                continue;
            }

            if (score < 0 || score > maxima[i])
                flagged.Add(i + 1);
        }

        return flagged;
    }

    public static string FormatDataCheckMessage(IReadOnlyList<int> flaggedQuestions)
    {
        if (flaggedQuestions.Count == 0)
            return "OK";

        return "Check Q: " + string.Join(", ", flaggedQuestions);
    }

    public static int CountInvalidStudentScores(ExamDataset dataset)
    {
        var count = 0;
        for (var studentIndex = 0; studentIndex < dataset.StudentCount; studentIndex++)
        {
            var rawScores = dataset.RawScoresByStudent[studentIndex];
            for (var questionIndex = 0; questionIndex < dataset.QuestionCount; questionIndex++)
            {
                var raw = questionIndex < rawScores.Count ? rawScores[questionIndex] : string.Empty;
                if (string.IsNullOrWhiteSpace(raw))
                    continue;

                if (!ScoreParser.TryParseScore(raw, out var score))
                {
                    count++;
                    continue;
                }

                if (score < 0 || score > dataset.QuestionMaxima[questionIndex])
                    count++;
            }
        }

        return count;
    }
}
