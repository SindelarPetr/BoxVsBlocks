// Author: Tommy James Parnell
// notes: Tutorial to show how admob works in xamarin
// email: tparnell8@gmail.com, parnell.tommy@hotmail.com

using Android.Gms.Ads;

namespace BoxVsBlocks.Android.ad
{
    class adlistener : AdListener
    {
        // Declare the delegate (if using non-generic pattern). 
        public delegate void AdLoadedEvent();
        public delegate void AdClosedEvent();
        public delegate void AdOpenedEvent();



        // Declare the event. 
        public event AdLoadedEvent AdLoaded;
        public event AdClosedEvent AdClosed;
        public event AdOpenedEvent AdOpened;

        public override void OnAdLoaded()
        {
            if (AdLoaded != null) this.AdLoaded();
            base.OnAdLoaded();
        }

        public override void OnAdClosed()
        {
            if (AdClosed != null) this.AdClosed();
            base.OnAdClosed();
        }
        public override void OnAdOpened()
        {
            if (AdOpened != null) this.AdOpened();
 	        base.OnAdOpened();
        }
    }
}