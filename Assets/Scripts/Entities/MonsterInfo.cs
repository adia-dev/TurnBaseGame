using TurnBaseGame.Combat;
using UnityEngine;



namespace TurnBaseGame.Entity
{

    [System.Serializable]
    public enum Element
    {
        Fire, Wind, Water, Light, Dark
    }

    [System.Serializable]
    public enum Type
    {
        ATTACK, DEFENSE, HP, SUPPORT
    }

    [System.Serializable]
    public enum Rarity
    {
        ONE = 1, TWO, THREE, FOUR, FIVE, SIX
    }


    [System.Serializable]
    public class MonsterInfo
    {

        const int NUMBER_OF_LEVELS = 10;
        const int NUMBER_OF_SKILLS = 3;

        [SerializeField] string _awakanedName = "Awakaned Monster";

        public Element Element;
        [SerializeField] Type _type = Type.ATTACK;
        [SerializeField] Rarity _rarity = Rarity.ONE;
        [SerializeField] bool _isAwakaned = false;

        [SerializeField] SkillInfo[] _skills = new SkillInfo[NUMBER_OF_SKILLS];
        [SerializeField] SkillInfo _passiveSkill;
        [SerializeField] StatsEvolution[] _statsEvolution = new StatsEvolution[NUMBER_OF_LEVELS];
        public Material ElementMaterial;


        public string AwakanedName => _awakanedName;
        public Rarity Rarity => _rarity;
        public bool IsAwakaned => _isAwakaned;

        public SkillInfo[] Skills => _skills;
        public SkillInfo PassiveSkill => _passiveSkill;
        public StatsEvolution[] StatsEvolution => _statsEvolution;

        public SkillInfo TryGetSkills(int index)
        {
            if (index < 0 || index >= _skills.Length) return null;

            return _skills[index];
        }


    }

}