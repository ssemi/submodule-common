using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text;

namespace common_module;

public class Runner
{
    private readonly ILogger<Runner> _logger;
    private readonly IWorker _worker;

    public Runner(ILogger<Runner> logger, IWorker worker)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _worker = worker;
    }

    public async Task DoAction(string optionQueueName = "")
    {
        var consumer = $"[{Environment.MachineName}]";
        var msg = $"[Queue setting result] consumer :  {consumer} ";
        
        
        await _worker.RunAsync(msg);
    }

    private async Task DoConsumeProcess(byte[] body)
    {
        var message = Encoding.UTF8.GetString(body);

        if (string.IsNullOrEmpty(message))
            return;

        Console.WriteLine($"[{DateTime.UtcNow.AddHours(9).ToString("yyyy-MM-dd HH:mm:ss")}] - {message}");

        if (IsValidJson(message))
            await _worker.RunAsync(message);
    }

    private static bool IsValidJson(string strInput)
    {
        strInput = strInput.Trim();
        if ((strInput.StartsWith("{") && strInput.EndsWith("}")) || //For object
            (strInput.StartsWith("[") && strInput.EndsWith("]"))) //For array
        {
            try
            {
                var obj = JToken.Parse(strInput);
                return true;
            }
            catch (JsonReaderException jex)
            {
                Console.WriteLine(jex.Message);
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return false;
            }
        }
        else
        {
            return false;
        }
    }

}