using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class Save {
    //public static string fileName = "statistics.json";

    public static void SaveJsons(string[] jsons, string file)
    {
        File.WriteAllLines(file, jsons);
    }
}
