using Silk.NET.OpenCL;

namespace Civlike.World.TectonicGeneration.OpenCL;

unsafe sealed class KernelRegistry : IDisposable {
	private readonly CL _cl;
	private readonly nint _ctx, _dev;
	private readonly Dictionary<string, (nint kernel, nint program)> _map = new( StringComparer.Ordinal );

	public KernelRegistry( OpenClHost host ) { this._cl = host.Cl; this._ctx = host.Context; this._dev = host.Device; }

	public void LoadFolder( string folder, string buildOpts = "" ) {
		foreach (string file in Directory.GetFiles( folder, "*.cl", SearchOption.TopDirectoryOnly ))
			LoadSingle( file, buildOpts );
	}

	private void LoadSingle( string path, string buildOpts ) {
		string expected = Path.GetFileNameWithoutExtension( path );
		string src = File.ReadAllText( path );
		int err;

		sbyte* s = (sbyte*) Silk.NET.Core.Native.SilkMarshal.StringToPtr( src );
		nint program;
		try {
			program = this._cl.CreateProgramWithSource( this._ctx, 1, (byte**) &s, null, &err );
		} finally {
			Silk.NET.Core.Native.SilkMarshal.Free( (nint) s );
		}
		if (err != 0)
			throw new Exception( $"CreateProgramWithSource({expected}): {err}" );

		sbyte* opts = (sbyte*) Silk.NET.Core.Native.SilkMarshal.StringToPtr( buildOpts );
		nint dev = this._dev;
		err = this._cl.BuildProgram( program, 1, &dev, (byte*) opts, null, null );
		Silk.NET.Core.Native.SilkMarshal.Free( (nint) opts );
		if (err != 0) { PrintBuildLog( program ); throw new Exception( $"clBuildProgram({expected}): {err}" ); }

		uint count = 0;
		this._cl.CreateKernelsInProgram( program, 0, null, &count );
		if (count != 1) {
			this._cl.ReleaseProgram( program );
			throw new InvalidOperationException(
			$"File '{path}' must define exactly 1 __kernel (found {count})." );
		}

		nint k;
		this._cl.CreateKernelsInProgram( program, 1, &k, null );

		// Verify kernel name == file name
		nuint sz = 0;
		this._cl.GetKernelInfo( k, KernelInfo.FunctionName, 0, null, &sz );
		byte[] buf = new byte[ sz ];
		fixed (byte* p = buf)
			this._cl.GetKernelInfo( k, KernelInfo.FunctionName, sz, p, null );
		string actual = Silk.NET.Core.Native.SilkMarshal.PtrToString(
			(nint) System.Runtime.CompilerServices.Unsafe.AsPointer( ref buf[ 0 ] ) )!;

		if (!string.Equals( expected, actual, StringComparison.Ordinal )) {
			this._cl.ReleaseKernel( k );
			this._cl.ReleaseProgram( program );
			throw new InvalidOperationException( $"Kernel name '{actual}' must equal file name '{expected}'." );
		}

		this._map.Add( actual, (k, program) ); // keep both alive (simple lifetime)
	}

	public nint Get( string kernelName ) => this._map[ kernelName ].kernel;

	private void PrintBuildLog( nint program ) {
		nuint sz = 0;
		this._cl.GetProgramBuildInfo( program, this._dev, ProgramBuildInfo.BuildLog, 0, null, &sz );
		byte[] buf = new byte[ sz ];
		fixed (byte* p = buf)
			this._cl.GetProgramBuildInfo( program, this._dev, ProgramBuildInfo.BuildLog, sz, p, null );
		Console.Error.WriteLine( Silk.NET.Core.Native.SilkMarshal.PtrToString(
			(nint) System.Runtime.CompilerServices.Unsafe.AsPointer( ref buf[ 0 ] ) )! );
	}

	public void Dispose() {
		foreach ((nint kernel, nint program) kv in this._map.Values) { this._cl.ReleaseKernel( kv.kernel ); this._cl.ReleaseProgram( kv.program ); }
		this._map.Clear();
	}
}