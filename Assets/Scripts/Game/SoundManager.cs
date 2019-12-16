using UnityEngine;

public class SoundManager : MonoBehaviour {

    private AudioClip[] _comboSoundList = new AudioClip[7];
    private AudioClip[] _BGMList = new AudioClip[2];
    private AudioSource _BGM;
    private AudioSource _rotationSound;
    private AudioSource _moveSound;
    private AudioSource _comboSound;

    private void Awake()
    {
        var AudioSourceList = gameObject.GetComponents<AudioSource>();
        _BGM = AudioSourceList[0];
        _moveSound = AudioSourceList[1];
        _rotationSound = AudioSourceList[2];
        _comboSound = AudioSourceList[3];

        _BGMList[0] = Resources.Load<AudioClip>("Game1");
        _BGMList[1] = Resources.Load<AudioClip>("Warn");
        for (int i = 0; i < 7; i++)
            _comboSoundList[i] = Resources.Load<AudioClip>("combo" + i);
        
    }
    /// <summary>
    /// init BGM. 배경음악 초기화
    /// </summary>
    void Start () {
        _BGM.clip = _BGMList[0];
        _BGM.Play();
    }
    /// <summary>
    /// play Sound. 사운드 플레이
    /// </summary>
    /// <param name="fx"></param>
    public void PlaySound(FX fx)
    {
        switch (fx)
        {
            case FX.BGM:
                if (GameVariable.isDanger == true && _BGM.clip.name != "Warn")
                    _BGM.clip = _BGMList[1];
                else if (_BGM.clip.name == "Warn")
                    _BGM.clip = _BGMList[0];
                break;
            case FX.Move:
                _moveSound.Play();
                break;
            case FX.Rotate:
                _rotationSound.Play();
                break;
            case FX.Combo:
                _comboSound.clip = _comboSoundList[GameVariable.currentCombo];
                _comboSound.Play();
                break;
        }
    }
    public void StopSound()
    {
        _BGM.Stop();
    }
}