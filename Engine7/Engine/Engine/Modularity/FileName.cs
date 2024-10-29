using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Modularity;
internal class FileName {
}


public abstract class ModuleBase {

	public IServiceProvider ServiceProvider { get; }
	public IServiceCatalog ServiceCatalog { get; }
	public IServiceLibrary ServiceLibrary { get; }

	public ModuleBase() {
		ServiceLibrary serviceLibrary = new();
		ServiceCatalog serviceCatalog = new( serviceLibrary );
		ServiceProvider serviceProvider = new( serviceCatalog );
		this.ServiceProvider = serviceProvider;
		this.ServiceCatalog = serviceCatalog;
		this.ServiceLibrary = serviceLibrary;
	}

}