using Engine.Rendering.Contexts.Objects.Meshes;
using System.Globalization;
using System.Numerics;
using Engine.Datatypes.Vertices;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using System.Diagnostics.CodeAnalysis;

namespace Engine.Rendering.Contexts.Services;

public sealed class MeshLoadingService : Identifiable, IContextService
{
	private readonly RenderBufferObjectService _renderBufferObjectService;

	public MeshLoadingService(RenderBufferObjectService renderBufferObjectService)
	{
		_renderBufferObjectService = renderBufferObjectService;
	}

	public LoadedAssetMesh? Load(string path)
	{
		var ext = Path.GetExtension(path);
		if (ext is null)
			return Log.WarningThenReturnDefault<LoadedAssetMesh>($"Path {path} must have an extension. Either \".obj\", \".d3m\" or \".d2m\"!");

		LoadedAssetMesh? mesh;
		switch (ext)
		{
			case ".obj":
				{
					LoadObjFile(path, out Vertex3[] vertices, out uint[] indices);
					uint vertexSize = (uint)Marshal.SizeOf<Vertex3>();
					var vertexSegment = _renderBufferObjectService.Get(typeof(Vertex3)).AllocateSegment((uint)vertices.Length * vertexSize)
						?? throw new OutOfMemoryException($"{_renderBufferObjectService.Get(typeof(Vertex3))} ran out of memory for vertex data");
					uint elementSize = sizeof(uint);
					var elementSegment = _renderBufferObjectService.ElementBuffer.AllocateSegment((uint)indices.Length * elementSize)
						?? throw new OutOfMemoryException($"{_renderBufferObjectService.ElementBuffer} ran out of memory for element data");
					vertexSegment.Write(0, vertices);
					elementSegment.Write(0, indices);
					mesh = new LoadedAssetMesh(typeof(Vertex3), vertexSegment, vertexSize, elementSegment, elementSize);
					break;
				}
			case ".d3m":
				{
					LoadD3MFile(path, out Vertex3[]? vertices, out uint[]? indices);
					if (vertices is null || indices is null)
						return null;
					uint vertexSize = (uint)Marshal.SizeOf<Vertex3>();
					var vertexSegment = _renderBufferObjectService.Get(typeof(Vertex3)).AllocateSegment((uint)vertices.Length * vertexSize)
						?? throw new OutOfMemoryException($"{_renderBufferObjectService.Get(typeof(Vertex3))} ran out of memory for vertex data");
					uint elementSize = sizeof(uint);
					var elementSegment = _renderBufferObjectService.ElementBuffer.AllocateSegment((uint)indices.Length * elementSize)
						?? throw new OutOfMemoryException($"{_renderBufferObjectService.ElementBuffer} ran out of memory for element data");
					vertexSegment.Write(0, vertices);
					elementSegment.Write(0, indices);
					mesh = new LoadedAssetMesh(typeof(Vertex3), vertexSegment, vertexSize, elementSegment, elementSize);
					break;
				}
			case ".d2m":
				{
					LoadD2MFile(path, out Vertex2[]? vertices, out uint[]? indices);
					if (vertices is null || indices is null)
						return null;
					uint vertexSize = (uint)Marshal.SizeOf<Vertex2>();
					var vertexSegment = _renderBufferObjectService.Get(typeof(Vertex2)).AllocateSegment((uint)vertices.Length * vertexSize)
						?? throw new OutOfMemoryException($"{_renderBufferObjectService.Get(typeof(Vertex2))} ran out of memory for vertex data");
					uint elementSize = sizeof(uint);
					var elementSegment = _renderBufferObjectService.ElementBuffer.AllocateSegment((uint)indices.Length * elementSize)
						?? throw new OutOfMemoryException($"{_renderBufferObjectService.ElementBuffer} ran out of memory for element data");
					vertexSegment.Write(0, vertices);
					elementSegment.Write(0, indices);
					mesh = new LoadedAssetMesh(typeof(Vertex2), vertexSegment, vertexSize, elementSegment, elementSize);
					break;
				}
			default:
				return Log.WarningThenReturnDefault<LoadedAssetMesh>($"Path {path} must have a correct extension. Either \".obj\", \".d3m\" or \".d2m\"!");
		}
		return mesh;
	}

	private static bool LoadD3MFile(string filepath, [NotNullWhen(true)] out Vertex3[]? vertices, [NotNullWhen(true)] out uint[]? indices)
	{
		var data = File.ReadAllBytes(filepath); //TODO file handling system. Allow for compressed .pak files or having everything in a .zip or something

		vertices = Array.Empty<Vertex3>();
		indices = Array.Empty<uint>();
		if (data.Length < 8)
			return Log.WarningThenReturn($"{filepath} does not contain enough data for a mesh!", false);

		unsafe
		{
			fixed (byte* srcPtr = data)
			{
				uint vertexCount = *(uint*)srcPtr;
				uint elementCount = *(uint*)srcPtr + sizeof(uint);
				vertices = new Vertex3[vertexCount];
				indices = new uint[elementCount];
				fixed (Vertex3* dstPtr = vertices)
					Buffer.MemoryCopy(srcPtr + sizeof(uint) * 2, dstPtr, vertexCount * sizeof(Vertex3), vertexCount * sizeof(Vertex3));
				fixed (uint* dstPtr = indices)
					Buffer.MemoryCopy(srcPtr + sizeof(uint) * 2 + vertexCount * sizeof(Vertex3), dstPtr, elementCount * sizeof(uint), elementCount * sizeof(uint));
			}
		}
		return true;
	}

