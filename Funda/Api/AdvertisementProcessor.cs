using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using Funda.Controllers;
using System.Diagnostics;
using System.Threading;
using System.Runtime.CompilerServices;
using System.EnterpriseServices;
using System.Net.Http.Headers;

namespace Funda.Api
{
    public class AdvertisementProcessor
    {
        //In case of request limit exception, I made a new class to handel it
        public class RequestLimitException : Exception {
            public RequestLimitException(string message) : base(message)
            {
            }
        }

        private void PrepareHttpClient(HttpClient pApiClient)
        {
            pApiClient.DefaultRequestHeaders.Accept.Clear();
            pApiClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        //Load the advertisments from the api
        public async Task<AdvertisementModel> LoadAdListAsync(string type, string search,  int PageId)
        {
            //set the parameters of the api
            const int page_size = 25;
            const string key = "ac1b0b1572524640a0ecc54de453ea9f";

            string url = $"http://partnerapi.funda.nl/feeds/Aanbod.svc/json/{ key }/?type={ type }&zo=/{ search }/&page={ PageId }&pagesize={ page_size }";

            using (HttpClient ApiClient = new HttpClient()) {
                PrepareHttpClient(ApiClient);
                using (HttpResponseMessage response = await ApiClient.GetAsync(url))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        Task<AdvertisementModel> ads = response.Content.ReadAsAsync<AdvertisementModel>();
                        return ads.Result;
                    }
                    else
                    {
                        if (response.ReasonPhrase.Contains("Request limit exceeded")) //if the exeption is request limit, then throw my own class
                            throw new RequestLimitException("request: " + url + "\n" + response.ReasonPhrase);
                        else
                            throw new Exception("request: " + url + "\n" + response.ReasonPhrase);
                    }

                }
            }          
        }

        public async Task<List<AgentModel>> GetTopAgentsAsync(string ad_type, string search)
        {
            //create a dictionary to handel the agent name and number of ads
           Dictionary<string, int> agent_map = new Dictionary<string, int>();

            int time_limit_error_count = 0;
            
            bool finished = false;
            int total_pages = -1;
            int page = 0;
            while (!finished)
            {
                page++;
                AdvertisementModel list = null;

                try {
                    list = await LoadAdListAsync(ad_type, search, page);
                }
                catch (RequestLimitException e) { //if the exception is request limit then retry after one minute
                    time_limit_error_count++;

                    if (time_limit_error_count == 3) // if we already tried for twice then quit and throw the error (to avoid contious loop)
                        throw e;

                    await Task.Delay(60000); // wait for a minute

                    page--; //we need to request the same page again

                    continue;
                }

                time_limit_error_count = 0; //clear the number of tries (number of tries is 2 per page)

                if (total_pages == -1) //if total pages is not set yet, then we set it
                    total_pages = list.Paging.AantalPaginas;

                if (list.Objects.Length == 0) // No data was returned due to data request limit 
                    throw new Exception("No data was retured for page: " + page);
          
                if (page == total_pages) //if we arrivied to last page, then set finished to true to exit the loop
                    finished = true;

                foreach (var item in list.Objects)
                {
                    if (!agent_map.ContainsKey(item.MakelaarNaam)) //if the agent is not found before, then add it with numnber of ads = 1
                    {
                        agent_map[item.MakelaarNaam] = 1;
                    }
                    else
                    {
                        agent_map[item.MakelaarNaam]++; //if the agent already exists, increase the number of adds
                    }
                }

            }

            //sort by number of ads and take the first 10
            var sortedDict = (from entry in agent_map orderby entry.Value descending select entry).Take(10);
            
            //convert the results to list of agent model
            List<AgentModel> agents = new List<AgentModel>(10);

            int num = 0;
            foreach (var item in sortedDict)
            {
                num++;
                agents.Add(new AgentModel { RowId = num, AgentName = item.Key, Total = item.Value });
            }

            return agents;
        }
    }
}