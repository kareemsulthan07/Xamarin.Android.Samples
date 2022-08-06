using Android.App;
using Android.OS;
using Android.Runtime;
using Android.Widget;
using AndroidX.AppCompat.App;
using LocationRequest = Android.Gms.Location.LocationRequest;
using LocationCallback = Android.Gms.Location.LocationCallback;
using Android.Gms.Location;
using Android.Content.PM;

namespace GPS
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        TextView locationTextView;
        FusedLocationProviderClient fusedLocationProviderClient;
        LocationRequest locationRequest;
        LocationCallback locationCallback;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.activity_main);

            locationTextView = FindViewById<TextView>(Resource.Id.locationTextView);

            // check the device whether it supports location service
            var hasLocationFeature = PackageManager.HasSystemFeature(PackageManager.FeatureLocation);
            if (hasLocationFeature)
            {
                // if the device supports location service then request permissions for gps/location
                RequestPermissions(new string[]
                {
                    Android.Manifest.Permission.AccessFineLocation,
                }, 999);
            }
        }


        protected override void OnDestroy()
        {
            base.OnDestroy();

            if (fusedLocationProviderClient != null)
            {
                fusedLocationProviderClient.RemoveLocationUpdates(locationCallback);
            }
        }


        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            if (requestCode == 999)
            {
                if (grantResults.Length > 0)
                {
                    if (grantResults[0] == 0)
                    {
                        // permission granted
                        fusedLocationProviderClient = LocationServices.GetFusedLocationProviderClient(this);

                        locationRequest = LocationRequest.Create();
                        locationRequest.SetInterval(60);
                        locationRequest.SetFastestInterval(30);
                        locationRequest.SetMaxWaitTime(2);
                        locationRequest.SetPriority(LocationRequest.PriorityHighAccuracy);

                        locationCallback = new MyLocationCallback(this);
                        fusedLocationProviderClient.RequestLocationUpdates(locationRequest, locationCallback, MainLooper);
                    }
                    else
                    {
                        // permission denied by user
                        // let him/her know why the app has to use gps/location service and take further action
                    }
                }
            }

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }


    public class MyLocationCallback : LocationCallback
    {
        AppCompatActivity activity;
        private TextView locationTextView;

        public MyLocationCallback(AppCompatActivity activity)
        {
            this.activity = activity;
            locationTextView = activity.FindViewById<TextView>(Resource.Id.locationTextView);
        }

        public override void OnLocationResult(LocationResult result)
        {
            base.OnLocationResult(result);

            locationTextView.Text = $"Lat : {result.LastLocation.Latitude}, Long : {result.LastLocation.Longitude}";
        }
    }
}