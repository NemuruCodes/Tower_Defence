using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;


    private string Settings = "SettingsMenu";
    private string mainMenu = "MainMenu";

    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject Hud;
    [SerializeField] private GameObject gameWonMenu;
    [SerializeField] private GameObject gameLostMenu;



    private bool isPaused = false;


    //[SerializeField] private InputHandle input;

    void Update()
    {
        bool PausePressed = Input.GetKeyDown(KeyCode.Escape);

        if (PausePressed)
        {
            PauseMenuControle();
        }
    }


    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

   

    

  //  private void OnEnable()
   // {
    //    pauseAction.action.performed += OnPausePerformed;
    //    pauseAction.action.Enable();


   // }

   // private void OnDisable()
   // {
     //   pauseAction.action.performed -= OnPausePerformed;
      //  pauseAction.action.Disable();


   // }

   // private void OnPausePerformed(InputAction.CallbackContext context)
   // {
   //     PauseMenuControle();
   // }


    public void FreezeGame()
    {
        Time.timeScale = 0f;
    }

    public void UnFreezeGame()
    {
        Time.timeScale = 1f;
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        Time.timeScale = 1;

        Debug.Log("Restart");
    }

    public void MainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(mainMenu);
    }


    public void SettingsLoad()
    {
        SceneManager.LoadScene(Settings, LoadSceneMode.Additive);
    }

    public void CloseSettings()
    {
        SceneManager.UnloadSceneAsync(Settings);
    }

    public void PauseMenuControle()
    {
        if (isPaused == false)
        {
            pauseMenu.SetActive(true);
            Hud.SetActive(false);
            isPaused = true;
            FreezeGame();

            //Cursor.lockState = CursorLockMode.None;
            //Cursor.visible = true;
        }

        else if (isPaused == true)
        {
            pauseMenu.SetActive(false);
            Hud.SetActive(true);
            isPaused = false;
            UnFreezeGame();

            //Cursor.lockState = CursorLockMode.Locked;
            //Cursor.visible = false;
        }
    }

    public void WonGame()
    {
        gameWonMenu.SetActive(true);
        Hud.SetActive(false);
        FreezeGame();
    }

    public void LostGame()
    {
        gameLostMenu.SetActive(true);
        Hud.SetActive(false);
        FreezeGame();
    }

}
