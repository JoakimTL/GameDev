using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lmao {
	class Program {
		static void Main( string[] args ) {
			Bitmap bmp = new Bitmap( @"F:\UNITY\projects\test\Test\Assets\Scenes\93900268_2839337872960043_1713862366816370688_n.png" );

			for( int y = 0; y < bmp.Height; y++ ) {
				for( int x = 0; x < bmp.Width; x++ ) {
					Color pre = bmp.GetPixel( x, y );
					int a = (pre.R + pre.G + pre.B) / 3;
					int r = pre.R;
					int g = pre.G;
					int b = pre.B;
					Color color = Color.FromArgb( a, r, g, b );
					bmp.SetPixel( x, y, color );
				}
			}

			bmp.Save( @"F:\UNITY\projects\test\Test\Assets\Scenes\fixed.png", ImageFormat.Png );

			Console.ReadLine();
		}
	}
}
