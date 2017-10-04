using System;

namespace MasterBlock.Windows
{
#if WINDOWS || LINUX
	/// <summary>
	/// The main class.
	/// </summary>
	public static class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main()
		{
			Xamarin.Forms.DependencyService.Register<AdService>();

			using (var game = new Game1())
				game.Run();
		}
	}
#endif
}
