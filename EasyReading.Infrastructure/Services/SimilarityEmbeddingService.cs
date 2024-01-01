using EasyReading.Application.Abstractions;

namespace EasyReading.Infrastructure.Services
{
    public class SimilarityEmbeddingService : ISimilarityEmbeddingService
    {
        public double Compare(double[] embedding1, double[] embedding2)
        {
            double dotProduct = DotProduct(embedding1, embedding2);
            double magnitudeA = Magnitude(embedding1);
            double magnitudeB = Magnitude(embedding2);

            return dotProduct / (magnitudeA * magnitudeB);
        }

        private double DotProduct(double[] embedding1, double[] embedding2)
        {
            return embedding1.Zip(embedding2, (a, b) => a * b).Sum();
        }

        private double Magnitude(double[] embedding)
        {
            return Math.Sqrt(embedding.Sum(v => v * v));
        }
    }
}
