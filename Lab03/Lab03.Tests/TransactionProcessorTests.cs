using Autofac;
using Lab03.Transaction;
using Xunit;

namespace Lab03.Tests;

public class TransactionProcessorTests : BaseTests
{
    [Fact]
    public void It_ProcessesTransaction_When_ProcessorInvoked_Should_CompleteSuccessfully()
    {
        // Arrange
        using var container = CreateContainer();
        var processor = container.Resolve<TransactionProcessor>();

        // Act & Assert
        processor.ProcessTransaction();
    }

    [Fact]
    public void It_CreatesSeparateTransactions_When_ProcessorExecutedMultipleTimes_Should_WriteStartMessages()
    {
        // Arrange
        using var container = CreateContainer();
        var processor = container.Resolve<TransactionProcessor>();
        var originalOut = Console.Out;
        var outputs = new List<string>();

        // Act
        try
        {
            using var sw = new StringWriter();
            Console.SetOut(sw);

            processor.ProcessTransaction();
            processor.ProcessTransaction();

            var output = sw.ToString();
            outputs.AddRange(output.Split('\n', StringSplitOptions.RemoveEmptyEntries));
        }
        finally
        {
            Console.SetOut(originalOut);
        }

        // Assert
        var startCount = outputs.Count(line => line.Contains("Starting new transaction"));
        Assert.Equal(2, startCount);
    }
}