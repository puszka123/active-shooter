using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class Save {
    public static string fileName = "statistics.json";

    public static void SaveJsons(string[] jsons)
    {
        File.WriteAllLines(fileName, jsons);
        //using (System.IO.StreamWriter file =
        //    new System.IO.StreamWriter(fileName, true))
        //{
        //    foreach (var item in jsons)
        //    {
        //        file.WriteLine(item);
        //    }
        //}
    }
}
