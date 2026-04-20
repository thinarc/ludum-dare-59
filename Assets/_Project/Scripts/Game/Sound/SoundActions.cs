using Cysharp.Threading.Tasks;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;

namespace _Project.Scripts.Game.Sound
{
    public class SoundActions
    {
        private EventInstance _onRadio;
        private EventInstance _constRadio;
        private EventInstance _searchRadio;
        private bool _onRadioState;
        private float _knobVal;

        public void RadioOn()
        {
            _onRadio = RuntimeManager.CreateInstance("event:/Radio/Turn_on");
            _onRadio.start();
            
            _constRadio = RuntimeManager.CreateInstance("event:/Radio/Const_noise");
            _constRadio.start();
            
            _searchRadio = RuntimeManager.CreateInstance("event:/Radio/Searching");
            _searchRadio.start();
            _onRadioState = true;
        }
        public void SearchRadio(float val)
        {
            if (!_onRadioState) return;
            _searchRadio.setParameterByName("Rolling the knob", Mathf.Clamp(_knobVal + val, 0, 1f));
        }

        public void RadioOff()
        {
            _searchRadio.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            _constRadio.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            RuntimeManager.CreateInstance("event:/Radio/Turn_off");
            _onRadioState = false;
            _searchRadio.setParameterByName("Rolling the knob", 0);
            _knobVal = 0f;
        }
        
        private EventInstance _ghostAttack;
        private EventInstance _focusMiss;
        
        public async void GhostAttack()
        {
            _ghostAttack.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            _ghostAttack = RuntimeManager.CreateInstance("event:/Ghost/Ghost Attack");
            _ghostAttack.start();
            await UniTask.Delay(1000);
            _ghostAttack.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        }
        public void KillGhost(int n)
        {
            var kill = RuntimeManager.CreateInstance("event:/Ghost/Killed the ghost " + n);
            kill.start();
        }

        public void Smert()
        {
            var miss = RuntimeManager.CreateInstance("event:/Player/Focus is missing");
            miss.start();
        }
        
        private EventInstance _clickMenu;
        private EventInstance _clickCard;
        
        public void ClickMenu()
        {
            _clickMenu = RuntimeManager.CreateInstance("event:/Player/Take a focus");
            _clickMenu.start();
        }
        public void ClickCard()
        {
            _clickCard = RuntimeManager.CreateInstance("event:/MainMenu/Click");
            _clickCard.start();
        }
        
        private EventInstance _final;

        public void Final()
        {
            _final = RuntimeManager.CreateInstance("event:/Player/Beam former");
            _final.start();
        }
    }
}