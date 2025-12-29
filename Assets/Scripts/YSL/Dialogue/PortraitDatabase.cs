using UnityEngine;
using System.Collections.Generic;

public static class PortraitDatabase
{
    // NPC : key = npcType (영문 폴더명)
    private static Dictionary<string, Sprite> npcTable = new();

    // Player : key = personality|emotion
    private static Dictionary<string, Sprite> playerTable = new();

    // personality 최대 개수 (지금 폴더 기준)
    private const int MAX_PERSONALITY = 6;

    // NPC 타입 목록 (폴더명 그대로)
    private static readonly string[] NPC_TYPES =
    {
        "PrayingDevotee",
        "PassionateDevotee",
        "EntrancedDevotee",
        "DevotedDevotee",
        "AmbitiousDevotee"
    };

    public static void LoadAll()
    {
        npcTable.Clear();
        playerTable.Clear();

        LoadNPC();
        LoadPlayer();

        Debug.Log($"[PortraitDatabase] Loaded NPC: {npcTable.Count}, Player: {playerTable.Count}");
    }

    private static void LoadNPC()
    {
        foreach (var type in NPC_TYPES)
        {
            Sprite[] sprites = Resources.LoadAll<Sprite>($"Characters/NPC/{type}");

            foreach (var sp in sprites)
            {
                // emotion 여러 개여도 일단 마지막 것만 사용
                npcTable[type] = sp;
            }
        }
    }

    private static void LoadPlayer()
    {
        for (int personality = 0; personality < MAX_PERSONALITY; personality++)
        {
            Sprite[] sprites = Resources.LoadAll<Sprite>(
                $"Characters/Player/personality_{personality}"
            );

            foreach (var sp in sprites)
            {
                // 파일명 규칙: emotion_0
                string[] parts = sp.name.Split('_');
                if (parts.Length != 2)
                    continue;

                if (!int.TryParse(parts[1], out int emotion))
                    continue;

                string key = MakePlayerKey(personality, emotion);
                playerTable[key] = sp;
            }
        }
    }

    public static Sprite GetNPC(string npcType)
    {
        npcTable.TryGetValue(npcType, out var sp);
        return sp;
    }

    public static Sprite GetPlayer(int personality, int emotion)
    {
        string key = MakePlayerKey(personality, emotion);

        if (playerTable.TryGetValue(key, out var sp))
            return sp;

        // fallback: emotion 0
        key = MakePlayerKey(personality, 0);
        playerTable.TryGetValue(key, out sp);
        return sp;
    }

    private static string MakePlayerKey(int personality, int emotion)
    {
        return $"{personality}|{emotion}";
    }
}
