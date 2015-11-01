using Smart_Grid__HackInOut_2._0_.Library.Communication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Maps.MapControl.WPF;
using Smart_Grid__HackInOut_2._0_.Library;

namespace Smart_Grid__HackInOut_2._0_
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        MapLayer MlGridLayer = null;
        MapLayer MlGridPolyLayer = null;

        Location LastLocation = null;

        bool IsAddGrid = false;
        bool IsAddPoly = false;
        bool IsRemove = false;
        bool IsPolyAdded = false;
        public static SmartGridSQLEntities1 PowerGrid = new SmartGridSQLEntities1();

        SmartGridSQLEntities1 S1 = new SmartGridSQLEntities1();
        HttpServer httpServer;
        Thread Main_Thread;


        public MainWindow()
        {
            
            InitializeComponent();

            MainMap.MouseDoubleClick += MainMap_MouseDoubleClick;
            MainMap.MouseUp += MainMap_MouseUp;

            HttpServer _Server = new MyHttpServer(8080);

            _Server.DeviceRegistration += _Server_DeviceRegistration;
            _Server.PingReceived += _Server_PingReceived;

            Thread thread = new Thread(new ThreadStart(_Server.listen));
            thread.Start();
            
        }

        public void Initialize(int HttpPortNumber = 8080)
        {
            httpServer = new MyHttpServer(HttpPortNumber);

            httpServer.PingReceived += HttpServer_PingReceived; ;
            httpServer.DeviceRegistration += HttpServer_DeviceRegistration;

            Main_Thread = new Thread(new ThreadStart(httpServer.listen));
            Main_Thread.Start();
        }

        private void HttpServer_PingReceived(string DeviceSerialNumber, int Status, int Voltage, int Frequency, DateTime PingTime)
        {
            GridStatu X = S1.GridStatus.Where(G => G.DeviceSerialNumber.Trim().Equals(DeviceSerialNumber)).FirstOrDefault();

            if (X != null)
            {
                X.Frequency = Frequency;
                X.Voltage = Voltage;
                X.Status = Status;
                X.LastPingTime = PingTime;
                UpdateChanges(X);
            }
            else
            {
                X = new GridStatu();
                X.DeviceSerialNumber = DeviceSerialNumber;
                X.Frequency = Frequency;
                X.Voltage = Voltage;
                X.Status = Status;
                X.LastPingTime = PingTime;
                S1.GridStatus.Add(X);
                S1.SaveChanges();
            }
        }

        private void HttpServer_DeviceRegistration(string DeviceSerialNumber, DateTime LastPingTime, DateTime InstalledDate, double Longitude, double Latitude)
        {
            int C = S1.CMDeviceInfoes.Where(T => T.DeviceSerialNumber.Trim().Equals(DeviceSerialNumber)).Count();
            if (C == 0)
            {
                CMDeviceInfo C1 = new CMDeviceInfo();
                C1.DeviceSerialNumber = DeviceSerialNumber;
                C1.InstallationDate = InstalledDate;
                C1.LastOnlineTime = LastPingTime;
                C1.Latitude = Latitude;
                C1.Longitude = Longitude;
                S1.CMDeviceInfoes.Add(C1);
                S1.SaveChanges();
            }
        }


        void UpdateChanges(object o1)
        {
            S1.Entry(o1).State = System.Data.Entity.EntityState.Modified;
            S1.SaveChanges();
        }


        void AddChanges(object o1)
        {
            S1.Entry(o1).State = System.Data.Entity.EntityState.Added;
            S1.SaveChanges();
        }

        private void MainMap_MouseUp(object sender, MouseButtonEventArgs e)
        {
            
        }
        
        private void MainMap_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (IsAddGrid)
            {
                IsAddGrid = false;

                Pushpin p = new Pushpin();
                p.Location = MainMap.ViewportPointToLocation(e.GetPosition(this));
                MainMap.Children.Add(p);
                
            }
            else if (IsAddPoly)
            {
                if (!IsPolyAdded)
                {
                    IsPolyAdded = true;
                    LastLocation = MainMap.ViewportPointToLocation(e.GetPosition(this));
                }
                else
                {
                    IsAddPoly = false;
                    IsPolyAdded = false;

                    MapPolyline MapPolyline = new MapPolyline();
                    MapPolyline.Locations.Add(LastLocation);

                    MapPolyline.Locations.Add(MainMap.ViewportPointToLocation(e.GetPosition(this)));
                    LastLocation = null;

                }
            }
        }

        private void _Server_PingReceived(string DeviceSerialNumber, int Status, int Voltage, int Frequency, DateTime PingTime)
        {
            foreach (var _Grid in PowerGrid.GridStatus)
            {
                if(_Grid.DeviceSerialNumber==DeviceSerialNumber)
                {
                    _Grid.Frequency = Frequency;
                    _Grid.LastPingTime = PingTime;
                    _Grid.Status = Status;
                    _Grid.Voltage = Voltage;
                    PowerGrid.SaveChanges();
                }
            }
        }

        private void _Server_DeviceRegistration(string DeviceSerialNumber, DateTime LastPingTime, DateTime InstalledDate, double Longitude, double Latitude)
        {
            
        }

        private void BrdAddGrid_MouseUp(object sender, MouseButtonEventArgs e)
        {
            IsAddGrid = true;
        }
        private void BrdAddGridLine_MouseUp(object sender, MouseButtonEventArgs e)
        {
            IsAddPoly = true;
        }
        private void BrdRemove_MouseUp(object sender, MouseButtonEventArgs e)
        {
            IsRemove = true;
        }

        private void GridOn_MouseUp(object sender, MouseButtonEventArgs e)
        {
            // TODO : Filter grid accroding to power on state
        }
        private void GridOff_MouseUp(object sender, MouseButtonEventArgs e)
        {
            // TODO : Filter grid according to power off state
        }
        private void GridMaintenance_MouseUp(object sender, MouseButtonEventArgs e)
        {
            // TODO : Filter grid according to grid in maintenance
        }
    }
}
