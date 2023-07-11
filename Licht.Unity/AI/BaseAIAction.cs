using System;
using System.Collections.Generic;
using Licht.Unity.Objects;

public abstract class BaseAIAction : BaseGameObject
{
    public abstract IEnumerable<IEnumerable<Action>> Execute(Func<bool> breakCondition);
    public abstract void OnInterrupt();

    public virtual void Prepare()
    {

    }
}