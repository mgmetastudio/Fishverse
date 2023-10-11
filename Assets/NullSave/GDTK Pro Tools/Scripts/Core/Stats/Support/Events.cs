#if GDTK
using System;
using System.Collections.Generic;

namespace NullSave.GDTK.Stats
{

    [Serializable]
    public delegate void AttributeEvent(GDTKAttribute attribute);

    [Serializable]
    public delegate void EventTrigger(GDTKEvent statEvent);

    [Serializable]
    public delegate void HeartbeatEvent(float time);

    [Serializable]
    public delegate void StatModifierEvent(GDTKStatModifier modifier);

    [Serializable]
    public delegate void StatusConditionEvent(GDTKStatusCondition statusCondition);

    [Serializable]
    public delegate void StatusEffectEvent(GDTKStatusEffect effect);

    [Serializable]
    public delegate void ClassEvent(GDTKClass playerClass);

    [Serializable]
    public delegate void PerkEvent(GDTKPerk perk);

    [Serializable]
    public delegate void TraitEvent(GDTKTrait trait);

    [Serializable]
    public delegate void StatFunction(string request, Dictionary<string, StatSource> sources, out string result);

    [Serializable]
    public delegate void StatFunctionSubscription(Dictionary<string, StatSource> sources, string request, SimpleEvent requester, List<SimpleEvent> subscriptionList);

    public delegate void StatSourceAction(object[] args, ref object result);

}
#endif