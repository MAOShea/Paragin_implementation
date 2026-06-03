using Paragin.Shared.Models;
using Paragin.Shared.Parsing;
using Paragin.Shared.Services;

namespace Paragin.Shared.Tests;

public class ExamAnalyzerTests
{
    private static readonly string UserDataRoot = FindUserDataRoot();

    [Fact]
    public void InputData1_MatchesExpectedPassFailCounts()
    {
        var dataset = LoadDataset("inputdata_1", "InputData_1");
        var analysis = ExamAnalyzer.Analyze(dataset, GradingConfiguration.Default);

        Assert.Equal(49, analysis.Dataset.QuestionCount);
        Assert.Equal(90, analysis.Dataset.MaxTotalScore);
        Assert.Equal(452, analysis.Dataset.StudentCount);
        Assert.Equal(68, analysis.DataChecks.PassCount);
        Assert.Equal(384, analysis.DataChecks.FailCount);
        Assert.Equal(0, analysis.DataChecks.StudentsWithScoreErrors);
    }

    [Fact]
    public void InputData2_HasExpectedStudentAndQuestionCounts()
    {
        var dataset = LoadDataset("inputdata_2", "InputData_2");
        var analysis = ExamAnalyzer.Analyze(dataset, GradingConfiguration.Default);

        Assert.Equal(49, analysis.Dataset.QuestionCount);
        Assert.Equal(452, analysis.Dataset.StudentCount);
        Assert.True(analysis.DataChecks.PassCount > 0);
        Assert.True(analysis.DataChecks.FailCount > 0);
    }

    [Theory]
    [InlineData(0.0, 1.0)]
    [InlineData(0.2, 1.0)]
    [InlineData(0.7, 5.5)]
    [InlineData(1.0, 10.0)]
    public void GradingCalculator_UsesCaesuraAnchors(double percentage, double expectedGrade)
    {
        var grade = GradingCalculator.CalculateGrade(percentage, GradingConfiguration.Default);
        Assert.Equal(expectedGrade, grade);
    }

    private static ExamDataset LoadDataset(string id, string name)
    {
        var fileName = name switch
        {
            "InputData_1" => "Developer Assignment - prototype.xlsx - InputData_1.csv",
            "InputData_2" => "Developer Assignment - prototype.xlsx - InputData_2.csv",
            _ => throw new ArgumentOutOfRangeException(nameof(name))
        };

        var path = Path.Combine(UserDataRoot, fileName);
        return ExamCsvParser.ParseFile(id, name, path);
    }

    private static string FindUserDataRoot()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null)
        {
            var candidate = Path.Combine(directory.FullName, "user_data");
            if (Directory.Exists(candidate))
                return candidate;

            directory = directory.Parent;
        }

        throw new DirectoryNotFoundException("Could not locate user_data directory.");
    }
}
