using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ILeaderboardEntryDTO
{
    int Place { get; set; }
    string Login { get; set; }
    string NickName { get; set; }
    Dictionary<string, Dictionary<string, float>> LeaderBoardData { get; set; }
}