namespace Paragin.Shared.Services;

internal static class Statistics
{
    public static double? PearsonCorrelation(IReadOnlyList<double> x, IReadOnlyList<double> y)
    {
        if (x.Count != y.Count || x.Count < 2)
            return null;

        var meanX = x.Average();
        var meanY = y.Average();
        double sumXY = 0;
        double sumX2 = 0;
        double sumY2 = 0;

        for (var i = 0; i < x.Count; i++)
        {
            var dx = x[i] - meanX;
            var dy = y[i] - meanY;
            sumXY += dx * dy;
            sumX2 += dx * dx;
            sumY2 += dy * dy;
        }

        if (sumX2 == 0 || sumY2 == 0)
            return null;

        return sumXY / Math.Sqrt(sumX2 * sumY2);
    }
}
