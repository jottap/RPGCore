﻿using RPGCore.Behaviour;
using RPGCore.Behaviour.Connections;
using RPGCore.Stats;
using System;

namespace RPGCore
{
	[NodeInformation ("Character/Grant Stat", "Attribute")]
	public class StatsNode : BehaviourNode
	{
		[CollectionType (typeof (StatCollection<>))]
		public CollectionEntry entry;

		public CharacterInput Target;
		public BoolInput Active;
		public FloatInput Effect;

		public AttributeInformation.ModifierType Scaling;
		public string Display = "{0}";

		public string Description (IBehaviourContext context)
		{
			ConnectionEntry<RPGCharacter> targetInput = Target[context];
			ConnectionEntry<float> effectInput = Effect[context];
			
			UnityEngine.Debug.Log (context.GetHashCode() + ": " + targetInput.Value);
			StatInstance inst = targetInput.Value.Stats[entry];

			return Display.Replace ("{0}", inst.Info.RenderModifier (effectInput.Value, Scaling));
		}

		protected override void OnSetup (IBehaviourContext context)
		{
			ConnectionEntry<RPGCharacter> targetInput = Target[context];
			ConnectionEntry<bool> activeInput = Active[context];
			ConnectionEntry<float> effectInput = Effect[context];

			UnityEngine.Debug.Log ("<b>Stats</b>" + context + ": " + targetInput.Value);

			StatInstance.Modifier modifier = null;
			bool isActive = false;

			Action changeHandler = () =>
			{
				if (targetInput.Value == null)
					return;

				if (activeInput.Value)
				{
					if (!isActive)
					{
						if (Scaling == AttributeInformation.ModifierType.Additive)
							modifier = targetInput.Value.Stats[entry].AddFlatModifier (effectInput.Value);
						else
							modifier = targetInput.Value.Stats[entry].AddMultiplierModifier (effectInput.Value);

						isActive = true;
					}
				}
				else if (isActive)
				{
					if (Scaling == AttributeInformation.ModifierType.Additive)
						targetInput.Value.Stats[entry].RemoveFlatModifier (modifier);
					else
						targetInput.Value.Stats[entry].RemoveMultiplierModifier (modifier);

					isActive = false;
				};
			};

			targetInput.OnBeforeChanged += () =>
			{

				if (targetInput.Value == null)
				{
					isActive = false;
					return;
				}

				if (!isActive)
					return;

				if (Scaling == AttributeInformation.ModifierType.Additive)
					targetInput.Value.Stats[entry].RemoveFlatModifier (modifier);
				else
					targetInput.Value.Stats[entry].RemoveMultiplierModifier (modifier);

				isActive = false;
			};

			targetInput.OnAfterChanged += changeHandler;
			activeInput.OnAfterChanged += changeHandler;

			changeHandler ();

			effectInput.OnAfterChanged += () =>
			{
				if (modifier == null)
					return;

				modifier.Value = effectInput.Value;
			};
		}

		protected override void OnRemove (IBehaviourContext context)
		{

		}
	}
}

