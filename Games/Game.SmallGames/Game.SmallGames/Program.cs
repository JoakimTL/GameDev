
using Engine.Data;
using Game.SmallGames.Benchmarks;
using GLFW;
using System.Numerics;
using System.Runtime.InteropServices;

Game.SmallGames.VulkanTest.Start.Run();

//BenchmarkDotNet.Running.BenchmarkRunner.Run<VectorKeyDictionaryBenchmarks>();

//Console.WriteLine( Marshal.SizeOf<Vector3>() );
//Console.WriteLine( Marshal.SizeOf<Vector3i>() );

//Random random = new Random();
//List<Vector3i> noMatching = new();
//List<Vector3i> noSameMatching = new();
//List<Vector3i> noSameMatchingBC = new();

//for (int i = 0; i < 10000; i++) {
//	Vector3i randomVector = new Vector3i( random.Next(), random.Next(), random.Next() );

//	Vector3 V3iToV3( Vector3i v3 ) {
//		int x = v3.X;
//		int y = v3.Y;
//		int z = v3.Z;
//		unsafe {
//			float xb = *(float*) &x;
//			float yb = *(float*) &y;
//			float zb = *(float*) &z;
//			return new Vector3( xb, yb, zb );
//		}
//	}
//	Vector3 V3iBitConvert( Vector3i v3 ) {
//		unsafe {
//			return *(Vector3*) &v3;
//		}
//	}

//	Vector3 toV3 = V3iToV3( randomVector );
//	Vector3 bcV3 = V3iBitConvert( randomVector );

//	if (toV3 != bcV3) {
//		noMatching.Add( randomVector );
//	}
//	if (V3iToV3( randomVector ) != toV3) {
//		noSameMatching.Add( randomVector );
//		if (toV3 != toV3)
//			Console.WriteLine( $"Wtf. {toV3}" );
//	}
//	if (V3iBitConvert( randomVector ) != bcV3) {
//		noSameMatchingBC.Add( randomVector );
//		if (bcV3 != bcV3)
//			Console.WriteLine( $"Wtf! {bcV3}" );
//	}
//}

Console.ReadLine();

//Games to recreate:
// 2D:
// Pong (singleplayer)
// Snake
// Tetris
// Space Invaders
// Pacman
// Breakout
// Asteroids
// Flappy Bird
// Minesweeper
// Connect Four
// Tic Tac Toe
// Sudoku
// Chess (/w multiplayer)
// Checkers
// Battleship (/w multiplayer)
// Mastermind
// Solitaire
// Pong (/w multiplayer)

// 3D:
// Wolfenstein 3D
// Doom
// Quake
// Lego Rock Raiders
// Half-Life
// Portal
// Minecraft

// UI Software:
// Paint
// Notepad
// Calculator
// File Explorer
// Web Browser
// Music Player
// Video Player
// Image Viewer
// Text Editor
// Terminal
// IDE