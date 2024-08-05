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
    public class SC2PulseWrapper
    {
        private readonly HttpClient _httpClient;

        public SC2PulseWrapper(string baseAddress)
        {
            _httpClient = new HttpClient { BaseAddress = new Uri(baseAddress) };
        }

        public async Task<HttpResponseMessage> GetPlayerIdAsync(string playerName)
        {
            var response = await _httpClient.GetAsync($"api/character/search?term={playerName}");
            if (response.IsSuccessStatusCode)
            {
                return response;
            }
            throw new InvalidOperationException("Failed to retrieve player ID.");
        }

        public async Task<HttpResponseMessage> GetGroupedProPlayerMatchHistoryAsync(int proPlayerId, string? date)
        {
            return await _httpClient.GetAsync($"api/group/match?proPlayerId={proPlayerId}&dateCursor={date}&typeCursor=_1V1");
        }

        public async Task<HttpResponseMessage> GetAllIdsFromProPlayerId(int proPlayerId)
        {
            return await _httpClient.GetAsync($"api/group/character/full?proPlayerId={proPlayerId}");
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