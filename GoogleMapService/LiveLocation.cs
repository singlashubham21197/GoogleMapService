using Android;
using Android.App;
using Android.Content.PM;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Android.Locations;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Support.V4.App;
using Android.Support.V4.Content;
using Android.Util;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GoogleMapService
{
    [Activity(Label = "LiveLocation")]
    public class LiveLocation : Activity, ILocationListener, IOnMapReadyCallback //HERE WE CAN CHANGE LIVE LOCATION//
    {
        private GoogleMap GMap;

        const long ONE_MINUTE = 60 * 1000;
        const long FIVE_MINUTES = 5 * ONE_MINUTE;
        static readonly string KEY_REQUESTING_LOCATION_UPDATES = "requesting_location_updates";

        static readonly int RC_LAST_LOCATION_PERMISSION_CHECK = 1000;
        static readonly int RC_LOCATION_UPDATES_PERMISSION_CHECK = 1100;

        //THIS BUTTON IS USED FOR CONFIRM TO FIND LOCATION//
        Double lat, lon;
        bool isRequestingLocationUpdates;
        internal TextView latitude;
        LocationManager locationManager;
        internal TextView longitude;
        internal TextView provider;
        String place;
        View rootLayout;
        private int addresses;

        public void OnLocationChanged(Android.Locations.Location location)
        {
            latitude.Text = Resources.GetString(Resource.String.latitude_string, location.Latitude);
            longitude.Text = Resources.GetString(Resource.String.longitude_string, location.Longitude);
            provider.Text = Resources.GetString(Resource.String.provider_string, location.Provider);
            lat = location.Latitude;
            lon = location.Longitude;
            //latitude.SetText(" "+location.Latitude);
            Geocoder geo = new Geocoder(this, Java.Util.Locale.English);
            IList<Address> addr = geo.GetFromLocation(lat, lon, 1);
            addr.ToList().ForEach((ad) =>
            {
                place = ad.Locality;
            });
            SetUpMap();
        }
        //THIS STRING IS UESD FOR STRING///
        public void OnProviderDisabled(string provider1)
        {
            isRequestingLocationUpdates = false;
            latitude.Text = string.Empty;
            longitude.Text = string.Empty;
            provider.Text = string.Empty;
        }

        public void OnProviderEnabled(string provider1)
        {
            // Nothing to do in this example.
            Log.Debug("LocationExample", "The provider " + provider1 + " is enabled.");
        }

        public void OnStatusChanged(string provider, Availability status, Bundle extras)
        {
            if (status == Availability.OutOfService)
            {
                StopRequestingLocationUpdates();
                isRequestingLocationUpdates = false;
            }
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
        {
            if (requestCode == RC_LAST_LOCATION_PERMISSION_CHECK || requestCode == RC_LOCATION_UPDATES_PERMISSION_CHECK)
            {
                if (grantResults.Length == 1 && grantResults[0] == Permission.Granted)
                {

                    isRequestingLocationUpdates = true;
                    StartRequestingLocationUpdates();
                }
                else
                {
                    //Snackbar.Make(rootLayout, Resource.String.permission_not_granted_termininating_app, Snackbar.LengthIndefinite)
                    //        .SetAction(Resource.String.ok, delegate { FinishAndRemoveTask(); })
                    //        .Show();
                    return;
                }
            }
            else
            {
                Log.Debug("LocationSample", "Don't know how to handle requestCode " + requestCode);
            }

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            locationManager = (LocationManager)GetSystemService(LocationService);

            if (bundle != null)
            {
                isRequestingLocationUpdates = bundle.KeySet().Contains(KEY_REQUESTING_LOCATION_UPDATES) &&
                                              bundle.GetBoolean(KEY_REQUESTING_LOCATION_UPDATES);
            }
            else
            {
                isRequestingLocationUpdates = false;
            }

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.livelocation);

            //requestLocationUpdatesButton = FindViewById<Button>(Resource.Id.request_location_updates_button);
            latitude = FindViewById<TextView>(Resource.Id.latitude2);
            longitude = FindViewById<TextView>(Resource.Id.longitude2);
            provider = FindViewById<TextView>(Resource.Id.provider2);

            if (locationManager.AllProviders.Contains(LocationManager.NetworkProvider) && locationManager.IsProviderEnabled(LocationManager.NetworkProvider))
            {
                if (isRequestingLocationUpdates)
                {
                    isRequestingLocationUpdates = false;
                    StopRequestingLocationUpdates();
                }
                else
                {
                    if (ContextCompat.CheckSelfPermission(this, Manifest.Permission.AccessFineLocation) == Permission.Granted)
                    {
                        StartRequestingLocationUpdates();
                        isRequestingLocationUpdates = true;
                    }
                    else
                    {
                        RequestLocationPermission(RC_LAST_LOCATION_PERMISSION_CHECK);
                    }
                }
            }
            else
            {
                //Snackbar.Make(rootLayout, Resource.String.missing_gps_location_provider, Snackbar.LengthIndefinite)
                //        .SetAction(Resource.String.ok, delegate { FinishAndRemoveTask(); })
                //        .Show();
            }



        }
        protected override void OnStart()
        {
            base.OnStart();
            locationManager = GetSystemService(LocationService) as LocationManager;
        }

        protected override void OnPause()
        {
            locationManager.RemoveUpdates(this);
            base.OnPause();
        }

        void RequestLocationPermission(int requestCode)
        {
            isRequestingLocationUpdates = false;
            if (ActivityCompat.ShouldShowRequestPermissionRationale(this, Manifest.Permission.AccessFineLocation))
            {
                Snackbar.Make(rootLayout, Resource.String.permission_location_rationale, Snackbar.LengthIndefinite)
                        .SetAction(Resource.String.ok,
                                   delegate
                                   {
                                       ActivityCompat.RequestPermissions(this, new[] { Manifest.Permission.AccessFineLocation }, requestCode);
                                   })
                        .Show();
            }
            else
            {
                ActivityCompat.RequestPermissions(this, new[] { Manifest.Permission.AccessFineLocation }, requestCode);
            }
        }

        void StartRequestingLocationUpdates()
        {
            //requestLocationUpdatesButton.SetText(Resource.String.request_location_in_progress_button_text);
            locationManager.RequestLocationUpdates(LocationManager.GpsProvider, ONE_MINUTE, 1, this);
        }

        void StopRequestingLocationUpdates()
        {
            latitude.Text = string.Empty;
            longitude.Text = string.Empty;
            provider.Text = string.Empty;

            //requestLocationUpdatesButton.SetText(Resource.String.request_location_button_text);
            locationManager.RemoveUpdates(this);
        }

        public void OnMapReady(GoogleMap googleMap)
        {
            this.GMap = googleMap;
            //getAddressAsync();
            LatLng latlng = new LatLng(lat, lon);
            CameraUpdate camera = CameraUpdateFactory.NewLatLngZoom(latlng, 3);
            GMap.MoveCamera(camera);
            MarkerOptions options = new MarkerOptions().SetPosition(latlng).SetTitle("I am here." + place);
            GMap.AddMarker(options);
        }

        private void SetUpMap()
        {
            if (GMap == null)
            {
                FragmentManager.FindFragmentById<MapFragment>(Resource.Id.map).GetMapAsync(this);
            }
        }
        //private async System.Threading.Tasks.Task getAddressAsync()
        //{
        //    try
        //    {
        //        //var lat = 47.673988;
        //        //var lon = -122.121513;

        //        var placemarks = await Geocoding.GetPlacemarksAsync(lat, lon) ;

        //        var placemark = placemarks?.FirstOrDefault();
        //        if (placemark != null)
        //        {
        //            var geocodeAddress =
        //                $"AdminArea:       {placemark.AdminArea}\n" +
        //                $"CountryCode:     {placemark.CountryCode}\n" +
        //                $"CountryName:     {placemark.CountryName}\n" +
        //                $"FeatureName:     {placemark.FeatureName}\n" +
        //                $"Locality:        {placemark.Locality}\n" +
        //                $"PostalCode:      {placemark.PostalCode}\n" +
        //                $"SubAdminArea:    {placemark.SubAdminArea}\n" +
        //                $"SubLocality:     {placemark.SubLocality}\n" +
        //                $"SubThoroughfare: {placemark.SubThoroughfare}\n" +
        //                $"Thoroughfare:    {placemark.Thoroughfare}\n";
        //            place = placemark.Locality;
        //            //Console.WriteLine(geocodeAddress);
        //        }
        //    }
        //    catch (FeatureNotSupportedException fnsEx)
        //    {
        //        // Feature not supported on device
        //    }
        //    catch (Exception ex)
        //    {
        //        // Handle exception that may have occurred in geocoding
        //    }
        //}

    }
}
