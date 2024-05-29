using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public PlayerController tank1Controller;
    public PlayerController tank2Controller;
    public TankShootController tank1ShootController;
    public TankShootController tank2ShootController;
    public TankHealth tank1Health;
    public TankHealth tank2Health;
    public TextMeshProUGUI turnText;
    public AudioClip victoryMusic;
    public Button turnButton;
    public Canvas gameCanvas;
    public Canvas victoryCanvas; // Canvas de victoria
    public TextMeshProUGUI victoryText; // Texto de victoria
    private AudioSource audioSource;

    private bool isTank1Turn = true;
    private bool holdingButton = false;

    private bool SeguirJugando = false;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        tank1Health.OnTankDestroyed += HandleTankDestroyed;
        tank2Health.OnTankDestroyed += HandleTankDestroyed;

        StartTurn();
        if (turnButton != null)
        {
            turnButton.onClick.AddListener(ChangeTurnOnClick);
        }

        // Asegúrate de que el Canvas de victoria esté desactivado al inicio
        if (victoryCanvas != null)
        {
            victoryCanvas.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        if (!SeguirJugando)
        {
            if (holdingButton && Input.GetMouseButtonUp(0))
            {
                holdingButton = false;
                EndTurn();
            }
        }
    }

    private void StartTurn()
    {
        if (!SeguirJugando)
        {
            if (turnText != null)
            {
                turnText.gameObject.SetActive(true);
                turnText.text = isTank1Turn ? "Turno de Tanque 1" : "Turno de Tanque 2";
            }

            EnableTank(isTank1Turn ? tank1Controller : tank2Controller, isTank1Turn ? tank1ShootController : tank2ShootController);
            DisableTank(isTank1Turn ? tank2Controller : tank1Controller, isTank1Turn ? tank2ShootController : tank1ShootController);
        }
    }

    private void EnableTank(PlayerController controller, TankShootController shootController)
    {
        controller.enabled = true;
        shootController.enabled = true;
        shootController.OnShoot += OnShoot;
    }

    private void DisableTank(PlayerController controller, TankShootController shootController)
    {
        controller.enabled = false;
        shootController.enabled = false;
        shootController.OnShoot -= OnShoot;
    }

    private void EndTurn()
    {
        if (isTank1Turn)
        {
            DisableTank(tank1Controller, tank1ShootController);
        }
        else
        {
            DisableTank(tank2Controller, tank2ShootController);
        }

        isTank1Turn = !isTank1Turn;
        StartTurn();
    }

    private void OnShoot()
    {
        holdingButton = true;
    }

    private void HandleTankDestroyed(GameObject destroyedTank)
    {
        SeguirJugando = true;
        string winner = destroyedTank == tank1Controller.gameObject ? "Tanque 2" : "Tanque 1";
        turnText.text = winner + " ha ganado!";

        gameCanvas.gameObject.SetActive(false);
        DisableTank(tank1Controller, tank1ShootController);
        DisableTank(tank2Controller, tank2ShootController);

        if (audioSource != null && victoryMusic != null)
        {
            audioSource.PlayOneShot(victoryMusic);
        }

        // Mostrar el Canvas de victoria y actualizar el texto
        if (victoryCanvas != null && victoryText != null)
        {
            victoryCanvas.gameObject.SetActive(true);
            victoryText.text = winner + " Winner!";
        }
    }

    public void ChangeTurnOnClick()
    {
        if (!SeguirJugando)
        {
            if (!holdingButton)
            {
                EndTurn();
            }
        }
    }
}
