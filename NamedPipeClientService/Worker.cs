using System.IO.Pipes;

namespace NamedPipeClientService;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;

    public Worker(ILogger<Worker> logger)
    {
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);

            await using var pipeClient = new NamedPipeClientStream(".", "testpipe", PipeDirection.In);

            // Connect to the pipe or wait until the pipe is available.
            _logger.LogInformation("Attempting to connect to pipe...");
            pipeClient.Connect();

            _logger.LogInformation("Connected to pipe.");

            using (var sr = new StreamReader(pipeClient))
            {
                // Display the read text to the console
                string? temp;
                while ((temp = sr.ReadLine()) != null)
                {
                    _logger.LogInformation("Received from server: {0}", temp);
                }
            }
            
            await Task.Delay(10000, stoppingToken);
        }
    }
}