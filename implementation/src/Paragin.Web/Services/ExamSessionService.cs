using Paragin.Shared.Models;
using Paragin.Shared.Parsing;
using Paragin.Shared.Services;

namespace Paragin.Web.Services;

public sealed class ExamSessionService
{
    private readonly Dictionary<string, ExamDataset> _datasets = new(StringComparer.OrdinalIgnoreCase);
    private GradingConfiguration _configuration = GradingConfiguration.Default;

    public ExamSessionService(IWebHostEnvironment environment)
    {
        LoadSeedData(environment);
    }

    public IReadOnlyList<ExamDataset> Datasets => _datasets.Values.OrderBy(dataset => dataset.Name).ToList();

    public GradingConfiguration Configuration => _configuration;

    public ExamDataset? GetActiveDataset()
    {
        if (string.IsNullOrWhiteSpace(_configuration.ActiveExamId))
            return Datasets.FirstOrDefault();

        return _datasets.GetValueOrDefault(_configuration.ActiveExamId);
    }

    public ExamAnalysis? GetActiveAnalysis()
    {
        var dataset = GetActiveDataset();
        return dataset is null ? null : ExamAnalyzer.Analyze(dataset, _configuration);
    }

    public void UpdateConfiguration(GradingConfiguration configuration)
    {
        _configuration = configuration;
    }

    public void SetActiveExam(string datasetId)
    {
        if (!_datasets.ContainsKey(datasetId))
            throw new InvalidOperationException($"Unknown dataset '{datasetId}'.");

        _configuration.ActiveExamId = datasetId;
    }

    public ExamDataset ImportDataset(string name, Stream csvStream)
    {
        var id = Guid.NewGuid().ToString("N");
        var dataset = ExamCsvParser.Parse(id, name, csvStream);
        _datasets[id] = dataset;
        _configuration.ActiveExamId = id;
        return dataset;
    }

    private void LoadSeedData(IWebHostEnvironment environment)
    {
        var userDataPath = FindUserDataPath(environment.ContentRootPath);
        if (userDataPath is null)
            return;

        ImportSeedFile("inputdata_1", "InputData_1", Path.Combine(userDataPath, "Developer Assignment - prototype.xlsx - InputData_1.csv"));
        ImportSeedFile("inputdata_2", "InputData_2", Path.Combine(userDataPath, "Developer Assignment - prototype.xlsx - InputData_2.csv"));
        _configuration.ActiveExamId = "inputdata_1";
    }

    private void ImportSeedFile(string id, string name, string path)
    {
        if (!File.Exists(path))
            return;

        _datasets[id] = ExamCsvParser.ParseFile(id, name, path);
    }

    private static string? FindUserDataPath(string contentRootPath)
    {
        var directory = new DirectoryInfo(contentRootPath);
        while (directory is not null)
        {
            var candidate = Path.Combine(directory.FullName, "user_data");
            if (Directory.Exists(candidate))
                return candidate;

            directory = directory.Parent;
        }

        return null;
    }
}
