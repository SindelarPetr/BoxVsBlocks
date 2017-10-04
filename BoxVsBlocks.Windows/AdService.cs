using GameEngine.Advertisement;
using MasterBlock.Windows;

[assembly: Xamarin.Forms.Dependency(typeof(AdService))]
namespace MasterBlock.Windows
{
	public class AdService : IAdService
	{
		public void ShowInterstitial(string interstitialAdId)
		{

		}
	}
}
