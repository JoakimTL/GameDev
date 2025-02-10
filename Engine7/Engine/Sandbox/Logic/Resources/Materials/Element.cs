namespace Sandbox.Logic.Resources.Materials;

public sealed class Element {

	public static Element Hydrogen { get; }
	public static Element Helium { get; }
	public static Element Lithium { get; }
	public static Element Beryllium { get; }
	public static Element Boron { get; }
	public static Element Carbon { get; }
	public static Element Nitrogen { get; }
	public static Element Oxygen { get; }
	public static Element Fluorine { get; }
	public static Element Neon { get; }
	public static Element Sodium { get; }
	public static Element Magnesium { get; }
	public static Element Aluminum { get; }
	public static Element Silicon { get; }
	public static Element Phosphorus { get; }
	public static Element Sulfur { get; }
	public static Element Chlorine { get; }
	public static Element Argon { get; }
	public static Element Potassium { get; }
	public static Element Calcium { get; }
	public static Element Scandium { get; }
	public static Element Titanium { get; }
	public static Element Vanadium { get; }
	public static Element Chromium { get; }
	public static Element Manganese { get; }
	public static Element Iron { get; }
	public static Element Cobalt { get; }
	public static Element Nickel { get; }
	public static Element Copper { get; }
	public static Element Zinc { get; }
	public static Element Gallium { get; }
	public static Element Germanium { get; }
	public static Element Arsenic { get; }
	public static Element Selenium { get; }
	public static Element Bromine { get; }
	public static Element Krypton { get; }
	public static Element Rubidium { get; }
	public static Element Strontium { get; }
	public static Element Yttrium { get; }
	public static Element Zirconium { get; }
	public static Element Niobium { get; }
	public static Element Molybdenum { get; }
	public static Element Technetium { get; }
	public static Element Ruthenium { get; }
	public static Element Rhodium { get; }
	public static Element Palladium { get; }
	public static Element Silver { get; }
	public static Element Cadmium { get; }
	public static Element Indium { get; }
	public static Element Tin { get; }
	public static Element Antimony { get; }
	public static Element Tellurium { get; }
	public static Element Iodine { get; }
	public static Element Xenon { get; }
	public static Element Caesium { get; }
	public static Element Barium { get; }
	public static Element Lanthanum { get; }
	public static Element Cerium { get; }
	public static Element Praseodynium { get; }
	public static Element Neodynium { get; }
	public static Element Promethium { get; }
	public static Element Samarium { get; }
	public static Element Europium { get; }
	public static Element Gadolinium { get; }
	public static Element Terbium { get; }
	public static Element Dysprosium { get; }
	public static Element Holmium { get; }
	public static Element Erbium { get; }
	public static Element Thulium { get; }
	public static Element Ytterbium { get; }
	public static Element Lutetium { get; }
	public static Element Hafnium { get; }
	public static Element Tantalum { get; }
	public static Element Tungsten { get; }
	public static Element Rhenium { get; }
	public static Element Osmium { get; }
	public static Element Iridium { get; }
	public static Element Platinum { get; }
	public static Element Gold { get; }
	public static Element Mercury { get; }
	public static Element Thallium { get; }
	public static Element Lead { get; }
	public static Element Bismuth { get; }
	public static Element Polonium { get; }
	public static Element Astatine { get; }
	public static Element Radon { get; }
	public static Element Francium { get; }
	public static Element Radium { get; }
	public static Element Actinium { get; }
	public static Element Thorium { get; }
	public static Element Protactinium { get; }
	public static Element Uranium { get; }
	public static Element Neptunium { get; }
	public static Element Plutonium { get; }
	public static Element Americium { get; }
	public static Element Curium { get; }
	public static Element Berkelium { get; }
	public static Element Californium { get; }
	public static Element Einsteinium { get; }
	public static Element Fermium { get; }
	public static Element Mendelevium { get; }
	public static Element Nobelium { get; }
	public static Element Lawrencium { get; }
	public static Element Rutherfordium { get; }
	public static Element Dubnium { get; }
	public static Element Seaborgium { get; }
	public static Element Bohrium { get; }
	public static Element Hassium { get; }
	public static Element Meitnerium { get; }
	public static Element Darmstadtium { get; }
	public static Element Roentgenium { get; }
	public static Element Copernicium { get; }
	public static Element Nihonium { get; }
	public static Element Flerovioum { get; }
	public static Element Moscovium { get; }
	public static Element Livermorium { get; }
	public static Element Tennessine { get; }
	public static Element Oganesson { get; }

