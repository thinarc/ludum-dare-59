using Cysharp.Threading.Tasks;
using FMOD.Studio;
using FMODUnity;

namespace _Project.Scripts.Game.Sound
{
    public class MusicLoop
    {
        string[] tracks =
        {
            "event:/MainMenu/Main_theme",
            "event:/Music/Chance",
            "event:/Music/Locked",
            "event:/Music/Signal in the noise",
            "event:/Music/Chance",
            "event:/Music/Locked",
            "event:/Music/Signal in the noise",
            "event:/Music/Chance",
            "event:/Music/Locked",
            "event:/Music/Signal in the noise",
            "event:/Music/Chance",
            "event:/Music/Locked",
            "event:/Music/Signal in the noise",
            "event:/Music/Chance",
            "event:/Music/Locked",
            "event:/Music/Signal in the noise",
            "event:/Music/Chance",
            "event:/Music/Locked",
            "event:/Music/Signal in the noise",
            "event:/Music/Chance",
            "event:/Music/Locked",
            "event:/Music/Signal in the noise",
            "event:/Music/Chance",
            "event:/Music/Locked",
            "event:/Music/Signal in the noise",
            "event:/Music/Chance",
            "event:/Music/Locked",
            "event:/Music/Signal in the noise",
            "event:/Music/Chance",
            "event:/Music/Locked",
            "event:/Music/Signal in the noise",
            "event:/Music/Chance",
            "event:/Music/Locked",
            "event:/Music/Signal in the noise",
            "event:/Music/Chance",
            "event:/Music/Locked",
            "event:/Music/Signal in the noise"
        };
        int currentIndex;
        EventInstance instance;

        public async void Initialize()
        {
            while (true)
            {
                while (!RuntimeManager.HaveAllBanksLoaded)
                    await UniTask.Yield();
                
                var inst = RuntimeManager.CreateInstance(tracks[currentIndex]);

                inst.getDescription(out var desc);
                desc.getLength(out int lengthMs);

                inst.start();

                await UniTask.Delay(lengthMs);

                inst.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
                inst.release();
                await UniTask.Delay(2000);

                currentIndex = (currentIndex + 1) % tracks.Length;
            }
        }
    }
}