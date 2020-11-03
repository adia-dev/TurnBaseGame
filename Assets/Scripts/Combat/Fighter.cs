using System.Collections;
using System.Collections.Generic;
using TurnBaseGame.Entity;
using UnityEngine;

namespace TurnBaseGame.Combat
{
    public class Fighter : MonoBehaviour
    {
        Monster _monster;


        void Awake()
        {
            _monster = GetComponent<Monster>();
        }

        void Update()
        {

        }

        public void UseSkill(Skill skill, List<Monster> targets)
        {

            if (targets.Count == 0) return;

            foreach (var target in targets)
            {
                if (skill.SkillInfo.SkillType == SkillType.ATTACK)
                {
                    target.Stats.TakeDamage((int)(_monster.Stats.CurrentATK * skill.SkillInfo.AtkMultiplier));
                }
                else if (skill.SkillInfo.SkillType == SkillType.HEAL)
                {
                    target.Stats.Heal(skill.SkillInfo._healPercent);
                }
            }
        }

    }

}