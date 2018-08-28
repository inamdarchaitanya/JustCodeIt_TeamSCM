using Newtonsoft.Json;
using OBD.NET.Common.Communication;
using OBD.NET.Common.Devices;
using OBD.NET.Common.Logging;
using OBD.NET.Common.OBDData;
using OBD.NET.Communication;
using ODB.NET.Desktop.Logging;
using SCM.Model;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace scm.rasp
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        ISerialConnection serialConnection;
        public MainPage()
        {
            this.InitializeComponent();
            var btn = connectionBtn.Content.ToString();
            Task.Factory.StartNew(async () =>
            {
                await ConnectDisconnect(btn);

                ContinuousPush(500, () => true);
            });
        }

        public static async Task Push(ISerialConnection connection, RealTimePayload realTimePayLoad)
        {
            HttpClient httpClient = new HttpClient();
            httpClient.MaxResponseContentBufferSize = 256000;
            var serviceUri = new Uri(@"http://obdmicroservice20180827101800.azurewebsites.net/api/realtimepayloads");
            StringContent realTimePayLoadContent = new StringContent(JsonConvert.SerializeObject(realTimePayLoad), System.Text.Encoding.UTF8, "application/json");
            HttpResponseMessage response = await httpClient.PostAsync(serviceUri, realTimePayLoadContent);
        }

        private static async Task<RealTimePayload> GetRealTimePayload(ELM327 device)
        {
            EngineRPM rpmData = await device.RequestDataAsync<EngineRPM>();
            VehicleSpeed speedData = await device.RequestDataAsync<VehicleSpeed>();
            EngineCoolantTemperature engineCoolantTemperatureData = await device.RequestDataAsync<EngineCoolantTemperature>();
            EngineOilTemperature engineOilTemperatureData = await device.RequestDataAsync<EngineOilTemperature>();
            FuelTankLevelInput fuelTankLevelInputData = await device.RequestDataAsync<FuelTankLevelInput>();

            RealTimePayload realTimePayLoad = new RealTimePayload()
            {
                CarId = "MH12KE2651",
                Rpm = rpmData.Rpm,
                Speed = speedData.Speed,
                CoolantTemperature = engineCoolantTemperatureData.Temperature,
                EngineOilTemperature = 43,
                FuelLevel = 90,
                PayloadTimestamp = DateTime.UtcNow.Ticks
            };
            return realTimePayLoad;
        }

        private volatile bool isCancellationRequested = false;

        private async void Push_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            if(serialConnection != null)
            {
                RealTimePayload realTimePayLoad = await GetRealTimePayload(dev);
                await Push(serialConnection, realTimePayLoad);
            }
        }
        ELM327 dev;
        private async void Connect_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            await ConnectDisconnect(connectionBtn.Content.ToString());
        }

        private async Task ConnectDisconnect(string currentState)
        {
            if (currentState.Equals("Connect"))
            {
                serialConnection = new BluetoothSerialConnection("SPP");

                dev = new ELM327(serialConnection, new OBDConsoleLogger(OBDLogLevel.Debug));

                while (!serialConnection.IsOpen)
                {
                    try
                    {
                        await dev.InitializeAsync();
                    }
                    catch (Exception)
                    {
                    }
                }

                connectionBtn.Content = "Disconnect";
            }
            else
            {
                serialConnection.Dispose();
                dev.Dispose();
                connectionBtn.Content = "Connect";
            }
        }

        private void ContinuousPush_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            var time = int.Parse(delay.Text);
            var tillMinutes = int.Parse(forMinuntes.Text);

            if (string.IsNullOrEmpty(delay.Text))
                return;

            var checkRuntime = string.IsNullOrEmpty(forMinuntes.Text);
            var startTime = DateTime.Now;
            TimeSpan minutes = TimeSpan.FromMinutes(tillMinutes);


            ContinuousPush(time, () =>
            {
                return CancellationCheck(checkRuntime, startTime, minutes);
            });
        }

        private void ContinuousPush(int speepTime, Func<bool> cancellationCriateria)
        {
            Task.Factory.StartNew(async () =>
            {
                while (cancellationCriateria())
                {
                    RealTimePayload realTimePayLoad = await GetRealTimePayload(dev);

                    if (realTimePayLoad.Rpm != 0)
                        await Push(this.serialConnection, realTimePayLoad);

                    if (!isCancellationRequested)
                        await Task.Delay(speepTime);
                }

                isCancellationRequested = false;
            });
        }

        private bool CancellationCheck(bool checkRuntime, DateTime startTime, TimeSpan minutes)
        {
            return !isCancellationRequested && checkRuntime && DateTime.Now < startTime.Add(minutes);
        }

        private void StopContinuosPush_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            isCancellationRequested = true;
        }

        private void ReadErrorCode_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            Task.Factory.StartNew(async () =>
            {
                var result = await dev.RequestErrorCode();
                var byteCodes = (byte[])result;

                errorCode.Text = $"{byteCodes[0]}{byteCodes[1]}{byteCodes[2]}";
            });
        }
    }
}
