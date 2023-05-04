# Text-Adventure
 Implement a text adventure that tells a story of your own making. A package containing fonts and music can be found on the Assets tab. Finding the visual assets will be a part of the exercise. Important: As a group you will create one single story. Square brackets show the requirements for 1/2/3 group members respectively (e.g. 'Create [2/3/4] items' means that you have to create 4 items if you're in a group of three).

# 1 Creating your Story (4 Points)

    Before you begin, play the example game linked on the icon. Think of a setting for a story of your own that can be told in a similar format. If you can't think of anything, you can use ChatGPT.
    The tasks below are minimum requirements. As long as they are satisfied, you will receive your points. Apart from that you're completely free to implement and design your game however you want.

    Write your own story. There should be at least [6/8/10] different states (or 'rooms') with multiple choices.
    At least [1/2/3] states are locked behind some kind of condition (e.g. collect a key to open a door, earn money to buy food, flip a switch to activate a machine, find a gun to kill a zombie, etc.).
    There are at least two different endings. An ending is a state without any further choices.
    There is one additional secret ending. This is reached by going out of your way to do something that is not part of the main story.
# 2 Game Loop (4 Points)

    Create a main menu. It should at least contain your game's title and a button to start your game. Make sure that all UI elements in your game scale correctly with the screen size.
    Create the basic UI elements for your game. There should be one text that shows your story, multiple texts to show the player's choices and a button to open the options menu.
    Implement the main game loop as follows:
        The player is always in one state and can make choices to go to a different state. The easiest way to implement this would be a simple state machine using an enum.
        The choices are displayed as text elements. Clicking on a choice loads the next room.
    [Hard] The options button toggles a menu on or off. This menu should contain a button that brings the player back to the main menu and a second button that displays a walkthrough of your game. 
 # 3 Audio and Graphics (3 Points)

    For the following tasks you will have to find your own assets. You can either use the websites linked on the Assets tab, search the Internet, or create your own.
    If you want your game to look really nice with minimal effort, I suggest creating images with an AI like Stable Diffusion. To use SD on your computer, I recommend using the SD Web UI (click here).

    Add at least two different music tracks to your game and play a sound effect whenever the player makes a choice. You can use the files from the package or use your own.
    [Hard] Add a UI slider to the options menu that changes the audio volume and a text that shows the current volume. This setting should be persistent and not reset after going back to the main menu.
    [Hard] Add graphics to your rooms. You can decorate the UI with random images that fit your story (e.g. from kenney.nl) or generate background illustrations with AI instead. See screenshot for examples.

 # Submission and Feedback (2 Points)

    Share your game in the Moodle forum Text Adventure (B1) as a group. Your submission has to include:
        A link to a working WebGL build and the names of everyone in your group.
        Any additional information needed to complete your game (which is also important for grading). If you already explained everything in the walkthrough, you don't need to add anything to your post.
    The deadline for this exercise is May 21st. Feedback should be posted between May 22nd and June 25th.
