using System;
using System.Collections.Generic;

public static class BirdDictionary
{
    public static Dictionary<string, List<string>> BirdDict = new Dictionary<string, List<string>>();

    // Static constructor to initialize BirdDict
    static BirdDictionary()
    {
        BirdDict["Bird"] = new List<string>() { "Bird1", "Bird2" };
    }
}
