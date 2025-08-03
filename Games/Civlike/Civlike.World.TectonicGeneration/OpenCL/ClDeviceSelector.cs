using Silk.NET.OpenCL;

namespace Civlike.World.TectonicGeneration.OpenCL;

public static unsafe class ClDeviceSelector {
	public sealed record DevicePick( nint Platform, nint Device, string PlatformName, string Vendor, string Name );

	public static DevicePick ChooseDedicatedGpu( CL cl ) {
		// 1) Enumerate platforms
		uint nplat = 0;
		cl.GetPlatformIDs( 0, null, &nplat );
		nint[] plats = new nint[ nplat ];
		fixed (nint* pp = plats)
			cl.GetPlatformIDs( nplat, pp, null );

		List<DevicePick> candidates = new();
		foreach (nint plat in plats) {
			// 2) Get all GPU devices on this platform
			uint ndev = 0;
			cl.GetDeviceIDs( plat, DeviceType.Gpu, 0, null, &ndev );
			if (ndev == 0)
				continue;
			nint[] devs = new nint[ ndev ];
			fixed (nint* pd = devs)
				cl.GetDeviceIDs( plat, DeviceType.Gpu, ndev, pd, null );

			string platName = GetPlatformString( cl, plat, PlatformInfo.Name );
			foreach (nint d in devs) {
				string vendor = GetDeviceString( cl, d, DeviceInfo.Vendor );
				string name = GetDeviceString( cl, d, DeviceInfo.Name );
				bool unified = GetDeviceBool( cl, d, DeviceInfo.HostUnifiedMemory ); // true → usually integrated
				uint cu = GetDeviceUInt( cl, d, DeviceInfo.MaxComputeUnits );
				nuint mem = GetDeviceNUInt( cl, d, DeviceInfo.GlobalMemSize );

				// Heuristic ranking: prefer non-unified (discrete), then vendor, then CU/memory
				int score = 0;
				if (!unified)
					score += 1000;
				if (vendor.Contains( "NVIDIA", StringComparison.OrdinalIgnoreCase ))
					score += 200;
				if (vendor.Contains( "Advanced Micro Devices", StringComparison.OrdinalIgnoreCase ) ||
					vendor.Contains( "AMD", StringComparison.OrdinalIgnoreCase ))
					score += 150;
				score += (int) Math.Min( cu, 512 );
				score += (int) Math.Min(  mem / (1024UL * 1024UL * 1024UL), 64UL ); // GiB

				candidates.Add( new DevicePick( plat, d, platName, vendor, name ) { /* score baked in via tuple */ } );
			}
		}
		if (candidates.Count == 0)
			throw new Exception( "No GPU devices found." );

		// Best candidate by the scoring above
		DevicePick best = candidates
			.Select( p => (p, score: Score( cl, p.Device )) )
			.OrderByDescending( t => t.score )
			.First().p;

		Console.WriteLine( $"Using: {best.Vendor} {best.Name} on {best.PlatformName}" );
		return best;

		// Local helpers -------------------------------------------------------
		static int Score( CL cl, nint dev ) {
			bool unified = GetDeviceBool( cl, dev, DeviceInfo.HostUnifiedMemory );
			string vendor = GetDeviceString( cl, dev, DeviceInfo.Vendor );
			uint cu = GetDeviceUInt( cl, dev, DeviceInfo.MaxComputeUnits );
			nuint mem = GetDeviceNUInt( cl, dev, DeviceInfo.GlobalMemSize );

			int s = 0;
			if (!unified)
				s += 1000;
			if (vendor.Contains( "NVIDIA", StringComparison.OrdinalIgnoreCase ))
				s += 200;
			if (vendor.Contains( "Advanced Micro Devices", StringComparison.OrdinalIgnoreCase ) ||
				vendor.Contains( "AMD", StringComparison.OrdinalIgnoreCase ))
				s += 150;
			s += (int) Math.Min( cu, 512 );
			s += (int) Math.Min(  mem / (1024UL * 1024UL * 1024UL), 64UL );
			return s;
		}

		static string GetPlatformString( CL cl, nint p, PlatformInfo info ) {
			nuint sz = 0;
			cl.GetPlatformInfo( p, info, 0, null, &sz );
			byte* buf = stackalloc byte[ (int) sz ];
			cl.GetPlatformInfo( p, info, sz, buf, null );
			return Silk.NET.Core.Native.SilkMarshal.PtrToString( (nint) buf )!;
		}
		static string GetDeviceString( CL cl, nint d, DeviceInfo info ) {
			nuint sz = 0;
			cl.GetDeviceInfo( d, (uint) info, 0, null, &sz );
			byte* buf = stackalloc byte[ (int) sz ];
			cl.GetDeviceInfo( d, (uint) info, sz, buf, null );
			return Silk.NET.Core.Native.SilkMarshal.PtrToString( (nint) buf )!;
		}
		static bool GetDeviceBool( CL cl, nint d, DeviceInfo info ) {
			nuint sz = 0;
			cl.GetDeviceInfo( d, (uint) info, 0, null, &sz );
			int val = 0;
			cl.GetDeviceInfo( d, (uint) info, sz, &val, null );
			return val != 0;
		}
		static uint GetDeviceUInt( CL cl, nint d, DeviceInfo info ) {
			uint val = 0;
			cl.GetDeviceInfo( d, (uint) info,  sizeof( uint ), &val, null );
			return val;
		}
		static nuint GetDeviceNUInt( CL cl, nint d, DeviceInfo info ) {
			nuint val = 0;
			cl.GetDeviceInfo( d, (uint) info, (nuint) sizeof( nuint ), &val, null );
			return val;
		}
	}
}