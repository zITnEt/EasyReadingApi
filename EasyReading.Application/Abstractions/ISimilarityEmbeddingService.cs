using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyReading.Application.Abstractions
{
    public interface ISimilarityEmbeddingService
    {
        public double Compare(double[] embedding1, double[] embedding2);
    }
}
