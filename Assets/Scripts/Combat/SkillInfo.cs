using UnityEngine;


namespace TurnBaseGame.Combat
{

    public enum SkillType
    {
        ATTACK, BUFF, DEBUFF, PASSIVE, HEAL, STRIP, CLEANSE
    }

    public enum SkillAreaOfAttack
    {
        MONOTARGET, MULTIPLE
    }


    public enum SkillTarget
    {
        ALLIE, ENEMY, ALL
    }

    public enum BUFF
    {
        ATK, ATKBAR, DEF, SPD, CR, UNRECOVERABLE, CONTINUOUS_DAMAGE, FREEZE, STUN, SLEEP, IMMUNITY, INVICIBILITY, ENDURE, REVENGE, RECOVERY, COUNTER, BLESSED_SOUL, REFLECT, SHIELD
    }

    [System.Serializable]
    public class SkillEffect
    {
        public BUFF buff;
        public SkillAreaOfAttack target;
        public SkillTarget targetType;
        public int amount;
        public int turn;

        public SkillEffect()
        {
            buff = BUFF.ATK;
            target = SkillAreaOfAttack.MONOTARGET;
            targetType = SkillTarget.ENEMY;
            amount = 0;
            turn = 1;
        }
    }

    [CreateAssetMenu(fileName = "Skill", menuName = "Turn Base Game/Create New Skill")]
    public class SkillInfo : ScriptableObject
    {

        [SerializeField] string _name = "Skill";
        [TextArea(5, 15)]
        [SerializeField] string _description = "...";
        [SerializeField] Sprite _skillIcon;
        [SerializeField] SkillType _skillType;
        [SerializeField] SkillTarget _skillTarget;
        [SerializeField] SkillAreaOfAttack _skillAreaOfAttack;
        [SerializeField] SkillEffect[] _skillEffects;

        public int _atkMultiplier = 125;
        public int _healPercent = 25;

        public string Name => _name;
        public string Description => _description;
        public Sprite SkillIcon => _skillIcon;

        public SkillType SkillType => _skillType;
        public SkillAreaOfAttack SkillAreaOfAttack => _skillAreaOfAttack;
        public SkillTarget SkillTarget => _skillTarget;
        public SkillEffect[] SkillEffects => _skillEffects;

        public float AtkMultiplier => _atkMultiplier / 100;

    }

}