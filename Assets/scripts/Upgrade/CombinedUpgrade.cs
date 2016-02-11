using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.scripts.Skills;
using Assets.scripts.Skills.Base;
using Assets.scripts.Skills.SkillEffects;
using UnityEngine;

namespace Assets.scripts.Upgrade
{
	public class CombinedUpgrade : AbstractUpgrade
	{
		public AbstractUpgrade first;
		public AbstractUpgrade second;

		public CombinedUpgrade(int level, AbstractUpgrade u1, AbstractUpgrade u2, bool collectableByPlayer = true) : base(level, collectableByPlayer)
		{
			first = u1;
			second = u2;
			Name = "";
		}

		public override AbstractUpgrade Init()
		{
			try
			{
				MainSprite = LoadSprite("lvl1.png");
			}
			catch (Exception)
			{
				MainSprite = Resources.Load<Sprite>("Sprite/Upgrades/Combined/lvl1");
			}

			return this;
		}

		public override void ApplySkillChanges(SkillSet set, ActiveSkill melee)
		{
			first.ApplySkillChanges(set, melee);
			second.ApplySkillChanges(set, melee);
		}

		public override void RestoreSkillChanges(SkillSet set, ActiveSkill melee)
		{
			first.RestoreSkillChanges(set, melee);
			second.RestoreSkillChanges(set, melee);
		}

		public override SkillEffect[] CreateAdditionalSkillEffects(Skill sk, SkillEffect[] effects)
		{
			if (!(sk is ActiveSkill))
				return null;

			SkillEffect[] ef1 = first.CreateAdditionalSkillEffects(sk, effects);
			SkillEffect[] ef2 = first.CreateAdditionalSkillEffects(sk, effects);

			SkillEffect[] finalEfs = new SkillEffect[ef1.Length + ef2.Length];
			ef1.CopyTo(finalEfs, 0);
			ef2.CopyTo(finalEfs, ef1.Length);
			return finalEfs;
		}

		public override void ModifySkillEffects(Skill sk, SkillEffect[] effects)
		{
			if (!(sk is ActiveSkill))
				return;

			first.ModifySkillEffects(sk, effects);
			second.ModifySkillEffects(sk, effects);
		}

		public override void ModifyRunSpeed(ref float runSpeed)
		{
			first.ModifyRunSpeed(ref runSpeed);
			second.ModifyRunSpeed(ref runSpeed);
		}

		public override void ModifyMaxHp(ref int maxHp)
		{
			first.ModifyMaxHp(ref maxHp);
			second.ModifyMaxHp(ref maxHp);
		}

		public override void ModifyMaxMp(ref int maxMp)
		{
			first.ModifyMaxMp(ref maxMp);
			second.ModifyMaxMp(ref maxMp);
		}

		public override void ModifyCriticalRate(ref int critRate)
		{
			first.ModifyCriticalRate(ref critRate);
			second.ModifyCriticalRate(ref critRate);
		}

		public override void ModifyCriticalDmg(ref float critDmg)
		{
			first.ModifyCriticalDmg(ref critDmg);
			second.ModifyCriticalDmg(ref critDmg);
		}

		public override void ModifySkillCooldown(ActiveSkill sk, ref float cooldown)
		{
			first.ModifySkillCooldown(sk, ref cooldown);
			second.ModifySkillCooldown(sk, ref cooldown);
		}

		public override void ModifySkillReuse(ActiveSkill sk, ref float reuse)
		{
			first.ModifySkillReuse(sk, ref reuse);
			second.ModifySkillReuse(sk, ref reuse);
		}

		public override void ModifySkillCasttime(ActiveSkill sk, ref float casttime)
		{
			first.ModifySkillCasttime(sk, ref casttime);
			second.ModifySkillCasttime(sk, ref casttime);
		}

		protected override void InitInfo()
		{
			//TODO
		}
	}
}
