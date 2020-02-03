﻿using Newtonsoft.Json;
using RPGCore.Behaviour;
using RPGCore.Behaviour.Manifest;
using RPGCore.Demo.Nodes;
using RPGCore.Packages;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace RPGCore.Demo
{
	public sealed class Simulation
	{
		public void Start()
		{
			var nodes = NodeManifest.Construct(
				new Type[] {
					typeof (AddNode),
					typeof (RollNode),
					typeof (OutputValueNode),
					typeof (ItemInputNode),
					typeof (ActivatableItemNode),
					typeof (IterateNode),
					typeof (GetStatNode),
				}
			);
			var types = TypeManifest.Construct(
				new Type[]
				{
					typeof(bool),
					typeof(string),
					typeof(int),
					typeof(byte),
					typeof(long),
					typeof(short),
					typeof(uint),
					typeof(ulong),
					typeof(ushort),
					typeof(sbyte),
					typeof(char),
					typeof(float),
					typeof(double),
					typeof(decimal),
					typeof(InputSocket),
					typeof(LocalId),
				},
				new Type[]
				{
					typeof(SerializedGraph),
					typeof(SerializedNode),
					typeof(PackageNodeEditor),
					typeof(PackageNodePosition),
					typeof(ExtraData)
				}
			);

			var manifest = new BehaviourManifest()
			{
				Nodes = nodes,
				Types = types
			};
			var serializer = new JsonSerializer();

			Console.WriteLine("Importing Graph...");

			var proj = ProjectExplorer.Load("Content/Core");

			var consoleRenderer = new BuildConsoleRenderer();

			var buildPipeline = new BuildPipeline()
			{
				Exporters = new List<ResourceExporter>()
				{
					new BhvrExporter()
				},
				BuildActions = new List<IBuildAction>()
				{
					consoleRenderer
				}
			};
			consoleRenderer.DrawProgressBar(32);
			proj.Export(buildPipeline, "Content/Temp");

			Console.WriteLine("Exported package...");
			var exportedPackage = PackageExplorer.Load("Content/Temp/Core.bpkg");

			var fireballAsset = exportedPackage.Resources["Fireball/Main.bhvr"];
			var data = fireballAsset.LoadStream();

			SerializedGraph packageItem;
			using (var sr = new StreamReader(data))
			using (var reader = new JsonTextReader(sr))
			{
				packageItem = serializer.Deserialize<SerializedGraph>(reader);
			}

			Console.WriteLine("Imported: " + packageItem.Name);
			var unpackedGraph = packageItem.Unpack();

			Console.WriteLine("Running Simulation...");

			var player = new DemoPlayer();

			IGraphInstance instancedItem = unpackedGraph.Create();
			instancedItem.Setup();
			instancedItem.SetInput(player);
			for (int i = 0; i < 5; i++)
			{
				Thread.Sleep(100);
				player.Health.Value -= 10;
			}
			instancedItem.Remove();

			var settings = new JsonSerializerSettings();
			settings.Converters.Add(new LocalIdJsonConverter());
			settings.Converters.Add(new SerializedGraphInstanceProxyConverter(null));

			string serializedGraph = JsonConvert.SerializeObject(instancedItem, settings);
			Console.WriteLine(serializedGraph);

			var deserialized = JsonConvert.DeserializeObject<SerializedGraphInstance>(serializedGraph);
			var unpackedInstance = deserialized.Unpack(unpackedGraph);

			unpackedInstance.SetInput(player);
			unpackedInstance.Setup();
			for (int i = 0; i < 5; i++)
			{
				Thread.Sleep(100);
				player.Health.Value -= 20;
			}
			unpackedInstance.Remove();

			for (int i = 0; i < 5; i++)
			{
				Thread.Sleep(100);
				player.Health.Value -= 20;
			}
		}
	}
}
