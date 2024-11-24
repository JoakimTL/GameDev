using Engine.Logging;
using Engine.Module.Render.Ogl.OOP;
using OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Module.Render.Ogl.Services;
public sealed class ShaderSourceService : DisposableIdentifiable {

	private static readonly IReadOnlyDictionary<string, ShaderType> _allowedExtensions = new Dictionary<string, ShaderType>() {
			{ ".vert", ShaderType.VertexShader },
			{ ".frag", ShaderType.FragmentShader },
			{ ".geom", ShaderType.GeometryShader },
			{ ".tesc", ShaderType.TessControlShader },
			{ ".tese", ShaderType.TessEvaluationShader },
			{ ".comp", ShaderType.ComputeShader }
		};

	private readonly Dictionary<string, OglShaderSource> _sources;

	public ShaderSourceService() {
		_sources = new();
	}

	public OglShaderSource? Get( string path ) {
		if (_sources.TryGetValue( path, out OglShaderSource? source ))
			return source;
		return Add( path );
	}

	private OglShaderSource? Add( string path ) {
		path = path.Replace( "/", "\\" );
		if (!path.StartsWith( "assets\\shaders\\" ))
			path = "assets\\shaders\\" + path;
		if (!File.Exists( path )) {
			this.LogWarning( $"File {path} does not exist!" );
#if DEBUG
			DebugUtilities.Breakpoint( this );
#endif
			return null;
		}

		string ext = Path.GetExtension( path );
		if (!_allowedExtensions.TryGetValue( ext, out ShaderType shaderType )) {
			this.LogWarning( $"Extension for {path} not allowed!" );
			return null;
		}

		OglShaderSource source = new( path, shaderType );
		_sources.Add( path, source );
		return source;
	}

	public void LoadAll( string directory ) {
		List<string> shaderFiles = new();
		SearchDirectory( shaderFiles, directory );
		foreach (string filePath in shaderFiles)
			Add( filePath );
	}

	private static void SearchDirectory( List<string> files, string path ) {
		if (!Directory.Exists( path )) {
			Log.Warning( $"Directory {path} does not exist!" );
			return;
		}
		//Find sub directories
		string[] subDirectories = Directory.GetDirectories( path );
		//Loop through subdirectories.
		for (int i = 0; i < subDirectories.Length; i++)
			SearchDirectory( files, subDirectories[ i ] );
		//Add files from this directory.
		files.AddRange( Directory.GetFiles( path ) );
	}

	protected override bool InternalDispose() {
		foreach (OglShaderSource source in _sources.Values)
			source.Dispose();
		_sources.Clear();
		return true;
	}
}
