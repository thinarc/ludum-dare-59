using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Project.Scripts.Player
{
    public class PlayerController : MonoBehaviour
    {
        public static PlayerController Instance { get; private set; }
        private void Awake() => Instance = this;
        
        [SerializeField] private Animator anim;
        public event Action PlayerDeath;

        public async void ToBattle()
        {
            await UniTask.Delay(200);
            anim.SetBool("battle", true);
        }
        public void ToIdle() => anim.SetBool("battle", false);

        public void SetSmert(bool s)
        {
            anim.SetBool("smert", s);
            PlayerDeath?.Invoke();
        }
        public void Pass() => anim.SetTrigger("pass");
    }
}