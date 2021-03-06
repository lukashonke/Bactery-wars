﻿// Copyright (c) 2015, Lukas Honke
// ========================
using System;
using Assets.scripts.Actor.PlayerClasses.Base;
using Assets.scripts.Skills;
using Assets.scripts.Skills.Base;

namespace Assets.scripts.Actor.PlayerClasses
{
	public class FluPlayerClass : ClassTemplate
	{
		public FluPlayerClass()
		{
			MaxHp = 100;
			MaxMp = 50;
			MaxSpeed = 10;
			Shield = 1.0f;
			CriticalRate = 0;
			CriticalDamageMul = 2f;
		}

		public override void InitSkillsOnPlayer(SkillSet set, ActiveSkill meleeSkill)
		{
			meleeSkill.castTime = 0f;
		}

		protected override void AddSkillsToTemplate()
		{
			TemplateSkills.Add(SkillTable.Instance.GetSkill(SkillId.Projectile)); // the projectile test skill
			TemplateSkills.Add(SkillTable.Instance.GetSkill(SkillId.ProjectileTriple)); // the projectile test skill triple
			TemplateSkills.Add(SkillTable.Instance.GetSkill(SkillId.ProjectileAllAround)); // aura
			TemplateSkills.Add(SkillTable.Instance.GetSkill(SkillId.JumpShort)); // jump
			TemplateSkills.Add(SkillTable.Instance.GetSkill(SkillId.ChainSkill)); // chain skill
			TemplateSkills.Add(SkillTable.Instance.GetSkill(SkillId.SkillAreaExplode)); // bomb skill
			TemplateSkills.Add(SkillTable.Instance.GetSkill(SkillId.MissileProjectile)); // missile projectile
			TemplateSkills.Add(SkillTable.Instance.GetSkill(SkillId.ChainedProjectile)); // chained projectile

			SetMeleeAttackSkill((ActiveSkill)SkillTable.Instance.GetSkill(SkillId.MeleeAttack));
		}

		public override ClassId GetClassId()
		{
			return ClassId.Flu;
		}
	}
}
