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
	public static Element Praseodymium { get; }
	public static Element Neodymium { get; }
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
	public static Element Flerovium { get; }
	public static Element Moscovium { get; }
	public static Element Livermorium { get; }
	public static Element Tennessine { get; }
	public static Element Oganesson { get; }

	static Element() {
		//https://pubchem.ncbi.nlm.nih.gov/periodic-table/#view=list
		Hydrogen		= new Element( "Hydrogen",			"H",	1,		1.008,		14.01,		20.28,		0.00008988 );
		Helium			= new Element( "Helium",			"He",	2,		4.0026,		0.95,		4.22,		0.0001785 );
		Lithium			= new Element( "Lithium",			"Li",	3,		6.941,		453.65,		1615,		0.534 );
		Beryllium		= new Element( "Beryllium",			"Be",	4,		9.0122,		1560,		2742,		1.85 );
		Boron			= new Element( "Boron",				"B",	5,		10.81,		2349,		4200,		2.34 );
		Carbon			= new Element( "Carbon",			"C",	6,		12.011,		3800,		4300,		2.267 );
		Nitrogen		= new Element( "Nitrogen",			"N",	7,		14.007,		63.15,		77.36,		0.0012506 );
		Oxygen			= new Element( "Oxygen",			"O",	8,		15.999,		54.36,		90.20,		0.001429 );
		Fluorine		= new Element( "Fluorine",			"F",	9,		18.998,		53.53,		85.03,		0.001696 );
		Neon			= new Element( "Neon",				"Ne",	10,		20.180,		24.56,		27.07,		0.0008999 );
		Sodium			= new Element( "Sodium",			"Na",	11,		22.990,		370.87,		1156,		0.971 );
		Magnesium		= new Element( "Magnesium",			"Mg",	12,		24.305,		923,		1363,		1.738 );
		Aluminum		= new Element( "Aluminum",			"Al",	13,		26.982,		933.47,		2792,		2.698 );
		Silicon			= new Element( "Silicon",			"Si",	14,		28.085,		1687,		3538,		2.329 );
		Phosphorus		= new Element( "Phosphorus",		"P",	15,		30.974,		317.30,		550,		1.82 );
		Sulfur			= new Element( "Sulfur",			"S",	16,		32.06,		388.36,		717.87,		2.067 );
		Chlorine		= new Element( "Chlorine",			"Cl",	17,		35.45,		171.6,		239.11,		0.003214 );
		Argon			= new Element( "Argon",				"Ar",	18,		39.948,		83.80,		87.30,		0.0017837 );
		Potassium		= new Element( "Potassium",			"K",	19,		39.098,		336.53,		1032,		0.862 );
		Calcium			= new Element( "Calcium",			"Ca",	20,		40.078,		1115,		1757,		1.54 );
		Scandium		= new Element( "Scandium",			"Sc",	21,		44.956,		1814,		3109,		2.989 );
		Titanium		= new Element( "Titanium",			"Ti",	22,		47.867,		1941,		3560,		4.54 );
		Vanadium		= new Element( "Vanadium",			"V",	23,		50.942,		2183,		3680,		6.11 );
		Chromium		= new Element( "Chromium",			"Cr",	24,		51.996,		2180,		2944,		7.19 );
		Manganese		= new Element( "Manganese",			"Mn",	25,		54.938,		1519,		2334,		7.21 );
		Iron			= new Element( "Iron",				"Fe",	26,		55.845,		1811,		3134,		7.874 );
		Cobalt			= new Element( "Cobalt",			"Co",	27,		58.933,		1768,		3200,		8.9 );
		Nickel			= new Element( "Nickel",			"Ni",	28,		58.693,		1728,		3186,		8.908 );
		Copper			= new Element( "Copper",			"Cu",	29,		63.546,		1357.77,	2835,		8.96 );
		Zinc			= new Element( "Zinc",				"Zn",	30,		65.38,		692.68,		1180,		7.14 );
		Gallium			= new Element( "Gallium",			"Ga",	31,		69.723,		302.91,		2673,		5.91 );
		Germanium		= new Element( "Germanium",			"Ge",	32,		72.63,		1211.40,	3106,		5.323 );
		Arsenic			= new Element( "Arsenic",			"As",	33,		74.922,		1090,		887,		5.776 );
		Selenium		= new Element( "Selenium",			"Se",	34,		78.96,		494,		958,		4.809 );
		Bromine			= new Element( "Bromine",			"Br",	35,		79.904,		265.8,		332,		3.122 );
		Krypton			= new Element( "Krypton",			"Kr",	36,		83.798,		115.79,		119.93,		0.003733 );
		Rubidium		= new Element( "Rubidium",			"Rb",	37,		85.468,		312.46,		961,		1.532 );
		Strontium		= new Element( "Strontium",			"Sr",	38,		87.62,		1050,		1655,		2.64 );
		Yttrium			= new Element( "Yttrium",			"Y",	39,		88.906,		1799,		3609,		4.469 );
		Zirconium		= new Element( "Zirconium",			"Zr",	40,		91.224,		2128,		4682,		6.52 );
		Niobium			= new Element( "Niobium",			"Nb",	41,		92.906,		2750,		5017,		8.57 );
		Molybdenum		= new Element( "Molybdenum",		"Mo",	42,		95.95,		2896,		4912,		10.22 );
		Technetium		= new Element( "Technetium",		"Tc",	43,		98,			2430,		4538,		11 );
		Ruthenium		= new Element( "Ruthenium",			"Ru",	44,		101.07,		2607,		4423,		12.37 );
		Rhodium			= new Element( "Rhodium",			"Rh",	45,		102.91,		2237,		3968,		12.41 );
		Palladium		= new Element( "Palladium",			"Pd",	46,		106.42,		1828.05,	3236,		12.02 );
		Silver			= new Element( "Silver",			"Ag",	47,		107.87,		1234.93,	2435,		10.49 );
		Cadmium			= new Element( "Cadmium",			"Cd",	48,		112.41,		594.22,		1040,		8.65 );
		Indium			= new Element( "Indium",			"In",	49,		114.82,		429.75,		2345,		7.31 );
		Tin				= new Element( "Tin",				"Sn",	50,		118.71,		505.08,		2875,		7.287 );
		Antimony		= new Element( "Antimony",			"Sb",	51,		121.76,		903.78,		1860,		6.685 );
		Tellurium		= new Element( "Tellurium",			"Te",	52,		127.60,		722.66,		1261,		6.232 );
		Iodine			= new Element( "Iodine",			"I",	53,		126.90,		386.85,		457.4,		4.93 );
		Xenon			= new Element( "Xenon",				"Xe",	54,		131.29,		161.4,		165.03,		0.005887 );
		Caesium			= new Element( "Caesium",			"Cs",	55,		132.91,		301.59,		944,		1.93 );
		Barium			= new Element( "Barium",			"Ba",	56,		137.33,		1000,		2170,		3.62 );
		Lanthanum		= new Element( "Lanthanum",			"La",	57,		138.91,		1193,		3737,		6.145 );
		Cerium			= new Element( "Cerium",			"Ce",	58,		140.12,		1068,		3716,		6.77 );
		Praseodymium	= new Element( "Praseodymium",		"Pr",	59,		140.91,		1208,		3793,		6.77 );
		Neodymium		= new Element( "Neodymium",			"Nd",	60,		144.24,		1297,		3347,		7.01 );
		Promethium		= new Element( "Promethium",		"Pm",	61,		145,		1315,		3273,		7.26 );
		Samarium		= new Element( "Samarium",			"Sm",	62,		150.36,		1345,		2067,		7.52 );
		Europium		= new Element( "Europium",			"Eu",	63,		151.96,		1099,		1802,		5.243 );
		Gadolinium		= new Element( "Gadolinium",		"Gd",	64,		157.25,		1585,		3546,		7.9 );
		Terbium			= new Element( "Terbium",			"Tb",	65,		158.93,		1629,		3503,		8.23 );
		Dysprosium		= new Element( "Dysprosium",		"Dy",	66,		162.50,		1680,		2840,		8.55 );
		Holmium			= new Element( "Holmium",			"Ho",	67,		164.93,		1734,		2993,		8.8 );
		Erbium			= new Element( "Erbium",			"Er",	68,		167.26,		1802,		3141,		9.066 );
		Thulium			= new Element( "Thulium",			"Tm",	69,		168.93,		1818,		2223,		9.321 );
		Ytterbium		= new Element( "Ytterbium",			"Yb",	70,		173.04,		1097,		1469,		6.965 );
		Lutetium		= new Element( "Lutetium",			"Lu",	71,		174.97,		1925,		3675,		9.84 );
		Hafnium			= new Element( "Hafnium",			"Hf",	72,		178.49,		2506,		4876,		13.31 );
		Tantalum		= new Element( "Tantalum",			"Ta",	73,		180.95,		3290,		5731,		16.654 );
		Tungsten		= new Element( "Tungsten",			"W",	74,		183.84,		3695,		5828,		19.25 );
		Rhenium			= new Element( "Rhenium",			"Re",	75,		186.21,		3459,		5869,		21.02 );
		Osmium			= new Element( "Osmium",			"Os",	76,		190.23,		3306,		5285,		22.59 );
		Iridium			= new Element( "Iridium",			"Ir",	77,		192.22,		2719,		4701,		22.56 );
		Platinum		= new Element( "Platinum",			"Pt",	78,		195.08,		2041.4,		4098,		21.45 );
		Gold			= new Element( "Gold",				"Au",	79,		196.97,		1337.33,	3129,		19.32 );
		Mercury			= new Element( "Mercury",			"Hg",	80,		200.59,		234.32,		629.88,		13.534 );
		Thallium		= new Element( "Thallium",			"Tl",	81,		204.38,		577,		1746,		11.85 );
		Lead			= new Element( "Lead",				"Pb",	82,		207.2,		600.61,		2022,		11.34 );
		Bismuth			= new Element( "Bismuth",			"Bi",	83,		208.98,		544.7,		1837,		9.78 );
		Polonium		= new Element( "Polonium",			"Po",	84,		209,		527,		1235,		9.196 );
		Astatine		= new Element( "Astatine",			"At",	85,		210,		575,		610,		7 );
		Radon			= new Element( "Radon",				"Rn",	86,		222,		202,		211.3,		0.00973 );
		Francium		= new Element( "Francium",			"Fr",	87,		223,		300,		950,		1.87 );
		Radium			= new Element( "Radium",			"Ra",	88,		226,		973,		2010,		5.5 );
		Actinium		= new Element( "Actinium",			"Ac",	89,		227,		1323,		3471,		10.07 );
		Thorium			= new Element( "Thorium",			"Th",	90,		232.04,		2023,		5061,		11.72 );
		Protactinium	= new Element( "Protactinium",		"Pa",	91,		231.04,		1841,		4300,		15.37 );
		Uranium			= new Element( "Uranium",			"U",	92,		238.03,		1405.3,		4404,		18.95 );
		Neptunium		= new Element( "Neptunium",			"Np",	93,		237,		917,		4273,		20.45 );
		Plutonium		= new Element( "Plutonium",			"Pu",	94,		244,		912.5,		3501,		19.84 );
		Americium		= new Element( "Americium",			"Am",	95,		243,		1449,		2880,		13.69 );
		Curium			= new Element( "Curium",			"Cm",	96,		247,		1613,		3383,		13.51 );
		Berkelium		= new Element( "Berkelium",			"Bk",	97,		247,		1259,		2900,		14.78 );
		Californium		= new Element( "Californium",		"Cf",	98,		251,		1173,		1743,		15.1 );
		Einsteinium		= new Element( "Einsteinium",		"Es",	99,		252,		1133,		1269,		8.84 );
		Fermium			= new Element( "Fermium",			"Fm",	100,	257,		1800,		0,			0 );
		Mendelevium		= new Element( "Mendelevium",		"Md",	101,	258,		1100,		0,			0 );
		Nobelium		= new Element( "Nobelium",			"No",	102,	259,		1100,		0,			0 );
		Lawrencium		= new Element( "Lawrencium",		"Lr",	103,	262,		1900,		0,			0 );
		Rutherfordium	= new Element( "Rutherfordium",		"Rf",	104,	267,		2400,		0,			0 );
		Dubnium			= new Element( "Dubnium",			"Db",	105,	270,		0,			0,			0 );
		Seaborgium		= new Element( "Seaborgium",		"Sg",	106,	271,		0,			0,			0 );
		Bohrium			= new Element( "Bohrium",			"Bh",	107,	270,		0,			0,			0 );
		Hassium			= new Element( "Hassium",			"Hs",	108,	277,		0,			0,			0 );
		Meitnerium		= new Element( "Meitnerium",		"Mt",	109,	278,		0,			0,			0 );
		Darmstadtium	= new Element( "Darmstadtium",		"Ds",	110,	281,		0,			0,			0 );
		Roentgenium		= new Element( "Roentgenium",		"Rg",	111,	282,		0,			0,			0 );
		Copernicium		= new Element( "Copernicium",		"Cn",	112,	285,		0,			0,			0 );
		Nihonium		= new Element( "Nihonium",			"Nh",	113,	286,		0,			0,			0 );
		Flerovium		= new Element( "Flerovium",			"Fl",	114,	289,		0,			0,			0 );
		Moscovium		= new Element( "Moscovium",			"Mc",	115,	290,		0,			0,			0 );
		Livermorium		= new Element( "Livermorium",		"Lv",	116,	293,		0,			0,			0 );
		Tennessine		= new Element( "Tennessine",		"Ts",	117,	294,		0,			0,			0 );
		Oganesson		= new Element( "Oganesson",			"Og",	118,	294,		0,			0,			0 );
	}

	public string Name { get; }
	public string Symbol { get; }
	public int AtomicNumber { get; }
	public double AtomicMass { get; }
	public double MeltingPoint { get; }
	public double BoilingPoint { get; }
	public double Density { get; }

	/// <param name="name"></param>
	/// <param name="symbol"></param>
	/// <param name="atomicNumber"></param>
	/// <param name="atomicMass"><c>dimensionless</c></param>
	/// <param name="meltingPoint"><c>K</c></param>
	/// <param name="boilingPoint"><c>K</c></param>
	/// <param name="density"><c>g/cm^3</c></param>
	private Element( string name, string symbol, int atomicNumber, double atomicMass, double meltingPoint, double boilingPoint, double density ) {
		this.Name = name;
		this.Symbol = symbol;
		this.AtomicNumber = atomicNumber;
		AtomicMass = atomicMass;
		MeltingPoint = meltingPoint;
		BoilingPoint = boilingPoint;
		Density = density;
	}
}