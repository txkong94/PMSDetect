using System;
using System.Net.Http;
using System.Xml;


namespace PMSDetect
{
    class PlexAPI
    {


        public static string baseURL = "";
        public static string plexToken = "";
        public static string userName = "";
        public static string serverName = "";
        public static string machineId = "";
        public static HttpClient client = new HttpClient();


        public static string getCurrentlyPlaying()
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

                if (doc.SelectSingleNode("MediaContainer").Attributes.GetNamedItem("size").Value == "0") return "";


                var videoNodes = doc.SelectNodes("MediaContainer/Video");
                XmlNode selectedNode = null;
                int addedAt = 0;

                foreach(XmlNode videoNode in videoNodes)
                {
                    string server = videoNode.SelectSingleNode("Player").Attributes.GetNamedItem("title").Value;
                    string machineIdentifier = videoNode.SelectSingleNode("Player").Attributes.GetNamedItem("machineIdentifier").Value;
                    string user = videoNode.SelectSingleNode("User").Attributes.GetNamedItem("title").Value;
                    int addedTime = Convert.ToInt32(videoNode.Attributes.GetNamedItem("addedAt").Value);
                    if (server == serverName && machineIdentifier == machineId && user == userName && addedTime > addedAt)
                    {
                        selectedNode = videoNode;
                        addedAt = addedTime;
                    }
                }

                if (selectedNode == null) return "";

                string type = selectedNode.Attributes.GetNamedItem("type").Value;
                string seriesName;

                if(type == "episode")
                {
                    seriesName = selectedNode.Attributes.GetNamedItem("grandparentTitle").Value + " - " + selectedNode.Attributes.GetNamedItem("index").Value.PadLeft(2, '0');
                }
                else
                {
                    seriesName = selectedNode.Attributes.GetNamedItem("title").Value;
                }

                return seriesName;
            }
            else
            {
                return String.Format("{0} ({1})", (int)response.StatusCode, response.ReasonPhrase);
            }
        }

    }
}
