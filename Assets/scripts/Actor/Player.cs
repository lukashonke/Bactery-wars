using System.Collections;
using Assets.scripts.Actor.PlayerClasses;
using Assets.scripts.Actor.Status;
using Assets.scripts.Base;
using Assets.scripts.Mono;
using Assets.scripts.Mono.ObjectData;
using Assets.scripts.Skills;
using Assets.scripts.Skills.Base;
using UnityEngine;

namespace Assets.scripts.Actor
{
	/// <summary>
	/// Datova trida reprezentujici informace o jednom hraci (jeho skilly, classu, stav (pocet HP, atd.))
	/// </summary>
	public class Player : Character
	{
		public ClassTemplate Template { get; set; }

		public Player(string name, PlayerData dataObject, ClassTemplate template) : base(name)
		{
			Data = dataObject;

			Template = template;
		}

		public new PlayerData GetData()
		{
			return (PlayerData) Data;
		}

		/// <summary>
		/// Inicializuje sablonu hrace
		/// </summary>
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

		/// <summary>
		/// Probiha kazdy snimek hry
		/// </summary>
		public override void OnUpdate()
		{
			
		}

		/// <summary>
		/// Inicializuje status hrace (HP, max. rychlost, atd.)
		/// </summary>
		protected override CharStatus InitStatus()
		{
			CharStatus st = new PlayerStatus(false, Template.MaxHp, Template.MaxMp, Template); //TODO
			GetData().SetVisibleHp(st.Hp);
			GetData().SetVisibleMaxHp(st.MaxHp);
			GetData().SetMoveSpeed(st.MoveSpeed);

			return st;
		}

		/// <summary>
		/// Inicializuje skillset hrace
		/// </summary>
		protected override SkillSet InitSkillSet()
		{
			SkillSet set = new SkillSet();
			return set;
		}

		/// <summary>
		/// Vytvori novy Task (vyuziva Unity Coroutiny)
		/// Task je ukol ktery muze probihat rozlozeny mezi nekolik snimku hry
		/// (prubeh metody se muze na urcitou dobu pozastavit a provest az v jinem, popr. hned nasledujicim snimku)
		/// </summary>
		public override Coroutine StartTask(IEnumerator skillTask)
		{
			return GetData().StartCoroutine(skillTask);
		}

		/// <summary>
		/// Predcasne ukonci Task
		/// </summary>
		/// <param name="c"></param>
		public override void StopTask(Coroutine c)
		{
			GetData().StopCoroutine(c);
		}

		public override void StopTask(IEnumerator t)
		{
			GetData().StopCoroutine(t);
		}
	}
}
