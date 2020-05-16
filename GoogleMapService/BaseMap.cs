using Android.App;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Android.OS;
using System;

namespace GoogleMapService
{
    [Activity(Label = "BaseMap")]
    public class BaseMap : Activity, IOnMapReadyCallback
    {
        private GoogleMap GMap;

        public void OnMapReady(GoogleMap googleMap)//HERE WE CAN CHANGE THE LIVE LOCATION OF THE MAP//
        {
            this.GMap = googleMap;
            LatLng latlng = new LatLng(Convert.ToDouble(-36.848461), Convert.ToDouble(174.763336));
            CameraUpdate camera = CameraUpdateFactory.NewLatLngZoom(latlng, 8);
            GMap.MoveCamera(camera);
            MarkerOptions options = new MarkerOptions().SetPosition(latlng).SetTitle("Auckland, New Zealand");
            GMap.AddMarker(options);
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.activity_base);

            SetUpMap();
        }
        private void SetUpMap()
        {
            if (GMap == null)
            {
                FragmentManager.FindFragmentById<MapFragment>(Resource.Id.map).GetMapAsync(this);
            }
        }
    }
}
