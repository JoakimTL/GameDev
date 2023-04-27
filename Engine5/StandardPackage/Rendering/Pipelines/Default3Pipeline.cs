using Engine;
using Engine.Datatypes.Composite;
using Engine.Datatypes.Projections;
using Engine.Datatypes.Views;
using Engine.Rendering;
using Engine.Rendering.Contexts.Objects;
using Engine.Rendering.Contexts.Services;
using Engine.Rendering.Contexts.Services.Assets;
using Engine.Rendering.OGL;
using Engine.Structure.Interfaces;
using OpenGL;
using StandardPackage.Rendering.Framebuffers;
using StandardPackage.Rendering.Scenes;
using System.Resources;
using System.Runtime.InteropServices;

namespace StandardPackage.Rendering.Pipelines;
public class Default3Pipeline : IRenderPipeline, IInitializable
{
    private ISceneRender _scene;
    public Perspective Perspective { get; private set; }
    public View3 View { get; private set; }
    public Camera Camera { get; private set; }

    public UniformBlock SceneCameraBlock { get; private set; }
    private DataBlockCollection _sceneDataBlocks;
    private DataBlockCollection _pfxUniforms;
    private UniformBlock _pfxBlock;
    private bool _normal;
    private GeometryBuffer _gBuffer;

    private readonly Window _window;
    private readonly Viewport _viewport;
    private readonly SceneService _sceneService;
    private readonly RenderBufferObjectService _renderBufferObjectService;
    private readonly VertexBufferObjectService _vertexBufferObjectService;
    private readonly ScreenRenderService _screenRenderService;
    private readonly ShaderPipelineService _shaderPipelineService;
    private readonly AssetIndexedTextureService _assetIndexedTextureService;

    //Make Default3Pipeline IInitializable
    //Ensure dependencies initialize before dependent services. So if a service require another service as a constructor parameter, it will automatically initialize after. 

    public Default3Pipeline(Window window, Viewport viewport, SceneService sceneService, RenderBufferObjectService renderBufferObjectService, VertexBufferObjectService vertexBufferObjectService, ScreenRenderService screenRenderService, ShaderPipelineService shaderPipelineService, AssetIndexedTextureService assetIndexedTextureService)
    {
        _window = window;
        _viewport = viewport;
        _sceneService = sceneService;
        _renderBufferObjectService = renderBufferObjectService;
        _vertexBufferObjectService = vertexBufferObjectService;
        _screenRenderService = screenRenderService;
        _shaderPipelineService = shaderPipelineService;
        _assetIndexedTextureService = assetIndexedTextureService;
    }

    public void Initialize()
    {
        _scene = _sceneService.Get(nameof(Default3Scene)).NotNull();
        _gBuffer = new(_viewport, _window);
        Camera = new Camera(View = new View3() { Translation = new(0, 0, 5)}, Perspective = new Perspective.Dynamic(_window, 90));
        _sceneDataBlocks = new DataBlockCollection(this.SceneCameraBlock = new UniformBlock(_vertexBufferObjectService.UniformBuffer, "SceneCameraBlock", (uint)Marshal.SizeOf<SceneCameraBlock>(), ShaderType.VertexShader), _assetIndexedTextureService.ShaderStorageBuffer);
        _pfxUniforms = new DataBlockCollection(this._pfxBlock = new UniformBlock(_vertexBufferObjectService.UniformBuffer, "PFXBlock", 24, ShaderType.FragmentShader));
    }

    public void Update(float time, float deltaTime)
    {
        this.SceneCameraBlock.Write(new SceneCameraBlock(this.Camera.Matrix, this.View.Rotation.Up(), this.View.Rotation.Right()));
        this._gBuffer.Clear();
        this._gBuffer.Bind();
        Gl.Enable(EnableCap.CullFace);
        Gl.Enable(EnableCap.DepthTest);
        _scene.Render("default", this._sceneDataBlocks, Transparency, prim: _window.Context.Service<InputEventService>().Keyboard[/*Should not be here later*/GlfwBinding.Enums.Keys.X] ? PrimitiveType.Lines : PrimitiveType.Triangles);

        //this.Lights.Update(this.Camera, this.Scenes, this.View);
        //this.Lights.Render(this.Camera, this.View, this.Perspective, this.GeometryBuffer);

        _window.SetTitle($"{this.View.Translation} .. {(System.Numerics.Vector3.Dot(-System.Numerics.Vector3.Normalize(this.View.Translation), this.View.Rotation.Forward()) + 1) * 50:N3}%");
    }

    private void Transparency(bool transparent)
    {
        if (transparent)
        {
            Gl.Enable(EnableCap.Blend);
            Gl.BlendFunc(GeometryBuffer.TransparencyColorTextureTarget, BlendingFactor.One, BlendingFactor.One);
            Gl.BlendFunc(GeometryBuffer.TransparencyRevealTextureTarget, BlendingFactor.Zero, BlendingFactor.OneMinusSrcColor);
            Gl.BlendEquation(BlendEquationMode.FuncAdd);
            Gl.DepthFunc(DepthFunction.Less);
            Gl.DepthMask(false);
        }
        else
        {
            Gl.Disable(EnableCap.Blend);
            Gl.DepthFunc(DepthFunction.Less);
            Gl.DepthMask(true);
        }
    }

    public void DrawToScreen()
    {
        this._pfxBlock.Write(new PFXBlock(
            this._gBuffer.DiffuseTexture?.GetHandle() ?? 0,
            this._gBuffer.TransparencyColorTexture?.GetHandle() ?? 0,
            this._gBuffer.TransparencyRevealTexture?.GetHandle() ?? 0
        ));
        _screenRenderService.RenderToScreen(_shaderPipelineService.Get<TestPFX2Shader>(), this._pfxUniforms);
    }

    public struct PFXBlock
    {
        public ulong handleDiffuse;
        public ulong handleAccumulation;
        public ulong handleReveal;

        public PFXBlock(ulong handleDiffuse, ulong handleAccumulation, ulong handleReveal)
        {
            this.handleDiffuse = handleDiffuse;
            this.handleAccumulation = handleAccumulation;
            this.handleReveal = handleReveal;
        }
    }

    public class TestPFX2Fragment : ShaderProgramBase
    {
        protected override void AttachShaders(ShaderSourceService shaderSourceService)
        {
            ShaderSource? source = shaderSourceService.Get("pfx.frag");
            if (source is null)
                return;
            AttachShader(source);
        }
    }

    public class TestPFXFragment : ShaderProgramBase
    {
        protected override void AttachShaders(ShaderSourceService shaderSourceService)
        {
            ShaderSource? source = shaderSourceService.Get("gBuffer.frag");
            if (source is null)
                return;
            AttachShader(source);
        }
    }

    internal class TestPFX2Shader : ShaderPipelineBase
    {
        public override bool UsesTransparency => true;

        protected override IEnumerable<ShaderProgramBase> GetShaderPrograms(ShaderProgramService shaderProgramService)
        {
            yield return shaderProgramService.Get<PfxShaderProgramVertex>();
            yield return shaderProgramService.Get<TestPFXFragment>();
        }
    }
}

public class PfxShaderProgramVertex : ShaderProgramBase
{
    protected override void AttachShaders(ShaderSourceService shaderSourceService)
    {
        ShaderSource? source = shaderSourceService.Get("pfx.vert");
        if (source is null)
            return;
        AttachShader(source);
    }
}