using GLFW;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.SmallGames.VulkanTest;
public static class Start {

	private static WindowPtr _window;

	private VkInstance _vkInstance;

	public static void Run() {
		InitWindow();

		MainLoop();

		Cleanup();

		//Vulkan.GetInstanceProcAddress();
	}

	private static void InitWindow() {
		Glfw.Init();
		Glfw.WindowHint(Hint.ClientApi, ClientApi.None);
		Glfw.WindowHint(Hint.Resizable, false);
		_window = Glfw.CreateWindow(800, 600, "Vulkan", MonitorPtr.None, WindowPtr.None);
	}

	private static void MainLoop() {
		while (!Glfw.WindowShouldClose( _window )) {
			Glfw.PollEvents();
		}
	}

	private static void Cleanup() {
		Glfw.DestroyWindow( _window );
		Glfw.Terminate();
	}
}
