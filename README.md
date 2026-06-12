# Unity-Inventory-Demo
A Unity inventory system prototype featuring stackable items, unlockable slots, and save/load functionality.

## Tech Stack
* **Unity 2022.3.62f1**: configured for Portrait mode.
* **Zenject (Dependency Injection)**: ensures loose coupling between components.
* **JSON Serialization**: handles data saving to `Application.persistentDataPath`.
* **ScriptableObjects**: manages item configurations and game balance.
* **LINQ**: powers efficient item searching and filtering.

## Features
* **SO-Based Balancing**: All item parameters (weight, damage, icons) are stored in ScriptableObjects, allowing balance tweaks without touching code.
* **Robust Save System**: Uses unique string IDs and an `ItemDatabase` to prevent save corruption if project files are renamed.
* **Dynamic Weight Calculation**: Total inventory and object weight is calculated on the fly using LINQ.
* **Drag and Drop System**:
  * Drag to empty slots
  * Merge stackable items
  * Swap item positions
  * Delete items via drag
* **Item Info PopUp**: Displays detailed statistics when inspecting an item.
* **Inventory Control Buttons**:
  * **Shoot**: Simulates weapon firing and ammo consumption.
  * **Remove Random Item**: Deletes a random item from the grid.
  * **Add Random Item**: Spawns a random item into an available slot.
  * **Add All Ammo Types**: Grants each ammo type in amounts defined by the config.
  * **Delete Saves**: Clears persistent save data.
  * **Add Coins**: Grants a configurable amount of currency.
  * **Unlock Slots**: Expands inventory capacity using coins.
 
## Extensibility & Customization
* **Custom Items**: Create new items instantly via the Unity Editor (`Right Click -> Create -> Inventory -> Choose Item Type`) and add them to the `ItemDatabase` to load them into the game.
* **Game Balance**: Modify core settings (starting coins, slot unlock costs, total/available slot counts, and more) directly through the configuration files.

## How to Run
1. Open the `MainScene` scene.
2. Ensure that references to `GameConfig` and `ItemDatabase` are properly assigned in the `SceneContext`.
3. Run the game. All action logs (shooting, adding items, errors) will be displayed in the Console with color-coded formatting.
