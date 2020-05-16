using Android.App;
using Android.OS;
using Android.Widget;
using System;
using System.Linq;
using Xamarin.Essentials;

namespace GoogleMapService
{
    [Activity(Label = "Distance")]
    public class Distance : Activity
    {
        EditText txtSrc, txtDest;
        TextView txtDis;
        Button btnCal;
        protected override void OnCreate(Bundle savedInstanceState)//HERE WE CAN CHANGE FIND DISTANCE FOR LOCATIONS//
        {

            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.distance);

            txtSrc = FindViewById<EditText>(Resource.Id.txtsrc);
            txtDest = FindViewById<EditText>(Resource.Id.txtdest);
            txtDis = FindViewById<TextView>(Resource.Id.txtdis);
            btnCal = FindViewById<Button>(Resource.Id.btncal);
            //THIS BUTTON IS USED FOR SELECT LOCATION//
            btnCal.Click += (object sender, EventArgs e) =>
            {
                btnCal_Click(sender, e);
            };
            // Create your application here
        }

        private async void btnCal_Click(object sender, EventArgs e)
        {
            try
            {
                var source = txtSrc.Text;
                var sourceLocation = await Geocoding.GetLocationsAsync(source);
                var destination = txtDest.Text;
                var destinationLocation = await Geocoding.GetLocationsAsync(destination);
                if (sourceLocation != null)
                {
                    var sourceLocations = sourceLocation?.FirstOrDefault();
                    var destinationLocations = destinationLocation?.FirstOrDefault();
                    Location sourceCoordinates = new Location(sourceLocations.Latitude, sourceLocations.Longitude);//HERE LOCATION AUTOMATICALLY SHOW DISTANCE//
                    Location destinationCoordinates = new Location(destinationLocations.Latitude, destinationLocations.Longitude);
                    double distance = Location.CalculateDistance(sourceCoordinates, destinationCoordinates, DistanceUnits.Kilometers);
                    txtDis.Text = "Approx " + Math.Round(distance).ToString() + " KM.";
                }



            }
            catch (FeatureNotSupportedException fnsEx)
            {
                //await DisplayAlert("Faild", fnsEx.Message, "OK");
            }
            catch (PermissionException pEx)
            {
                // await DisplayAlert("Faild", pEx.Message, "OK");
            }
            catch (Exception ex)
            {
                //await DisplayAlert("Faild", ex.Message, "OK");
            }

        }
    }
}
