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
            }

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