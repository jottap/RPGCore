﻿using RPGCore.Behaviour;
using RPGCore.Events;
using RPGCore.Traits;

namespace RPGCore.View
{
	public class ViewCharacter : Entity
	{
		public TraitCollection? Traits;

		public EventField<string> Name = new EventField<string>();
		public EventField<EntityRef> SelectedTarget = new EventField<EntityRef>();

		public ViewCharacter()
		{
			Id = new EntityRef()
			{
				EntityId = LocalId.NewId()
			};
		}

		/// <inheritdoc/>
		public override string ToString()
		{
			return $"{Name.Value}({Id}, {string.Join(", ", Traits?.States)}";
		}
	}
}
