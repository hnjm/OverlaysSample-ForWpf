using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using ThinkGeo.MapSuite;
using ThinkGeo.MapSuite.Shapes;
using ThinkGeo.MapSuite.Wpf;

namespace Overlays
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private string selectedBaseMap;
        private string selectedMapType;
        private Dictionary<string, List<string>> baseMaps = new Dictionary<string, List<string>>();

        public event PropertyChangedEventHandler PropertyChanged;

        public MainWindow()
        {
            InitializeComponent();
            InitializeMap();

            DataContext = this;

            baseMaps.Add("Google Maps", new List<string>() { "RoadMap", "Mobile", "Satellite", "Terrain", "Hybrid" });
            baseMaps.Add("Bing Maps", new List<string>() { "Road", "AerialWithLabels", "Aerial" });
            baseMaps.Add("World Kit Map", new List<string>() { "Road", "AerialWithLabels", "Aerial" });
            baseMaps.Add("Open Street Map", new List<string>() { "" });
        }

        public Dictionary<string, List<string>> BaseMaps
        {
            get { return baseMaps; }
            set { baseMaps = value; }
        }

        public string SelectedBaseMap
        {
            get { return selectedBaseMap; }
            set
            {
                selectedBaseMap = value;
                OnPropertyChanged("SelectedBaseMap");
                SelectedMapType = baseMaps[selectedBaseMap].First();
            }
        }

        public string SelectedMapType
        {
            get { return selectedMapType; }
            set
            {
                selectedMapType = value;
                OnPropertyChanged("SelectedMapType");
                RefreshMap();
            }
        }

        protected void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }

        private void InitializeMap()
        {
            map.MapUnit = GeographyUnit.Meter;
            WorldStreetsAndImageryOverlay worldOverlay = new WorldStreetsAndImageryOverlay();
            worldOverlay.IsVisible = false;
            worldOverlay.Projection = WorldStreetsAndImageryProjection.SphericalMercator;
            worldOverlay.MapType = WorldStreetsAndImageryMapType.Road;
            map.Overlays.Add("World Kit Map", worldOverlay);

            GoogleMapsOverlay googleMapOverlay = new GoogleMapsOverlay();
            googleMapOverlay.IsVisible = false;
            map.Overlays.Add("Google Maps", googleMapOverlay);

            BingMapsOverlay bingMapOverlay = new BingMapsOverlay();
            bingMapOverlay.ApplicationId = ""; //Please set your application id.
            bingMapOverlay.IsVisible = false;
            map.Overlays.Add("Bing Maps", bingMapOverlay);

            OpenStreetMapOverlay osmOverlay = new OpenStreetMapOverlay();
            osmOverlay.IsVisible = false;
            map.Overlays.Add("Open Street Map", osmOverlay);
        }

        private void RefreshMap()
        {
            foreach (var overlay in map.Overlays)
                overlay.IsVisible = false;

            var selectedOverlay = map.Overlays[SelectedBaseMap];
            selectedOverlay.IsVisible = true;

            switch (SelectedBaseMap)
            {
                case "World Kit Map":
                    var worldOverlay = selectedOverlay as WorldStreetsAndImageryOverlay;
                    worldOverlay.MapType = (WorldStreetsAndImageryMapType)Enum.Parse(typeof(WorldStreetsAndImageryMapType), selectedMapType);
                    break;
                case "Google Maps":
                    var googleMapsOverlay = selectedOverlay as GoogleMapsOverlay;
                    googleMapsOverlay.MapType = (GoogleMapsMapType)Enum.Parse(typeof(GoogleMapsMapType), selectedMapType);
                    break;
                case "Bing Maps":
                    var bingMapsOverlay = selectedOverlay as BingMapsOverlay;
                    bingMapsOverlay.MapType = (BingMapsMapType)Enum.Parse(typeof(BingMapsMapType), selectedMapType);
                    break;
                case "Open Street Map":
                default:
                    break;
            }

            map.CurrentExtent = new RectangleShape(-13086298.60, 7339062.72, -8111177.75, 2853137.62);
            map.Refresh(selectedOverlay);
        }
    }
}
