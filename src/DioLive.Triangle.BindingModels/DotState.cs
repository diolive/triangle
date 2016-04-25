namespace DioLive.Triangle.BindingModels
{
    public enum DotState
    {
        // No beams
        Free = 0,

        // One beam
        Fired = 1,

        // Two beams
        Stunned = 2,

        // Destroyed or disconnected
        Destroyed = 3,
    }
}