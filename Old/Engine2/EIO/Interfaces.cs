using System;
using System.Collections.Generic;
using System.Text;

namespace ED {
	public interface IIdentifiable {
		string Name { get; }
	}
	public interface ITransferable {
		byte[] GetTransferData();
	}
}
