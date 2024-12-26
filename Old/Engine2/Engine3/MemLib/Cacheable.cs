using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.MemLib {
	public abstract class Cacheable : IDisposable {

		public string Name { get; private set; }
		public uint RefID { get; private set; }

		public Cacheable( string name ) {
			this.Name = name;
			RefID = Mem.ReferenceVault.Add( this );
		}

		public abstract void Dispose();

		public override string ToString() {
			return $"{GetType().Name}[{Name}:{RefID}]";
		}
	}
}
