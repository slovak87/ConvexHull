using ConvexHull.Features.InputParser;
using ConvexHull.Models;

namespace ConvexHull.Tests.Features;

public class InputParserTests
{
    // Testy pro pùvodní formáty
    [Fact]
    public void ParseInputArguments_ValidCommaInput_ReturnsPoints()
    {
        var args = new[] { "1,2 3,4" };
        var points = InputParser.ParseInputArguments(args);

        Assert.Equal(2, points.Count);
        Assert.Contains(new Point(1, 2), points);
        Assert.Contains(new Point(3, 4), points);
    }

    [Fact]
    public void ParseInputArguments_ValidSpaceInput_ReturnsPoints()
    {
        var args = new[] { "1", "2", "3", "4" };
        var points = InputParser.ParseInputArguments(args);

        Assert.Equal(2, points.Count);
        Assert.Contains(new Point(1, 2), points);
        Assert.Contains(new Point(3, 4), points);
    }

    [Fact]
    public void ParseInputArguments_ValidSemicolonInput_ReturnsPoints()
    {
        var args = new[] { "1;2 3;4" };
        var points = InputParser.ParseInputArguments(args);

        Assert.Equal(2, points.Count);
        Assert.Contains(new Point(1, 2), points);
        Assert.Contains(new Point(3, 4), points);
    }

    // Testy pro CSV soubory
    [Fact]
    public void ParseInputArguments_ValidCsvFile_ReturnsPoints()
    {
        // Vytvoøení doèasného CSV souboru
        var csvContent = "1;2\n3;4";
        var tempFilePath = Path.GetTempFileName() + ".csv";
        File.WriteAllText(tempFilePath, csvContent);

        try
        {
            var args = new[] { tempFilePath };
            var points = InputParser.ParseInputArguments(args);

            Assert.Equal(2, points.Count);
            Assert.Contains(new Point(1, 2), points);
            Assert.Contains(new Point(3, 4), points);
        }
        finally
        {
            File.Delete(tempFilePath);
        }
    }

    [Fact]
    public void ParseInputArguments_CsvFileWithMultiplePointsPerLine_ReturnsPoints()
    {
        // Vytvoøení doèasného CSV souboru
        var csvContent = "1;2;3;4\n5;6";
        var tempFilePath = Path.GetTempFileName() + ".csv";
        File.WriteAllText(tempFilePath, csvContent);

        try
        {
            var args = new[] { tempFilePath };
            var points = InputParser.ParseInputArguments(args);

            Assert.Equal(3, points.Count);
            Assert.Contains(new Point(1, 2), points);
            Assert.Contains(new Point(3, 4), points);
            Assert.Contains(new Point(5, 6), points);
        }
        finally
        {
            File.Delete(tempFilePath);
        }
    }

    [Fact]
    public void ParseInputArguments_CsvFileWithComments_IgnoresComments()
    {
        // Vytvoøení doèasného CSV souboru
        var csvContent = "# Toto je komentáø\n1;2\n# Toto je další komentáø\n3;4";
        var tempFilePath = Path.GetTempFileName() + ".csv";
        File.WriteAllText(tempFilePath, csvContent);

        try
        {
            var args = new[] { tempFilePath };
            var points = InputParser.ParseInputArguments(args);

            Assert.Equal(2, points.Count);
            Assert.Contains(new Point(1, 2), points);
            Assert.Contains(new Point(3, 4), points);
        }
        finally
        {
            File.Delete(tempFilePath);
        }
    }

    [Fact]
    public void ParseInputArguments_EmptyInput_ReturnsEmptyList()
    {
        var args = Array.Empty<string>();
        var points = InputParser.ParseInputArguments(args);
        Assert.Empty(points);
    }

    [Fact]
    public void ParseInputArguments_NonExistentCsvFile_ThrowsException()
    {
        var nonExistentFile = "nonexistent.csv";
        var args = new[] { nonExistentFile };
        var exception = Assert.Throws<ArgumentException>(() => InputParser.ParseInputArguments(args));
        Assert.Contains("nebyl nalezen", exception.Message);
    }

    // Testy pro èásteènì platná data
    [Fact]
    public void ParseInputArguments_MixedValidAndInvalid_ReturnsValidPoints()
    {
        var args = new[] { "1,2 invalid 3,4" };
        var points = InputParser.ParseInputArguments(args);

        Assert.Equal(2, points.Count);
        Assert.Contains(new Point(1, 2), points);
        Assert.Contains(new Point(3, 4), points);
    }
}
