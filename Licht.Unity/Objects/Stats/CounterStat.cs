using System;
using Licht.Unity.Objects;
using Licht.Unity.Objects.Stats;
using UnityEngine;

[AddComponentMenu("L. Counters: Int Stat")]
public class CounterStat : BaseGameObject
{
    private int _value;

    public int Value
    {
        get => _value;
        set {
            var old = _value;
            _value = value;
            OnChange?.Invoke(new ScriptIntegerStat.StatUpdate
            {
                OldValue = old,
                NewValue = value
            });
        }
    }

    [field: SerializeField]
    public int InitialValue { get; private set; }

    public event Action<ScriptIntegerStat.StatUpdate> OnChange;

    protected override void OnEnable()
    {
        base.OnEnable();
        _value = InitialValue;
    }
}