	private static bool LoadD2MFile(string filepath, [NotNullWhen(true)] out Vertex2[]? vertices, [NotNullWhen(true)] out uint[]? indices)
	{
		var data = File.ReadAllBytes(filepath);

		vertices = Array.Empty<Vertex2>();
		indices = Array.Empty<uint>();
		if (data.Length < 8)
			return Log.WarningThenReturn($"{filepath} does not contain enough data for a mesh!", false);

		unsafe
		{
			fixed (byte* srcPtr = data)
			{
				uint vertexCount = *(uint*)srcPtr;
				uint elementCount = *(uint*)srcPtr + sizeof(uint);
				vertices = new Vertex2[vertexCount];
				indices = new uint[elementCount];
				fixed (Vertex2* dstPtr = vertices)
					Buffer.MemoryCopy(srcPtr + sizeof(uint) * 2, dstPtr, vertexCount * sizeof(Vertex2), vertexCount * sizeof(Vertex2));
				fixed (uint* dstPtr = indices)
					Buffer.MemoryCopy(srcPtr + sizeof(uint) * 2 + vertexCount * sizeof(Vertex2), dstPtr, elementCount * sizeof(uint), elementCount * sizeof(uint));
			}
		}
		return true;
	}

	private static void LoadObjFile(string filepath, out Vertex3[] vertices, out uint[] indices)
	{
		static string[] RemoveEmptyStrings(string[] data)
		{
			List<string> result = new();

			for (int i = 0; i < data.Length; i++)
				if (!data[i].Equals(""))
					result.Add(data[i]);

			return result.ToArray();
		}

		List<Vertex3> tVertices = new();
		List<uint> tIndices = new();

		string? line;
		StreamReader file = new($"{Directory.GetCurrentDirectory()}\\{filepath}");
		while ((line = file.ReadLine()) is not null)
		{

			string[] tokens = RemoveEmptyStrings(line.Split(' '));

			if (tokens.Length == 0 || tokens[0].Equals("#"))
				continue;
			else if (tokens[0].Equals("v"))
				tVertices.Add(
					new Vertex3(
						new Vector3(
							float.Parse(tokens[1], CultureInfo.InvariantCulture.NumberFormat),
							float.Parse(tokens[2], CultureInfo.InvariantCulture.NumberFormat),
							float.Parse(tokens[3], CultureInfo.InvariantCulture.NumberFormat)
						)
					)
				);
			else if (tokens[0].Equals("f"))
			{
				tIndices.Add(uint.Parse(tokens[1].Split('/')[0]) - 1);
				tIndices.Add(uint.Parse(tokens[2].Split('/')[0]) - 1);
				tIndices.Add(uint.Parse(tokens[3].Split('/')[0]) - 1);

				if (tokens.Length > 4)
				{
					tIndices.Add(uint.Parse(tokens[1].Split('/')[0]) - 1);
					tIndices.Add(uint.Parse(tokens[3].Split('/')[0]) - 1);
					tIndices.Add(uint.Parse(tokens[4].Split('/')[0]) - 1);
				}
			}
		}

		file.Close();

		vertices = tVertices.ToArray();
		indices = tIndices.ToArray();

		Log.Line($"Mesh Loaded: {Directory.GetCurrentDirectory()}\\{filepath}", Log.Level.NORMAL);
	}

	public static Vertex3[] CalculateNormals(Vertex3[] internalVertexList, uint[] internalIndexList)
	{
		Vertex3[] newList = new Vertex3[internalVertexList.Length];
		for (int i = 0; i < newList.Length; i++)
		{
			newList[i] = internalVertexList[i];
			Vertex3 n = newList[i];
			n = n.SetNormal(Vector3.Zero);
			newList[i] = n;
		}

		for (int i = 0; i < internalIndexList.Length; i += 3)
		{
			uint i0 = internalIndexList[i];
			uint i1 = internalIndexList[i + 1];
			uint i2 = internalIndexList[i + 2];

			Vector3 v1 = newList[i1].Translation - newList[i0].Translation;
			Vector3 v2 = newList[i2].Translation - newList[i0].Translation;

			Vector3 normal = Vector3.Normalize(Vector3.Cross(v1, v2));

			Vertex3 n0 = newList[i0];
			n0 = n0.SetNormal(n0.Normal + normal);
			Vertex3 n1 = newList[i1];
			n1 = n1.SetNormal(n1.Normal + normal);
			Vertex3 n2 = newList[i2];
			n2 = n2.SetNormal(n2.Normal + normal);
			newList[i0] = n0;
			newList[i1] = n1;
			newList[i2] = n2;
		}

		for (int i = 0; i < newList.Length; i++)
		{
			Vertex3 n = newList[i];
			n = n.SetNormal(Vector3.Normalize(n.Normal));
			newList[i] = n;
		}

		return newList;
	}

}