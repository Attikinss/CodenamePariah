using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public static class Telemetry
{
    private static string s_FilePath = "Telemetry/DataSets";
    private static int s_CurrWriteIteration = 0;
    private static float s_CurrentTraceTime = 0.0f;

    private static Dictionary<string, List<string>> s_DataSets = new Dictionary<string, List<string>>();

    public static void TracePosition(string name, Vector3 position, float traceFrequency = 0.0f, int writeInterval = 0)
    {
        // Add the new data set if it doesn't exist
        if (!s_DataSets.Keys.Contains(name))
            s_DataSets.Add(name, new List<string>());

        // Format and add new line to data set
        if (s_CurrentTraceTime >= traceFrequency)
        {
            s_DataSets[name].Add( $"#{name}_{position.x},{position.y},{position.z}");
            s_CurrentTraceTime = 0.0f;
        }
        else
            s_CurrentTraceTime += Time.deltaTime;

        // Ensure file writes only happen periodically
        if (ReadyToWrite(s_CurrWriteIteration++, writeInterval))
        {
            // Write to file
            WriteToFile(name);
            s_CurrWriteIteration = 0;
        }
    }

    private static void WriteToFile(string fileName)
    {
        // FOrmat file path
        string filepath = $"{s_FilePath}/{fileName}.dat";

        // Check if directory exists
        // Create it if not
        if (!Directory.Exists(s_FilePath))
            Directory.CreateDirectory(s_FilePath);

        // Check if file exists
        // Create it if not
        if (!File.Exists(filepath))
            File.Create(filepath).Dispose();

        // Add entries to file data
        string fileData = "";
        foreach (var data in s_DataSets[fileName])
            fileData += data + "\n";

        // Clear the dataset to prevent data duplication
        s_DataSets[fileName].Clear();

        // Write back to file
        File.AppendAllText(filepath, fileData);
    }

    public static Dictionary<string, List<Vector3>> Retrieve()
    {
        Dictionary<string, List<Vector3>> parsedData = new Dictionary<string, List<Vector3>>();

        string[] fileNames = Directory.GetFiles(s_FilePath, "*.dat");

        foreach (var name in fileNames)
        {
            Debug.Log(name);

            if (new FileInfo(name).Length > 0)
            {
                string[] inputData = File.ReadAllLines(name);

                string dataSetName = "#unnamed#";
                List<Vector3> positions = new List<Vector3>();

                foreach (var line in inputData)
                {
                    string[] split = line.Split('#', '_', ',');

                    dataSetName = split[1];
                    positions.Add(new Vector3(Mathf.RoundToInt(float.Parse(split[2])), Mathf.RoundToInt(float.Parse(split[3])), Mathf.RoundToInt(float.Parse(split[4]))));
                }

                if (parsedData.ContainsKey(dataSetName))
                    parsedData[dataSetName].AddRange(positions);
                else
                    parsedData.Add(dataSetName, positions);
            }
        }

        return parsedData;
    }

    private static bool ReadyToWrite(int index, int quantity)
    {
        return index >= quantity;
    }
}