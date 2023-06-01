using System.IO.Pipes;

namespace EngineLogging;

public class Program
{
    public static void Main(string[] args)
    {
        if (args.Length <= 0)
            throw new ArgumentException("No name given!");
        string pipeName = args[0];
        string path = $"logs/{DateTime.Now:yyyy_MM_dd_HH_mm_ss_fff}.log";
        NamedPipeClientStream pipeClient = new(".", pipeName, PipeDirection.In);

        pipeClient.Connect();
        byte[] data = new byte[ushort.MaxValue + 1];

        while (pipeClient.IsConnected || pipeClient.Length > 0)
        {
            try
            {
                int read = pipeClient.Read(data, 0, data.Length);
                unsafe
                {
                    fixed (byte* ptr = data)
                    {
                        char* charPtr = (char*)ptr;
                        LogString(path, new string(charPtr, 0, read / sizeof(char)));
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
        pipeClient.Close();
        pipeClient.Dispose();
        Console.WriteLine("Disconnected Logging");
    }

    public static void LogString(string path, string content)
    {
        while (true)
        {
            try
            {
                string? directory = Path.GetDirectoryName(path);
                if (directory is null)
                    return;
                if (!Directory.Exists(directory))
                    Directory.CreateDirectory(directory);
                File.AppendAllText(path, content);
                return;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}