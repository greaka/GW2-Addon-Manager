﻿using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;

namespace GW2_Addon_Manager
{
    class UpdateHelpers
    {
        //TODO: Add catch if Github API is down/rejects call
        public static dynamic GitReleaseInfo(string gitUrl)
        {
            var client = new WebClient();
            client.Headers.Add("User-Agent", "request");
            try
            {
                string release_info_json = client.DownloadString(gitUrl);
                return JsonConvert.DeserializeObject(release_info_json);

            }
            catch (WebException)
            {
                MessageBox.Show("Github Servers returned an error; please try again in a few minutes.", "Github API Error", MessageBoxButton.OK, MessageBoxImage.Error);
                SelfUpdate.startUpdater();
                Application.Current.Shutdown();
                return null;
            }
            
        }

        public static async void UpdateAll()
        {
            UpdatingViewModel viewModel = UpdatingViewModel.GetInstance;

            LoaderSetup settingUp = new LoaderSetup();
            await settingUp.HandleLoaderUpdate();

            List<AddonInfoFromYaml> addons = (List<AddonInfoFromYaml>)Application.Current.Properties["Selected"];
            
            foreach (AddonInfoFromYaml addon in addons.Where(add => add != null))
            {
                GenericUpdater updater = new GenericUpdater(addon);
                await updater.Update();
            }

            viewModel.ProgBarLabel = "Updates Complete";
            viewModel.DownloadProgress = 100;
            viewModel.CloseBtnEnabled = true;
        }
    }
}
