﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Tomlyn;
using Tomlyn.Model;

namespace ESDStudio;

public class GameInfo
{
    public enum Game
    {
        DarkSoulsIII
    }

    public Game Name;
    public Dictionary<string, string> MapDescriptions = new();
    public Dictionary<string, Dictionary<int, string>> ESDDescriptions = new();

    public GameInfo(string text)
    {
        if (text.EndsWith("DarkSoulsIII.exe", StringComparison.OrdinalIgnoreCase) ||
            text.Equals("DS3", StringComparison.OrdinalIgnoreCase))
        {
            Name = Game.DarkSoulsIII;
            ReadDefaultMapDescriptions("DS3");
            ReadDefaultESDDescriptions("DS3");
        }
    }

    private void ReadDefaultMapDescriptions(string gameId)
    {
        string tomlPath = AppDomain.CurrentDomain.BaseDirectory + @"DefaultDescriptions\DefaultMapDescriptions.toml";
        TomlTable defaultDescriptionsModel = Toml.ToModel(File.ReadAllText(tomlPath), tomlPath);
        defaultDescriptionsModel.TryGetValue(gameId, out object gameObj);
        TomlTable gameModel = (TomlTable)gameObj;
        foreach (string mapName in gameModel.Keys)
        {
            MapDescriptions.Add(mapName, (string)gameModel[mapName]);
        }
    }

    private void ReadDefaultESDDescriptions(string gameId)
    {
        string tomlPath = AppDomain.CurrentDomain.BaseDirectory + $"DefaultDescriptions\\DefaultESDDescriptions_{gameId}.toml";
        TomlTable defaultDescriptionsModel = Toml.ToModel(File.ReadAllText(tomlPath), tomlPath);
        foreach (string mapName in defaultDescriptionsModel.Keys.Where(x => Regex.IsMatch(x, @"m\d\d_\d\d_\d\d_\d\d")))
        {
            TomlTable mapModel = (TomlTable) defaultDescriptionsModel[mapName];
            Dictionary<int, string> esdDictionary = new();
            foreach (int id in mapModel.Keys.Select(int.Parse))
            {
                esdDictionary.Add(id, (string)mapModel[id.ToString()]);
            }
            ESDDescriptions.Add(mapName, esdDictionary);
        }
    }

    public override string ToString()
    {
        if (Name == Game.DarkSoulsIII)
        {
            return "DS3";
        }

        return "Unknown";
    }
}