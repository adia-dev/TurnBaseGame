using TurnBaseGame.Controls;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace TurnBaseGame.Combat
{
    public class Skill : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {

        [SerializeField] public Image _skillIcon;
        PlayerController _playerController;
        public SkillInfo SkillInfo;

        void Awake()
        {
            _playerController = FindObjectOfType<PlayerController>();
        }



        public void OnPointerClick(PointerEventData eventData)
        {
            if (SkillInfo == null) return;
            if (_playerController.CurrentSkill == this || SkillInfo.SkillType == SkillType.PASSIVE) return;

            _playerController.CurrentSkill = this;
            _playerController.ChangeCurrentSkill();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {

        }

        public void OnPointerExit(PointerEventData eventData)
        {

        }
    }

}