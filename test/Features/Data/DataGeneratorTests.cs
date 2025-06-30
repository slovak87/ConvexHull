using ConvexHull.Features.Data;

namespace ConvexHull.Tests.Features.Data
{
    public class DataGeneratorTests
    {
        [Fact]
        public void GenerateDemoData_ReturnsCorrectCount()
        {
            int count = 50;
            var points = DataGenerator.GenerateDemoData(count);

            Assert.Equal(count, points.Count);
        }
    }
}