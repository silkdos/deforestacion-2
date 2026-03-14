using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Deforestation
{
public class GameOverManager : MonoBehaviour
    {
        [SerializeField] private GameObject gameOverUI;

        [SerializeField] private HealthSystem playerHealth;
        [SerializeField] private HealthSystem machineHealth;

        private void Start()
        {
            gameOverUI.SetActive(false);

            playerHealth.OnDeath += ShowGameOver;
            machineHealth.OnDeath += ShowGameOver;
        }

        public void ShowGameOver()
        {
            gameOverUI.SetActive(true);

            // Desbloquear cursor para usar el ratón
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            // Parar el juego
            Time.timeScale = 0f;
        }

        public void Retry()
        {
            Time.timeScale = 1f;

            Destroy(GameController.Instance.gameObject);

            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        public void QuitGame()
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene("Main Menu");
        }
    }
}