using System.Collections.Generic;
using TMPro;
using TurnBaseGame.Combat;
using UnityEngine;
using UnityEngine.UI;

namespace TurnBaseGame.Entity
{

    public enum ElementInfluence
    {
        ADVANTAGE, BALANCED, DISADVANTAGE
    }

    public class Monster : MonoBehaviour
    {

        [SerializeField] MonsterFamily _monsterFamily;
        [SerializeField] Element _element;
        [Range(1, 10)]
        [SerializeField] int _monsterLevel = 1;


        public GameObject SelectionArrow;
        public Image SelectionArrowUI;
        public Image ElementUI;
        public TextMeshProUGUI LevelUI;
        public MonsterInfo MonsterInfo { get; private set; }
        public Stats Stats { get; private set; }

        int MonsterLevelArray => _monsterLevel - 1;


        private void Awake()
        {
            Stats = GetComponent<Stats>();
        }

        void Start()
        {
            if (_monsterFamily != null)
            {
                MonsterInfo = _monsterFamily.GetMonsterInfo(_element);
                GetComponent<MeshRenderer>().material = MonsterInfo.ElementMaterial;
                Stats.InitializeStats(MonsterInfo.StatsEvolution[MonsterLevelArray]);
                Stats.Level = _monsterLevel;
            }

        }

        public void UseSkill(Skill skill, List<Monster> targets)
        {

            if (targets.Count == 0) return;

            foreach (var target in targets)
            {
                if (skill.SkillInfo.SkillType == SkillType.ATTACK)
                {
                    target.Stats.TakeDamage((int)(Stats.CurrentATK * 0 * skill.SkillInfo.AtkMultiplier));
                }
                else if (skill.SkillInfo.SkillType == SkillType.HEAL)
                {
                    target.Stats.Heal(skill.SkillInfo._healPercent);
                }
            }

            Stats.ResetATB();
        }

        public ElementInfluence GetElementInfluence(Monster monster)
        {
            if (_element == monster._element) return ElementInfluence.BALANCED;

            switch (_element)
            {
                case Element.Fire:
                    if (monster._element == Element.Water) return ElementInfluence.ADVANTAGE;
                    else if (monster._element == Element.Wind) return ElementInfluence.DISADVANTAGE;
                    break;

                case Element.Wind:
                    if (monster._element == Element.Fire) return ElementInfluence.ADVANTAGE;
                    else if (monster._element == Element.Water) return ElementInfluence.DISADVANTAGE;
                    break;

                case Element.Water:
                    if (monster._element == Element.Wind) return ElementInfluence.ADVANTAGE;
                    else if (monster._element == Element.Fire) return ElementInfluence.DISADVANTAGE;
                    break;
                case Element.Light:
                    if (monster._element == Element.Dark) return ElementInfluence.ADVANTAGE;
                    break;
                case Element.Dark:
                    if (monster._element == Element.Light) return ElementInfluence.ADVANTAGE;
                    break;
            }

            return ElementInfluence.BALANCED;
        }

    }

}