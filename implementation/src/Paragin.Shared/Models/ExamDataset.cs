namespace Paragin.Shared.Models;

public sealed class ExamDataset
{
    public required string Id { get; init; }
    public required string Name { get; init; }
    public required IReadOnlyList<string> StudentIds { get; init; }
    public required IReadOnlyList<IReadOnlyList<string>> RawScoresByStudent { get; init; }
    public required IReadOnlyList<double> QuestionMaxima { get; init; }

    public int StudentCount => StudentIds.Count;
    public int QuestionCount => QuestionMaxima.Count;
    public double MaxTotalScore => QuestionMaxima.Sum();
}
