using System.Text;

namespace ElasticsearchAPI.Services.Implementation;

public class LoggerService : ILoggerService
{
    private readonly string _logFilePath = "Resources/LogFile.txt";

    public async Task LogToFile(Exception occurredException)
    {
        const string loggerTag = nameof(LoggerService);
        
        const string rowDelimiter =
            "\n===============================\n";
        var timeOccurrence =
            $"{loggerTag} | Occurrence Time: {DateTime.Now:dddd, dd MMMM yyyy HH:mm:ss}";
        var exceptionDetails =
            $"{loggerTag} | Exception Type: {occurredException.GetType().Name}\n{loggerTag} | Exception Message: {occurredException.Message}";

        var informationToBeWritten = new StringBuilder();
        informationToBeWritten.AppendLine(rowDelimiter);
        informationToBeWritten.AppendLine(timeOccurrence);
        informationToBeWritten.AppendLine(exceptionDetails);
        informationToBeWritten.AppendLine(rowDelimiter);

        await WriteToFile(informationToBeWritten.ToString());
    }

    private async Task WriteToFile(string information)
    {
        await File.AppendAllLinesAsync(_logFilePath, new[] { information });
    }
}