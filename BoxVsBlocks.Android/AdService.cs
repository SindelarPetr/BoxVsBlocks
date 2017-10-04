using Android.Gms.Ads;
using Android.Widget;
using BoxVsBlocks.Android;
using BoxVsBlocks.Android.ad;
using GameEngine.Advertisement;

[assembly: Xamarin.Forms.Dependency(typeof(AdService))]
namespace BoxVsBlocks.Android
{
	public class AdService : IAdService
	{
		AdView _bannerad;

		public void ShowInterstitial(string interstitialAdId = "ca-app-pub-6639044173799596/7932523865")
		{
			var FinalAd = AdWrapper.ConstructFullPageAdd(Game1.Activity, interstitialAdId);
			var intlistener = new adlistener();
			intlistener.AdLoaded += () => { if (FinalAd.IsLoaded) FinalAd.Show(); };
			FinalAd.AdListener = intlistener;
			FinalAd.CustomBuild();
		}

		public void ShowBanner(int layoutId, string bannerAdId = "ca-app-pub-6639044173799596/8220339064")
		{
			_bannerad = AdWrapper.ConstructStandardBanner(Game1.Activity, AdSize.SmartBanner, bannerAdId);
			var listener = new adlistener();
			listener.AdLoaded += () => { };
			_bannerad.AdListener = listener;
			_bannerad.CustomBuild();
			var layout = Game1.Activity.FindViewById<LinearLayout>(layoutId);
			layout.AddView(_bannerad);
		}
	}
}