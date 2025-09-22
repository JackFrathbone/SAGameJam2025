using RenderHeads.Services;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoService
{
    private AudioSource _clipAudioSource;

    private void Start()
    {
        _clipAudioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (Input.GetButtonDown("Cancel"))
        {
            Application.Quit();
        }
    }

    public void PlayAudioClip(AudioClip clip, float volume = 0.5f, bool randomPitch = false, float minPitch = 0.95f, float maxPitch = 1.05f, bool loop = false)
    {
        _clipAudioSource.pitch = randomPitch ? Random.Range(minPitch, maxPitch) : 1;

        if (_clipAudioSource.loop)
        {
            _clipAudioSource.Stop();
        }

        _clipAudioSource.loop = loop;

        _clipAudioSource.PlayOneShot(clip, volume);
    }

    public void PauseGame()
    {
        Time.timeScale = 0f;
    }

    public void UnPauseGame()
    {
        Time.timeScale = 1f;
    }

    public void LoadLevel(int i)
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        SceneManager.LoadScene(i);
    }

    public void RestartLevel()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}
