public static class Utility
{
    public static T[] ShuffleArray<T>(T[] array, int seed)
    {
        System.Random prng = new System.Random(seed);

        for (int i = 0; i < array.Length - 1; i++)
        {
            int randomIndex = prng.Next(i, array.Length);

            (array[randomIndex], array[i]) = (array[i], array[randomIndex]);
        }

        return array;
    }
}