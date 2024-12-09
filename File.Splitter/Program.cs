if (args.Length == 0)
{
    Console.WriteLine("Usage: Execute this program, followed by the name/filepath of the file you'd like to divide.");
    return;
}

if (args.Length > 0 && args[0] == "-version")
{
    Console.WriteLine("Version 0.1.0");
    Environment.Exit(0);
}


string filePath = args[0];

if (!File.Exists(filePath))
{
    Console.WriteLine("The specified file does not exist.");
    return;
}

const long chunkSize = 15 * 1024 * 1024;
FileInfo fileInfo = new(filePath);

if (fileInfo.Length <= chunkSize)
{
    Console.WriteLine("The file is not larger than 15 MB. No splitting will be performed.");
    return;
}

try
{
    // Split the file.
    using (FileStream sourceStream = new(filePath, FileMode.Open, FileAccess.Read))
    {
        byte[] buffer = new byte[chunkSize];
        int bytesRead;
        string originalFileName = Path.GetFileName(filePath);
        string originalDirectory = Path.GetDirectoryName(filePath) ?? Directory.GetCurrentDirectory();
        string partsFolder = Path.Combine(originalDirectory, $"{Path.GetFileNameWithoutExtension(originalFileName)}_parts");

        if (!Directory.Exists(partsFolder))
        {
            Directory.CreateDirectory(partsFolder);
        }

        int partNumber = 1;
        while ((bytesRead = sourceStream.Read(buffer, 0, buffer.Length)) > 0)
        {
            string partFileName = Path.Combine(partsFolder, $"{Path.GetFileNameWithoutExtension(originalFileName)}_part{partNumber++}{Path.GetExtension(originalFileName)}");

            using (FileStream partStream = new(partFileName, FileMode.Create, FileAccess.Write))
            {
                partStream.Write(buffer, 0, bytesRead);
            }

            Console.WriteLine($"Created: {partFileName}");
        }

    }

    Console.WriteLine("File split successfully.");
}
catch (Exception ex)
{
    Console.WriteLine($"Something bad happened :( {ex.Message}");
}