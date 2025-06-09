using UnityEngine;

public static class RandomDist
{
    /// <summary>
    /// Generates a random number from a uniform distribution between min and max.
    /// </summary>
    public static float Uniform(float min, float max)
    {
        return min + (max - min) * Random.value;
    }

    /// <summary>
    /// Generates a random number from an exponential distribution with the given lambda.
    /// </summary>
    public static float Exponential(float mu)
    {
        return - mu * Mathf.Log(Random.value);
    }
}