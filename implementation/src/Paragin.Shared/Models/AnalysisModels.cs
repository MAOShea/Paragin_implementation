namespace Paragin.Shared.Models;

public sealed class ExamAnalysis
{
    public required ExamDataset Dataset { get; init; }
    public required GradingConfiguration Configuration { get; init; }
    public required IReadOnlyList<StudentResult> Students { get; init; }
    public required IReadOnlyList<ItemAnalytic> Items { get; init; }
    public required DataChecksSummary DataChecks { get; init; }
}

public sealed class StudentResult
{
    public required string StudentId { get; init; }
    public required double TotalScore { get; init; }
    public required double MaxTotalScore { get; init; }
    public required double Percentage { get; init; }
    public required double Grade { get; init; }
    public required bool Passed { get; init; }
    public required string PassFail { get; init; }
    public required string DataCheckMessage { get; init; }
    public required bool HasScoreErrors { get; init; }
}

public sealed class ItemAnalytic
{
    public required int QuestionNumber { get; init; }
    public required double MaxScore { get; init; }
    public required double AverageScore { get; init; }
    public double? PPrime { get; init; }
    public double? Rit { get; init; }
    public string? DifficultyLabel { get; init; }
    public string? DiscriminationLabel { get; init; }
}

public sealed class DataChecksSummary
{
    public required string ActiveExamName { get; init; }
    public required int StudentCount { get; init; }
    public required int QuestionCount { get; init; }
    public required int StudentsWithScoreErrors { get; init; }
    public required int InvalidMaximumScores { get; init; }
    public required int InvalidStudentScores { get; init; }
    public required int PPrimeOutOfRange { get; init; }
    public required int GradesOutOfRange { get; init; }
    public required int PassCount { get; init; }
    public required int FailCount { get; init; }
    public required string StatusMessage { get; init; }
}

public sealed class DifficultyBandCounts
{
    public int TooHard { get; init; }
    public int Ok { get; init; }
    public int TooEasy { get; init; }
}

public sealed class DiscriminationBandCounts
{
    public int Weak { get; init; }
    public int Review { get; init; }
    public int Good { get; init; }
}
