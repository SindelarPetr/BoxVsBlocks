using BoxVsBlock.TextProperties;
using GameEngine.CameraEngine;
using GameEngine.MathEngine;
using GameEngine.Menu.Screens;
using GameEngine.Menu.Screens.TextureObjects;
using GameEngine.PropertiesEngine;
using Microsoft.Xna.Framework;

namespace BoxVsBlock.Menu.MenuMain
{
	public class RecordStar : RotatingTexture
	{
		public RecordStar(Camera camera, Vector2 position, float height, IScreenParentObject parent)
			: base(camera, position, MyMath.ScaleByY(new MyTexture2D(ContentProperties.Star).Size, height), parent,
				  new MyTexture2D(ContentProperties.Star), 0.001f)
		{
		}
	}
}
