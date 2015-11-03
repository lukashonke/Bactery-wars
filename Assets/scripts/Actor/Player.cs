using System.Collections;
using Assets.scripts.Actor.PlayerClasses;
using Assets.scripts.Actor.Status;
using Assets.scripts.Base;
using Assets.scripts.Mono;
using Assets.scripts.Skills;
using Assets.scripts.Skills.Base;
using UnityEngine;

namespace Assets.scripts.Actor
{
	public class Player : Character
	{
		public readonly PlayerData data;

		public ClassTemplate Template { get; set; }

		public Player(string name, PlayerData dataObject, ClassTemplate template) : base(name)
		{
			data = dataObject;

			Template = template;
		}

		public void InitTemplate()
		{
			int i = 0;
			foreach(Skill templateSkill in Template.TemplateSkills)
			{
				// vytvorit novy objekt skillu
				Skill newSkill = SkillTable.Instance.CreateSkill(templateSkill.Id);
				newSkill.SetOwner(this);

				Skills.AddSkill(newSkill);

				i++;
				Debug.Log("adding skill to " + i + ": " + newSkill.Name);
			}
		}

		// happens every frame
		public override void OnUpdate()
		{
			
		}

		protected override CharStatus InitStatus()
		{
			CharStatus st = new PlayerStatus(false, 10, 10); //TODO
			return st;
		}

		protected override SkillSet InitSkillSet()
		{
			SkillSet set = new SkillSet();
			return set;
		}

		public override Coroutine StartTask(IEnumerator skillTask)
		{
			return data.StartCoroutine(skillTask);
		}

		public override void StopTask(Coroutine c)
		{
			data.StopCoroutine(c);
		}

		public override void StopTask(IEnumerator t)
		{
			data.StopCoroutine(t);
		}
	}
}
