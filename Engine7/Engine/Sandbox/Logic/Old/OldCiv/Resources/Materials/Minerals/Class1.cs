namespace Sandbox.Logic.Old.OldCiv.Resources.Materials.Minerals;
internal class Class1 {
}


public sealed class OtherMinerals : IChemicalProvider {
	public static IReadOnlyList<Chemical> Chemicals => [
		new("Graphite", "C", "A form of carbon used in pencils, lubricants, batteries, and as a refractory material.", "native", "carbon", "mineral"),
		new("Diamond", "C", "A crystalline form of carbon highly valued as a gemstone and industrial abrasive.", "native", "carbon", "gemstone"),
		new("Sulfur", "S", "A native element used in fertilizers, chemicals, and the vulcanization of rubber.", "native", "nonmetal", "sulfur"),
		new("Corundum", "Al2O3", "A crystalline form of aluminum oxide used as an abrasive and as gemstones (ruby, sapphire).", "oxide", "gemstone", "abrasive"),
		new("Limestone", "CaCO3", "A sedimentary rock primarily composed of calcite, essential for cement and construction.", "carbonate", "construction"),
		new("Marble", "CaCO3", "A metamorphic rock derived from limestone, prized for sculpture and building decoration.", "carbonate", "construction", "gemstone"),
		new("Barite", "BaSO4", "A barium sulfate mineral used in drilling mud, paint, and other industrial applications.", "sulfate", "industrial"),
		new("Dolomite", "CaMg(CO3)2", "A carbonate mineral used in construction and as a source of magnesium.", "carbonate", "construction", "magnesium"),
		new("Serpentine", "Mg3Si2O5(OH)4", "A group of greenish silicate minerals used as asbestos and ornamental stone.", "silicate", "asbestos", "ornamental"),
		new("Calcite", "CaCO3", "A widespread carbonate mineral forming the basis of limestone and marble, commonly found in sedimentary environments.", "carbonate", "calcite", "sedimentary"),
		new("Apatite", "Ca5(PO4)3(F,Cl,OH)", "A key phosphate mineral crucial for fertilizer production and encountered in various rock types.", "phosphate", "apatite", "fertilizer")
	];
}

public sealed class ClayMinerals : IChemicalProvider {
	public static IReadOnlyList<Chemical> Chemicals => [
		new("Kaolinite", "Al2Si2O5(OH)4", "A common clay mineral used in ceramics, paper, and cosmetics.", "clay", "kaolinite", "ceramic"),
		new("Illite", "K(Al,Si)4O10(OH)2", "A fine-grained clay found in sedimentary rocks, valued in ceramics and as a soil conditioner.", "clay", "illite", "ceramic"),
		new("Montmorillonite", "(Na(Al,Mg)2Si4O10(OH)2)3", "A smectite clay known for its high swelling capacity, used in drilling mud and cat litter (hydration implicit).", "clay", "montmorillonite", "smectite"),
		new("Bentonite", "Al2Si4O10(OH)2", "An absorbent clay composed primarily of montmorillonite, used in drilling mud, sealants, and binders.", "clay", "bentonite", "absorbent"),
		new("Chlorite", "Mg5Al(AlSi3O10)(OH)8", "A greenish clay mineral formed under low-grade metamorphism and used in various industrial applications.", "clay", "chlorite", "metamorphic"),
		new("Vermiculite", "Mg3AlSi4O10(OH)2·(H2O)4", "A hydrous phyllosilicate that expands when heated, used in insulation, soil conditioning, and as a lightweight aggregate.", "clay", "vermiculite", "insulation"),
		new("Halloysite", "Al2Si2O5(OH)4·H2O", "A clay mineral similar to kaolinite but with a tubular structure; used in ceramics and emerging nanocomposite applications.", "clay", "halloysite", "ceramic"),
		new("Palygorskite", "Al2Mg2Si8O20(OH)2", "A fibrous clay mineral (also known as attapulgite) used as an adsorbent and in cat litter; formula simplified.", "clay", "palygorskite", "attapulgite")
	];
}

