using Engine.Module.Entities.Container;
using Engine.Module.Entities.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Module.Render.Entities.Providers;
public sealed class SynchronizedEntityContainerProvider : IServiceProvider {

	private readonly SynchronizedEntityContainerService _synchronizedEntityContainerService;

	public SynchronizedEntityContainerProvider( SynchronizedEntityContainerService synchronizedEntityContainerService ) {
		this._synchronizedEntityContainerService = synchronizedEntityContainerService;
	}

	public IReadOnlyList<SynchronizedEntityContainer> SynchronizedContainers => this._synchronizedEntityContainerService.SynchronizedContainers;
}
