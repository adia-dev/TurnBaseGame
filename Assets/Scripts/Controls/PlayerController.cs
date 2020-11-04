using System;
using System.Collections.Generic;
using System.Linq;
using TurnBaseGame.Combat;
using TurnBaseGame.Entity;
using UnityEngine;

namespace TurnBaseGame.Controls
{

    [System.Serializable]
    public struct ElementColor
    {
        public Element Element;
        public Color color;
    }

    public class PlayerController : MonoBehaviour
    {
        [SerializeField] Color _advantageColor = Color.green;
        [SerializeField] Color _balancedColor = Color.yellow;
        [SerializeField] Color _disadvantageColor = Color.red;
        [SerializeField] ElementColor[] _elementColors;

        [SerializeField] public Monster[] _allieMonsters = new Monster[3];
        [SerializeField] public Monster[] _enemieMonsters;
        [SerializeField] public Monster _currentMonster;
        [SerializeField] public Skill[] _skills = new Skill[4];
        [SerializeField] public Skill CurrentSkill = null;
        [SerializeField] GameObject turnIndicator;

        Camera _camera;
        bool _monsterWithATBReady = false;


        Ray MouseRay => _camera.ScreenPointToRay(Input.mousePosition);

        public void ChangeCurrentSkill()
        {
            Action changeCurrentSkill = OnChangeCurrentSkill;
            if (changeCurrentSkill != null)
            {
                changeCurrentSkill.Invoke();
            }
        }
        public event Action OnChangeCurrentSkill = delegate { };
        public event Action OnTick = delegate { };

        void Awake()
        {
            _camera = Camera.main;
        }

        void Start()
        {
            OnTick?.Invoke();

            foreach (var monster in FindObjectsOfType<Monster>())
            {
                foreach (var elementColor in _elementColors)
                {
                    if (monster.MonsterInfo.Element == elementColor.Element)
                    {
                        monster.ElementUI.color = elementColor.color;
                        break;
                    }
                }
                monster.LevelUI.text = monster.Stats.Level.ToString();
            }
        }

        private void OnEnable()
        {
            OnTick += SetCurrentMonster;
            OnChangeCurrentSkill += UpdateArrowSelection;
        }

        private void OnDisable()
        {
            OnTick -= SetCurrentMonster;
            OnChangeCurrentSkill -= UpdateArrowSelection;
        }


        void InitializeSkills()
        {
            for (int i = 0; i < _skills.Length; i++)
            {
                _skills[i].SkillInfo = _currentMonster.MonsterInfo.TryGetSkills(i);

            }
            CurrentSkill = _skills[0];
            OnChangeCurrentSkill?.Invoke();
        }

        void Update()
        {
            turnIndicator.transform.position = _currentMonster.transform.position + Vector3.up * 1.5f;

            if (!_monsterWithATBReady)
            {
                foreach (var monster in FindObjectsOfType<Monster>())
                {
                    if (monster.Stats.CurrentATKBAR > 100)
                    {
                        _monsterWithATBReady = true;
                        break;
                    }
                }

                OnTick?.Invoke();
                return;
            }

            HandlePlayerInteraction();
        }

        void HandlePlayerInteraction()
        {
            if (Physics.Raycast(MouseRay, out RaycastHit hitInfo) && Input.GetMouseButtonDown(0))
            {
                var monster = hitInfo.collider.GetComponent<Monster>();

                if (monster == null) return;

                InteractWithMonster(monster);
            }
        }