public sealed class SilicateMinerals : IChemicalProvider {
	public static IReadOnlyList<Chemical> Chemicals => [
		// Silicate Minerals
		new("Quartz", "SiO2", "A ubiquitous silicate mineral used in glassmaking, electronics, and jewelry.", "silicate", "quartz", "gemstone"),
		new("Orthoclase Feldspar", "KAlSi3O8", "A potassium feldspar used in ceramics and glass production.", "silicate", "feldspar", "ceramic"),
		new("Anorthite", "CaAl2Si2O8", "A calcium-rich feldspar important in ceramics and glassmaking.", "silicate", "feldspar", "ceramic"),
		new("Muscovite", "KAl2(AlSi3O10)(OH,F)2", "A mica mineral used as an electrical insulator and in cosmetics; formula simplified.", "silicate", "mica"),
		new("Biotite", "K(Mg,Fe)3AlSi3O10(OH)2", "A dark mica mineral used in insulation and minor industrial applications.", "silicate", "mica"),
		new("Talc", "Mg3Si4O10(OH)2", "A soft silicate mineral used in talcum powder, cosmetics, and as a lubricant.", "silicate", "phyllosilicate"),
		new("Diopside", "CaMgSi2O6", "A clinopyroxene silicate used in ceramics and as a gemstone.", "silicate", "pyroxene"),
		new("Olivine", "(Mg,Fe)2SiO4", "A common silicate mineral used in refractory materials and as the gemstone peridot.", "silicate", "olivine", "gemstone"),
		new("Garnet", "Fe3Al2(SiO4)3", "A group of silicate minerals used as abrasives and gemstones; simplified for almandine garnet.", "silicate", "garnet", "gemstone"),
		new("Lapis Lazuli", "((Na,Ca)8(AlSiO4)6(S,SO4,Cl)2)", "A deep blue metamorphic rock valued as a gemstone and pigment; formula simplified.", "silicate", "gemstone", "pigment"),
		new("Topaz", "Al2SiO4(F,OH)2", "A silicate mineral prized as a gemstone, often occurring in a range of colors.", "silicate", "gemstone"),
		new("Sillimanite", "Al2SiO5", "A high-temperature aluminosilicate used in refractory and ceramic applications.", "silicate", "refractory"),
	];
}

public sealed class NativeMetals : IChemicalProvider {
	public static IReadOnlyList<Chemical> Chemicals => [
		// Native Metals
		new("Gold", "Au", "A precious native metal used in jewelry, coinage, and electronics.", "native", "metal", "precious"),
		new("Silver", "Ag", "A valuable native metal prized for its conductivity and decorative appeal.", "native", "metal", "precious"),
		new("Copper", "Cu", "A native metal historically used for tools and currency; found in its pure form.", "native", "metal", "copper"),
		new("Platinum", "Pt", "A rare and valuable native metal used in catalytic converters, jewelry, and industry.", "native", "metal", "precious"),
		new("Palladium", "Pd", "A precious native metal with applications in electronics and catalytic converters.", "native", "metal", "precious"),
		new("Native Iron", "Fe", "A rare native form of iron, significant in early metallurgy.", "native", "metal", "iron"),
	];
}

