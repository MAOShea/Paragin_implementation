using System.Text;
using Paragin.Shared.Models;

namespace Paragin.Shared.Parsing;

public static class ExamCsvParser
{
    private const string MaxScoreLabel = "Max question score:";

    public static ExamDataset Parse(string id, string name, Stream stream)
    {
        using var reader = new StreamReader(stream, new UTF8Encoding(encoderShouldEmitUTF8Identifier: false, throwOnInvalidBytes: true), detectEncodingFromByteOrderMarks: true);

        var headerLine = reader.ReadLine() ?? throw new InvalidDataException("CSV is missing the header row.");
        var maxLine = reader.ReadLine() ?? throw new InvalidDataException("CSV is missing the per-question maxima row.");

        var headers = SplitCsvLine(headerLine);
        if (headers.Length < 2)
            throw new InvalidDataException("CSV header row must include a student identifier and at least one question column.");

        var maxFields = SplitCsvLine(maxLine);
        if (!maxFields[0].Trim().Equals(MaxScoreLabel, StringComparison.OrdinalIgnoreCase))
            throw new InvalidDataException($"Row 2 must start with '{MaxScoreLabel}'.");

        var questionCount = headers.Length - 1;
        if (maxFields.Length - 1 < questionCount)
            throw new InvalidDataException("Per-question maxima row is shorter than the number of question columns.");

        var maxima = new double[questionCount];
        for (var i = 0; i < questionCount; i++)
        {
            if (!ScoreParser.TryParseScore(maxFields[i + 1], out maxima[i]))
                throw new InvalidDataException($"Invalid maximum score for question {i + 1}.");
        }

        var studentIds = new List<string>();
        var rawScores = new List<IReadOnlyList<string>>();

        string? line;
        while ((line = reader.ReadLine()) is not null)
        {
            if (string.IsNullOrWhiteSpace(line))
                continue;

            var fields = SplitCsvLine(line);
            if (fields.Length == 0)
                continue;

            var scores = new string[questionCount];
            for (var i = 0; i < questionCount; i++)
                scores[i] = i + 1 < fields.Length ? fields[i + 1] : string.Empty;

            studentIds.Add(fields[0].Trim());
            rawScores.Add(scores);
        }

        return new ExamDataset
        {
            Id = id,
            Name = name,
            StudentIds = studentIds,
            RawScoresByStudent = rawScores,
            QuestionMaxima = maxima
        };
    }

    public static ExamDataset ParseFile(string id, string name, string path)
    {
        using var stream = File.OpenRead(path);
        return Parse(id, name, stream);
    }

    private static string[] SplitCsvLine(string line) =>
        line.Split(',', StringSplitOptions.TrimEntries);
}
