using UnityEngine;
using TMPro;
using Deforestation.Recolectables;
using System;
using Deforestation.Interaction;
using UnityEngine.UI;
using UnityEngine.Audio;

namespace Deforestation.UI
{
	public class UIGameController : MonoBehaviour
	{
		#region Properties
		#endregion

		#region Fields
		private Inventory _inventory => GameController.Instance.Inventory;		
		private InteractionSystem _interactionSystem => GameController.Instance.InteractionSystem;

		[Header("Settings")]
		[SerializeField] private AudioMixer _mixer;
		[SerializeField] private Button _settingsButton;
		[SerializeField] private GameObject _settingsPanel;
		[SerializeField] private Slider _musicSlider;
		[SerializeField] private Slider _fxSlider;
        [SerializeField] private Slider _sensitivitySlider;
        [SerializeField] private Toggle _fullscreenToggle;

        [Header("Inventory")]
		[SerializeField] private TextMeshProUGUI _crystal1Text;
		[SerializeField] private TextMeshProUGUI _crystal2Text;
        [SerializeField] private TextMeshProUGUI _crystal3Text;
        [Header("Interacytion")]
		[SerializeField] private InteractionPanel _interactionPanel;
		[Header("Live")]
		[SerializeField] private Slider _machineSlider;
		[SerializeField] private Slider _playerSlider;

		private bool _settingsOn = false;
		#endregion

		#region Unity Callbacks
		// Start is called before the first frame update
		void Start()
		{
			_settingsPanel.SetActive(false);

			//My Events
			_inventory.OnInventoryUpdated += UpdateUIInventory;
			_interactionSystem.OnShowInteraction += ShowInteraction;
			_interactionSystem.OnHideInteraction += HideInteraction;
			//Settings events
			_settingsButton.onClick.AddListener(SwitchSettings);
			_musicSlider.onValueChanged.AddListener(MusicVolumeChange);
			_fxSlider.onValueChanged.AddListener(FXVolumeChange);
            _sensitivitySlider.onValueChanged.AddListener(SensitivityChange);
            _fullscreenToggle.onValueChanged.AddListener(ToggleFullscreen);

            LoadSettings();
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                SwitchSettings();
            }
        }

        private void SwitchSettings()
        {
            _settingsOn = !_settingsOn;
            _settingsPanel.SetActive(_settingsOn);

            if (_settingsOn)
            {
                // Pausar juego
                Time.timeScale = 0f;

                // Activar ratón
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            else
            {
                // Reanudar juego
                Time.timeScale = 1f;

                // Bloquear ratón para FPS
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }

        internal void UpdateMachineHealth(float value)
		{
			_machineSlider.value = value;
		}

		internal void UpdatePlayerHealth(float value)
		{
			_playerSlider.value = value;
		}

		#endregion

		#region Public Methods
		public void ShowInteraction(string message)
		{
			_interactionPanel.Show(message);
		}
		public void HideInteraction()
		{
			_interactionPanel.Hide();

		}

        #endregion

        #region Private Methods
        private void LoadSettings()
        {
            float music = PlayerPrefs.GetFloat("MusicVolume", 1f);
            float fx = PlayerPrefs.GetFloat("FXVolume", 1f);
            float sensitivity = PlayerPrefs.GetFloat("MouseSensitivity", 1f);
            int fullscreen = PlayerPrefs.GetInt("Fullscreen", 1);


            _musicSlider.value = music;
            _fxSlider.value = fx;
            _sensitivitySlider.value = sensitivity;
            _fullscreenToggle.isOn = fullscreen == 1;

            MusicVolumeChange(music);
            FXVolumeChange(fx);
            SensitivityChange(sensitivity);
            ToggleFullscreen(fullscreen == 1);

            StarterAssets.FirstPersonController player =
               FindObjectOfType<StarterAssets.FirstPersonController>();

            if (player != null)
            {
                player.RotationSpeed = sensitivity;
            }
        }
        private void UpdateUIInventory()
		{
			if (_inventory.InventoryStack.ContainsKey(RecolectableType.SuperCrystal))
				_crystal1Text.text = _inventory.InventoryStack[RecolectableType.SuperCrystal].ToString();
			else
				_crystal1Text.text = "0";
			if (_inventory.InventoryStack.ContainsKey(RecolectableType.HyperCrystal))
				_crystal2Text.text = _inventory.InventoryStack[RecolectableType.HyperCrystal].ToString();
			else
				_crystal2Text.text = "0";
            if (_inventory.InventoryStack.ContainsKey(RecolectableType.JumpCrystal))
                _crystal3Text.text = _inventory.InventoryStack[RecolectableType.JumpCrystal].ToString();
            else
                _crystal3Text.text = "0";
        }

        private void FXVolumeChange(float value)
        {
            _mixer.SetFloat("FXVolume", Mathf.Lerp(-60f, 0f, value));
            PlayerPrefs.SetFloat("FXVolume", value);
        }

        private void MusicVolumeChange(float value)
        {
            _mixer.SetFloat("MusicVolume", Mathf.Lerp(-60f, 0f, value));
            PlayerPrefs.SetFloat("MusicVolume", value);
        }

        private void SensitivityChange(float value)
        {
            PlayerPrefs.SetFloat("MouseSensitivity", value);

            StarterAssets.FirstPersonController player =
                FindObjectOfType<StarterAssets.FirstPersonController>();

            if (player != null)
            {
                player.RotationSpeed = value;
            }
        }

        private void ToggleFullscreen(bool value)
        {
            Screen.fullScreen = value;

            PlayerPrefs.SetInt("Fullscreen", value ? 1 : 0);
        }
        #endregion
    }

}