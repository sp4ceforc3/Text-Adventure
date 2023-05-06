using UnityEngine;
using UnityEngine.UI;   // This contains Image, Slider and the legacy Text
using TMPro;            // This contains TextMeshProUGUI, which you should use for text
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

// Define all of your states (or 'rooms') here
// None is just a default state that was added so that the previous room can be set to nothing in the beginning
public enum State {None, tavern, dungeon_entrance, dragon_lair,
                   goblin_outpost, the_crypt, crypt_chambers,
                   goblin_den, treasure_room, gluttony}

public class StateMachine : MonoBehaviour
{
    // This is a parent object that contains your entire menu panel
    [SerializeField] GameObject menu;
    [SerializeField] GameObject WalkthroughText;
    [SerializeField] GameObject mainMenuUI;
    [SerializeField] GameObject gameUI;

    // These need to be created in your Scene. storyText will be the main text element
    [SerializeField] TextMeshProUGUI storyText;

    // These need to have either a Button or Event Trigger on them
    [SerializeField] TextMeshProUGUI choiceAText;
    [SerializeField] TextMeshProUGUI choiceBText;
    [SerializeField] TextMeshProUGUI choiceCText;
    [SerializeField] TextMeshProUGUI title;
    [SerializeField] TextMeshProUGUI startButton;
    
    [SerializeField] AudioSource bgmSrc;    
    [SerializeField] AudioSource sfxSrc; 
    [SerializeField] AudioMixer mixer;
    [SerializeField] Slider audioSlider;

    // These variables are for holding the current state, as well as the previous state
    // You might not need previousState, but it will let you check where you came from
    State currentState;
    State previousState;


    // These are the 'conditions' mentioned in the exercise
    bool hasKey = false;
    int sleepCounter = 0;

    void Start()
    {
        currentState = State.None;
        previousState = State.None; // There's no previous state yet

        DisplayState();
    }

    // This toggles the menu on or off
    public void ShowMenu() => menu.SetActive(!menu.activeSelf);
    public void ShowWalkthrough() => WalkthroughText.SetActive(!WalkthroughText.activeSelf);
    public void changeVolume() => mixer.SetFloat("Master", audioSlider.value);
    
    public void backToMainClick(){
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    public void startClick(){
        sfxSrc.PlayOneShot(sfxSrc.clip);
        gameUI.SetActive(true);
        mainMenuUI.SetActive(false);
        currentState = State.tavern;
    }

    // This function is just for changing the texts, images and music. The logic for our state machine is in SelectChoice()
    // You can show different texts in the same state using the condition and previousState variables
    void DisplayState()
    {
        switch (currentState)
        {
            case State.tavern:
                storyText.text = previousState == State.tavern ? $"You sleep more, total sleep: {sleepCounter}" : "In bedroom";
                choiceAText.text = "Go to house";
                choiceBText.text = "Sleep more";
                choiceCText.text = "";
                break;
            case State.dungeon_entrance:
                storyText.text = hasKey ? "In House, you have the key" : "In House, need key";
                choiceAText.text = "Back to bed";
                choiceBText.text = "Go outside";
                choiceCText.text = hasKey ? "" : "Collect key";
                break;
            case State.dragon_lair:
                storyText.text = "Outside";
                choiceAText.text = "Back to title";
                choiceBText.text = "";
                choiceCText.text = "";
                break;
            default:
                break;
        }

        // These statements deactivate all choice texts that aren't containing any texts
        // If you just set the text to be empty, you might still be able to click on the UI element and trigger the event
        choiceBText.gameObject.SetActive(choiceBText.text != "");
        choiceCText.gameObject.SetActive(choiceCText.text != "");
    }

    // This function contains the actual logic for our state machine
    // The choice parameter is given by the OnClick() or OnPointerDown() event found on the Button / Event Trigger component
    public void SelectChoice(int choice)
    {
        previousState = currentState;
        sfxSrc.PlayOneShot(sfxSrc.clip);

        switch (currentState)
        {
            case State.tavern:
                if (choice == 1) currentState = State.tavern;
                else if (choice == 2)
                {
                    sleepCounter++;
                    currentState = State.tavern;   // You don't really need to set the same state again, but it helps to keep an overview
                }
                break;
            case State.dungeon_entrance:
                if (choice == 1) currentState = State.tavern;
                else if (choice == 2)
                {
                    if (hasKey) currentState = State.dragon_lair;
                    else currentState = State.tavern;
                }
                else if (choice == 3)
                {
                    hasKey = true;
                    currentState = State.tavern;
                }
                break;
            case State.dragon_lair:
                if (choice == 1)
                {
                    hasKey = false;
                    sleepCounter = 0;
                    currentState = State.tavern;
                }
                break;
            default:
                break;
        }

        DisplayState();
    }
}