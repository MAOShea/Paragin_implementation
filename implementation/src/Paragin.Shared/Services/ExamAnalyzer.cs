using Paragin.Shared.Models;
using Paragin.Shared.Parsing;

namespace Paragin.Shared.Services;

public static class ExamAnalyzer
{
    public static ExamAnalysis Analyze(ExamDataset dataset, GradingConfiguration configuration)
    {
        var maxTotal = dataset.MaxTotalScore;
        var studentTotals = new double[dataset.StudentCount];
        var studentResults = new List<StudentResult>(dataset.StudentCount);

        for (var studentIndex = 0; studentIndex < dataset.StudentCount; studentIndex++)
        {
            var rawScores = dataset.RawScoresByStudent[studentIndex];
            var total = 0.0;
            for (var questionIndex = 0; questionIndex < dataset.QuestionCount; questionIndex++)
            {
                var raw = questionIndex < rawScores.Count ? rawScores[questionIndex] : string.Empty;
                total += ScoreParser.ScoreForTotal(raw);
            }

            studentTotals[studentIndex] = total;
        }

        for (var studentIndex = 0; studentIndex < dataset.StudentCount; studentIndex++)
        {
            var rawScores = dataset.RawScoresByStudent[studentIndex];
            var flagged = StudentValidator.GetFlaggedQuestions(rawScores, dataset.QuestionMaxima);
            var total = studentTotals[studentIndex];
            var percentage = maxTotal == 0 ? 0 : total / maxTotal;
            var grade = GradingCalculator.CalculateGrade(percentage, configuration);
            var passed = GradingCalculator.IsPass(percentage, configuration);

            studentResults.Add(new StudentResult
            {
                StudentId = dataset.StudentIds[studentIndex],
                TotalScore = total,
                MaxTotalScore = maxTotal,
                Percentage = percentage,
                Grade = grade,
                Passed = passed,
                PassFail = passed ? "Pass" : "Fail",
                DataCheckMessage = StudentValidator.FormatDataCheckMessage(flagged),
                HasScoreErrors = flagged.Count > 0
            });
        }

        var items = BuildItemAnalytics(dataset, studentTotals);
        var dataChecks = BuildDataChecksSummary(dataset, configuration, studentResults, items);

        return new ExamAnalysis
        {
            Dataset = dataset,
            Configuration = configuration,
            Students = studentResults,
            Items = items,
            DataChecks = dataChecks
        };
    }

    private static IReadOnlyList<ItemAnalytic> BuildItemAnalytics(ExamDataset dataset, IReadOnlyList<double> studentTotals)
    {
        var items = new List<ItemAnalytic>(dataset.QuestionCount);

        for (var questionIndex = 0; questionIndex < dataset.QuestionCount; questionIndex++)
        {
            var maxScore = dataset.QuestionMaxima[questionIndex];
            var itemScores = new double[dataset.StudentCount];

            for (var studentIndex = 0; studentIndex < dataset.StudentCount; studentIndex++)
            {
                var rawScores = dataset.RawScoresByStudent[studentIndex];
                var raw = questionIndex < rawScores.Count ? rawScores[questionIndex] : string.Empty;
                itemScores[studentIndex] = ScoreParser.ScoreForTotal(raw);
            }

            var average = dataset.StudentCount == 0 ? 0 : itemScores.Average();
            double? pPrime = maxScore == 0 ? null : average / maxScore;
            var rit = Statistics.PearsonCorrelation(itemScores, studentTotals);

            items.Add(new ItemAnalytic
            {
                QuestionNumber = questionIndex + 1,
                MaxScore = maxScore,
                AverageScore = average,
                PPrime = pPrime,
                Rit = rit,
                DifficultyLabel = LabelDifficulty(pPrime),
                DiscriminationLabel = LabelDiscrimination(rit)
            });
        }

        return items;
    }

    private static DataChecksSummary BuildDataChecksSummary(
        ExamDataset dataset,
        GradingConfiguration configuration,
        IReadOnlyList<StudentResult> students,
        IReadOnlyList<ItemAnalytic> items)
    {
        var studentsWithErrors = students.Count(student => student.HasScoreErrors);
        var invalidMaxima = dataset.QuestionMaxima.Count(max => max <= 0);
        var invalidStudentScores = StudentValidator.CountInvalidStudentScores(dataset);
        var pPrimeOutOfRange = items.Count(item => item.PPrime is < 0 or > 1);
        var gradesOutOfRange = students.Count(student => student.Grade < 1 || student.Grade > 10);
        var passCount = students.Count(student => student.Passed);
        var failCount = students.Count - passCount;

        var statusMessage = studentsWithErrors == 0
            ? "Input data looks valid."
            : "Input data issues found — review before trusting grades and analytics.";

        return new DataChecksSummary
        {
            ActiveExamName = dataset.Name,
            StudentCount = dataset.StudentCount,
            QuestionCount = dataset.QuestionCount,
            StudentsWithScoreErrors = studentsWithErrors,
            InvalidMaximumScores = invalidMaxima,
            InvalidStudentScores = invalidStudentScores,
            PPrimeOutOfRange = pPrimeOutOfRange,
            GradesOutOfRange = gradesOutOfRange,
            PassCount = passCount,
            FailCount = failCount,
            StatusMessage = statusMessage
        };
    }

    public static DifficultyBandCounts CountDifficultyBands(IReadOnlyList<ItemAnalytic> items)
    {
        var tooHard = 0;
        var ok = 0;
        var tooEasy = 0;

        foreach (var item in items)
        {
            if (item.PPrime is null)
                continue;

            if (item.PPrime < 0.30)
                tooHard++;
            else if (item.PPrime <= 0.85)
                ok++;
            else
                tooEasy++;
        }

        return new DifficultyBandCounts
        {
            TooHard = tooHard,
            Ok = ok,
            TooEasy = tooEasy
        };
    }

    public static DiscriminationBandCounts CountDiscriminationBands(IReadOnlyList<ItemAnalytic> items)
    {
        var weak = 0;
        var review = 0;
        var good = 0;

        foreach (var item in items)
        {
            if (item.Rit is null)
                continue;

            if (item.Rit < 0.20)
                weak++;
            else if (item.Rit < 0.30)
                review++;
            else
                good++;
        }

        return new DiscriminationBandCounts
        {
            Weak = weak,
            Review = review,
            Good = good
        };
    }

    private static string? LabelDifficulty(double? pPrime)
    {
        if (pPrime is null)
            return null;

        if (pPrime < 0.30)
            return "Too hard";
        if (pPrime > 0.85)
            return "Too easy";

        return "OK";
    }

    private static string? LabelDiscrimination(double? rit)
    {
        if (rit is null)
            return "Weak";

        if (rit < 0.20)
            return "Weak";
        if (rit < 0.30)
            return "Review";

        return "Good";
    }
}
