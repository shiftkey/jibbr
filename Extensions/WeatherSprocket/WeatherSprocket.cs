using System;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using Jabbot.Core;
using Newtonsoft.Json.Linq;

namespace WeatherSprocket
{
    public class WeatherSprocket : RegexSprocket
    {
        private readonly IProxy _proxy;

        public WeatherSprocket(IProxy proxy)
        {
            _proxy = proxy;
        }

        public override Regex Pattern
        {
            get { return new Regex(@"(?<=\bweather[ ])\d{4,5}", RegexOptions.IgnoreCase); }
        }

        public override void ProcessMatch(Match match, ChatMessage chatMessage, IBot bot)
        {
            if (match.Length > 0)
            {
                var matchResult = match.Captures[0].ToString();
                bot.Send(getWeather(matchResult), chatMessage.Room);
            }
        }

        private string getWeather(string zipcode)
        {
            var requestUri = String.Format("http://api.wunderground.com/api/ffb2f3f9960dd675/geolookup/conditions/forecast/q/{0}.json",zipcode);
            var responseFromServer = _proxy.Get(requestUri);
            JObject obj = JObject.Parse(responseFromServer);
            JToken currentObservation = obj["current_observation"];
            JToken displayLocation = obj["current_observation"]["display_location"];
            
            var cityState = (string)displayLocation["full"];
            var weather = (string)currentObservation["weather"];
            var temperature = (string)currentObservation["temperature_string"];
            var output = string.Format("Weather in {0} is {1} and {2}",cityState,temperature,weather);
            return output;
        }

        // TODO: review implementation
        private static string GetResponse(string requestUri)
        {
            var request = (HttpWebRequest) WebRequest.Create(requestUri);
            var response = request.GetResponse();
            var dataStream = response.GetResponseStream();
            var reader = new StreamReader(dataStream);
            var responseFromServer = reader.ReadToEnd();
            reader.Close();
            dataStream.Close();
            response.Close();
            return responseFromServer;
        }
    }
}