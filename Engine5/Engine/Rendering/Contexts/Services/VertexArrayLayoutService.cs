using Engine.Rendering.Contexts.Objects.VAOs;
using Engine.Structure.Attributes;
using Engine.Structure.Interfaces;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Engine.Rendering.Contexts.Services;

[ProcessAfter<VertexBufferObjectService, IInitializable>]
public class VertexArrayLayoutService : Identifiable, IContextService, IInitializable
{
    private readonly VertexBufferObjectService _vertexBufferObjectService;
    private Dictionary<Type, VertexArrayLayout> _layouts;

    public VertexArrayLayoutService(VertexBufferObjectService vertexBufferObjectService)
    {
        _vertexBufferObjectService = vertexBufferObjectService ?? throw new ArgumentNullException(nameof(vertexBufferObjectService));
        _layouts = new();
    }

    public void Initialize()
    {
        var types = AppDomain.CurrentDomain.GetAssemblies().SelectMany(p => p.GetTypes()).Where(p => p.IsValueType);
        var layoutTypes = types.Where(p => p.GetCustomAttribute<VAO.SetupAttribute>() is not null);

        foreach (var layoutType in layoutTypes)
        {
            var structLayout = layoutType.StructLayoutAttribute;
            if (structLayout is null)
            {
                Log.Warning($"{layoutType.Name} is missing the {nameof(StructLayoutAttribute)}!");
                continue;
            }

            if (structLayout.Value != LayoutKind.Explicit)
            {
                Log.Warning($"{layoutType.Name} has a {nameof(StructLayoutAttribute)}, but the layout is not {nameof(LayoutKind.Explicit)}!");
                continue;
            }

            var valSetup = layoutType.GetCustomAttribute<VAO.SetupAttribute>().NotNull();

            int strideBytes = valSetup.StrideBytesOverride >= 0
                ? valSetup.StrideBytesOverride
                : Marshal.SizeOf(layoutType) + valSetup.TextureCount * sizeof(ushort);

            var vbo = _vertexBufferObjectService.Get(layoutType);

            List<VertexArrayLayoutFieldData> fields = new();

            foreach (var field in layoutType.GetFields())
            {
                var data = field.GetCustomAttribute<VAO.DataAttribute>();
                if (data is null)
                    continue;

                if (!field.FieldType.IsValueType)
                {
                    Log.Warning($"{layoutType.Name}.{field.Name} is not a value type!");
                    continue;
                }

                var fieldOffset = field.GetCustomAttribute<FieldOffsetAttribute>();
                if (fieldOffset is null)
                {
                    Log.Warning($"{layoutType.Name}.{field.Name} is missing the {nameof(FieldOffsetAttribute)}!");
                    continue;
                }

                uint vertices = data.VertexCount;
                int offsetBytes = data.RelativeOffsetBytesOverride >= 0 ? data.RelativeOffsetBytesOverride : fieldOffset.Value;
                uint sizePerVertex = (uint)Marshal.SizeOf(field.FieldType) / vertices;

                if (sizePerVertex == 0)
                    this.LogWarning($"Bytesize per vertex for {layoutType.Name}.{field.Name} is {sizePerVertex}!");
                if (sizePerVertex > 4 && data.AttributeType != VertexArrayAttributeType.LARGE)
                    this.LogWarning($"Bytesize per vertex for {layoutType.Name}.{field.Name} is {sizePerVertex} while not using the {nameof(VertexArrayAttributeType.LARGE)} {nameof(VertexArrayAttributeType)}!");

                while (vertices > 0)
                {
                    uint addedVertices = Math.Min(vertices, 4);

                    fields.Add(new VertexArrayLayoutFieldData(data.VertexAttributeType, addedVertices, (uint)offsetBytes, data.AttributeType, data.Normalized));
                    offsetBytes += (int)(addedVertices * sizePerVertex);
                    vertices -= addedVertices;
                }
            }

            int offset = Marshal.SizeOf(layoutType);
            for (int tex = 0; tex < valSetup.TextureCount; tex += 4)
            {
                int addedTextures = Math.Min(valSetup.TextureCount - tex, 4);
                fields.Add(new VertexArrayLayoutFieldData(OpenGL.VertexAttribType.UnsignedShort, (uint)addedTextures, (uint)offset, VertexArrayAttributeType.INTEGER, false));
                offset += addedTextures * sizeof(ushort);
            }
            _layouts.Add(layoutType, new VertexArrayLayout(layoutType, vbo, valSetup.OffsetBytes, strideBytes, valSetup.InstanceDivisor, fields));
        }
    }

    public VertexArrayLayout? Get(Type t) => _layouts.GetValueOrDefault(t);
}
