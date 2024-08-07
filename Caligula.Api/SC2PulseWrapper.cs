using Caligula.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Caligula.Service
{

    //TODO: Remove this, and use existing functionality located in correct places.
    public class SC2PulseWrapper
    {
        private readonly HttpClient _httpClient;

        public SC2PulseWrapper(string baseAddress)
        {
            _httpClient = new HttpClient { BaseAddress = new Uri(baseAddress) };
        }

        public async Task<HttpResponseMessage> GetNameFromId(int id)
        {
            return await _httpClient.GetAsync($"api/character/{id}");
        }        
        
        
        public async Task<HttpResponseMessage> GetProPlayerName(int id)
        {
            return await _httpClient.GetAsync($"api/group?characterId={id}");
        }



    }
}