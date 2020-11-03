using System;
using System.Collections.Generic;
using System.Linq;
using TurnBaseGame.Combat;
using TurnBaseGame.Entity;
using UnityEngine;

namespace TurnBaseGame.Controls
{

    public class PlayerController : MonoBehaviour
    {
        [SerializeField] Color _advantageColor = Color.green;
        [SerializeField] Color _balancedColor = Color.yellow;
        [SerializeField] Color _disadvantageColor = Color.red;

        [SerializeField] public Monster[] _allieMonsters = new Monster[3];
        [SerializeField] public Monster[] _enemieMonsters;
        [SerializeField] public Monster _currentMonster;
        [SerializeField] public Skill[] _skills = new Skill[4];
        [SerializeField] public Skill CurrentSkill = null;


        Camera _camera;
        Fighter _fighter;
        Ray MouseRay => _camera.ScreenPointToRay(Input.mousePosition);

        public event Action OnChangeCurrentMonster = delegate { };
        public void ChangeCurrentSkill()
        {
            Action changeCurrentSkill = OnChangeCurrentSkill;
            if (changeCurrentSkill != null)
            {
                changeCurrentSkill.Invoke();
            }
        }
        public event Action OnChangeCurrentSkill = delegate { };

        void Awake()
        {
            _camera = Camera.main;
        }

        void Start()
        {
            _currentMonster = _allieMonsters[0];
            _fighter = _currentMonster.GetComponent<Fighter>();
            OnChangeCurrentMonster?.Invoke();

            CurrentSkill = _skills[0];
            OnChangeCurrentSkill?.Invoke();
        }

        private void OnEnable()
        {
            OnChangeCurrentMonster += InitializeSkills;
            OnChangeCurrentSkill += UpdateArrowSelection;
        }

        private void OnDisable()
        {
            OnChangeCurrentMonster -= InitializeSkills;
            OnChangeCurrentSkill -= UpdateArrowSelection;
        }


        void InitializeSkills()
        {
            for (int i = 0; i < _skills.Length; i++)
            {
                _skills[i].SkillInfo = _currentMonster.MonsterInfo.TryGetSkills(i);
                //_skills[i]._skillIcon.sprite = _currentMonster.MonsterInfo.TryGetSkills(i).SkillIcon;
            }
        }

        void Update()
        {
            MonsterTurn();
        }

        void MonsterTurn()
        {
            if (Physics.Raycast(MouseRay, out RaycastHit hitInfo) && Input.GetMouseButtonDown(0))
            {
                var monster = hitInfo.collider.GetComponent<Monster>();

                if (monster == null || monster.Stats.IsDead) return;

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
                    _fighter.UseSkill(CurrentSkill, targets);
                    IncrementAlly();
                }
                else if (CurrentSkill.SkillInfo.SkillTarget == SkillTarget.ENEMY && _enemieMonsters.Contains(monster))
                {
                    targets.AddRange(_enemieMonsters);
                    _fighter.UseSkill(CurrentSkill, targets);
                    IncrementAlly();
                }
            }
            else if (CurrentSkill.SkillInfo.SkillAreaOfAttack == SkillAreaOfAttack.MONOTARGET)
            {
                if (CurrentSkill.SkillInfo.SkillTarget == SkillTarget.ALLIE && _allieMonsters.Contains(monster))
                {
                    targets.Add(monster);
                    _fighter.UseSkill(CurrentSkill, targets);
                    IncrementAlly();
                }
                else if (CurrentSkill.SkillInfo.SkillTarget == SkillTarget.ENEMY && _enemieMonsters.Contains(monster))
                {
                    targets.Add(monster);
                    _fighter.UseSkill(CurrentSkill, targets);
                    IncrementAlly();
                }
            }

        }

        //Temp
        int index = 0;
        void IncrementAlly()
        {
            index = (index + 1) % _allieMonsters.Length;

            _currentMonster = _allieMonsters[index];
            _fighter = _currentMonster.GetComponent<Fighter>();
            OnChangeCurrentMonster?.Invoke();


            CurrentSkill = _skills[0];
            OnChangeCurrentSkill?.Invoke();
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

            Rect effectRect = new Rect(10, 45, 200, 0);

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