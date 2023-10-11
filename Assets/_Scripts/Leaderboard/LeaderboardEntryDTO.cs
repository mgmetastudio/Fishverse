using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;


public class LeaderboardEntryDTO : ILeaderboardEntryDTO
{
    [JsonProperty("place")]
    public int Place { get; set; }

    [JsonProperty("login")]
    public string Login { get; set; }

    [JsonProperty("nickName")]
    public string NickName { get; set; }

    [JsonProperty("leaderBoardData")]
    public Dictionary<string, Dictionary<string, float>> LeaderBoardData { get; set; }

    public LeaderboardEntryDTO()
    {
        LeaderBoardData = new Dictionary<string, Dictionary<string, float>>();   
    }
}