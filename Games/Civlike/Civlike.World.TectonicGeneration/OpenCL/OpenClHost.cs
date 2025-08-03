using Silk.NET.OpenCL;

namespace Civlike.World.TectonicGeneration.OpenCL;

unsafe sealed class OpenClHost : IDisposable {
	public readonly CL Cl = CL.GetApi();
	public readonly nint Context;
	public readonly nint Device;
	public readonly nint Queue;

	public OpenClHost() {
		// Platform & device (prefer GPU, fallback CPU)
		ClDeviceSelector.DevicePick pick = ClDeviceSelector.ChooseDedicatedGpu( this.Cl );

		// Create context on that *specific* device
		int err;
		nint* props = stackalloc nint[ 3 ];
		props[ 0 ] = (nint) ContextProperties.Platform;
		props[ 1 ] = pick.Platform;
		props[ 2 ] = 0;
		nint dev = pick.Device;
		this.Context = this.Cl.CreateContext( props, 1, &dev, null, null, &err );
		if (err != 0)
			throw new Exception( $"clCreateContext: {err}" );
		this.Device = pick.Device;

		this.Queue = this.Cl.CreateCommandQueue( this.Context, this.Device, CommandQueueProperties.None, &err );
		if (err != 0)
			throw new Exception( $"clCreateCommandQueue: {err}" );
	}

	public nint CreateBufferBytes( nuint bytes, MemFlags flags ) {
		int err;
		nint mem = this.Cl.CreateBuffer( this.Context, flags, bytes, null, &err );
		if (err != 0)
			throw new Exception( $"clCreateBuffer: {err}" );
		return mem;
	}

	public void WriteBuffer<T>( nint mem, ReadOnlySpan<T> src ) where T : unmanaged {
		fixed (T* p = src)
			this.Cl.EnqueueWriteBuffer( this.Queue, mem, true, 0, (nuint) (src.Length * sizeof( T )), p, 0, null, null );
	}

	public void ReadBuffer<T>( nint mem, Span<T> dst ) where T : unmanaged {
		fixed (T* p = dst)
			this.Cl.EnqueueReadBuffer( this.Queue, mem, true, 0, (nuint) (dst.Length * sizeof( T )), p, 0, null, null );
	}

	public void ReadBufferRegion<T>( nint mem, nuint byteOffset, Span<T> dst ) where T : unmanaged {
		fixed (T* p = dst)
			this.Cl.EnqueueReadBuffer( this.Queue, mem, true, byteOffset, (nuint) (dst.Length * sizeof( T )), p, 0, null, null );
	}

	public static void SetArgMem( nint kernel, int index, nint mem ) {
		nint tmp = mem;
		CL.GetApi().SetKernelArg( kernel, (uint) index, (nuint) nint.Size, &tmp );
	}
	public static void SetArgVal<T>( nint kernel, int index, in T val ) where T : unmanaged {
		fixed (T* p = &val)
			CL.GetApi().SetKernelArg( kernel, (uint) index, (nuint) sizeof( T ), p );
	}

	public void Run1D( nint kernel, nuint global, nuint? local = null ) {
		nuint g = global;
		nuint* templp = stackalloc nuint[ 1 ];
		nuint* lp = local.HasValue ? templp : null;
		if (lp != null)
			lp[ 0 ] = local!.Value;
		this.Cl.EnqueueNdrangeKernel( this.Queue, kernel, 1, null, &g, lp, 0, null, null );
		this.Cl.Finish( this.Queue );
	}

	public void Dispose() {
		this.Cl.ReleaseCommandQueue( this.Queue );
		this.Cl.ReleaseContext( this.Context );
	}
}
