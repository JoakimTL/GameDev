using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.MemLib {
	public interface IOGLDisposable : IDisposable{
		bool IsOGLDependant();
	}
}
