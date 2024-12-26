using System;
using System.Collections.Generic;
using System.Text;

namespace Engine.Pipelines.Default {
	public class JunctionPipelineExecutor : Junction {

		private Pipeline pipe;

		public JunctionPipelineExecutor( string name, Pipeline pipe ) : base( name, null ) {
			this.pipe = pipe;
			Effect = Execute;
		}

		private void Execute() {
			pipe.Exectute();
		}

	}
}
