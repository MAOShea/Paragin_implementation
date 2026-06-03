namespace Paragin.Shared.Models;

public sealed class GradingConfiguration
{
    public string? ActiveExamId { get; set; }

    public double MinPct { get; set; } = 0.2;
    public double PassPct { get; set; } = 0.7;
    public double MaxPct { get; set; } = 1.0;

    public double MinGrade { get; set; } = 1.0;
    public double PassGrade { get; set; } = 5.5;
    public double MaxGrade { get; set; } = 10.0;

    public static GradingConfiguration Default { get; } = new();
}
