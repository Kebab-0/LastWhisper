using UnityEngine;
using UnityEngine.UI;

public class ButtonSound : MonoBehaviour
{
    [Header("Настройки звука")]
    [SerializeField] private AudioClip soundClip; // Звуковой файл
    [SerializeField][Range(0f, 1f)] private float volume = 1f; // Громкость

    private AudioSource audioSource;
    private Button button;

    void Start()
    {
        // Получаем или создаем AudioSource
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        // Настраиваем AudioSource
        audioSource.playOnAwake = false;
        audioSource.clip = soundClip;
        audioSource.volume = volume;

        // Получаем компонент Button
        button = GetComponent<Button>();

        // Добавляем обработчик нажатия
        if (button != null)
        {
            button.onClick.AddListener(PlaySound);
        }
    }

    void PlaySound()
    {
        if (soundClip != null && audioSource != null)
        {
            audioSource.PlayOneShot(soundClip, volume);
        }
    }

    void OnDestroy()
    {
        // Очищаем слушатели при уничтожении объекта
        if (button != null)
        {
            button.onClick.RemoveListener(PlaySound);
        }
    }
}