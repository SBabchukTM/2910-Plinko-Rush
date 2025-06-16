using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Application.Services;
using Application.Services.UserData;
using Core;
using Core.Factory;
using UnityEngine;
using Zenject;

public class RecordsFactory : IInitializable
{
    private readonly UserDataService _userDataService;
    private readonly IAssetProvider _assetProvider;
    private readonly GameObjectFactory _gameObjectFactory;

    private GameObject _recordPrefab;

    public RecordsFactory(UserDataService userDataService, IAssetProvider assetProvider,
        GameObjectFactory gameObjectFactory)
    {
        _userDataService = userDataService;
        _assetProvider = assetProvider;
        _gameObjectFactory = gameObjectFactory;
    }

    public async void Initialize()
    {
        _recordPrefab = await _assetProvider.Load<GameObject>(ConstPrefabs.RecordPrefab);
    }

    public List<RecordDisplay> CreateRecordDisplayList()
    {
        var recordsData = CreateRecordsDataList();

        List<RecordDisplay> result = new List<RecordDisplay>(recordsData.Count);

        for (int i = 0; i < recordsData.Count; i++)
        {
            var display = _gameObjectFactory.Create<RecordDisplay>(_recordPrefab);
            display.Initialize(i + 1, recordsData[i].Name, recordsData[i].Score);
            result.Add(display);
        }

        return result;
    }

    private List<RecordData> CreateRecordsDataList()
    {
        var records = CreateFakeRecords();

        var usedData = _userDataService.GetUserData();
        var userRecord = new RecordData(usedData.UserAccountData.Username, usedData.UserInventory.Balance);
        records.Add(userRecord);

        records = records.OrderByDescending(x => x.Score).ToList();
        return records;
    }

    private List<RecordData> CreateFakeRecords() => new()
    {
        new RecordData("Sandy", 10892),
        new RecordData("Sophie", 5802),
        new RecordData("Mike", 3593),
        new RecordData("Mona", 2034),
        new RecordData("John", 1752),
        new RecordData("Lenny", 805),
        new RecordData("Marta", 239),
    };

    private class RecordData
    {
        public string Name;
        public int Score;

        public RecordData(string name, int score)
        {
            Name = name;
            Score = score;
        }
    }
}
