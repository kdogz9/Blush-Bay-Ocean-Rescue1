using UnityEngine;

public static class RescuedFishStorage
{
    public static bool HasRescuedFish;

    public static string FishName;
    public static Sprite FishSprite;
    public static int StartingHealth;
    public static int MaxHealth;
    public static string IllnessName;

    public static void SaveFish(string fishName, Sprite fishSprite, int startingHealth, int maxHealth, string illnessName)
    {
        HasRescuedFish = true;

        FishName = fishName;
        FishSprite = fishSprite;
        StartingHealth = startingHealth;
        MaxHealth = maxHealth;
        IllnessName = illnessName;
    }

    public static void Clear()
    {
        HasRescuedFish = false;

        FishName = "";
        FishSprite = null;
        StartingHealth = 0;
        MaxHealth = 0;
        IllnessName = "";
    }
}