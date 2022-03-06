using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using System.Xml.Linq;

namespace PMSDetect
{
    public partial class PMSDetect : Form
    {
        public PMSDetect()
        {
            var settings = GetSettings("settings.xml");

            settings.TryGetValue("baseURL", out PlexAPI.baseURL);
            settings.TryGetValue("machineIdentifier", out PlexAPI.machineId);
            settings.TryGetValue("server", out PlexAPI.serverName);
            settings.TryGetValue("username", out PlexAPI.userName);
            settings.TryGetValue("token", out PlexAPI.plexToken);

            InitializeComponent();
        }


        public Dictionary<string, string> GetSettings(string path)
        {

            var document = XDocument.Load(path);

            var root = document.Root;
            var results =
              root
                .Elements()
                .ToDictionary(element => element.Name.ToString(), element => element.Value);

            return results;

        }


        private void PMSDetect_Load(object sender, EventArgs e)
        {
            Thread thread = new Thread(new ThreadStart(getCurrentlyPlayingInfo));
            thread.IsBackground = true;
            thread.Start();
        }

        private void changeCurrentlyPlaying(PlexVideoMetaData currentlyPlaying)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => changeCurrentlyPlaying(currentlyPlaying)));
            }
            else
            {
                string status = currentlyPlaying.isPlaying ? "Playing" : "Paused";
                this.Text = currentlyPlaying.title;
                currentlyPlayingLabel.Text = currentlyPlaying.title;
                userLabel.Text = "User: " + currentlyPlaying.user;
                statusLabel.Text = "Status: " + status;
            }
        }



        private void getCurrentlyPlayingInfo()
        {
            while(true)
            {
                PlexVideoMetaData currentlyPlaying = PlexAPI.getCurrentlyPlaying();
                changeCurrentlyPlaying(currentlyPlaying);
                Thread.Sleep(1000);
            }
        }

    }
}
