using UnityEngine;
using System.Collections.Generic;
using System.IO;

public static class DialogueCSVLoader
{
    public static List<DialogueNode> LoadDialogueNodes(string resourcePath)
    {
        TextAsset csv = Resources.Load<TextAsset>(resourcePath);
        if (csv == null)
        {
            Debug.LogError($"CSV not found: {resourcePath}");
            return new List<DialogueNode>();
        }

        List<DialogueNode> result = new();
        StringReader reader = new StringReader(csv.text);

        bool isHeader = true;

        while (true)
        {
            string line = reader.ReadLine();
            if (line == null)
                break;

            if (isHeader)
            {
                isHeader = false;
                continue;
            }

            string[] cols = ParseCSVLine(line);
            if (cols.Length < 13)
                continue;

            DialogueNode node = new DialogueNode
            {
                id = int.Parse(cols[0]),
                scene_id = int.Parse(cols[1]),
                speaker = cols[2],
                emotion = int.Parse(cols[3]),
                text = Clean(cols[4]),

                condition_key = cols[5],
                condition_value = cols[6],

                choice0_text = cols[7],
                choice0_next = ParseInt(cols[8]),

                command = cols[9],
                command_target = cols[10],
                command_value = cols[11],

                next_id = ParseInt(cols[12])
            };

            result.Add(node);
        }

        return result;
    }

    private static int ParseInt(string v)
    {
        return int.TryParse(v, out var i) ? i : 0;
    }

    private static string Clean(string s)
    {
        return s.Replace("\"\"\"", "").Replace("\"", "").Replace("\\n", "\n");
    }

    private static string[] ParseCSVLine(string line)
    {
        List<string> cols = new();
        bool inQuotes = false;
        string cur = "";

        foreach (char c in line)
        {
            if (c == '"')
            {
                inQuotes = !inQuotes;
                continue;
            }

            if (c == ',' && !inQuotes)
            {
                cols.Add(cur);
                cur = "";
            }
            else
            {
                cur += c;
            }
        }

        cols.Add(cur);
        return cols.ToArray();
    }
}
