using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Widget;
using System;

namespace GoogleMapService
{
    [Activity(Label = "GoogleMapService", Theme = "@style/MainTheme", MainLauncher = true)] //HERE WE CAN SEE MAIN MAP APP//
    public class MainActivity : AppCompatActivity
    {
        private Button btnBase;
        private Button btnLive;
        private Button btnDis;
        private Button btnQuit;
        private TextView res;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_main);
            //THESE COMMANDS ARE USED FOR CONFIRM BUTTONS//
            res = FindViewById<TextView>(Resource.Id.txtres);
            btnBase = FindViewById<Button>(Resource.Id.btnbase);
            btnBase.Click += (object sender, EventArgs e) =>
            {
                btnBase_Click(sender, e);
            };

            btnLive = FindViewById<Button>(Resource.Id.btnlive);
            btnLive.Click += (object sender, EventArgs e) =>
            {
                btnLive_Click(sender, e);
            };

            btnDis = FindViewById<Button>(Resource.Id.btndis);
            btnDis.Click += (object sender, EventArgs e) =>
            {
                btnDis_Click(sender, e);
            };
            btnQuit = FindViewById<Button>(Resource.Id.btnQuit);
            btnQuit.Click += (object sender, EventArgs e) =>
            {
                btnQuit_Click(sender, e);
            };
        }

        private void btnQuit_Click(object sender, EventArgs e)
        {
            System.Environment.Exit(0);
        }

        private void btnDis_Click(object sender, EventArgs e)
        {
            StartActivity(typeof(Distance));
            Finish();
        }

        private void btnLive_Click(object sender, EventArgs e)
        {
            StartActivity(typeof(LiveLocation));
            Finish();
        }

        private void btnBase_Click(object sender, EventArgs e)
        {
            StartActivity(typeof(BaseMap));
            Finish();
        }
    }
}
