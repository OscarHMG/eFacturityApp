using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using eFacturityApp.Droid.Renderers;
using eFacturityApp.Infraestructure.ExtendedControls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(CustomFrame), typeof(CustomFrameRenderer))]
namespace eFacturityApp.Droid.Renderers
{
    public class CustomFrameRenderer : Xamarin.Forms.Platform.Android.AppCompat.FrameRenderer
    {
        public CustomFrameRenderer(Context context) : base(context)
        {
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Frame> e)
        {
            base.OnElementChanged(e);

            try
            {
                var elem = (CustomFrame)this.Element;
                if (elem != null)
                {
                    //shadow
                    CardElevation = 30f;
                    TranslationZ = 0;
                    SetZ(30f);
                    //
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}