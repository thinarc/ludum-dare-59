using UnityEngine;

namespace _Project.Scripts.Game.Wave
{
    public class WaveController : MonoBehaviour
    {
        public bool battle;
        private bool _battle;
        
        public void ChangeState() => battle = !battle;
        
        private void Update()
        {
            if (battle && !_battle) ToBattle();
            else if (!battle && _battle) ToIdle();
            
            if (Input.GetKeyDown(KeyCode.F)) ChangeState();
        }

        private void ToBattle()
        {
            _battle = battle;
            CamChanger.Instance.ChangeView();
        }
        
        private void ToIdle()
        {
            _battle = battle;
            CamChanger.Instance.ChangeView();
        }
    }
}