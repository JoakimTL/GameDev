using Engine.GlobalServices;
using OpenGL;
using System.Text;

namespace Engine.Rendering.Contexts.Objects;
public sealed class ShaderSource : Identifiable, IFileSource, IDisposable
{
    public uint ShaderID { get; private set; }
    public ShaderType ShaderType { get; private set; }
    public string Filepath { get; }

    public event Action? FileChanged;

    public ShaderSource(string path, ShaderType shaderType)
    {
        Filepath = path;

        List<string> dependencies = new();
        GetDependencies(path, dependencies);
        foreach (var d in dependencies)
            Global.Get<FileWatchingService>().Track(d, FileChange);

        ShaderID = Gl.CreateShader(shaderType);
        ShaderType = shaderType;
        string source = GetData();
        Gl.ShaderSource(ShaderID, new string[] { source }, new int[] { source.Length });
        Gl.CompileShader(ShaderID);

        Gl.GetShader(ShaderID, ShaderParameterName.CompileStatus, out int status);
        if (status == 0)
        {
            StringBuilder ss = new(1024);
            Gl.GetShaderInfoLog(ShaderID, ss.Capacity, out int logLength, ss);
            this.LogWarning($"{logLength}-{ss}");
            Dispose();
        }
	}

#if DEBUG
	~ShaderSource()
	{
		System.Diagnostics.Debug.Fail($"{this} was not disposed!");
	}
#endif

	private void FileChange(string path) => FileChanged?.Invoke();

    public string GetData() => ReadSource(Filepath, out string source) ? source : string.Empty;

    private static bool ReadSource(string path, out string source)
    {
        source = "";
        try
        {
            if (!File.Exists(path))
            {
                Log.Warning($"Couldn't find file {path}!");
                return false;
            }

            string? dir = Path.GetDirectoryName(path);

            StringBuilder sb = new();
            StreamReader reader = new(File.OpenRead(path));

            while (!reader.EndOfStream)
            {
                string? line = reader.ReadLine();

                if (line?.StartsWith("#include ") ?? false)
                {
                    string pathInclude = dir + "/" + line["#include ".Length..];
                    ReadSource(pathInclude, out line);
                }

                sb.AppendLine(line);
            }

            source = sb.ToString();
            return true;
        }
        catch (Exception e)
        {
            Log.Error(e);
        }
        source = "";
        return false;
    }

    private static void GetDependencies(string path, List<string> dependencies)
    {
        if (!File.Exists(path))
        {
            Log.Warning($"Couldn't find file {path}!");
            return;
        }
        string? dir = Path.GetDirectoryName(path);
        dependencies.Add(path);

        StreamReader reader = new(File.OpenRead(path));
        while (!reader.EndOfStream)
        {
            string? line = reader.ReadLine();

            if (line?.StartsWith("#include ") ?? false)
            {
                string pathInclude = dir + "/" + line["#include ".Length..];
                GetDependencies(pathInclude, dependencies);
            }
        }
    }

    public void Dispose()
    {
        Gl.DeleteShader(ShaderID);
        GC.SuppressFinalize(this);
    }
}
