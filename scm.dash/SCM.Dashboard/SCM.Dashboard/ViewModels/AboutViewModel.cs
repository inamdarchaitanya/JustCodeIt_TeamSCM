using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Input;
using Newtonsoft.Json;
using SCM.Dashboard.Models;
using Xamarin.Forms;

namespace SCM.Dashboard.ViewModels
{
    public class AboutViewModel : BaseViewModel
    {
        private double speed;
        private double rpm;
        private double fuelLevel;
        private const string RequestUri = "http://obdmicroservice20180827101800.azurewebsites.net/api/realtimepayloads/latest";

        public AboutViewModel()
        {
            Title = "Real Time Analysis";

            OpenWebCommand = new Command(() => Device.OpenUri(new Uri("https://xamarin.com/platform")));

            ContinuousPull();
        }

        private async void ContinuousPull()
        {
            Task.Factory.StartNew(async () =>
            {
                while (true)
                {
                    using (var httpClient = new HttpClient())
                    {
                        HttpResponseMessage responce = await httpClient.GetAsync(RequestUri);

                        if (responce.IsSuccessStatusCode)
                        {
                            if (responce.Content != null)
                            {
                                var readAsStringAsync = await responce.Content.ReadAsStringAsync();

                                var realTimePayload = JsonConvert.DeserializeObject<RealTimePayload>(readAsStringAsync);

                                Device.BeginInvokeOnMainThread(() =>
                                {
                                    Speed = realTimePayload?.Speed ?? 0;
                                    Rpm = (realTimePayload?.Rpm ?? 0) / 100d;
                                    FuelLevel = realTimePayload?.FuelLevel ?? 0;
                                });
                            }
                        }
                    }

                    await Task.Delay(500);
                }
            });
        }

        public ICommand OpenWebCommand { get; }

        public double Speed
        {
            get => speed;
            set => SetProperty(ref speed, value);
        }

        public double Rpm
        {
            get => rpm;
            set => SetProperty(ref rpm, value);
        }

        public double FuelLevel
        {
            get => fuelLevel;
            set => SetProperty(ref fuelLevel, value);
        }
    }
}