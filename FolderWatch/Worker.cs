using Microsoft.Extensions.FileSystemGlobbing.Abstractions;

namespace FolderWatch;
public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;

    private const string FolderToWatch = @"C:\Watch";
    private const string DestinationFolder = @"C:\Dest";
    
    private const int DelayInMilliseconds = 10000;

    public Worker(ILogger<Worker> logger)
    {
        // we could (and should) inject configuration here
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // create FolderToWatch if it doesn't exist
        if (!Directory.Exists(FolderToWatch)) Directory.CreateDirectory(FolderToWatch);

        while (!stoppingToken.IsCancellationRequested)
        {
            // check FolderToWatch for files
            var files = Directory.GetFiles(FolderToWatch);


            
            // delete any file older than 1 minute
            foreach (var file in files)
            {
                var fileInfo = new FileInfo(file);
                var sourceF = Path.Combine(fileInfo.DirectoryName, fileInfo.Name);
                var destinationF = Path.Combine(DestinationFolder, fileInfo.Name);
                if (fileInfo.CreationTime < DateTime.Now.AddMinutes(-1))
                {
                    _logger.LogInformation($"Copying {fileInfo.Name}");
                    File.Copy(sourceF, destinationF);

                    _logger.LogInformation($"Deleting {fileInfo.Name}");
                    fileInfo.Delete();
                }
            }

            await Task.Delay(DelayInMilliseconds, stoppingToken);
        }
    }
}
