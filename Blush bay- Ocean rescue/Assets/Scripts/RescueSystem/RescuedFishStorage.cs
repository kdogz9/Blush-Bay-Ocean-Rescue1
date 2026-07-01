using UnityEngine;

public static class RescuedFishStorage
{
    // This tells the aquarium scene if a fish has just been rescued
    public static bool HasRescuedFish;

    // Fish details
    public static string FishName;
    public static Sprite FishSprite;
    public static int StartingHealth;
    public static int MaxHealth;
    public static string IllnessName;

    public static void SaveFish(string fishName, Sprite fishSprite, int startingHealth, int maxHealth, string illnessName)
    {
        // Store the rescued fish data
        HasRescuedFish = true;

        FishName = fishName;
        FishSprite = fishSprite;
        StartingHealth = startingHealth;
        MaxHealth = maxHealth;
        IllnessName = illnessName;
    }

    public static void Clear()
    {
        // Clear the saved fish after it has been added to the tank
        HasRescuedFish = false;

        FishName = "";
        FishSprite = null;
        StartingHealth = 0;
        MaxHealth = 0;
        IllnessName = "";
    }
}