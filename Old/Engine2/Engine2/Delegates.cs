using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine {
	public delegate bool SingleValueConditional<T>( T value );
	public delegate void SingleValueChange<T>( T value );
}
