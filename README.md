# SaveDataRelocator
Copy data from local to remote location and then starts and application. When the application shuts down the data will be copied back to local.

## Usage
 * Place SaveDataRelocator.exe next to the game you are running. This is crucial since this folder will implicitly be the "Local" folder.
 * drag and drop the exe onto SaveDataRelocator.exe and you will be asked to pick a directory. Find the games save data directory. See below note 
 * a new shortcut will be created for you, which you can copy to your desktop.

### Location Note:
Save data's storage locations vary from game to game, and there is no simple solution to find the folder other than Google.
If anything i'd recommend you take a look at the folders 

 * AppData/Roaming
 * AppData/Local
 * AppData/LocalLow
 * Documents/
 * Documents/My Games

search each of theses folders and check for any folder named either the game name or the company name and possible nested game name. And check inside to see if the folder actually contains save data.
 * Note that some games stores the save data and the configuration data seperately.
 * Note that Steam emulators will usually create a folder with the emulator name and the hackers name nested and then some steamgameid. Try cheking the emulator config .ini files for information
 

### Command line arguments:
[0] = Executebale to run

[1..] = Remote location to manage data (you can input many parameters)

#### Why?
A lot of games will produce garbage folders all around your file system like.
Personally i'd like to be able to manage the game folder that is: copy, move or delete. However saveData residing in a different location makes this a hassle to deal with and on copy you will most likely loose your savedata on your new system or on delete you will end up with orphaned folders. It's quite some work for something simple, but if you're like me and running DeepFreeze for resilience, having junk in AppData is simply unacceptable.
