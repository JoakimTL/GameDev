using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace Sandbox.Logic.Resources.Materials;

public sealed class Chemical {
	public Chemical( string name, string condensedFormula/*params (Element, uint)[] elements*/ ) {
		Name = name;
		CondensedFormula = condensedFormula;
		ElementCount = ParseFormula( condensedFormula );
		ElementMassPercentage = GetMassPercentagesOfElements( ElementCount );
	}

	public string Name { get; }
	public string CondensedFormula { get; }
	public IReadOnlyDictionary<Element, uint> ElementCount { get; }
	public IReadOnlyDictionary<Element, double> ElementMassPercentage { get; }

	public override string ToString() => Name;

	private static IReadOnlyDictionary<Element, double> GetMassPercentagesOfElements( IReadOnlyDictionary<Element, uint> elementCount ) {
		double totalMass = 0;
		foreach (KeyValuePair<Element, uint> kvp in elementCount)
			totalMass += kvp.Key.AtomicMass * kvp.Value;

		Dictionary<Element, double> massPercentages = [];
		foreach (KeyValuePair<Element, uint> kvp in elementCount)
			massPercentages[ kvp.Key ] = (kvp.Key.AtomicMass * kvp.Value) / totalMass;

		return massPercentages;
	}

	private static Dictionary<Element, uint> ParseFormula( string formula ) {
		Stack<Dictionary<Element, uint>> stack = [];
		stack.Push( [] );

		int index = 0;
		while (index < formula.Length) {
			char c = formula[ index ];

			if (char.IsWhiteSpace( c )) {
				index++;
				continue;
			}

			if (char.IsUpper( c )) {
				string elementSymbol = (index + 1 < formula.Length && char.IsLower( formula[ index + 1 ] )) ? formula[ index..(index + 2) ] : c.ToString();
				Element element = Element.GetBySymbol( elementSymbol ) ?? throw new Exception( $"Unknown element symbol: {elementSymbol}" );
				index += elementSymbol.Length;
				int count = ParseNumber( formula, ref index );
				if (count == -1) // Part of a charge
					count = ParseNumber( formula, ref index );
				if (count == 0) // No count specified
					count = 1;
				Dictionary<Element, uint> currentCounts = stack.Peek();
				uint newCount = (uint) count;
				if (currentCounts.TryGetValue( element, out uint elementCount ))
					newCount += elementCount;
				currentCounts[ element ] = newCount;
				continue;
			}

			if (c == '(' || c == '[' || c == '{') {
				stack.Push( [] );
				index++;
				continue;
			}

			if (c == ')' || c == ']' || c == '}') {
				Dictionary<Element, uint> groupCounts = stack.Pop();
				index++;
				int multiplier = ParseNumber( formula, ref index );
				if (multiplier == -1) // Part of a charge
					multiplier = ParseNumber( formula, ref index );
				if (multiplier == 0) // No count specified
					multiplier = 1;
				foreach (KeyValuePair<Element, uint> kvp in groupCounts) {
					Dictionary<Element, uint> currentCounts = stack.Peek();
					uint newCount = kvp.Value * (uint) multiplier;
					if (currentCounts.TryGetValue( kvp.Key, out uint elementCount ))
						newCount += elementCount;
					currentCounts[ kvp.Key ] = newCount;
				}
				continue;
			}

			index++;
		}

		return stack.Pop();
	}

	private static int ParseNumber( string formula, ref int index ) {
		int startIndex = index;
		while (index < formula.Length && char.IsDigit( formula[ index ] )) {
			index++;
		}
		// If the number is immediately before a '+' or '-', it's part of a charge
		if (index < formula.Length && (formula[ index ] == '+' || formula[ index ] == '-' || formula[index] == '−')) {
			index++;
			return -1;
		}
		if (startIndex < index) {
			return int.Parse( formula[ startIndex..index ] );
		} else {
			return 0;
		}
	}
}

public static class ChemicalList {

	private static readonly List<Chemical> _chemicals;
	public static readonly Dictionary<Element, List<Chemical>> _chemicalsContainingElement;

	static ChemicalList() {
		_chemicals = [];

		IEnumerable<Type> mineralProviders = TypeManager.Registry.ImplementationTypes.Where( t => t.IsAssignableTo( typeof( IChemicalProvider ) ) );
		foreach (Type mineralProvider in mineralProviders) {
			TypePropertyAccessor propertyAccessor = TypeManager.ResolveType( mineralProvider ).GetPropertyAccessor( BindingFlags.Public | BindingFlags.Static, nameof( IChemicalProvider.Chemicals ) );
			IReadOnlyList<Chemical>? chemicalList = propertyAccessor.ReadProperty( null ) as IReadOnlyList<Chemical> ?? throw new InvalidCastException( $"Unable to read mineral information from {mineralProvider.Name}." );
			_chemicals.AddRange( chemicalList );
		}

		_chemicalsContainingElement = [];
		foreach (Element element in Element.AllElements) {
			_chemicalsContainingElement[ element ] = _chemicals.Where( m => m.ElementCount.ContainsKey( element ) ).ToList();
		}
	}

	public static Chemical? GetMineral( string name ) => _chemicals.FirstOrDefault( m => m.Name == name );

	public static IReadOnlyList<Chemical> GetChemicalsContainingElement( Element element ) => _chemicalsContainingElement[ element ];
}