public sealed class OreMinerals : IChemicalProvider {
	public static IReadOnlyList<Chemical> Chemicals => [
		new("Hematite", "Fe2O3", "An iron oxide mineral and a major ore of iron.", "ore", "iron", "oxide"),
		new("Magnetite", "Fe3O4", "A magnetic iron oxide ore used in steel production.", "ore", "iron", "magnetic", "oxide"),
		new("Limonite", "FeO(OH)", "A hydrated iron oxide mineral used as an iron ore.", "ore", "iron", "hydrated", "oxide"),
		new("Siderite", "FeCO3", "An iron carbonate mineral that serves as an ore of iron.", "ore", "iron", "carbonate"),

		new("Gibbsite", "Al(OH)3", "A mineral form of aluminum hydroxide and a key component in aluminum ores.", "ore", "aluminum", "hydroxide"),
		new("Diaspore", "AlO(OH)", "An aluminum oxide hydroxide mineral used as an ore of aluminum.", "ore", "aluminum", "oxide", "hydroxide"),
		// Bauxite is a complex ore; simplified here as aluminum oxide hydroxide.
		new("Bauxite", "AlO(OH)", "A complex ore of aluminum; represented here with a simplified formula.", "ore", "aluminum"),

		new("Chalcopyrite", "CuFeS2", "The most common copper ore, a copper iron sulfide mineral.", "ore", "copper", "iron", "sulfide"),
		new("Bornite", "Cu5FeS4", "A copper iron sulfide ore with a distinctive iridescent tarnish.", "ore", "copper", "iron", "sulfide"),
		new("Chalcocite", "Cu2S", "A copper sulfide mineral and an important copper ore.", "ore", "copper", "sulfide"),
		new("Malachite", "Cu2CO3(OH)2", "A green copper carbonate hydroxide mineral used as an ore of copper.", "ore", "copper", "carbonate", "hydroxide"),
		new("Azurite", "Cu3(CO3)2(OH)2", "A deep blue copper carbonate hydroxide mineral and ore of copper.", "ore", "copper", "carbonate", "hydroxide"),

		new("Cinnabar", "HgS", "The primary ore of mercury, a mercury sulfide mineral.", "ore", "mercury", "sulfide"),
		new("Sphalerite", "ZnS", "The most important zinc ore, a zinc sulfide mineral.", "ore", "zinc", "sulfide"),
		new("Galena", "PbS", "The primary ore of lead, a lead sulfide mineral.", "ore", "lead", "sulfide"),
		new("Cerussite", "PbCO3", "A lead carbonate mineral that is an important ore of lead.", "ore", "lead", "carbonate"),
		new("Anglesite", "PbSO4", "A lead sulfate mineral used as an ore of lead.", "ore", "lead", "sulfate"),
		new("Cassiterite", "SnO2", "The chief ore of tin, a tin oxide mineral.", "ore", "tin", "oxide"),

		new("Pentlandite", "(Fe,Ni)9S8", "A nickel iron sulfide mineral and primary nickel ore; simplified representation of a solid solution.", "ore", "nickel", "iron", "sulfide"),
		new("Pyrite", "FeS2", "An iron sulfide known as 'fool's gold', an important ore of iron.", "ore", "iron", "sulfide"),
		new("Arsenopyrite", "FeAsS", "An iron arsenic sulfide mineral that serves as a source of arsenic (and sometimes iron).", "ore", "iron", "arsenic", "sulfide"),

		new("Chromite", "FeCr2O4", "The primary ore of chromium, an iron chromium oxide mineral.", "ore", "chromium", "oxide"),
		new("Ilmenite", "FeTiO3", "A titanium-iron oxide mineral used as an ore of titanium.", "ore", "titanium", "iron", "oxide"),
		new("Rutile", "TiO2", "A titanium dioxide mineral and a major ore of titanium.", "ore", "titanium", "oxide"),
		new("Anatase", "TiO2", "A polymorph of titanium dioxide used as an ore of titanium.", "ore", "titanium", "oxide"),

		new("Pyrolusite", "MnO2", "A manganese dioxide mineral and the principal ore of manganese.", "ore", "manganese", "oxide"),
		new("Cobaltite", "CoAsS", "A cobalt arsenic sulfide mineral and an ore of cobalt.", "ore", "cobalt", "arsenic", "sulfide"),
		new("Wolframite", "(Fe,Mn)WO4", "An iron manganese tungstate mineral and the chief ore of tungsten.", "ore", "tungsten", "oxide"),
		new("Scheelite", "CaWO4", "A calcium tungstate mineral and an important ore of tungsten.", "ore", "tungsten", "calcium", "oxide"),

		new("Stibnite", "Sb2S3", "The primary ore of antimony, an antimony sulfide mineral.", "ore", "antimony", "sulfide"),
		// Tetrahedrite is complex in nature; simplified here for game purposes.
		new("Tetrahedrite", "Cu12Sb4S13", "A complex copper antimony sulfide mineral used as an ore of copper; simplified formula provided.", "ore", "copper", "antimony", "sulfide"),

		new("Molybdenite", "MoS2", "The chief ore of molybdenum, a molybdenum sulfide mineral.", "ore", "molybdenum", "sulfide"),
		new("Bismuthinite", "Bi2S3", "A bismuth sulfide mineral that serves as an ore of bismuth.", "ore", "bismuth", "sulfide"),
		new("Realgar", "As4S4", "An arsenic sulfide mineral historically used as an ore of arsenic.", "ore", "arsenic", "sulfide"),
		new("Orpiment", "As2S3", "A bright yellow arsenic sulfide mineral used as an ore of arsenic.", "ore", "arsenic", "sulfide"),

		new("Columbite-Tantalite", "(Fe,Mn)(Nb,Ta)2O6", "A composite ore of niobium and tantalum; represented here with a simplified formula.", "ore", "niobium", "tantalum", "oxide"),
		new("Uraninite", "UO2", "The primary ore of uranium, a uranium oxide mineral.", "ore", "uranium", "oxide"),
		// Monazite is a complex rare earth phosphate ore; formula simplified for gameplay.
		new("Monazite", "(Ce,La,Nd,Th)PO4", "A rare earth phosphate mineral used as an ore of rare earth elements; simplified representation.", "ore", "rare earth", "phosphate"),
		new("Zircon", "ZrSiO4", "A zirconium silicate mineral used as an ore of zirconium.", "ore", "zirconium", "silicate"),

		new("Fluorite", "CaF2", "A calcium fluoride mineral sometimes mined as an ore of fluorine.", "ore", "fluorine", "mineral"),
		new("Halite", "NaCl", "Common salt, the mineral form of sodium chloride.", "ore", "sodium", "chloride"),
		new("Gypsum", "CaSO4·(H2O)2", "A hydrated calcium sulfate mineral used in construction and as a sulfur ore.", "ore", "calcium", "sulfate", "hydrated"),

		new("Beryl", "Be3Al2Si6O18", "A mineral that can serve as an ore of beryllium.", "ore", "beryllium", "silicate"),
		new("Vanadinite", "Pb5(VO4)3Cl", "A lead chlorovanadate mineral used as an ore of vanadium and lead.", "ore", "vanadium", "lead", "chloride"),
		new("Magnesite", "MgCO3", "A primary ore of magnesium, a magnesium carbonate mineral.", "ore", "magnesium", "carbonate")
	];
}
//https://en.wikipedia.org/wiki/List_of_minerals