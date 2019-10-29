namespace RPGCore.Behaviour
{
	public sealed class EventFieldMirrorHandler<T> : IEventFieldHandler
	{
		public IEventField<T> SourceField;
		public EventField<T> Target;

		public EventFieldMirrorHandler (IEventField<T> source, EventField<T> target)
		{
			SourceField = source;
			Target = target;
		}

		public void OnBeforeChanged ()
		{
		}

		public void OnAfterChanged ()
		{
			Target.Value = SourceField.Value;
		}

		public void Dispose ()
		{

		}
	}
}