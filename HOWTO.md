
# Haru Game Collection - How To

## 1. Project Setup

The project can be found at [https://github.com/SvenWalter201/HaruGameCollection]. You can pull it from there using git and then follow the instructions of the README.md file. It also should be noted that a lot of the content of the project cannot be used without the Azure Kinect Camera. You can still run the application but any features requiring Motion Tracking will just not do anything.

The project was built in Unity version 2020.3.15. This version can be downloaded here: https://unity3d.com/de/get-unity/download/archive

## 2. Code Structure

### 2.1. GameController and Game

As the project-name implies, multiple games are already implemented and we created interfaces to make it easier to implement new games.

Below you find the classes `GameController` and `Game`. 

The `GameController` has the ability to launch and unload games. This is done additively (the scene from which the game is started doesn't get unloaded while the game is active), which has the advantage, that you can easily return to it and also have objects exist outside of the game, like the Virtual World background, that can be seen while playing the game.

The abstract `Game` class defines three Coroutines (`Play()`, `Initialize()` and `Execute()`). If you are unfamiliar with coroutines, here is Unity's documentation on them https://docs.unity3d.com/Manual/Coroutines.html. In essence what they allow you to do, is execute a method over an extended period of time, waiting for various things like UserInput in the meantime and then continuing once a condition is met.

`GameController` calls `StartGame()` on the current game, which in turn starts `Play()`, which then executes `Initialize()` and `Execute()` one after another. The latter two methods can be overwritten in your concrete game implementation and all of what the game does during it's execution should be put inside of these methods. All the stuff, that happens at the beginning in `Initialize()` and the rest in `Execute()`. (you can look at how the existing game classes look, if you are confused how this looks in practice)

At the end of `Play()` the event `OnGameFinished()` gets called and this allows `GameController` to know when the execution of the game is finished, so that it can unload the scene again.


```csharp
public class GameController : Singleton<GameController>
{
    Game currentGame;

    int loadedLevelBuildIndex = 0;

    public void StartGame(int index)
    {
        StartCoroutine(LoadLevel(index));
    }

    IEnumerator LoadLevel(int levelBuildIndex)
    {
        yield return SceneManager.LoadSceneAsync(levelBuildIndex, LoadSceneMode.Additive);

        FlipMainScene();

        if (AppManager.useVirtualWorld)
            VirtualWorldController.Instance.FlipMainSceneControl();

        SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex(levelBuildIndex));

        loadedLevelBuildIndex = levelBuildIndex;

        currentGame = FindObjectOfType<Game>();
        currentGame.OnGameFinished += OnCurrentGameFinished;
    }

    public void OnCurrentGameFinished(object sender, EventArgs e) => StartCoroutine(GameFinished());

    IEnumerator GameFinished()
    {
        yield return SceneManager.UnloadSceneAsync(loadedLevelBuildIndex);

        FlipMainScene();

        if (AppManager.useVirtualWorld)
            VirtualWorldController.Instance.FlipMainSceneControl();

        loadedLevelBuildIndex = 0;
    }

    void FlipMainScene()
    {
        //toggle various components of the main scene on and off (e.g. UI)
    }
```

```csharp
public abstract class Game : MonoBehaviour
{
    protected bool useAppConfig = true, isExecuting = false;

    public bool IsExecuting => isExecuting;

    bool paused = false;

    public void PlayGame()
    {
        StartCoroutine(Play());
    }

    public void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
            Finish();

        if (Input.GetKeyDown(KeyCode.Space))
        {
            paused = !paused;
            Time.timeScale = (paused) ? 0f : 1f;
        }
    }

    IEnumerator Play()
    {
        yield return new WaitForEndOfFrame();

        if (useAppConfig)
            ConfigSetup();

        yield return Init();

        yield return Execute();

        Finish();
    }

    protected virtual void ConfigSetup() {}

    protected virtual IEnumerator Init()
    {
        yield break;
    }

    protected virtual IEnumerator Execute()
    {
        yield break;
    }

    public void Finish() => OnGameFinished?.Invoke(this, null);

    public event EventHandler<EventArgs> OnGameFinished;
}
```

### 2.2. Virtual World Stuff

Since the Game Scenes will be loaded additively into the virtual world, all GameObjects of the Virtual World still exist while playing the games. This means that checking for Input in the Virtual World with the conventional method (just checking for Input in Update) that Input would be carried out while playing the game, which generally is undesirable. This is why all Input in the Virtual World should be put into a designated class that extends `InputSource`, which automatically registeres at the `VirtualWorldController` and doesn't check for Input while a Game is currently active.

```csharp
public class ExitScene : InputSource
{
    public override void TestForInput()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            //do stuff
        }
    }
}
```

### 2.3. String Resources

Since this project had potentially users in many different languages (English, German, Japanese, Spanish), a designated json-file exists, where all of the string-resources are located. One entry in this long list looks like this:

```json
  "_WelcomeText": {
    "EN": "Welcome",
    "DE": "Willkommen"
  }
```

If you want to add new UI Elements, the text inside of them should be set like this,

```csharp
answerFeedback.text = StringRes.Get("_RightAnswer");
```
provided you added the entry `"_RightAnswer"` in the StringResources.json file (found inside the Resources folder)

### 2.4. Build Indizes

A lot of logic is dependant on *Build Indizes*. A Build Index is basically an Integer, that is unique per scene. In Unity, if you go to File/Build Settings you can press `Add Open Scenes` and your scene will be added to the list. The assigned index can be seen on the right. It is probably a good idea to only ever add scenes to the bottom of the list, so that the indizes for existing scenes will remain the same.

### 2.5. Implementing a Game in the Virtual World

It is desirable, that all games can be run in the Virtual World and in a Standalone Scene. This is why in their implementation of the Game class an `if-block` as the one below is implemented (The example is from `MotionMemoryHouse.cs`). Basically the script holds referenced for the camera and light of the standalone-Scene and disables them if the Virtual World should be used, since in that case light and camera of the Virtual World are used. Additionally some Game Objects get deactivated, in this example the grass and the house itself get deactivated, since those exist in the Virtual World as well and will be used instead.

The last block of code is responsible for providing a smooth camera transition from where ever the player is currently in the Virtual World, to where ever the "Game Canvas", (in this case the house) is located. The `readonly` position and rotation are the position and rotation of the camera in the standalone scene, relative to the house position/rotation in the scene. We can add the positions of the house in the Virtual World with `cameraPositionOffset` and multiply its rotation with the `cameraRotation`, to get a correct camera transformation in the Virtual World.

If this is confusing to you, please check out the already implemented games in their standalone scenes and in the Virtual World.

```csharp
    readonly Vector3 cameraPositionOffset = new Vector3(0.3f, 8.8f, 13f);
    readonly Quaternion cameraRotation = Quaternion.Euler(17f, 180f, 0f);
    
    void Start()
    {
        PlayGame();
    }

    protected override IEnumerator Init()
    {
        if (AppManager.useVirtualWorld)
        {
            Camera virtualWorldCam = GameController.Instance.mainSceneCamera;
            mainLight.enabled = false;
            gameCamera.enabled = false;

            windowController.gameObject.SetActive(false);
            sceneryGO.SetActive(false);
            windowController = VirtualWorldController.Instance.windowController;
            Transform house = windowController.transform;

            Vector3 positionOffset = house.right * cameraPositionOffset.x + house.up *    cameraPositionOffset.y + house.forward * cameraPositionOffset.z;
            yield return StartCoroutine(Tween.TweenPositionAndRotation(virtualWorldCam.transform, house.position + positionOffset, house.rotation * cameraRotation, 3f));
        }

```

## 3. Building the project

If you want to build the project, you also need to follow the instructions of the `README.md` file at the root of the Git-Repository. Also, once you start the application, you will see a folder called `EditableResources`. In this folder, there is an `AppConfig.json`, which you can use to manually configure parameters of the games and Kinect setup. (Alternatively you can use the GUI, provided in the Options Menu)

## 4. Expanding the existing games

### 4.1. Trivia Quiz

The currently used Questions can be found under ***ScriptableObjects/QuestionCatalogueThisOrThat***. 
You could extend this catalogue by adding more QuestionCards similar to the ones already existing or create a new Question Catalogue from scratch (create new Object of type ScriptableObjects/QuestionCatalogue or duplicate the existing one with ctrl. + d). 

### 4.2. Motion Memory

Motions can be added using the Scene Kinect Motion Capture Studio. To do so, enter the scene, press `StartBodyTracking`, then press `Capture`, perform your motion and press `Capture` again afterwards. Then you can view your recorded motion by switching from `DisplayTracked` to `DisplayLoaded` to review your pose. You can now either save the entire recorded motion or just one frame of the motion by typing a name in the `filename` field and pressing the respective save button. 

Additionally you can add *LimbConstraints* to your Pose/Motion. Basically any bodypart, that is not really relevant for the recorded pose. (This could allow people with disabilities to play games like Motion Memory, since you could only use motions where certain parts of the body are not relevant and will be ignored by the JointCompare method). Currently these Constraints don't change anything, but in the future could be respected.

The Motions currently get saved in the local game files, which you can navigate to by pressing Windows + r, searching for %appdata%, then navigating up one folder and going to ***/LocalLow/HondaResearchInstitute/HaruGameCollection***. There also exist some default poses, which the Motion Memory currently uses. These are stored in ***Resources/DefaultPoses***.

### 4.3. Duplik
**Adding new textures and 3D models:** To make the game easily expandable it was important to write the code in a way that makes it possible for designers with no programming background to enlarge the vocabulary and hand in hand with that the corresponding graphical elements. For now you can add new 3D objects, poses, new textures to dress the character or add emotions and new colourpalettes for the animals. 

A new 3D object is added in two steps 
1. Adding a new ***subject*** object in the `MasterData.json` (can be found in ***Assets/Resources***): 
- Depending on what object you are adding it is important to define the field *type* as either "person", "thing" or "animal". 
- The *name* field will be the vocabulary that will be shown in the sentence and for the grammar generation it is important to define *gender* as "m","f" or "n".
- The *model* is where the generator tries to find the 3D model.
- *rotatable* and *scalable* require booleans as strings of "true" and "false"
- *positions* holds an array of strings at which positions the subject is allowed to spawn. All possible positions are *"links","vorne links","rechts","vorne rechts","im Hintergrund","hinten links","hinten rechts","unten" and "im Vordergund"*. 


2. Adding the 3D object into to project structure:
- The prefab of the .fbx model file has to be placed in the folder ***Resources/Prefabs*** and needs to have the same name as the string in the *model* field. In case the subject is a person and has assets like hair or a hat, the prefab has to be placed at the same location as the subject and the corresponding string in the *assets* field. 



Adding new colours, actions or moods require two steps as well. The subject just holds a string array with the keywords where the colour, mood or action can be found. Colour, action and mood are seperate objects in the .json file. To add a new colour two things need to be done. 

1. Add a new json object in the `MasterData.json` file with correct name,type and translation. 
2. Add the colourpalette, which is a 2x3 pixel atlas into ***Resources/Textures/ColorPalettes*** with the correct name, precisely the name of the colour in the json file. The 3D animal which is supposed to be coloured must be UV mapped correctly (a tutorial on how to UV-Color low poly objects can be found here: https://youtu.be/1jHUY3qoBu8?t=132). 

Actions and animations need to be added into the json as well. Additionally the field *animation* needs to hold the same string as the file in ***Resources/Animations***.

To add new moods the texture mapped on the head needs to be placed in ***Resources/Textures/Facial*** and the name in the new mood object needs to be corresponding.





