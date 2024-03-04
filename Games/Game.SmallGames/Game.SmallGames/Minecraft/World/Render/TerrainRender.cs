using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.SmallGames.Minecraft.World.Render;
public class TerrainRender {

}

public class TerrainRenderFragment {

	//A 256x256x256 voxel volume. This render fragment manages the rendering of all the faces in this 256x256x256 volume. This means allocation, location, removal and deallocation of vertex buffers.
}

public sealed class RenderFragment {

	//A collection of faces. This render fragment is just a pointer to memeory in the buffer. It allows the TerrainRenderFragment to manage it's memory, and for the RenderFragment to be removed from the TerrainRenderFragment without causing issues.
}

public sealed class TerrainFragment {

	//An 8x8x8 voxel volume. This is just a subvolume of the Terrain, it has a TerrainRenderFragment that manages it's rendering, and when a voxel needs to be rendered this fragment is refreshed. This is to allow for fine control while also generating the terrain render data in bulk.
	//The process is this:
	/*
	 * Camera enters within range of TerrainFragment, causing the game to queue the TerrainFragment for rendering.
	 * The terrain fragment is assigned a thread to generate the render data.
	 * The previous terrain render data for this fragment is removed (if any)
	 * The fragment then generates all the data locally, before pushing it as a big bulk of bytes. (Use an UnmanagedList)
	 * The fragment is then given a RenderFragment to remove at a later time if the data is no longer up to date and needs to be regerenated.
	 */

	//To be able to do this the engine needs to allow for data transfer to the mesh component.

	//Maybe design a mesh component that takes in vertex/element data, and one that takes in indexed data? Or allow the user free reigns, but create a nice interface with errors if the user does something wrong.
	//Creating a system that updates on the game logic thread and one that updates in the render loop should be easy. Maybe use an attribute, and a lack of this attribute defaults to the game logic thread?
}