	static Element() {
		Hydrogen = new Element( "Hydrogen", "H", 1, 1.008, 14.01, 20.28, 0.08988 );
		//Helium = new Element( "Helium", 2, 4.0026, 0.95, 4.22, 0.1785 );
		//Lithium = new Element( "Lithium", 3, 6.94, 453.65, 1615, 0.534 );
		//Beryllium = new Element( "Beryllium", 4, 9.0122, 1560, 2742, 1.85 );
		//Boron = new Element( "Boron", 5, 10.81, 2349, 4200, 2.34 );
		//Carbon = new Element( "Carbon", 6, 12.011, 3800, 4300, 2.267 );
		//Nitrogen = new Element( "Nitrogen", 7, 14.007, 63.15, 77.36, 0.0012506 );
		//Oxygen = new Element( "Oxygen", 8, 15.999, 54.36, 90.20, 0.001429 );
		//Fluorine = new Element( "Fluorine", 9, 18.998, 53.53, 85.03, 0.001696 );
		//Neon = new Element( "Neon", 10, 20.180, 24.56, 27.07, 0.0008999 );
		//Sodium = new Element( "Sodium", 11, 22.990, 370.87, 1156, 0.971 );
		//Magnesium = new Element( "Magnesium", 12, 24.305, 923, 1363, 1.738 );
		//Aluminum = new Element( "Aluminum", 13, 26.982, 933.47, 2792, 2.698 );
		//Silicon = new Element( "Silicon", 14, 28.085, 1687, 3538, 2.329 );
		//Phosphorus = new Element( "Phosphorus", 15, 30.974, 317.3, 550, 1.82 );
		//Sulfur = new Element( "Sulfur", 16, 32.06, 388.36, 717.87, 2.067 );
		//Chlorine = new Element( "Chlorine", 17, 35.45, 171.6, 239.11, 0.003214 );
		//Argon = new Element( "Argon", 18, 39.948, 83.80, 87.30, 0.0017837 );
		//Potassium = new Element( "Potassium", 19, 39.098, 336.53, 1032, 0.862 );
		//Calcium = new Element( "Calcium", 20, 40.078, 1115, 1757, 1.55 );
		//Scandium = new Element( "Scandium", 21, 44.956, 1814, 3109, 2.985 );
		//Titanium = new Element( "Titanium", 22, 47.867, 1941, 3560, 4.506 );
		//Vanadium = new Element( "Vanadium", 23, 50.942, 2183, 3680, 6.11 );
		//Chromium = new Element( "Chromium", 24, 51.996, 2180, 2944, 7.14 );
		//Manganese = new Element( "Manganese", 25, 54.938, 1519, 2334, 7.44 );
		//Iron = new Element( "Iron", 26, 55.845, 1811, 3134, 7.874 );
		//Cobalt = new Element( "Cobalt", 27, 58.933, 1768, 3200, 8.86 );
		//Nickel = new Element( "Nickel", 28, 58.693, 1728, 3186, 8.912 );
		//Copper = new Element( "Copper", 29, 63.546, 1357.77, 2835, 8.96 );
		//Zinc = new Element( "Zinc", 30, 65.38, 692.68, 1180, 7.134 );
		//Gallium = new Element( "Gallium", 31, 69.723, 302.91, 2477, 5.907 );
		//Germanium = new Element( "Germanium", 32, 72.63, 1211.40, 3106, 5.323 );
		//Arsenic = new Element( "Arsenic", 33, 74.922, 1090, 887, 5.776 );
		//Selenium = new Element( "Selenium", 34, 78.971, 494, 958, 4.809 );
		//Bromine = new Element( "Bromine", 35, 79.904, 265.8, 332.0, 3.122 );
		//Krypton = new Element( "Krypton", 36, 83.798, 115.79, 119.93, 0.003733 );


		//Hydrogen.Isotopes = new List<Isotope> {
		//	new Isotope( 1, 3.39e-9 ),
		//	new Isotope( 2, 12.32 )
		//};
	}

	public string Name { get; }
	public string Shorthand { get; }
	public int AtomicNumber { get; }
	public double AtomicMass { get; }
	public double MeltingPoint { get; }
	public double BoilingPoint { get; }
	public double Density { get; }
	//public IReadOnlyList<Isotope> Isotopes { get; private set; }

	private Element( string name, string shorthand, int atomicNumber, double atomicMass, double meltingPoint, double boilingPoint, double density ) {
		this.Name = name;
		this.AtomicNumber = atomicNumber;
		AtomicMass = atomicMass;
		MeltingPoint = meltingPoint;
		BoilingPoint = boilingPoint;
		Density = density;
		//Isotopes = [];
	}
}

//public sealed class Isotope {
//	public Element Element { get; }
//	public int NucleonNumber { get; }
//	public double HalfLife { get; }

//}

//public sealed class IsotopeDecayReaction {
//	public Isotope Parent { get; }
//	public Isotope[] Daughters { get; }
//	public double DecayConstant { get; }
//	public IsotopeDecayReaction( Isotope parent, Isotope[] daughters, double decayConstant ) {
//		Parent = parent;
//		Daughters = daughters;
//		DecayConstant = decayConstant;
//	}
//}