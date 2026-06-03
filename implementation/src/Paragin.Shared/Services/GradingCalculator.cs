using Paragin.Shared.Models;

namespace Paragin.Shared.Services;

public static class GradingCalculator
{
    public static double CalculateGrade(double percentage, GradingConfiguration config)
    {
        double grade;
        if (percentage <= config.MinPct)
            grade = config.MinGrade;
        else if (percentage <= config.PassPct)
            grade = config.MinGrade + (percentage - config.MinPct) * (config.PassGrade - config.MinGrade) / (config.PassPct - config.MinPct);
        else if (percentage <= config.MaxPct)
            grade = config.PassGrade + (percentage - config.PassPct) * (config.MaxGrade - config.PassGrade) / (config.MaxPct - config.PassPct);
        else
            grade = config.MaxGrade;

        return Math.Round(grade, 1, MidpointRounding.AwayFromZero);
    }

    public static bool IsPass(double percentage, GradingConfiguration config) =>
        percentage >= config.PassPct;
}
