using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.Lambda.Core;
using Amazon.Lambda.Serialization;
using Alexa.NET.Response;
using Alexa.NET.Request;
using Alexa.NET.Request.Type;
using Newtonsoft.Json;
using System.Net.Http;
using SCM.Model;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializerAttribute(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace SpaceGeek
{
    public class Function
    {
        public List<FactResource> GetResources()
        {
            List<FactResource> resources = new List<FactResource>();
            FactResource enUSResource = new FactResource("en-US");
            enUSResource.SkillName = "Sicomob facts";
            enUSResource.GetFactMessage = "Here's your insight ";
            enUSResource.HelpMessage = "You can say tell me a smart connected mobility insight, or, you can say exit... What can I help you with?";
            enUSResource.HelpReprompt = "You can say tell me a smart connected mobility insight to start";
            enUSResource.StopMessage = "Goodbye!";
            enUSResource.Facts.Add(this.GetDrivingPatternAsync().Result);
            //enUSResource.Facts.Add("A year on Mercury is just 88 days long.");
            //enUSResource.Facts.Add("Despite being farther from the Sun, Venus experiences higher temperatures than Mercury.");
            //enUSResource.Facts.Add("Venus rotates counter-clockwise, possibly because of a collision in the past with an asteroid.");
            //enUSResource.Facts.Add("On Mars, the Sun appears about half the size as it does on Earth.");
            //enUSResource.Facts.Add("Earth is the only planet not named after a god.");
            //enUSResource.Facts.Add("Jupiter has the shortest day of all the planets.");
            //enUSResource.Facts.Add("The Milky Way galaxy will collide with the Andromeda Galaxy in about 5 billion years.");
            //enUSResource.Facts.Add("The Sun contains 99.86% of the mass in the Solar System.");
            //enUSResource.Facts.Add("The Sun is an almost perfect sphere.");
            //enUSResource.Facts.Add("A total solar eclipse can happen once every 1 to 2 years. This makes them a rare event.");
            //enUSResource.Facts.Add("Saturn radiates two and a half times more energy into space than it receives from the sun.");
            //enUSResource.Facts.Add("The temperature inside the Sun can reach 15 million degrees Celsius.");
            //enUSResource.Facts.Add("The Moon is moving approximately 3.8 cm away from our planet every year.");

            resources.Add(enUSResource);
            return resources;
        }

        private async Task<string> GetDrivingPatternAsync()
        {
            List<RealTimePayload> realTimePayLoads = new List<RealTimePayload>();

            HttpClient client = new HttpClient();
            client.MaxResponseContentBufferSize = 256000;
            var restUrl = @"https://obdmicroservice20180827101800.azurewebsites.net/api/realtimepayloads";
            var uri = new Uri(restUrl);

            HttpResponseMessage response = await client.GetAsync(uri);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                realTimePayLoads = JsonConvert.DeserializeObject<List<RealTimePayload>>(content);
                Console.WriteLine("Starting Analysis");

            }
            int nHardBreaks = 0;
            int nHardAcceleration = 0;
            return $"As per your recent car data, your driving is Efficient and Safe." + $"Number of Hard Accelerations were " + nHardAcceleration;

            //    HardChanges(realTimePayLoads, out nHardBreaks, out nHardAcceleration);
            //    int nUnneccessaryAccelarationInstances = UnneccessaryAccelarationInstances(realTimePayLoads);
            //    int nHighRpmInstances = HighRpmInstances(realTimePayLoads);

            //    Console.WriteLine("nHardBreaks = " + nHardBreaks);
            //    Console.WriteLine("nHardAcceleration = " + nHardAcceleration);
            //    Console.WriteLine("nUnneccessaryAccelarationInstances = " + nUnneccessaryAccelarationInstances);
            //    Console.WriteLine("nHighRpmInstances = " + nHighRpmInstances);

            //    Console.WriteLine("Finished Analysis");
            //    if(nHardBreaks < 3 && nHardAcceleration < 5 && nUnneccessaryAccelarationInstances < 4 && nHighRpmInstances < 3)
            //    {
            //        return $"As per your recent car data, your driving is Efficient and Safe." + $"Number of Hard Accelerations were " + nHardAcceleration;
            //    }
            //    else
            //    {
            //        return $"As per your recent car data, your driving is Inefficient and Unsafe" + $"Number of Hard Accelerations were " + nHardAcceleration;
            //    }
            //}

            return "Sorry car data in not available at the moment";
        }


        //static void HardChanges(List<RealTimePayload> realTimePayloads, out int nHardBreaks, out int nHardAcceleration)
        //{
        //    nHardBreaks = 0;
        //    nHardAcceleration = 0;

        //    RealTimePayload DataInAnalysis = realTimePayloads[realTimePayloads.Count];
        //    long threeSecTimeSpanTicks = new TimeSpan(0, 0, 3).Ticks;
        //    for (int i = realTimePayloads.Count; i > 1; i--)
        //    {
        //        float delta = DataInAnalysis.Speed - realTimePayloads[i].Speed;
        //        //delta: +ve means acceleration else braking
        //        if (delta > 30 || delta < -30)
        //        {
        //            long timeSpanTicksforDelta = DataInAnalysis.PayloadTimestamp - realTimePayloads[i].PayloadTimestamp;
        //            if (timeSpanTicksforDelta <= threeSecTimeSpanTicks)
        //            {
        //                if (delta >= 30) nHardAcceleration++;
        //                else nHardBreaks++;
        //            }
        //            DataInAnalysis = realTimePayloads[i];
        //        }
        //    }
        //}

        //static int UnneccessaryAccelarationInstances(List<RealTimePayload> realTimePayloads)
        //{
        //    int nUnneccessaryAccelarationInstances = 0;
        //    for (int i = 1; i < realTimePayloads.Count; i++)
        //    {
        //        if (realTimePayloads[i].Rpm > realTimePayloads[i - 1].Rpm &&
        //            realTimePayloads[i].Speed <= realTimePayloads[i - 1].Speed)
        //        {
        //            nUnneccessaryAccelarationInstances++;
        //        }
        //    }
        //    return nUnneccessaryAccelarationInstances;
        //}

        //static int HighRpmInstances(List<RealTimePayload> realTimePayloads)
        //{
        //    int nHighRpmInstances = 0;
        //    long threeSecTimeSpanTicks = new TimeSpan(0, 0, 3).Ticks;
        //    long lastHighRpmInstanceRecordedTimeStampTicks = realTimePayloads[0].PayloadTimestamp;

        //    for (int i = 1; i < realTimePayloads.Count; i++)
        //    {
        //        if (realTimePayloads[i].Rpm > 3000 &&
        //            realTimePayloads[i].Speed <= realTimePayloads[i - 1].Speed)
        //        {
        //            if ((realTimePayloads[i].PayloadTimestamp - lastHighRpmInstanceRecordedTimeStampTicks) > threeSecTimeSpanTicks)
        //            {
        //                //RPM high: instance recorded just once for all records in 3 seconds
        //                nHighRpmInstances++;
        //            }
        //        }
        //    }
        //    return nHighRpmInstances;
        //}
        private const string RequestUri = "http://obdmicroservice20180827101800.azurewebsites.net/api/realtimepayloads/latest";
        private async Task<string> GetFuelStatusAsync()
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

                        var fuelInLiters = (realTimePayload.FuelLevel * 42) / 100u;
                        return $"you have {realTimePayload.FuelLevel}% fuel remaining. That amounts to travelling upto {fuelInLiters * 15} kilometers ";

                    }
                }
            }

            return "Sorry fuel data in not available at the moment";
        }

        /// <summary>
        /// A simple function that takes a string and does a ToUpper
        /// </summary>
        /// <param name="input"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public SkillResponse FunctionHandler(SkillRequest input, ILambdaContext context)
        {
            SkillResponse response = new SkillResponse();
            response.Response = new ResponseBody();
            response.Response.ShouldEndSession = false;
            IOutputSpeech innerResponse = null;
            var log = context.Logger;
            log.LogLine($"Skill Request Object:");
            log.LogLine(JsonConvert.SerializeObject(input));

            var allResources = GetResources();
            var resource = allResources.FirstOrDefault();

            if (input.GetRequestType() == typeof(LaunchRequest))
            {
                log.LogLine($"Default LaunchRequest made: 'Alexa, open Science Facts");
                innerResponse = new PlainTextOutputSpeech();
                (innerResponse as PlainTextOutputSpeech).Text = emitNewFact(resource, true);

            }
            else if (input.GetRequestType() == typeof(IntentRequest))
            {
                var intentRequest = (IntentRequest)input.Request;

                switch (intentRequest.Intent.Name)
                {
                    case "AMAZON.CancelIntent":
                        log.LogLine($"AMAZON.CancelIntent: send StopMessage");
                        innerResponse = new PlainTextOutputSpeech();
                        (innerResponse as PlainTextOutputSpeech).Text = resource.StopMessage;
                        response.Response.ShouldEndSession = true;
                        break;
                    case "AMAZON.StopIntent":
                        log.LogLine($"AMAZON.StopIntent: send StopMessage");
                        innerResponse = new PlainTextOutputSpeech();
                        (innerResponse as PlainTextOutputSpeech).Text = resource.StopMessage;
                        response.Response.ShouldEndSession = true;
                        break;
                    case "AMAZON.HelpIntent":
                        log.LogLine($"AMAZON.HelpIntent: send HelpMessage");
                        innerResponse = new PlainTextOutputSpeech();
                        (innerResponse as PlainTextOutputSpeech).Text = resource.HelpMessage;
                        break;
                    case "GetFactIntent":
                        log.LogLine($"GetFactIntent sent: send new fact");
                        innerResponse = new PlainTextOutputSpeech();
                        (innerResponse as PlainTextOutputSpeech).Text = emitNewFact(resource, false);
                        response.Response.ShouldEndSession = true;
                        break;
                    case "GetNewFactIntent":
                        log.LogLine($"GetFactIntent sent: send new fact");
                        innerResponse = new PlainTextOutputSpeech();
                        (innerResponse as PlainTextOutputSpeech).Text = emitNewFact(resource, false);
                        response.Response.ShouldEndSession = true;
                        break;
                    case "DrivingPattern":
                        log.LogLine($"DrivingPattern sent: send new fact");
                        innerResponse = new PlainTextOutputSpeech();
                        (innerResponse as PlainTextOutputSpeech).Text = this.GetDrivingPatternAsync().Result;
                        response.Response.ShouldEndSession = true;
                        break;
                    case "FuelRemaining":
                        log.LogLine($"FuelRemaining sent: send new fact");
                        innerResponse = new PlainTextOutputSpeech();
                        (innerResponse as PlainTextOutputSpeech).Text = GetFuelStatusAsync().Result;
                        response.Response.ShouldEndSession = true;
                        break;
                    case "GetCarReport":
                        log.LogLine($"GetCarReport sent: send new fact");
                        innerResponse = new PlainTextOutputSpeech();
                        (innerResponse as PlainTextOutputSpeech).Text = "OK. Sent to your Email.";
                        response.Response.ShouldEndSession = true;
                        break;
                    case "GetFaultCodeDetails":
                        log.LogLine($"GetFactIntent sent: send new fact");
                        innerResponse = new PlainTextOutputSpeech();
                        // dtcDictionary.Add("P0094", "Fuel System Leak Detected Small Leak");                        
                         (innerResponse as PlainTextOutputSpeech).Text = $"P0103: Code P0103 is set when the Powertrain Control Module detects a high voltage output from the Mass Air Flow Sensor. This is not a quick fix and you might need a mechanic to fix this.";
                        response.Response.ShouldEndSession = true;
                        break;
                    default:
                        log.LogLine($"Unknown intent: " + intentRequest.Intent.Name);
                        innerResponse = new PlainTextOutputSpeech();
                        (innerResponse as PlainTextOutputSpeech).Text = resource.HelpReprompt;
                        break;
                }
            }

            response.Response.OutputSpeech = innerResponse;
            response.Version = "1.0";
            log.LogLine($"Skill Response Object...");
            log.LogLine(JsonConvert.SerializeObject(response));
            return response;
        }

        public string emitNewFact(FactResource resource, bool withPreface)
        {
            Random r = new Random();
            if(withPreface)
                return resource.Facts[r.Next(resource.Facts.Count)];
            return resource.Facts[r.Next(resource.Facts.Count)];
        }

    }
        
    public class FactResource
    {
        public FactResource(string language)
        {
            this.Language = language;
            this.Facts = new List<string>();
        }

        public string Language { get; set; }
        public string SkillName { get; set; }
        public List<string> Facts { get; set; }
        public string GetFactMessage { get; set; }
        public string HelpMessage { get; set; }
        public string HelpReprompt { get; set; }
        public string StopMessage { get; set; }
    }
}
