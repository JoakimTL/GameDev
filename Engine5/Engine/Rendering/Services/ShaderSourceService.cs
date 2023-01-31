using Engine.Rendering.Objects;
using OpenGL;

namespace Engine.Rendering.Services;

public sealed class ShaderSourceService : Identifiable, IContextService, IDisposable
{

    private static readonly IReadOnlyDictionary<string, ShaderType> _allowedExtensions = new Dictionary<string, ShaderType>() {
            { ".vert", ShaderType.VertexShader },
            { ".frag", ShaderType.FragmentShader },
            { ".geom", ShaderType.GeometryShader },
            { ".tesc", ShaderType.TessControlShader },
            { ".tese", ShaderType.TessEvaluationShader },
            { ".comp", ShaderType.ComputeShader }
        };

    private readonly Dictionary<string, ShaderSource> _sources;

    public ShaderSourceService()
    {
        _sources = new();
    }

    public ShaderSource? Get(string path)
    {
        if (_sources.TryGetValue(path, out var source))
            return source;
        return Add(path);
    }

    private ShaderSource? Add(string path)
    {
        if (!File.Exists(path))
        {
            this.LogWarning($"File {path} does not exist!");
            return null;
        }

        string ext = Path.GetExtension(path);
        if (!_allowedExtensions.TryGetValue(ext, out ShaderType shaderType))
        {
            this.LogWarning($"Extension for {path} not allowed!");
            return null;
        }

        ShaderSource source;
        this._sources.Add(path, source = new(path, shaderType));
        return source;
    }

    public void LoadAll(string directory)
    {
        List<string> shaderFiles = new();
        SearchDirectory(shaderFiles, directory);
        foreach (string filePath in shaderFiles)
            Add(filePath);
    }

    private static void SearchDirectory(List<string> files, string path)
    {
        if (!Directory.Exists(path))
        {
            Log.Warning($"Directory {path} does not exist!");
            return;
        }
        //Find sub directories
        string[] subDirectories = Directory.GetDirectories(path);
        //Loop through subdirectories.
        for (int i = 0; i < subDirectories.Length; i++)
            SearchDirectory(files, subDirectories[i]);
        //Add files from this directory.
        files.AddRange(Directory.GetFiles(path));
    }

    public void Dispose()
    {
        foreach (var source in _sources.Values)
            source.Dispose();
        _sources.Clear();
    }
}
