using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Assets.scripts.Actor;
using Debug = UnityEngine.Debug;

namespace Assets.scripts.AI.Base
{
	public class AIGroup
	{
		private Character leader;
		private List<Character> members; 
		public AIGroup(Character leader)
		{
			this.leader = leader;
			members = new List<Character>();
			AddMember(leader);
		}

		public void SetLeader(Character ch)
		{
			if (IsInGroup(ch) == false)
			{
				Debug.LogError("setting leader to a membe who is not in the group");
				return;
			}

			if (leader != null)
			{
				leader.AI.IsGroupLeader = false;
			}

			leader = ch;
			leader.AI.IsGroupLeader = true;
		}

		public Character GetLeader()
		{
			return leader;
		}

		public void AddMember(Character ch)
		{
			if (IsInGroup(ch))
				return;

			members.Add(ch);
			ch.AI.Group = this;

			Debug.Log("adding " + ch.Name + " to group of " + leader.Name);
		}

		public bool IsInGroup(Character ch)
		{
			return members.Contains(ch);
		}

		public void RemoveMember(Character ch)
		{
			ch.AI.Group = null;
			ch.AI.IsGroupLeader = false;
			members.Remove(ch);
		}
	}
}
