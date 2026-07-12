public static class GameProgress
{
    public static bool MedusaUnlocked { get; private set; }

    public static void StartNewGame()
    {
        MedusaUnlocked = false;
    }

    public static void UnlockMedusa()
    {
        MedusaUnlocked = true;
    }
}
