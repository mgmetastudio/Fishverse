using System;

namespace NullSave.GDTK.Stats
{

    public enum AddOnMode
    {
        PickAny = 0,
        AllInList = 1,
        PickFromList = 2,
        PickFromGroup = 3,
        RandomFromList = 4,
        RandomFromGroup = 5
    }

    public enum ConditionEndMode
    {
        ConditionTrue = 0,
        EventTriggered = 1,
        TimeElapsed = 2,
        TurnsElapsed = 3
    }

    public enum ConditionStartMode
    {
        ConditionTrue = 0,
        EventTriggered = 1
    }

    public enum DetectionType
    {
        ColliderHit = 0,
        SphereCast = 1,
        CubeCast = 2,
        SphereConeCast = 3,
        RayCast = 4
    }

    public enum EffectExpiry
    {
        Automatically = 0,
        AfterSeconds = 1,
        AfterTokens = 2
    }

    [Flags]
    public enum InteractableActions
    {
        ActivateObject = 1,
        AddClass = 2,
        AddStatEffect = 4,
        AddStatModifiers = 8,
        AddStatusCondition = 16,
        CustomReward = 65536,
        LoadData = 32,
        RaiseEvent = 32768,
        RemoveBackground = 64,
        RemoveClass = 128,
        RemoveStatusCondition = 256,
        RemoveStatEffect = 512,
        RemoveRace = 1024,
        SaveData = 2048,
        SetBackground = 4096,
        SetRace = 8192,
        SpawnObject = 16384,
    }

    public enum ModifierApplication
    {
        Immediately = 0,
        RecurringOverSeconds = 1,
        RecurringOverTurns = 2,
        UntilRemoved = 3,
        SetValueOnce = 4,
        SetValueUntilRemoved = 5
    }

    public enum ModifierChangeType
    {
        Add = 0,
        AddMultiplier = 1,
        Subtract = 2,
        SubtractMultiplier = 3,
    }

    public enum ModifierTarget
    {
        Value = 0,
        Maximum = 1,
        Minimum = 2,
        RegenerationDelay = 3,
        RegenerationRate = 4,
        Special = 5
    }

    public enum StatBinding
    {
        Value = 1,
        Maximum = 2,
        Minimum = 4,
        RegenerationDelay = 8,
        RegenerationRate = 16,
        Special = 32,
        Everything = 63
    }

    public enum StatSourceReference
    {
        DirectReference = 0,
        FindInRegistry = 1,
    }

    public enum Unlocking
    {
        Immediately = 0,
        AtCharacterLevel = 1,
        AtClassLevel = 2,
        ByExpression = 3
    }

    public enum ValueType
    {
        Standard = 0,
        RandomRange = 1,
        Conditional = 2
    }

}