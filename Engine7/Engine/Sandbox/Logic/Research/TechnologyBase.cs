using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sandbox.Logic.Research;
public abstract class TechnologyBase {

	protected TechnologyBase( string displayName ) {
		this.DisplayName = displayName;
	}

	public string DisplayName { get; }
}

public enum TechnologyKind {
	/// <summary>
	/// The first initial discovery of a technology.
	/// </summary>
	Discovery,
	/// <summary>
	/// The research of a discovery to uncover its secrets.
	/// </summary>
	Research,
	/// <summary>
	/// The improvement of a technology to make it more efficient.
	/// </summary>
	Improvement,
}