        void InteractWithMonster(Monster monster)
        {
            List<Monster> targets = new List<Monster>();

            if (CurrentSkill.SkillInfo.SkillAreaOfAttack == SkillAreaOfAttack.MULTIPLE)
            {
                if (CurrentSkill.SkillInfo.SkillTarget == SkillTarget.ALLIE && _allieMonsters.Contains(monster))
                {
                    targets.AddRange(_allieMonsters);
                    _currentMonster.UseSkill(CurrentSkill, targets);

                    OnTick?.Invoke();
                    _monsterWithATBReady = false;
                }
                else if (CurrentSkill.SkillInfo.SkillTarget == SkillTarget.ENEMY && _enemieMonsters.Contains(monster))
                {
                    targets.AddRange(_enemieMonsters);
                    _currentMonster.UseSkill(CurrentSkill, targets);

                    OnTick?.Invoke();
                    _monsterWithATBReady = false;
                }
            }
            else if (CurrentSkill.SkillInfo.SkillAreaOfAttack == SkillAreaOfAttack.MONOTARGET)
            {
                if (CurrentSkill.SkillInfo.SkillTarget == SkillTarget.ALLIE && _allieMonsters.Contains(monster))
                {
                    targets.Add(monster);
                    _currentMonster.UseSkill(CurrentSkill, targets);

                    OnTick?.Invoke();
                    _monsterWithATBReady = false;
                }
                else if (CurrentSkill.SkillInfo.SkillTarget == SkillTarget.ENEMY && _enemieMonsters.Contains(monster))
                {
                    targets.Add(monster);
                    _currentMonster.UseSkill(CurrentSkill, targets);

                    OnTick?.Invoke();
                    _monsterWithATBReady = false;
                }
            }

        }

        void SetCurrentMonster()
        {
            Dictionary<Monster, int> monsterAttackBar = new Dictionary<Monster, int>();

            foreach (var monster in FindObjectsOfType<Monster>())
            {
                monsterAttackBar.Add(monster, monster.Stats.CurrentATKBAR);
            }

            _currentMonster = monsterAttackBar.FirstOrDefault(m => m.Value == monsterAttackBar.Values.Max()).Key;

            InitializeSkills();
        }



        void UpdateArrowSelection()
        {

            if (CurrentSkill == null) return;

            foreach (var monster in FindObjectsOfType<Monster>())
            {
                monster.SelectionArrow.SetActive(false);
                monster.SelectionArrowUI.color = _advantageColor;
            }

            if (CurrentSkill.SkillInfo.SkillTarget == SkillTarget.ALLIE)
            {
                foreach (var monster in _allieMonsters)
                {
                    if (monster.Stats.IsDead) continue;

                    monster.SelectionArrow.SetActive(true);

                }
            }
            else if (CurrentSkill.SkillInfo.SkillTarget == SkillTarget.ENEMY)
            {
                foreach (var monster in _enemieMonsters)
                {
                    if (monster.Stats.IsDead) continue;

                    monster.SelectionArrow.SetActive(true);
                    SetArrowColor(monster);
                }
            }
        }

        private void SetArrowColor(Monster monster)
        {
            switch (monster.GetElementInfluence(_currentMonster))
            {
                case ElementInfluence.BALANCED:
                    monster.SelectionArrowUI.color = _balancedColor;
                    break;
                case ElementInfluence.ADVANTAGE:
                    monster.SelectionArrowUI.color = _advantageColor;
                    break;
                case ElementInfluence.DISADVANTAGE:
                    monster.SelectionArrowUI.color = _disadvantageColor;
                    break;
            }
        }

        private void OnGUI()
        {
            GUI.TextArea(new Rect(10, 10, 200, 25), CurrentSkill.SkillInfo.Name + (CurrentSkill.SkillInfo.SkillAreaOfAttack == SkillAreaOfAttack.MULTIPLE ? ": AOE" : ": Monotarget"));
            string effects = string.Empty;

            SkillTarget monsterType = SkillTarget.ALL;

            if (_allieMonsters.Contains(_currentMonster))
                monsterType = SkillTarget.ALLIE;
            else if (_enemieMonsters.Contains(_currentMonster))
                monsterType = SkillTarget.ENEMY;

            GUI.TextArea(new Rect(10, 45, 200, 25), "Monster : " + _currentMonster.MonsterInfo.AwakanedName + " - " + monsterType.ToString());

            GUI.TextArea(new Rect(10, 80, 200, 25), "ATB : " + _currentMonster.Stats.CurrentATKBAR.ToString());

            Rect effectRect = new Rect(10, 115, 200, 0);

            if (CurrentSkill.SkillInfo.SkillEffects.Length == 0)
            {
                effectRect.height += 20;
                GUI.TextArea(effectRect, "No effects");
                return;
            }


            foreach (var effect in CurrentSkill.SkillInfo.SkillEffects)
            {
                effects += effect.buff.ToString();
                effects += "\n";
                effectRect.height += 20;
            }

            GUI.TextArea(effectRect, effects);



        }

    }

}