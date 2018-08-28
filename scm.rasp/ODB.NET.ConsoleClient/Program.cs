using System;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using OBD.NET.Common.Communication;
using OBD.NET.Common.Devices;
using OBD.NET.Common.Extensions;
using OBD.NET.Common.Logging;
using OBD.NET.Common.OBDData;
using ODB.NET.Desktop.Communication;
using ODB.NET.Desktop.Logging;
using SCM.Model;

namespace ODB.NET.ConsoleClient
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //if (args.Length < 1)
            //{
            //    Console.WriteLine("Parameter ComPort needed.");
            //    return;
            //}

            //string comPort = "COM6";

            //using (SerialConnection connection = new SerialConnection(comPort))
            //using (ELM327 dev = new ELM327(connection, new OBDConsoleLogger(OBDLogLevel.Debug)))
            //{

            //    //Realtime

            //    dev.SubscribeDataReceived<EngineRPM>((sender, data) => Console.WriteLine("EngineRPM: " + data.Data.Rpm));
            //    dev.SubscribeDataReceived<VehicleSpeed>((sender, data) => Console.WriteLine("VehicleSpeed: " + data.Data.Speed));
            //    dev.SubscribeDataReceived<EngineCoolantTemperature>((sender, data) => Console.WriteLine("Coolant Temperature: " + data.Data.Temperature));
            //    dev.SubscribeDataReceived<EngineOilTemperature>((sender, data) => Console.WriteLine("Engine Oil Temperature: " + data.Data.Temperature));
            //    dev.SubscribeDataReceived<FuelTankLevelInput>((sender, data) => Console.WriteLine("Coolant Temperature: " + data.Data.Level));

            //    //dev.SubscribeDataReceived<IOBDData>((sender, data) => Console.WriteLine($"PID {data.Data.PID.ToHexString()}: {data.Data}"));

            //    //Analytics

            //    //Calculate Hard Braking, Hard Acceleration, Frequent stoppages, RPM Vs Speed.

            //    //Error Codes
            //    dev.SubscribeDataReceived<EGRError>((sender, data) => Console.WriteLine("EngineRPM: " + data.Data));


            //    dev.Initialize();
            //    dev.RequestData<FuelType>();
            //    for (int i = 0; i < 5; i++)
            //    {
            //        dev.RequestData<EngineRPM>();
            //        dev.RequestData<VehicleSpeed>();
            //        Thread.Sleep(1000);
            //    }
            //}
            //Console.ReadLine();

            //Async example

            connection = new SerialConnection("COM2");
            //MainAsync("COM6").Wait();


            ContinuousPush(3000, () => true);

            Console.ReadKey();
            //Console.ReadLine();

        }
        static ISerialConnection connection;

        /// <summary>
        /// Async example using new RequestDataAsync
        /// </summary>
        /// <param name="comPort">The COM port.</param>
        /// <returns></returns>
        public static async Task MainAsync(string comPort)
        {
            using (ELM327 dev = new ELM327(connection, new OBDConsoleLogger(OBDLogLevel.Debug)))
            {
                dev.Initialize();

                EngineRPM rpmData = await dev.RequestDataAsync<EngineRPM>();
                VehicleSpeed speedData = await dev.RequestDataAsync<VehicleSpeed>();
                EngineCoolantTemperature engineCoolantTemperatureData = await dev.RequestDataAsync<EngineCoolantTemperature>();
                //EngineOilTemperature engineOilTemperatureData = await dev.RequestDataAsync<EngineOilTemperature>();
                //FuelTankLevelInput fuelTankLevelInputData = await dev.RequestDataAsync<FuelTankLevelInput>();

                RealTimePayload realTimePayLoad = new RealTimePayload()
                {
                    CarId = "MH12KE2651",
                    Rpm = rpmData?.Rpm,
                    Speed = speedData?.Speed,
                    CoolantTemperature = engineCoolantTemperatureData?.Temperature,
                    EngineOilTemperature = 87,
                    FuelLevel = 90,
                    PayloadTimestamp = DateTime.UtcNow.Ticks
                };

                HttpClient httpClient = new HttpClient();
                httpClient.MaxResponseContentBufferSize = 256000;
                var serviceUri = new Uri(@"http://obdmicroservice20180827101800.azurewebsites.net/api/realtimepayloads");
                StringContent realTimePayLoadContent = new StringContent(JsonConvert.SerializeObject(realTimePayLoad), Encoding.UTF8, "application/json");
                HttpResponseMessage response = await httpClient.PostAsync(serviceUri, realTimePayLoadContent);
            }
        }

        private static void ContinuousPush(int speepTime, Func<bool> cancellationCriateria)
        {
            ELM327 dev = new ELM327(connection, new OBDConsoleLogger(OBDLogLevel.Debug));
            dev.Initialize();

            Task.Factory.StartNew(async () =>
            {
                dev.SubscribeDataReceived<EGRError>((sender, data) => Console.WriteLine("EngineRPM: " + data.Data));
                connection.DataReceived += Connection_DataReceived;

                float lastRpmPushed = - 1;
                while (cancellationCriateria())
                {
                    RealTimePayload realTimePayLoad = await GetRealTimePayload(dev);

                    if (lastRpmPushed != 0 || realTimePayLoad.Rpm != 0)
                    {
                        await Push(connection, realTimePayLoad);
                        lastRpmPushed = realTimePayLoad.Rpm;
                    }
                     await Task.Delay(speepTime);
                }
            });
        }

        private static void Connection_DataReceived(object sender, OBD.NET.Common.Communication.EventArgs.DataReceivedEventArgs e)
        {
        }

        static byte[] GetBytes(string str)
        {
            byte[] bytes = new byte[str.Length * sizeof(char)];
            System.Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
            return bytes;
        }

        private static async Task<RealTimePayload> GetRealTimePayload(ELM327 device)
        {
            EngineRPM rpmData = await device.RequestDataAsync<EngineRPM>();
            VehicleSpeed speedData = await device.RequestDataAsync<VehicleSpeed>();
            EngineCoolantTemperature engineCoolantTemperatureData = await device.RequestDataAsync<EngineCoolantTemperature>();
            EngineOilTemperature engineOilTemperatureData = await device.RequestDataAsync<EngineOilTemperature>();
            FuelTankLevelInput fuelTankLevelInputData = await device.RequestDataAsync<FuelTankLevelInput>();
            ThrottlePosition throttlePossition = await device.RequestDataAsync<ThrottlePosition>();

            RealTimePayload realTimePayLoad = new RealTimePayload()
            {
                CarId = "MH12KE2651",
                Rpm = rpmData.Rpm,
                Speed = speedData.Speed,
                CoolantTemperature = engineCoolantTemperatureData.Temperature,
                EngineOilTemperature = 43,
                FuelLevel = throttlePossition.Position,
                PayloadTimestamp = DateTime.UtcNow.Ticks
            };
            return realTimePayLoad;
        }

        public static async Task Push(ISerialConnection connection, RealTimePayload realTimePayLoad)
        {
            HttpClient httpClient = new HttpClient();
            httpClient.MaxResponseContentBufferSize = 256000;
            var serviceUri = new Uri(@"http://obdmicroservice20180827101800.azurewebsites.net/api/realtimepayloads");
            StringContent realTimePayLoadContent = new StringContent(JsonConvert.SerializeObject(realTimePayLoad), Encoding.UTF8, "application/json");
            HttpResponseMessage response = await httpClient.PostAsync(serviceUri, realTimePayLoadContent);

            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine($"Speed: {realTimePayLoad.Speed} : RPM: {realTimePayLoad.Rpm} : Fuel Level: {realTimePayLoad.FuelLevel}");
            } else
            {
                Console.WriteLine("FAILED");
            }

        }

        //{
        //    dev.Initialize();
        //    EngineRPM data = await dev.RequestDataAsync<EngineRPM>();
        //    Console.WriteLine("Data: " + data.Rpm);
        //    data = await dev.RequestDataAsync<EngineRPM>();
        //    Console.WriteLine("Data: " + data.Rpm);
        //    VehicleSpeed data2 = await dev.RequestDataAsync<VehicleSpeed>();
        //    Console.WriteLine("Data: " + data2.Speed);
        //    data = await dev.RequestDataAsync<EngineRPM>();
        //    Console.WriteLine("Data: " + data.Rpm);
        //}
    }
}
