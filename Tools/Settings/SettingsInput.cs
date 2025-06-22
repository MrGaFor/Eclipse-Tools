using Sirenix.OdinInspector;
using UnityEngine;

namespace EC.Settings
{
    [HideMonoScript]
    public class SettingsInput : MonoBehaviour
    {

        public void ChangeMusic(bool newState) => Bus.BusSystem.Set<bool>("Music", newState);
        public void ChangeMusicVolume(float newVolume) => Bus.BusSystem.Set<float>("MusicVolume", newVolume);
        public void ChangeSound(bool newState) => Bus.BusSystem.Set<bool>("Sound", newState);
        public void ChangeSoundVolume(float newVolume) => Bus.BusSystem.Set<float>("SoundVolume", newVolume);
        public void ChangeVibration(bool newState) => Bus.BusSystem.Set<bool>("Vibration", newState);
        public void ChangeNotification(bool newState) => Bus.BusSystem.Set<bool>("Notification", newState);
        public void ChangePostProcess(bool newState) => Bus.BusSystem.Set<bool>("PostProcess", newState);
        public void ChangeLanguage(int newValue) => Bus.BusSystem.Set<int>("Language", newValue);
    }
}