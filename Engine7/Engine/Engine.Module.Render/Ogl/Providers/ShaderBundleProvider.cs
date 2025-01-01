using Engine.Module.Render.Ogl.OOP.Shaders;
using Engine.Module.Render.Ogl.Services;

namespace Engine.Module.Render.Ogl.Providers;

public sealed class ShaderBundleProvider( ShaderBundleService shaderBundleService ) : IRenderServiceProvider {
	private readonly ShaderBundleService _shaderBundleService = shaderBundleService;
	public TShaderBundle? GetShaderBundle<TShaderBundle>() where TShaderBundle : ShaderBundleBase => this._shaderBundleService.Get( typeof( TShaderBundle ) ) as TShaderBundle;
}
