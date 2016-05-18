using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.scripts.AI;
using Assets.scripts.Skills.Base;
using Assets.scripts.Skills.SkillEffects;
using UnityEngine;

namespace Assets.scripts.Skills.ActiveSkills
{
	public class Teleport : ActiveSkill
	{
		public int jumpSpeed = 100;

		public Teleport()
		{
			castTime = 0f;
			reuse = 1.0f;
			coolDown = 0f;
			requireConfirm = true;
			breaksMouseMovement = false;
			resetMoveTarget = false;

			range = 10;
		}

		public override SkillId GetSkillId()
		{
			return SkillId.Teleport;
		}

		public override string GetVisibleName()
		{
			return "Teleport";
		}

		public override Skill Instantiate()
		{
			return new Teleport();
		}

		public override SkillEffect[] CreateEffects(int param)
		{
			return null;
		}

		public override void InitTraits()
		{
			AddTrait(SkillTraits.Teleport);
		}

		public override bool OnCastStart()
		{
			if (castTime > 0)
				CreateCastingEffect(true, "SkillTemplate");

			return true;
		}

		public override void OnLaunch()
		{
			DeleteCastingEffect();

			if (GetOwnerData().GetOwner().AI is PlayerAI)
			{
				Vector3 targetPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
				targetPos.z = 0;

				if (Utils.DistanceSqr(GetOwnerData().GetBody().transform.position, targetPos) > range*range)
				{
					Vector3 direction = Utils.GetDirectionVector(targetPos, GetOwnerData().GetBody().transform.position).normalized;
					targetPos = GetOwnerData().GetBody().transform.position + direction*range;
				}

				/*if (!Utils.CanSee(GetOwnerData().GetBody().transform.position, targetPos))
				{
					AbortCast();
					return;
				}*/

				if (Utils.IsNotAccessible(GetOwnerData().GetBody().transform.position, targetPos))
				{
					AbortCast();
					return;
				}

				GetOwnerData().Teleport(targetPos, range);
			}
			else
			{
				if (initTarget != null)
				{
					bool found = false;

					Vector3 targetPos = initTarget.transform.position;
					float targetRange = Utils.DistanceSqr(GetOwnerData().GetBody().transform.position, targetPos);

					Vector3 target = new Vector3();

					if (targetRange > range*range)
					{
						Vector3 direction = Utils.GetDirectionVector(targetPos, GetOwnerData().GetBody().transform.position).normalized;
						target = GetOwnerData().GetBody().transform.position + direction * range;

						found = true;

						if (Utils.IsNotAccessible(GetOwnerData().GetBody().transform.position, target))
						{
							AbortCast();
							return;
						}
					}
					else
					{
						int limit = 15;
						while (limit > 0)
						{
							target = Utils.GenerateRandomPositionAround(targetPos, 3f, 1f);
							if (Utils.IsNotAccessible(GetOwnerData().GetBody().transform.position, target))
							{
								limit--;
								continue;
							}
							else
							{
								found = true;
								break;
							}
						}
					}

					if(found)
						GetOwnerData().Teleport(target, range);
				}
				else
				{
					Vector3 targetPos = fixedTarget;
					targetPos.z = 0;

					if (Utils.DistanceSqr(GetOwnerData().GetBody().transform.position, targetPos) > range * range)
					{
						Vector3 direction = Utils.GetDirectionVector(targetPos, GetOwnerData().GetBody().transform.position).normalized;
						targetPos = GetOwnerData().GetBody().transform.position + direction * range;
					}

					/*if (!Utils.CanSee(GetOwnerData().GetBody().transform.position, targetPos))
					{
						AbortCast();
						return;
					}*/

					if (Utils.IsNotAccessible(GetOwnerData().GetBody().transform.position, targetPos))
					{
						AbortCast();
						return;
					}

					GetOwnerData().Teleport(targetPos, range);
				}
			}
		}

		public override void UpdateLaunched()
		{
		}

		public override void OnAbort()
		{ 
		}

		public override void OnFinish()
		{
		}

		public override void MonoUpdate(GameObject gameObject, bool fixedUpdate)
		{
		}

		public override bool CanMove()
		{
			return !IsActive(); // cant move unless the jump is finished
		}

		public override bool CanRotate()
		{
			return CanMove();
		}
	}
}
