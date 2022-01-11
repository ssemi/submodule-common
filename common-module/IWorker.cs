namespace common_module;

public interface IWorker
{
    Task RunAsync(string message);
}