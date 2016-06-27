// Copyright (c) 2015, Lukas Honke
// ========================
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using Assets.scripts.Actor;
using Assets.scripts.Actor.MonsterClasses.Base;
using Assets.scripts.Base;
using Assets.scripts.Mono.MapGenerator.Levels;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets.scripts.Mono.MapGenerator
{
	public class MonsterGenerator
	{
		private static MonsterGenerator instance = null;
		public static MonsterGenerator Instance
		{
			get
			{
				if (instance == null)
					instance = new MonsterGenerator();

				return instance;
			}
		}

		public List<MobGroup> mobGroups = new List<MobGroup>(); 

		public MonsterGenerator()
		{
			try
			{
				LoadXmlFile();
			}
			catch (Exception e)
			{
				Debug.LogError("chyba nacitani xml spawndata - check spawndata_errors.txt");
				System.IO.StreamWriter file = new System.IO.StreamWriter("SpawnData_errors.txt");
				file.WriteLine(e.Message);
				file.Close();
			}
		}

		public enum RoomType
		{
			START_ROOM,
			END_ROOM,
			MAIN_ROOM,
			SIDE_ROOM,
			BONUS_ROOM,
			BOSS_ROOM,

			EXTRA_ROOM // spawned manually
		}

		public enum RoomSize
		{
			LARGE,
			MEDIUM,
			SMALL
		}

		public void LoadXmlFile()
		{
			mobGroups.Clear();
			XmlDocument doc = new XmlDocument();
			doc.Load("SpawnData.xml");

			int nextId = 0;

			foreach (XmlNode roomNode in doc.DocumentElement.ChildNodes)
			{
				if (roomNode.NodeType == XmlNodeType.Comment || roomNode.Name.Equals("xml")) continue;

				RoomType currentRoomType = RoomType.MAIN_ROOM;

				switch (roomNode.Name)
				{
					case "main_room":
						currentRoomType = RoomType.MAIN_ROOM;
						break;
					case "side_room":
						currentRoomType = RoomType.SIDE_ROOM;
						break;
					case "bonus_room":
						currentRoomType = RoomType.BONUS_ROOM;
						break;
					case "boss_room":
						currentRoomType = RoomType.BOSS_ROOM;
						break;
					case "end_room":
						currentRoomType = RoomType.END_ROOM;
						break;
					case "start_room":
						currentRoomType = RoomType.START_ROOM;
						break;
					case "extra_room":
						currentRoomType = RoomType.EXTRA_ROOM;
						break;
				}

				foreach (XmlNode groupNode in roomNode.ChildNodes)
				{
					if (groupNode.NodeType == XmlNodeType.Comment) continue;

					int id = ++nextId;
					int minLevel = 2;
					int maxLevel = 99;
					int minWorld = 1;
					int maxWorld = 99;
					int frequency = 5;
					RegionSize minRegionSize = RegionSize.NULL;
					RegionSize maxRegionSize = RegionSize.NULL;

					foreach (XmlAttribute attr in groupNode.Attributes)
					{
						if (attr.Name == "id")
						{
							id = Int32.Parse(attr.Value);
							nextId = id;
						}

						if (attr.Name == "min_level")
						{
							minLevel = Int32.Parse(attr.Value);
						}

						if (attr.Name == "max_level")
						{
							maxLevel = Int32.Parse(attr.Value);
						}

						if (attr.Name == "min_world")
						{
							minWorld = Int32.Parse(attr.Value);
						}

						if (attr.Name == "max_world")
						{
							maxWorld = Int32.Parse(attr.Value);
						}

						if (attr.Name == "frequency")
						{
							frequency = Int32.Parse(attr.Value);
						}

						if (attr.Name == "max_region_size")
						{
							switch (attr.Value)
							{
								case "small":
									maxRegionSize = RegionSize.SMALL;
									break;
								case "medium":
									maxRegionSize = RegionSize.MEDIUM;
									break;
								case "large":
									maxRegionSize = RegionSize.LARGE;
									break;
							}
						}

						if (attr.Name == "min_region_size")
						{
							switch (attr.Value)
							{
								case "small":
									minRegionSize = RegionSize.SMALL;
									break;
								case "medium":
									minRegionSize = RegionSize.MEDIUM;
									break;
								case "large":
									minRegionSize = RegionSize.LARGE;
									break;
							}
						}
					}

					MobGroup group = new MobGroup(currentRoomType, id, minLevel, maxLevel, minWorld, maxWorld, frequency, minRegionSize, maxRegionSize);

					//Debug.Log("added group id " + id + " to " + currentRoomType);

					int mobIds = 0;
					foreach (XmlNode mobNode in groupNode.ChildNodes)
					{
						if (groupNode.NodeType == XmlNodeType.Comment) continue;

						if (mobNode.Name == "mob")
						{
							int mobId = ++mobIds;
							int idParent = -1;
							string type = null;
							string location = null;

							int count = 1;
							int level = 1;
							int chance = 100;
							string roomSize = "small";
							bool exclude = true;
							RegionSize minSize = RegionSize.NULL;

							foreach (XmlAttribute attr in mobNode.Attributes)
							{
								if (attr.Name == "type")
								{
									type = attr.Value;
								}

								if (attr.Name == "location")
								{
									location = attr.Value;
								}

								if (attr.Name == "id")
								{
									mobId = Int32.Parse(attr.Value);
									mobIds = mobId;
								}

								if (attr.Name == "id_parent")
								{
									idParent = Int32.Parse(attr.Value);
								}

								if (attr.Name == "count")
								{
									count = Int32.Parse(attr.Value);
								}

								if (attr.Name == "level")
								{
									level = Int32.Parse(attr.Value);
								}

								if (attr.Name == "chance")
								{
									chance = Int32.Parse(attr.Value);
								}

								if (attr.Name == "room_size")
								{
									roomSize = attr.Value;
								}

								if (attr.Name == "exclude")
								{
									exclude = attr.Value == "true";
								}

								if (attr.Name == "min_region_size")
								{
									switch (attr.Value)
									{
										case "small":
											minSize = RegionSize.SMALL;
											break;
										case "medium":
											minSize = RegionSize.MEDIUM;
											break;
										case "large":
											minSize = RegionSize.LARGE;
											break;
									}
								}
							}

							if (type == null || location == null)
							{
								throw new Exception("type a location musi byt nastaveno");
							}

							try
							{
								MobData mob = new MobData(mobId, idParent, type, count, level, chance, location, roomSize, exclude, minSize);
								group.AddMob(mob);
							}
							catch (Exception e)
							{
								Debug.LogError("chyba vnacitani - check spawndata_errors.txt");
								System.IO.StreamWriter file = new System.IO.StreamWriter("SpawnData_errors.txt");
								file.WriteLine(e.Message);
								file.Close();
							}
						}
						else if (mobNode.Name == "extra_group")
						{
							int groupId = -1;
							int chance = 100;

							int stackId = -1;

							foreach (XmlAttribute attr in mobNode.Attributes)
							{
								if (attr.Name == "id")
								{
									groupId = Int32.Parse(attr.Value);
								}

								if (attr.Name == "chance")
								{
									chance = Int32.Parse(attr.Value);
								}

								if (attr.Name == "stack_id")
								{
									stackId = Int32.Parse(attr.Value);
								}
							}

							if (id == -1)
								throw new Exception("extra grup - id musi byt nastaveno (" + id + " = group Id)");

							try
							{
								group.AddExtraGroup(groupId, chance, stackId);
							}
							catch (Exception e)
							{
								Debug.LogError("chyba v nacitani extragroup - check spawndata_errors.txt");
								System.IO.StreamWriter file = new System.IO.StreamWriter("SpawnData_errors.txt");
								file.WriteLine(e.Message);
								file.Close();
							}
						}
					}

					mobGroups.Add(group);
				}
			}

			Debug.Log("Nacteno " + mobGroups.Count + " mobgroupu");
		}

		public enum RegionSize
		{
			NULL = 0,
			SMALL = 1, // 1x1
			MEDIUM = 2, // 1x2
			LARGE = 3 // 2x2
		}

		public void GenerateGenericEnemyGroup(MapRoom room, AbstractLevelData level, RoomType roomType, int difficulty, MapRegion regionData, int forcedId = -1)
		{
			Player player = GameSystem.Instance.CurrentPlayer;
			int playerLevel = 1;
			if (player != null)
			{
				playerLevel = player.Level;
			}

			RegionSize size = RegionSize.SMALL;
			if (regionData != null)
			{
				int sInt = regionData.GetFamilyRegions().Count;
				if (sInt == 2 || sInt == 3)
					size = RegionSize.MEDIUM;
				else if (sInt >= 4)
					size = RegionSize.LARGE;
			}

			int world = WorldHolder.instance.worldLevel;

			int selectedId = forcedId;

			if (forcedId < 0)
			{
				int soucet = 0;
				foreach (MobGroup group in mobGroups)
				{
					if (group.minRegionSize != RegionSize.NULL && group.minRegionSize > size)
						continue;

					if (group.maxRegionSize != RegionSize.NULL && group.maxRegionSize < size)
						continue;

					if (roomType == group.roomType && group.minLevel <= playerLevel && group.maxLevel >= playerLevel &&
					    group.maxWorld >= world && group.minWorld <= world)
					{
						soucet += group.frequency;
					}
				}

				int[] sum = new int[soucet];
				soucet = 0;

				foreach (MobGroup group in mobGroups)
				{
					if (group.minRegionSize != RegionSize.NULL && group.minRegionSize > size)
						continue;

					if (group.maxRegionSize != RegionSize.NULL && group.maxRegionSize < size)
						continue;

					if (roomType == group.roomType && group.minLevel <= playerLevel && group.maxLevel >= playerLevel &&
						 group.maxWorld >= world && group.minWorld <= world)
					{
						for (int i = 0; i < group.frequency; i++)
						{
							sum[soucet + i] = group.id;
						}

						soucet += group.frequency;
					}
				}

				if (sum.Length == 0)
					return;

				int rnd = Random.Range(0, sum.Length);
				selectedId = sum[rnd];
			}

			if (selectedId < 0)
			{
				Debug.LogError("chyba - nenalezena spravna groupa ");
				return;
			}

			bool spawned = false;

			foreach (MobGroup group in mobGroups)
			{
				if(group.id == selectedId)
				//if ((roomType == group.roomType && group.minLevel <= playerLevel && group.maxLevel >= playerLevel && group.maxWorld >= world && group.minWorld <= world && forcedId < 0) || (forcedId > 0 && group.id == forcedId))
				{
					List<MonsterSpawnInfo> infos = new List<MonsterSpawnInfo>();

					int count = 0;

					foreach (MobData mob in group.mobs)
					{
						Tile tile;

						if (mob.minSize != RegionSize.NULL)
						{
							if (size < mob.minSize)
								continue;
						}

						if (mob.location == MapRoom.DIRECTION_LARGEST_ROOM)
							tile = room.GetLargestSubRoom(mob.exclude);
						else
							tile = room.GetSubRoom(mob.roomSize, mob.location, mob.exclude);

						if (tile == null)
							continue;

						MonsterSpawnInfo monsterInfo;

						if (mob.count > 1)
						{
							for (int i = 0; i < mob.count; i++)
							{
								monsterInfo = level.SpawnMonsterToRoom(room, mob.monsterTypeName, tile, true, mob.level, mob.chance);

								if (monsterInfo != null)
								{
									count++;
									monsterInfo.tempId = mob.mobId;

									if (mob.idParent > 0)
									{
										try
										{
											MonsterSpawnInfo parent = null;

											foreach (MonsterSpawnInfo inf in infos)
												if (inf.tempId == mob.idParent)
												{
													parent = inf;
													break;
												}

											monsterInfo.master = parent;
										}
										catch (Exception)
										{
											throw new Exception("pokousim se nastavit idparent na " + mob.idParent + " ale takovy mob neexistuje! kouka se jen na moby ktere byly nacteny drive. ID nacitaneho moba je " + mob.mobId);
										}
									}

									infos.Add(monsterInfo);
								}
							}
						}
						else
						{
							monsterInfo = level.SpawnMonsterToRoom(room, mob.monsterTypeName, tile, false, mob.level, mob.chance);

							if (monsterInfo != null)
							{
								count++;
								monsterInfo.tempId = mob.mobId;

								if (mob.idParent > 0)
								{
									try
									{
										MonsterSpawnInfo parent = null;

										foreach (MonsterSpawnInfo inf in infos)
											if (inf.tempId == mob.idParent)
											{
												parent = inf;
												break;
											}

										monsterInfo.master = parent;
									}
									catch (Exception)
									{
										throw new Exception("pokousim se nastavit idparent na " + mob.idParent + " ale takovy mob neexistuje! kouka se jen na moby ktere byly nacteny drive. ID nacitaneho moba je " + mob.mobId);
									}
								}

								infos.Add(monsterInfo);
							}
						}
					}

					if (group.extraGroups.Any())
					{
						int minStack = 10000;
						int maxStack = 0;

						foreach (ExtraGroupData extraGroup in group.extraGroups)
						{
							// spawn all -1 groups
							if (extraGroup.stackId == -1)
							{
								if (Random.Range(0, 100) < extraGroup.chance)
								{
									GenerateGenericEnemyGroup(room, level, RoomType.EXTRA_ROOM, difficulty, regionData, extraGroup.groupId);
								}
							}
							else // optimize for next for loop 
							{
								if (extraGroup.stackId > maxStack)
								{
									maxStack = extraGroup.stackId;
								}

								if (extraGroup.stackId < minStack)
								{
									minStack = extraGroup.stackId;
								}
							}
						}

						// are there any groups that have stackid set?
						if (maxStack > 0)
						{
							// shuffle
							System.Random rnd = new System.Random();
							IOrderedEnumerable<ExtraGroupData> shuffled = group.extraGroups.OrderBy(item => rnd.Next());

							for (int i = minStack; i <= maxStack; i++)
							{
								foreach (ExtraGroupData extraGroup in shuffled)
								{
									if (extraGroup.stackId == i)
									{
										if (Random.Range(0, 100) < extraGroup.chance)
										{
											GenerateGenericEnemyGroup(room, level, RoomType.EXTRA_ROOM, difficulty, regionData, extraGroup.groupId);
											break; // only one spawned group per stack
										}
									}
								}
							}
						}
					}

					spawned = true;

					Debug.Log("spawnuto " + count + " ze skup. " + selectedId);
					
					break;
				}
			}

			if (!spawned)
			{
				Debug.LogError("nenalezena spawngroup do mistnosti typu " + roomType.ToString() + " " + room.region.x + "-" + room.region.y + " in world " + level.ToString());
			}

			if(forcedId > 0 && WorldHolder.instance.activeMap != null)
				WorldHolder.instance.activeMap.ConfigureMonstersAfterSpawn();
		}
	}

	public class MobGroup
	{
		public MonsterGenerator.RoomType roomType;

		public int id;
		public int minLevel;
		public int maxLevel;
		public int minWorld;
		public int maxWorld;
		public int frequency;
		public MonsterGenerator.RegionSize minRegionSize;
		public MonsterGenerator.RegionSize maxRegionSize;

		public List<MobData> mobs = new List<MobData>();
		public List<ExtraGroupData> extraGroups = new List<ExtraGroupData>(); 

		public MobGroup(MonsterGenerator.RoomType roomType, int id, int minLevel, int maxLevel, int minWorld, int maxWorld, int frequency, MonsterGenerator.RegionSize minRegionSize, MonsterGenerator.RegionSize maxRegionSize)
		{
			this.roomType = roomType;

			this.id = id;
			this.minLevel = minLevel;
			this.maxLevel = maxLevel;
			this.minWorld = minWorld;
			this.maxWorld = maxWorld;
			this.frequency = frequency;
			this.minRegionSize = minRegionSize;
			this.maxRegionSize = maxRegionSize;
		}

		public void AddMob(MobData mob)
		{
			mobs.Add(mob);
			//mob.PrintInfo();
		}

		public void AddExtraGroup(int groupId, int chance, int stackId)
		{
			extraGroups.Add(new ExtraGroupData(groupId, chance, stackId));
		}
	}

	public class ExtraGroupData
	{
		public int groupId;
		public int chance;
		public int stackId;

		public ExtraGroupData(int groupId, int chance, int stackId)
		{
			this.groupId = groupId;
			this.chance = chance;
			this.stackId = stackId;
		}
	}

	public class MobData
	{
		public int mobId;
		public int idParent;
		public string monsterTypeName;
		public int count;
		public int level;
		public int chance;

		public int location;
		public MapRoom.RoomType roomSize;

		public bool exclude;

		public MonsterGenerator.RegionSize minSize;

		public MobData(int mobId, int idParent, string monsterType, int count, int level, int chance, string location, string roomSize, bool exclude, MonsterGenerator.RegionSize minSize)
		{
			this.mobId = mobId;
			this.idParent = idParent;
			this.count = count;
			this.level = level;
			this.chance = chance;
			this.exclude = exclude;
			this.minSize = minSize;

			this.monsterTypeName = monsterType;

			/*try
			{
				this.type = (MonsterId)Enum.Parse(typeof(MonsterId), monsterType);
			}
			catch (Exception)
			{
				throw new Exception("neznamy typ moba " + monsterType);
			}*/
			
			this.roomSize = (MapRoom.RoomType) Enum.Parse(typeof (MapRoom.RoomType), roomSize.ToUpper());

			switch (location)
			{
				case "center":
					this.location = MapRoom.DIRECTION_CENTER;
					break;
				case "left":
					this.location = MapRoom.DIRECTION_LEFT;
					break;
				case "right":
					this.location = MapRoom.DIRECTION_RIGHT;
					break;
				case "up":
					this.location = MapRoom.DIRECTION_UP;
					break;
				case "down":
					this.location = MapRoom.DIRECTION_DOWN;
					break;
				case "largest":
					this.location = MapRoom.DIRECTION_LARGEST_ROOM;
					break;
			}
		}

		public void PrintInfo()
		{
			Debug.Log(mobId + " " + idParent + " " + monsterTypeName + " " + count + " " + level + " " + chance + " " + location + " " + roomSize + " exclude," + exclude);
		}
	}
}
