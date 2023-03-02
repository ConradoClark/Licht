using Licht.Unity.CharacterControllers;
using Licht.Unity.Objects.Stats;

namespace Licht.Unity.Links
{
    public static class MoveControllerLinkExtensions
    {
        public static StatLink<float, float> LinkSpeed(this LichtPlatformerMoveController moveController, 
            ScriptFloatStat floatStat, StatLinkParams<float, float> linkParams)
        {
            void Action(ScriptStat<float>.StatUpdate update) =>
                moveController.MaxSpeed = linkParams.Update(update.NewValue);

            floatStat.OnChange += Action;

            return new StatLink<float, float>
            {
                Unlink = () => floatStat.OnChange -= Action,
                Params = linkParams
            };
        }

        public static StatLink<float, float> LinkAccelerationTime(this LichtPlatformerMoveController moveController,
            ScriptFloatStat floatStat, StatLinkParams<float, float> linkParams)
        {
            void Action(ScriptStat<float>.StatUpdate update) =>
                moveController.AccelerationTime = linkParams.Update(update.NewValue);

            floatStat.OnChange += Action;

            return new StatLink<float, float>
            {
                Unlink = () => floatStat.OnChange -= Action,
                Params = linkParams
            };
        }

        public static StatLink<float, float> LinkDecelerationTime(this LichtPlatformerMoveController moveController,
            ScriptFloatStat floatStat, StatLinkParams<float, float> linkParams)
        {
            void Action(ScriptStat<float>.StatUpdate update) =>
                moveController.DecelerationTime = linkParams.Update(update.NewValue);

            floatStat.OnChange += Action;

            return new StatLink<float, float>
            {
                Unlink = () => floatStat.OnChange -= Action,
                Params = linkParams
            };
        }

        public static StatLink<int, float> LinkSpeed(this LichtPlatformerMoveController moveController,
            ScriptIntegerStat integerStat, StatLinkParams<int, float> linkParams)
        {
            void Action(ScriptStat<int>.StatUpdate update) =>
                moveController.MaxSpeed = linkParams.Update(update.NewValue);

            integerStat.OnChange += Action;

            return new StatLink<int, float>
            {
                Unlink = () => integerStat.OnChange -= Action,
                Params = linkParams
            };
        }

        public static StatLink<int, float> LinkAccelerationTime(this LichtPlatformerMoveController moveController,
            ScriptIntegerStat integerStat, StatLinkParams<int, float> linkParams)
        {
            void Action(ScriptStat<int>.StatUpdate update) =>
                moveController.AccelerationTime = linkParams.Update(update.NewValue);

            integerStat.OnChange += Action;

            return new StatLink<int, float>
            {
                Unlink = () => integerStat.OnChange -= Action,
                Params = linkParams
            };
        }

        public static StatLink<int, float> LinkDecelerationTime(this LichtPlatformerMoveController moveController,
            ScriptIntegerStat integerStat, StatLinkParams<int, float> linkParams)
        {
            void Action(ScriptStat<int>.StatUpdate update) =>
                moveController.DecelerationTime = linkParams.Update(update.NewValue);

            integerStat.OnChange += Action;

            return new StatLink<int, float>
            {
                Unlink = () => integerStat.OnChange -= Action,
                Params = linkParams
            };
        }
    }
}
