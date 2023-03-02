using System;

namespace Licht.Unity.Links
{
    public struct StatLink<TInput, TOutput>
    {
        public Action Unlink;
        public StatLinkParams<TInput,TOutput> Params;
    }

    public struct StatLinkParams<TInput, TOutput>
    {
        public Func<TInput, TOutput> Update;
    }

    public static class StatLinkDefaults
    {
        public static StatLinkParams<float, float> Proportional(float initialValue, float proportion)
        {
            return new StatLinkParams<float, float>
            {
                Update = input => initialValue + proportion * input
            };
        }
    }
}