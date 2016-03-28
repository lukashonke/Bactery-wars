using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.scripts.Mono.MapGenerator.Levels;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets.scripts.Mono.MapGenerator
{
	public class LevelTree : IEnumerable<LevelTree>
	{
		private List<LevelTree> childs = new List<LevelTree>();
		public List<LevelTree> Childs
		{
			get { return childs; }
		}

		public AbstractLevelData levelData;

		public int Id { get; set; }
		public int Depth { get; set; }

		// 1=main line, 2=special
		public int LevelNodeType { get; set; }
		public bool IsLastNode { get; set; }
		public int Difficulty { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
		public string RewardDescription { get; set; }
		public LevelParams LevelParams { get; set; }

		public bool Unlocked { get; set; }
		public bool CurrentlyActive { get; set; }

		public const int LEVEL_MAIN = 1;
		public const int LEVEL_EXTRA = 2;

		public const int DIFF_EASY = 1;
		public const int DIFF_MEDIUM = 2;
		public const int DIFF_HARD = 3;

		public LevelTree(int nodeType, int id, int difficulty, int depth=-1, LevelParams levelParams=null, string name = null, string desc = null, string reward = null)
		{
			levelData = null;

			Unlocked = false;

			this.LevelNodeType = nodeType;
			this.Id = id;
			this.Depth = depth;
			this.Difficulty = difficulty;
			this.LevelParams = levelParams;
			this.Name = name;
			this.Description = desc;
			this.RewardDescription = reward;
		}

		public void AddChild(LevelTree child)
		{
			childs.Add(child);
		}

		public LevelTree GetLastMainNode()
		{
			if (childs.Count > 0)
			{
				foreach (LevelTree n in childs)
				{
					if (n.LevelNodeType == LEVEL_MAIN)
					{
						return n.GetLastMainNode();
					}
				}				
			}

			return this;
		}

		public int GetNodesCount(bool onlyMain)
		{
			int count = 0;
			Queue<LevelTree> nodes = new Queue<LevelTree>();
			nodes.Enqueue(this);
			while(nodes.Count > 0)
			{
				LevelTree n = nodes.Dequeue();
				if (onlyMain)
				{
					if (n.LevelNodeType == LEVEL_MAIN)
						count++;
				}
				else
					count++;

				foreach(LevelTree ch in n.Childs)
					nodes.Enqueue(ch);
			}

			return count;
		}

		public string PrintInfo()
		{
			StringBuilder sb = new StringBuilder();

			List<LevelTree> nodes = GetAllNodes();
			bool done = false;

			for (int i = 0; i < 100; i++)
			{
				sb.Append("[");
				foreach (LevelTree t in nodes)
				{
					if (t.Depth == i)
					{
						sb.Append("" + t.Name + ":" + t.Depth + " ");

						if (t.IsLastNode)
						{
							done = true;
						}
					}
				}
				sb.Append("]");

				if (done)
					break;
			}

			return sb.ToString();
		}

		public List<LevelTree> GetAllNodes()
		{
			List<LevelTree> result = new List<LevelTree>();
			Queue<LevelTree> nodes = new Queue<LevelTree>();
			nodes.Enqueue(this);
			while (nodes.Count > 0)
			{
				LevelTree n = nodes.Dequeue();
				result.Add(n);

				foreach (LevelTree ch in n.Childs)
					nodes.Enqueue(ch);
			}

			return result;
		}

		public List<LevelTree> GetAllSpecialNodes()
		{
			List<LevelTree> result = new List<LevelTree>();
			Queue<LevelTree> nodes = new Queue<LevelTree>();
			nodes.Enqueue(this);
			while (nodes.Count > 0)
			{
				LevelTree n = nodes.Dequeue();

				if (n.LevelNodeType == LEVEL_EXTRA)
					result.Add(n);

				foreach (LevelTree ch in n.Childs)
					nodes.Enqueue(ch);
			}

			return result;
		}

		public List<LevelTree> GetAllMainNodes()
		{
			List<LevelTree> result = new List<LevelTree>();
			Queue<LevelTree> nodes = new Queue<LevelTree>();
			nodes.Enqueue(this);
			while (nodes.Count > 0)
			{
				LevelTree n = nodes.Dequeue();

				if(n.LevelNodeType == LEVEL_MAIN)
					result.Add(n);

				foreach (LevelTree ch in n.Childs)
					nodes.Enqueue(ch);
			}

			return result;
		}

		public LevelTree GetRandomMainNode(bool includeLast=false)
		{
			List<LevelTree> mainNodes = GetAllMainNodes();

			int max = mainNodes.Count;

			if (includeLast)
				max++;

			int rndDepth = Random.Range(0, max-1);
			return mainNodes[rndDepth];
		}

		public LevelTree GetMainNode(int depth)
		{
			List<LevelTree> mainNodes = GetAllMainNodes();

			foreach (LevelTree t in mainNodes)
			{
				if (t.Depth == depth)
					return t;
			}

			return null;
		}

		public LevelTree GetNode(int id)
		{
			foreach (LevelTree t in GetAllNodes())
			{
				if (t.Id == id)
					return t;
			}
			return null;
		}

		public LevelTree GetRandomSpecialNode()
		{
			List<LevelTree> specialNodes = GetAllSpecialNodes();

			int max = specialNodes.Count;

			int rndIndex = Random.Range(0, max);
			return specialNodes[rndIndex];
		}

		public IEnumerator<LevelTree> GetEnumerator()
		{
			return childs.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}
