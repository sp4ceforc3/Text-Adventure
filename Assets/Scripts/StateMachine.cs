using UnityEngine;
using UnityEngine.UI;   // This contains Image, Slider and the legacy Text
using TMPro;            // This contains TextMeshProUGUI, which you should use for text
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

// Define all of your states (or 'rooms') here
// None is just a default state that was added so that the previous room can be set to nothing in the beginning
public enum State {
    None,
    tavern,
    gluttony,
    dungeon_entrance,
    dragon_lair,
    dragon_death,
    goblin_outpost,
    goblin_den,
    the_crypt,
    crypt_chambers,
    treasure_room
}

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
    
    // Audio
    [SerializeField] AudioSource bgmSrc;
    [SerializeField] AudioClip athmosphereClip;
    [SerializeField] AudioClip treasureClip;    
    [SerializeField] AudioClip figthClip;
    [SerializeField] AudioClip cryptClip;
    [SerializeField] AudioClip goblinDenClip;
    [SerializeField] AudioClip deadClip;
    [SerializeField] AudioSource sfxSrc; 
    [SerializeField] AudioMixer mixer;
    [SerializeField] Slider audioSlider;

    // These variables are for holding the current state, as well as the previous state
    // You might not need previousState, but it will let you check where you came from
    State currentState;
    State previousState;

    // These are the 'conditions' mentioned in the exercise
    bool hasKey     = false;
    bool hasTorch   = false;
    bool hasSword   = false;
    int mealCounter = 0;

    // Just some variables
    bool trySneak   = false;
    bool forceOpen  = false;
    bool visitedChampers = false;

    // called at first frame
    void Start() {
        currentState = State.None;
        previousState = State.None; // There's no previous state yet
        bgmSrc.clip = athmosphereClip;
        bgmSrc.Play();
        DisplayState();
    }

    // This toggles the menu on or off
    public void ShowMenu() => menu.SetActive(!menu.activeSelf);
    public void ShowWalkthrough() => WalkthroughText.SetActive(!WalkthroughText.activeSelf);
    public void changeVolume() => mixer.SetFloat("Master", audioSlider.value);
    
    // Reload Scene / Restart Game
    public void backToMainClick() => SceneManager.LoadScene(SceneManager.GetActiveScene().name);

    // Adjust background audio
    private void PlayBgmMusic(AudioClip clip) {
        bgmSrc.clip = clip;
        bgmSrc.Play();
    }

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
        
        // reset to atmospheric audio
        PlayBgmMusic(athmosphereClip);

        switch (currentState) 
        {    //** Tavern
            case State.tavern:
                if (previousState == State.tavern)
                    storyText.text  = $"You eat more, total meals eaten: {mealCounter}";
                else
                    storyText.text  = "You start off in a bustling tavern, surrounded by adventurers and travelers. You are low on money and have no mission right now.";
                choiceAText.text    = "Talk to the bartender";
                choiceBText.text    = "Approach a group of adventurers";
                choiceCText.text    = "Enjoy a meal";
                background.sprite   = Resources.Load<Sprite>("AI/StableDiffusion/tavern/00021-1123229113");
                break;

            //** Dungeon

            case State.dungeon_entrance:
                if (hasSword)
                    storyText.text  = "You find a shiny sword from which emanates immense power. Who leaves such a sword at an dungeon entrance?";
                else
                    storyText.text  = "You arrive at the entrance of the dungeon you were told about at the tavern. There does not seem to be anything special at the entrance.";
                choiceAText.text    = "Go inside";
                choiceBText.text    = hasSword ? "" : "Search the surroundings";
                choiceCText.text    = "";
                background.sprite   = Resources.Load<Sprite>("AI/StableDiffusion/dungeon-entrance/00004-736294144");
                break;

            //** Dragon Liar

            case State.dragon_lair:
                PlayBgmMusic(figthClip);
                storyText.text      = "You stumble upon a sleeping dragon, guarding a treasure hoard.";
                choiceAText.text    = "Try to kill the dragon";
                choiceBText.text    = "Try sneak past the dragon";
                choiceCText.text    = "";
                background.sprite   = Resources.Load<Sprite>("AI/StableDiffusion/dragons-lair/00018-665087056");
                break;

            //** Dragon Death

            case State.dragon_death:
                PlayBgmMusic(deadClip);
                if (!trySneak) {
                    storyText.text  = "You tried to fight a dragon with your bare hands. This would have been an epic story, but the dragon did not really care about fist bumps and roasted you.";
                    background.sprite = Resources.Load<Sprite>("AI/StableDiffusion/dragon_death/00002-2924447476");
                }
                else {
                    storyText.text  = "You tried to pass the dragon... But the dragon mentioned you as a nice snack at evening and ate you.";
                    background.sprite = Resources.Load<Sprite>("AI/StableDiffusion/dragon_death/00006-2218202480"); 
                }
                choiceAText.text    = "";
                choiceBText.text    = "";
                choiceCText.text    = "";                 
                break;

            //** Goblin Outpost

            case State.goblin_outpost:
                storyText.text      = "You travel through the area together with the adventurers in search for a goblin nest. Not long after your departure you find an outpost.";
                choiceAText.text    = "Attack the goblins head-on.";
                choiceBText.text    = "Sneak around and try to avoid them." ;
                choiceCText.text    = "Try to negotiate with the goblins";
                background.sprite   = Resources.Load<Sprite>("AI/StableDiffusion/goblin-outpost/00006-3029384622");
                break;
            
            //** Goblin Den

            case State.goblin_den:
                PlayBgmMusic(goblinDenClip);
                if (previousState == State.goblin_den) {
                    storyText.text      = "You forgot the gigant Titan-Goblins! Luckly they don't ate you, but take your whole group into slavery...";
                    choiceAText.text    = "";
                    choiceBText.text    = "";
                    choiceCText.text    = "";
                    background.sprite   = Resources.Load<Sprite>("AI/StableDiffusion/goblins-den/00004-1333357559");                    
                } else {
                    storyText.text      = "You found a map showing the location of the goblin's den in the outpost. After half a day you finally arrive at it";
                    choiceAText.text    = "Attack the goblins head-on.";
                    choiceBText.text    = "Smoke out the goblin's den";
                    choiceCText.text    = "";
                    background.sprite   = Resources.Load<Sprite>("AI/StableDiffusion/goblins-den/00010-2754461966");
                }
                break;

            //** Crypt

            case State.the_crypt:
                PlayBgmMusic(cryptClip);
                if (!hasTorch)
                    storyText.text = "For sparing their lives the goblins show you a crypt in which lays more gold as one can spent in a life. But maybe they fooled you.";
                else
                    storyText.text = "In search for something useful you went back to the crypt's entrance. Thanks to he torch you see something shiny on the ground... A key!";
                
                if (hasKey)
                    storyText.text = "You took the key! Looks like it would match an old door?";

                choiceAText.text   = "Go and see whats beyond the door.";               
                choiceBText.text    = visitedChampers ? "Go to chambers." : "Investigate engravings on wall.";
                choiceCText.text    = hasTorch && !hasKey ? "Take the key" : "";
                background.sprite   = Resources.Load<Sprite>("AI/StableDiffusion/crypt/00012-1040397683");
                break;

            //** Crypt Chambers

            case State.crypt_chambers:
                PlayBgmMusic(cryptClip);
                if (!hasTorch) {
                    storyText.text   = "You accidentally activated a mechanism which revealed a path downstairs. It leads you to chambers with a closed door";
                    choiceBText.text = hasKey ? "Open door" : "Take Torch";
                }
                else {
                    storyText.text   = "Nice! Now you can see everything in the dark crypts.";
                    choiceBText.text = hasKey ? "Open door" : "";
                }

                if (forceOpen)
                    storyText.text   = "You are way to weak to achieve anything this way.";

                choiceAText.text    = "Try to force open the old door";
                choiceCText.text    = "Go back";
                background.sprite   = Resources.Load<Sprite>("AI/StableDiffusion/crypts-chambers/00013-3606665709");
                break;

            //** Treasure Room

            case State.treasure_room:
                PlayBgmMusic(treasureClip);
                if (previousState == State.dragon_lair) {
                    storyText.text      = "You defeated the dragon with one blow of the sword. It's power is really questionable."
                                        + "But why should you care. The dragon has hoarded more than enough gold for 3 lives.";
                    choiceAText.text    = "";
                    choiceBText.text    = "";
                    choiceCText.text    = "";
                } else if(previousState == State.crypt_chambers) {
                    storyText.text      = "You find yourself in a room filled with gold. The goblins did not lie. There is more than enough gold for all of you!";
                    choiceAText.text    = "";
                    choiceBText.text    = "";
                    choiceCText.text    = "";
                }
                background.sprite = Resources.Load<Sprite>("AI/StableDiffusion/treasure-room/00012-2035474278");
                break;

            //** Gluttony

            case State.gluttony:
                PlayBgmMusic(deadClip);
                storyText.text = "Although low on money you ordered more and more meals. The other tavern visitors began to question how much one can eat."
                               + "And whether you have enough money to pay for it. At least the first question was answered the moment you choked on your 42nd meal and died a miserable death."
                               + "At least you did not need to pay for it!";
                choiceAText.text    = "";
                choiceBText.text    = "";
                choiceCText.text    = "";   
                background.sprite = Resources.Load<Sprite>("AI/StableDiffusion/gluttony/00000-1788898021");
                break;

            default:
                // "None" state is equal to our main menu state 
                // Fallback / Error is main menu too
                background.sprite   = Resources.Load<Sprite>("AI/StableDiffusion/main-menu/00032-903667890");
                break;
        }

        // These statements deactivate all choice texts that aren't containing any texts
        // If you just set the text to be empty, you might still be able to click on the UI element and trigger the event
        choiceAText.gameObject.transform.parent.gameObject.SetActive(choiceAText.text != "");
        choiceBText.gameObject.transform.parent.gameObject.SetActive(choiceBText.text != "");
        choiceCText.gameObject.transform.parent.gameObject.SetActive(choiceCText.text != "");
    }

    // This function contains the actual logic for our state machine
    // The choice parameter is given by the OnClick() or OnPointerDown() event found on the Button / Event Trigger component
    public void SelectChoice(int choice)
    {
        previousState = currentState;
        sfxSrc.PlayOneShot(sfxSrc.clip);

        switch (currentState)
        {   //** Tavern
            case State.tavern:
                if (choice == 0)
                    currentState = State.dungeon_entrance;
                else if (choice == 1)
                    currentState = State.goblin_outpost;
                else if (choice == 2) {
                    mealCounter++;
                    currentState = State.tavern;   // You don't really need to set the same state again, but it helps to keep an overview
                }
                if (mealCounter == 42)
                    currentState = State.gluttony;
                break;

            //** Dungeon
            
            case State.dungeon_entrance:
                if (choice == 0)
                    currentState = State.dragon_lair;
                else if (choice == 1) {
                    currentState = State.dungeon_entrance;
                    hasSword = true;
                }
                break;

            //** Dragon Liar

            case State.dragon_lair:
                if (choice == 0) {
                    if(hasSword)
                        currentState = State.treasure_room;
                    else
                        currentState = State.dragon_death;
                } else if (choice == 1) {
                    trySneak = true;
                    currentState = State.dragon_death;
                }
                break;

            //** Goblin Outpost

            case State.goblin_outpost:
                if (choice == 0)
                    currentState = State.goblin_den;
                else if(choice == 1)
                    currentState = State.dragon_lair;
                else if(choice == 2)
                    currentState = State.the_crypt;
                break;

            //** Crypt

            case State.the_crypt:
                if (choice == 0)
                    currentState = State.dragon_lair;
                else if(choice == 1)
                    currentState = State.crypt_chambers;
                else if (choice == 2) {
                    currentState = State.the_crypt;
                    hasKey = true;
                }
                break;

            //** Crypt Chambers

            case State.crypt_chambers:
                visitedChampers = true;
                if (choice == 0) {
                    currentState = State.crypt_chambers;
                    forceOpen = true;
                }
                else if (choice == 1) {
                    if (hasKey)
                        currentState = State.treasure_room;
                    else {
                        hasTorch = true;
                        currentState = State.crypt_chambers;
                    }    
                    forceOpen = false;                    
                } else if (choice == 2) {
                    currentState = State.the_crypt;
                    forceOpen = false;
                }
                break;

            default:
                break;
        }

        DisplayState();
    }
}