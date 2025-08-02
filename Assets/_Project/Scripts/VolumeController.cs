using UnityEngine;
using UnityEngine.UI;

public class VolumeController : MonoBehaviour
{
    public Slider volumeSlider;
    private const string VolumePrefKey = "Volume";

    private void Start()
    {
        // Получаем сохранённое значение громкости или устанавливаем значение по умолчанию (1.0)
        float savedVolume = PlayerPrefs.GetFloat(VolumePrefKey, 1f);

        // Устанавливаем громкость в AudioManager, если он существует
        if (AudioManager.Instance != null && AudioManager.Instance.bgMusic != null)
        {
            AudioManager.Instance.bgMusic.volume = savedVolume;
        }
        
        // Настраиваем слайдер, если он есть
        if (volumeSlider != null)
        {
            volumeSlider.value = savedVolume;
            volumeSlider.onValueChanged.AddListener(OnVolumeChanged);
        }
    }

    private void OnVolumeChanged(float value)
    {
        // Изменяем громкость в AudioManager
        if (AudioManager.Instance != null && AudioManager.Instance.bgMusic != null)
        {
            AudioManager.Instance.bgMusic.volume = value;
        }
        // Сохраняем новое значение громкости
        PlayerPrefs.SetFloat(VolumePrefKey, value);
        PlayerPrefs.Save();
    }

    private void OnDestroy()
    {
        if (volumeSlider != null)
        {
            volumeSlider.onValueChanged.RemoveListener(OnVolumeChanged);
        }
    }
}
