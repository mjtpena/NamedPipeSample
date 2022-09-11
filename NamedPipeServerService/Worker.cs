using System.IO.Pipes;

namespace NamedPipeServerService;

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

            await using var pipeServer = new NamedPipeServerStream("testpipe", PipeDirection.Out);
            Console.WriteLine("NamedPipeServerStream object created.");

            // Wait for a client to connect
            Console.Write("Waiting for client connection...");
            await pipeServer.WaitForConnectionAsync(stoppingToken);

            Console.WriteLine("Client connected.");
            try
            {
                // Read user input and send that to the client process.
                await using var sw = new StreamWriter(pipeServer);
                sw.AutoFlush = true;
                Console.Write("Enter text: ");
                sw.WriteLine(Console.ReadLine());
            }
            // Catch the IOException that is raised if the pipe is broken
            // or disconnected.
            catch (IOException e)
            {
                Console.WriteLine("ERROR: {0}", e.Message);
            }
            
            await Task.Delay(10000, stoppingToken);
        }
    }
}