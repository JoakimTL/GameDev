using System;
using System.Collections.Generic;
using System.Text;

namespace Engine.Utilities.Data {
	public interface IIdentifiable {
		string Name { get; }
	}
	public interface ITransferable {
		byte[] GetTransferData();
	}
}
