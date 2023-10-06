#if GDTK
using System;
using System.Collections.Generic;
using UnityEngine;

namespace NullSave.GDTK.Stats
{
    [AutoDoc("This UI component displays information for a specific Status Effect.")]
    public class StatusEffectUI : statsInfoUI
    {

        #region Fields

        [Tooltip("Display modifiers added by status effect")] public bool showModifiers;
        [Tooltip("Prefab to create for displaying modifier")] public StatModifierUI modifierPrefab;
        [Tooltip("Label used to display remaining life")] public Label remainingLife;
        [Tooltip("Format to use when displaying remaining life")] public string remainingLifeFormat;
        [Tooltip("Display attributes added by status effect")] public bool showAttributes;
        [Tooltip("Prefab to create for displaying attribute")] public BasicInfoUI attributePrefab;
        [Tooltip("Progressbar used to display remaining life")] public Progressbar lifeProgressbar;
        [Tooltip("Apply info color to preogressbar")] public bool useColors;
        [Tooltip("Format to use when displaying Tooltip UI"), TextArea(2, 5)] public string tooltipFormat;

        private GDTKStatusEffect target;
        private Dictionary<string, StatModifierUI> modifierList;
        private Dictionary<string, BasicInfoUI> attributeList;
        private StatsDatabase db;
        private TooltipClient tooltip;

        #endregion

        #region Unity Methods

        private void Awake()
        {
            db = ToolRegistry.GetComponent<StatsDatabase>();
            tooltip = GetComponent<TooltipClient>();
        }

        private void OnDestroy()
        {
            if (target != null)
            {
                target.onLifeChanged -= UpdateUI;
                target.onRemoved -= RemoveEffect;
                target.onModifiersChanged -= UpdateUI;
            }

            if (modifierList != null)
            {
                foreach (var entry in modifierList)
                {
                    if (entry.Value.gameObject != null)
                    {
                        Destroy(entry.Value.gameObject);
                    }
                }
            }

            if (attributeList != null)
            {
                foreach (var entry in attributeList)
                {
                    if (entry.Value.gameObject != null)
                    {
                        Destroy(entry.Value.gameObject);
                    }
                }
            }
        }

        private void Reset()
        {
            remainingLifeFormat = "{0} seconds";
            tooltipFormat = "<b>{title}</b>\r\n{description}";
        }

        #endregion

        #region Public Methods

        [AutoDoc("Loads a Status Effect for display", "using NullSave.GDTK;<br/>using NullSave.GDTK.Stats;<br/>using UnityEngine;<br/><br/>public class Example : MonoBehaviour<br/>{<br/><br/>    StatusEffectUI source;<br/><br/>    public void ExampleMethod()<br/>    {<br/>        StatsDatabase database = ToolRegistry.GetComponent<StatsDatabase>();<br/>        GDTKStatusEffect effect = database.effects[0];<br/>        source.Load(effect);<br/>    }<br/><br/>}")]
        [AutoDocParameter("Status Effect to load")]
        public void Load(GDTKStatusEffect effect)
        {
            target = effect;

            target.onLifeChanged += UpdateUI;
            target.onRemoved += RemoveEffect;
            target.onModifiersChanged += UpdateUI;

            if(tooltip != null)
            {
                tooltip.tooltip = tooltipFormat
                    .Replace("{id}", effect.info.id)
                    .Replace("{title}", effect.info.title)
                    .Replace("{abbr}", effect.info.abbr)
                    .Replace("{description}", effect.info.description)
                    .Replace("{group}", effect.info.groupName)
                    .Replace("{life}", effect.lifeRemaining.ToString())
                    ;
            }

            UpdateUI();
        }

        #endregion

        #region Private Methods

        private Tuple<float, float> GetDynamicExpiration()
        {
            float maxLife = 0;
            float maxRemain = 0;

            foreach(GDTKStatModifier mod in target.livingModifiers)
            {
               switch(mod.applies)
                {
                    case ModifierApplication.RecurringOverSeconds:
                    case ModifierApplication.RecurringOverTurns:
                        maxLife = Mathf.Max(maxLife, mod.lifespan);
                        maxRemain = Mathf.Max(maxRemain, (float)mod.lifeRemaining);
                        break;
                }
            }

            return new Tuple<float, float>(maxLife, maxRemain);
        }

        private void RemoveEffect()
        {
            if(target != null)
            {
                target.onLifeChanged -= UpdateUI;
                target.onRemoved -= RemoveEffect;
                target.onModifiersChanged -= UpdateUI;
            }
            Destroy(gameObject);
        }

        private void UpdateUI()
        {
            info = target.info;

            ApplyImage();

            foreach (TemplatedLabel label in labels)
            {
                label.target.text = FormatInfo(label.format);
            }

            if (showModifiers)
            {
                foreach (GDTKStatModifier modifier in target.livingModifiers)
                {
                    if (modifierList == null) modifierList = new Dictionary<string, StatModifierUI>();

                    if (!modifierList.ContainsKey(modifier.instanceId))
                    {
                        StatModifierUI ui = Instantiate(modifierPrefab, transform.parent);
                        ui.LoadStatusEffect(modifier);
                        modifierList.Add(modifier.instanceId, ui);
                    }
                }
            }

            if (showAttributes && db != null)
            {
                foreach (string id in target.attributeIds)
                {
                    if (attributeList == null) attributeList = new Dictionary<string, BasicInfoUI>();

                    if (!attributeList.ContainsKey(target.info.id + id))
                    {
                        BasicInfoUI ui = Instantiate(attributePrefab, transform.parent);
                        ui.Load(db.GetAttribute(id).info);
                        attributeList.Add(target.info.id + id, ui);
                    }
                }
            }

            if (remainingLife)
            {
                if (target.expires == EffectExpiry.Automatically)
                {
                    Tuple<float, float> dynamicLife = GetDynamicExpiration();
                    if (dynamicLife.Item1 == 0)
                    {
                        remainingLife.gameObject.SetActive(false);
                    }
                    else
                    {
                        remainingLife.text = remainingLifeFormat.Replace("{0}", Mathf.Round(dynamicLife.Item2).ToString());
                    }
                }
                else
                {
                    remainingLife.text = remainingLifeFormat.Replace("{0}", Mathf.Round(target.lifeRemaining).ToString());
                }
            }

            if(lifeProgressbar)
            {
                if (target.expires == EffectExpiry.Automatically)
                {
                    Tuple<float, float> dynamicLife = GetDynamicExpiration();
                    if (dynamicLife.Item1 == 0)
                    {
                        lifeProgressbar.gameObject.SetActive(false);
                    }
                    else
                    {
                        lifeProgressbar.minValue = 0;
                        lifeProgressbar.maxValue = dynamicLife.Item1;
                        lifeProgressbar.value = dynamicLife.Item2;
                    }
                }
                else
                {
                    lifeProgressbar.minValue = 0;
                    lifeProgressbar.maxValue = target.expiryTime;
                    lifeProgressbar.value = target.lifeRemaining;
                }
                if (useColors)
                {
                    lifeProgressbar.targetGraphic.color = target.info.color;
                }
            }

        }

        #endregion

    }
}
#endif