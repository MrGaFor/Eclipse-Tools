using UnityEngine;

namespace EC.Settings
{
    public static class SettingsSystem
    {
        public static void Init()
        {
            Bus.BusSystem.Set<bool>("Music", PlayerPrefs.GetInt("Music", 1) == 1 ? true : false);
            Bus.BusSystem.Set<float>("MusicVolume", PlayerPrefs.GetFloat("MusicVolume", 1f));
            Bus.BusSystem.Set<bool>("Sound", PlayerPrefs.GetInt("Sound", 1) == 1 ? true : false);
            Bus.BusSystem.Set<float>("SoundVolume", PlayerPrefs.GetFloat("SoundVolume", 1f));
            Bus.BusSystem.Set<bool>("Vibration", PlayerPrefs.GetInt("Vibration", 1) == 1 ? true : false);
            Bus.BusSystem.Set<bool>("Notification", PlayerPrefs.GetInt("Notification", 1) == 1 ? true : false);
            Bus.BusSystem.Set<bool>("PostProcess", PlayerPrefs.GetInt("PostProcess", 1) == 1 ? true : false);
            Bus.BusSystem.Set<int>("Language", PlayerPrefs.GetInt("Language", 0));

            Bus.BusSystem.Subscribe<bool>("Music", ChangeMusic);
            Bus.BusSystem.Subscribe<float>("MusicVolume", ChangeMusicVolume);
            Bus.BusSystem.Subscribe<bool>("Sound", ChangeSound);
            Bus.BusSystem.Subscribe<float>("SoundVolume", ChangeSoundVolume);
            Bus.BusSystem.Subscribe<bool>("Vibration", ChangeVibration);
            Bus.BusSystem.Subscribe<bool>("Notification", ChangeNotification);
            Bus.BusSystem.Subscribe<bool>("PostProcess", ChangePostProcess);
            Bus.BusSystem.Subscribe<int>("Language", ChangeLanguage);
        }

        public static void ChangeMusic(bool newState)
        {
            PlayerPrefs.SetInt("Music", newState ? 1 : 0);
            PlayerPrefs.Save();
        }
        public static void ChangeMusicVolume(float newVolume)
        {
            PlayerPrefs.SetFloat("MusicVolume", newVolume);
            PlayerPrefs.Save();
        }
        public static void ChangeSound(bool newState)
        {
            PlayerPrefs.SetInt("Sound", newState ? 1 : 0);
            PlayerPrefs.Save();
        }
        public static void ChangeSoundVolume(float newVolume)
        {
            PlayerPrefs.SetFloat("SoundVolume", newVolume);
            PlayerPrefs.Save();
        }
        public static void ChangeVibration(bool newState)
        {
            PlayerPrefs.SetInt("Vibration", newState ? 1 : 0);
            PlayerPrefs.Save();
        }
        public static void ChangeNotification(bool newState)
        {
            PlayerPrefs.SetInt("Notification", newState ? 1 : 0);
            PlayerPrefs.Save();
        }
        public static void ChangePostProcess(bool newState)
        {
            PlayerPrefs.SetInt("PostProcess", newState ? 1 : 0);
            PlayerPrefs.Save();
        }
        public static void ChangeLanguage(int newValue)
        {
            PlayerPrefs.SetInt("Language", newValue);
            PlayerPrefs.Save();
        }
    }
}