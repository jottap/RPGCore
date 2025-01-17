using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace RPGCore.DataEditor.Manifest
{
	/// <summary>
	/// A manifest used to configure data editing.
	/// </summary>
	public sealed class ProjectManifest
	{
		internal readonly IReadOnlyDictionary<string, SchemaType> objectTypes;
		internal readonly IReadOnlyDictionary<string, SchemaNode> nodeTypes;
		internal readonly IReadOnlyDictionary<string, SchemaTypeConversion> typeConversions;

		internal ProjectManifest(
			IEnumerable<SchemaType> objectTypes,
			IEnumerable<SchemaNode> nodeTypes,
			IEnumerable<SchemaTypeConversion> typeConversions)
		{
			this.objectTypes = objectTypes.ToDictionary(t => t.Name, t => t);
			this.nodeTypes = nodeTypes.ToDictionary(t => t.Name, t => t);
			this.typeConversions = typeConversions.ToDictionary(t => t.FromType, t => t);
		}

		/// <summary>
		/// Writes the current state of the <see cref="ProjectManifestBuilder"/> to a destination <see cref="Stream"/>.
		/// </summary>
		/// <param name="stream">The destination <see cref="Stream"/> to write to.</param>
		/// <returns>Returns the current instance of the <see cref="ProjectManifestBuilder"/>.</returns>
		public void WriteTo(Stream stream)
		{
			using var jsonWriter = new Utf8JsonWriter(stream, new JsonWriterOptions()
			{
				Indented = true
			});

			JsonSerializer.Serialize(jsonWriter, this, new JsonSerializerOptions()
			{
			});
		}

		public SchemaType? GetTypeInformation(string type)
		{
			if (objectTypes != null)
			{
				if (objectTypes.TryGetValue(type, out var objectType))
				{
					return objectType;
				}
			}
			if (nodeTypes != null)
			{
				if (nodeTypes.TryGetValue(type, out var nodeType))
				{
					return nodeType;
				}
			}
			return null;
		}
	}
}
