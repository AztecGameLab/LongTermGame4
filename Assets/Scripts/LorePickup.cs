using UnityEngine;

public class LorePickup : Interactable
{
    [TextArea(5, 20)] public string text;
    public Sprite backgroundImage;
    [SerializeField] private Sound pickupSound = default;
    
    private AudioManager _audioManager;
    private SoundInstance _pickupSound;
    
    private void Start()
    {
        _audioManager = AudioManager.Instance();
        _pickupSound = pickupSound.GenerateInstance();
    }

    protected override void OnInteract(Transform userTransform)
    {
        _audioManager.PlaySound(_pickupSound);
        PlayerManager.instance.DisplayLore(text, backgroundImage);
    }
}