using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Xml;
using System.Linq;


namespace PMSDetect
{
        public struct PlexVideoMetaData
        {
            public PlexVideoMetaData(string title, string user, bool isPlaying, int addedIndex)
            {
                this.title = title;
                this.user = user;
                this.isPlaying = isPlaying;
                this.addedIndex = addedIndex;
            }

            public string title { get; }
            public string user { get; }
            public bool isPlaying { get; }
            // Lower index = newer stream.
            public int addedIndex { get; }


        }

    class PlexAPI
    {


        public static string baseURL = "";
        public static string plexToken = "";
        public static string userName = "";
        public static string serverName = "";
        public static string machineId = "";
        public static HttpClient client = new HttpClient();


        public static PlexVideoMetaData getCurrentlyPlaying()
        {
            if(client.BaseAddress == null)
            {
                client.BaseAddress = new Uri(baseURL + "/status/sessions");
            }
            string urlParam = "?X-Plex-Token=" + plexToken;

            HttpResponseMessage response = client.GetAsync(urlParam).Result;

            if (response.IsSuccessStatusCode)
            {
                // Parse the response body.
                string result = response.Content.ReadAsStringAsync().Result;

                XmlDocument doc = new XmlDocument();
                doc.LoadXml(result);

                if (doc.SelectSingleNode("MediaContainer").Attributes.GetNamedItem("size").Value == "0") return new PlexVideoMetaData("", "", false, 0);

                var videoNodes = doc.SelectNodes("MediaContainer/Video");

                List <PlexVideoMetaData> videos = new List<PlexVideoMetaData>();
                int index = 0;
                foreach(XmlNode videoNode in videoNodes)
                {
                    string user = videoNode.SelectSingleNode("User").Attributes.GetNamedItem("title").Value;

                    if (user != userName)
                    {
                        continue;
                    }


                    //string server = videoNode.SelectSingleNode("Player").Attributes.GetNamedItem("title").Value;
                    //string machineIdentifier = videoNode.SelectSingleNode("Player").Attributes.GetNamedItem("machineIdentifier").Value;
                    bool isPlaying = videoNode.SelectSingleNode("Player").Attributes.GetNamedItem("state").Value == "playing";

                    string type = videoNode.Attributes.GetNamedItem("type").Value;
                    string title;

                    if (type == "episode")
                    {
                        title = videoNode.Attributes.GetNamedItem("grandparentTitle").Value + " - " + videoNode.Attributes.GetNamedItem("index").Value.PadLeft(2, '0');
                    }
                    else
                    {
                        title = videoNode.Attributes.GetNamedItem("title").Value;
                    }

                    videos.Add(new PlexVideoMetaData(title, user, isPlaying, index));
                    index++;
                }
                Console.WriteLine(videos.Count);
                if (videos.Count == 0) return new PlexVideoMetaData("", "", false, 0);

                return videos.OrderByDescending(video => video.isPlaying).ThenBy(video => video.addedIndex).First();
            }
            else
            {
                string error = String.Format("{0} ({1})", (int)response.StatusCode, response.ReasonPhrase);
                return new PlexVideoMetaData(error, error, false, 0);
            }
        }

    }
}
