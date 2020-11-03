using System.Collections;
using System.Collections.Generic;
using TurnBaseGame.Combat;
using UnityEngine;


namespace TurnBaseGame.Entity
{
    [CreateAssetMenu(fileName = "New Monster Family", menuName = "Turn Base Game/Create New Monster Family", order = 0)]
    public class MonsterFamily : ScriptableObject
    {
        [SerializeField] int fastConfigIndex = 0;
        [SerializeField] string _name = "Monster";
        [SerializeField] MonsterInfo[] _monsterInfos = new MonsterInfo[5];


        public MonsterInfo GetMonsterInfo(Element element)
        {
            foreach (var monsterInfo in _monsterInfos)
            {
                if (monsterInfo.Element == element)
                    return monsterInfo;
            }
            return null;
        }


        public MonsterInfo GetMonsterInfo(int index)
        {
            if (index < 0 || index >= _monsterInfos.Length) return null;

            return _monsterInfos[index];
        }


        public string Name => _name;

        [ContextMenu("Fast Config")]
        public void FastConfiguration()
        {
            _monsterInfos[0].Element = Element.Fire;
            _monsterInfos[1].Element = Element.Wind;
            _monsterInfos[2].Element = Element.Water;

            _monsterInfos[3].Element = Element.Light;
            _monsterInfos[4].Element = Element.Dark;
        }


        [ContextMenu("Set Stat Evolution Proportionnal")]
        public void ProportionalStatEvolution()
        {
            for (int i = 1; i < _monsterInfos[fastConfigIndex].StatsEvolution.Length; i++)
            {

                _monsterInfos[fastConfigIndex].StatsEvolution[i].HP = (int)(_monsterInfos[fastConfigIndex].StatsEvolution[i - 1].HP * 1.15f);
                _monsterInfos[fastConfigIndex].StatsEvolution[i].ATK = (int)(_monsterInfos[fastConfigIndex].StatsEvolution[i - 1].ATK * 1.15f);
                _monsterInfos[fastConfigIndex].StatsEvolution[i].DEF = (int)(_monsterInfos[fastConfigIndex].StatsEvolution[i - 1].DEF * 1.15f);
                _monsterInfos[fastConfigIndex].StatsEvolution[i].SPD = _monsterInfos[fastConfigIndex].StatsEvolution[0].SPD;
            }
        }
    }

}