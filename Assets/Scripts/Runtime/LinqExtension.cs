namespace WorldEnsemble
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public static class LinqExtension
    {
        public static double StandardDeviation(this IEnumerable<double> values)
        {
            if (values.Count() == 0)
            {
                return 0.0;
            }
            double average = values.Average();
            double sum = values.Sum(v => (v - average) * (v - average));
            return Math.Sqrt(sum / values.Count());
        }
    }
}
