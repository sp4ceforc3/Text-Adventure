using UnityEngine;
using UnityEngine.UI;   // This contains Image, Slider and the legacy Text
using TMPro;            // This contains TextMeshProUGUI, which you should use for text
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

// Define all of your states (or 'rooms') here
// None is just a default state that was added so that the previous room can be set to nothing in the beginning
public enum State {None, tavern, dungeon_entrance, dragon_lair,
                   goblin_outpost, the_crypt, crypt_chambers,
                   goblin_den, treasure_room, gluttony, dragon_death}

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

    // Background Sprite
    [SerializeField] SpriteRenderer background;
    
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
    bool hasTorch = false;
    bool hasSword = false;
    int mealCounter = 0;

    // called at first frame
    void Start() {
        currentState = State.None;
        previousState = State.None; // There's no previous state yet
        DisplayState();
    }

    // This toggles the menu on or off
    public void ShowMenu() => menu.SetActive(!menu.activeSelf);
    public void ShowWalkthrough() => WalkthroughText.SetActive(!WalkthroughText.activeSelf);
    public void changeVolume() => mixer.SetFloat("Master", audioSlider.value);
    
    // Reload Scene / Restart Game
    public void backToMainClick() => SceneManager.LoadScene(SceneManager.GetActiveScene().name);

    public void startClick() {
        sfxSrc.PlayOneShot(sfxSrc.clip);
        gameUI.SetActive(true);
        mainMenuUI.SetActive(false);
        currentState = State.tavern;
        DisplayState();
    }

    // This function is just for changing the texts, images and music. The logic for our state machine is in SelectChoice()
    // You can show different texts in the same state using the condition and previousState variables
    void DisplayState() {
        switch (currentState) {
            case State.tavern:
                if (previousState == State.tavern)
                {
                    storyText.text = $"You eat more, total meals eaten: {mealCounter}";
                }else
                {
                    storyText.text = "You start off in a bustling tavern, surrounded by adventurers and travelers."
                                     + "You are low on money and have no mission right now.";
                }
                choiceAText.text    = "Talk to the bartender";
                choiceBText.text    = "Approach a group of adventurers";
                choiceCText.text    = "Enjoy a meal";
                background.sprite   = Resources.Load<Sprite>("AI/StableDiffusion/tavern/00021-1123229113");
                
                break;
            case State.dungeon_entrance:
                if (hasSword)
                    storyText.text = "You find a shiny sword from which emanates immense power. Who leaves such a sword at an dungeon entrance?";
                else
                    storyText.text = "You arrive at the entrance of the dungeon you were told about at the tavern. There does not seem to be anything special at the entrance.";
                
                choiceAText.text    = "Go inside";
                choiceBText.text    = "Search the surroundings";
                // TODO Hide third Button
                choiceCText.text    = "";
                background.sprite   = Resources.Load<Sprite>("AI/StableDiffusion/dungeon-entrance/00004-736294144");
                
                break;
            case State.dragon_lair:
                storyText.text      = "You stumble upon a sleeping dragon, guarding a treasure hoard.";
                choiceAText.text    = "Try to kill the dragon";
                choiceBText.text    = "Try sneak past the dragon";
                choiceCText.text    = "";
                background.sprite   = Resources.Load<Sprite>("AI/StableDiffusion/dragons-lair/00018-665087056");
                
                break;
            case State.goblin_outpost:
                storyText.text      = "You travel through the area together with the adventurers in search for a goblin nest. Not long after yoout departure you find an outpost.";
                choiceAText.text    = "Attack the goblins head-on.";
                choiceBText.text    = "Sneak around and try to avoid them." ;
                choiceCText.text    = "Try to negotiate with the goblins";
                background.sprite   = Resources.Load<Sprite>("AI/StableDiffusion/goblin-outpost/00006-3029384622");

                break;
            case State.the_crypt:
                if(previousState == State.goblin_outpost)
                {
                    storyText.text = "For sparing their lives the goblins show you a crypt in which lays more gold as one can spent in a life. But maybe they fooled you.";
                } else // ToDO: differentiate when taking the key
                    storyText.text = "In search for something usefull you went back to the crypt's entrance. Thanks to he torch you see something shiny on the ground";
                
                choiceAText.text    = "TODO";
                choiceBText.text    = "Invstigate engravings on wall";
                choiceCText.text    = hasTorch ? "Take the key" : "";
                // ToDo Hide third Option if empty
                background.sprite   = Resources.Load<Sprite>("AI/StableDiffusion/crypt/00012-1040397683");

                break;
            case State.crypt_chambers:
                // TODO: differentiate between taking torch and  trying to force open
                if(previousState == State.crypt_chambers)
                {
                    storyText.text = "You are way to weak to achieve anything this way";
                }else
                    storyText.text = "You accidentlay activated a mechanism which revealed a path downstairs. It leads you to chamber with a closed door";
                choiceAText.text    = "Try to force open the door";
                choiceBText.text    = hasKey ? "Open door" : "Take torch";
                choiceCText.text    = "Go back";
                // ToDo Hide third Option if empty
                background.sprite   = Resources.Load<Sprite>("AI/StableDiffusion/crypts-chambers/00013-3606665709");

                break;
            case State.goblin_den:
                storyText.text = "You found a map showing the location of the goblin's den in the outpost. After half a day you finally arrive at it";
                choiceAText.text    = "Attack the goblins head-on.";
                choiceBText.text    = "Smoke out the goblin's den";
                // ToDo Hide Button
                choiceCText.text    = "";
                background.sprite   = Resources.Load<Sprite>("AI/StableDiffusion/goblins-den/00010-2754461966");

                break;
            case State.treasure_room:
                if (previousState == State.dragon_lair)
                {
                    storyText.text      = "You defeated the dragon with one blow of the sword. It's power is really questionable."
                                        + "But why should you care. The dragon has hoarded more than enpugh gold for 3 lives.";
                    // ToDo Hide Buttons
                    choiceAText.text    = "";
                    choiceBText.text    = "";
                    choiceCText.text    = "";
                } else if(previousState == State.crypt_chambers)
                {
                    // ToDo Hide Button
                    storyText.text      = "You find yourself in a room filled with gold. The goblins did not lie. There is more than enough gold for all of you";
                    choiceAText.text    = "";
                    choiceBText.text    = "";
                    choiceCText.text    = "";
                }
                background.sprite = Resources.Load<Sprite>("AI/StableDiffusion/treasure-room/00012-2035474278");
                break;
            case State.gluttony:
                storyText.text = "Although low on money you ordered more and more meals. The other tavern visitors began to question how much one can eat."
                            + "And whether you have enough money to pay for it. At least the first question was answered the moment you choked on you 42th meal and died a miserable death."
                            + "At least you did not need to pay for it!";
                // TODO Hide Buttons
                choiceAText.text    = "";
                choiceBText.text    = "";
                choiceCText.text    = "";   
                background.sprite = Resources.Load<Sprite>("AI/StableDiffusion/gluttony/glut_linus_1");
                break;
            case State.dragon_death:
                //TODO: differntianet with sneaking past death
                storyText.text = "You tried to fight a dragon with your bare hands. This wold have been an epic story, but the dragon did not really care about fist bumps and roasted you.";
                // TODO Hide Buttons
                choiceAText.text    = "";
                choiceBText.text    = "";
                choiceCText.text    = "";   
                background.sprite = Resources.Load<Sprite>("AI/StableDiffusion/dragon_death/");             
                break;
            default:
                // "None" state is equal to our main menu state 
                // Fallback / Error is main menu too
                background.sprite   = Resources.Load<Sprite>("AI/StableDiffusion/main-menu/00032-903667890");
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
                if (choice == 0)
                {
                    currentState = State.dungeon_entrance;
                }else if (choice == 1)
                {
                    currentState = State.goblin_outpost;
                }
                else if (choice == 2)
                {
                    mealCounter++;
                    currentState = State.tavern;   // You don't really need to set the same state again, but it helps to keep an overview
                }
                if (mealCounter == 42)
                    currentState = State.gluttony;
                break;
            case State.dungeon_entrance:
                if (choice == 0)
                    currentState = State.dragon_lair;
                else if (choice == 1)
                {
                    currentState = State.dungeon_entrance;
                    hasSword = true;
                }
                break;
            case State.dragon_lair:
                if (choice == 0)
                {
                    if(hasSword)
                    {
                        currentState = State.treasure_room;
                    } else
                        currentState = State.dragon_death;
                } else if (choice == 1)
                {
                    // TODO: different Text than fighting death in display
                    currentState = State.dragon_death;
                }
                break;
            case State.goblin_outpost:
                if (choice == 0)
                {
                    currentState = State.goblin_den;
                } else if(choice == 1)
                {
                    currentState = State.dragon_lair;
                } else if(choice == 2)
                {
                    currentState = State.the_crypt;
                }
                break;
            case State.the_crypt:
                if (choice == 0)
                {
                    currentState = State.the_crypt;
                } else if(choice == 1)
                {
                    currentState = State.crypt_chambers;
                } else if (choice == 2)
                    currentState = State.the_crypt;
                    hasKey = true;
                break;
            case State.crypt_chambers:
                if (choice == 0)
                {
                    currentState = State.crypt_chambers;
                } else if (choice == 1)
                {
                    if (hasKey)
                    {
                        currentState = State.treasure_room;
                    }else
                    {
                        hasTorch = true;
                        currentState = State.crypt_chambers;
                    }                        
                }else if (choice == 2)
                {
                    currentState = State.the_crypt;
                }
                break;
            case State.goblin_den:
                //TODO ending state for Goblin victory                
                break;
            default:
                break;
        }

        DisplayState();
    }
}