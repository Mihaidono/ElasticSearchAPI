namespace ElasticsearchAPI.Services;

public interface ILoggerService
{
    // writes to a file details about an occured exception
    public Task LogToFile(Exception occurredException);
}