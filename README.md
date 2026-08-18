# Scene Selector

Adds a scene dropdown to the Unity Editor toolbar so you can jump between scenes without hunting through the Project window.

## Features

- Dropdown in the Editor toolbar listing scenes from folders you register
- Register any number of scene folders via `Tools/Scene Selector/Add Scenes Folder...`
- Dropdown stays in sync with the currently open scene
- Refresh button to rescan registered folders for new/removed scenes

## Installation

### Via Unity Package Manager (Git URL)

1. Open **Window > Package Manager**
2. Click the **+** button and choose **Add package from git URL...**
3. Enter:

   ```
   https://github.com/<your-user-or-org>/SceneSelectorPackage.git
   ```

To pin a specific version, append `#<tag>`, e.g. `https://github.com/<your-user-or-org>/SceneSelectorPackage.git#1.0.0`.

### Via manifest.json

Add the following to your project's `Packages/manifest.json`:

```json
"com.vaqifquluzada.sceneselector": "https://github.com/<your-user-or-org>/SceneSelectorPackage.git"
```

## Usage

1. Go to `Tools/Scene Selector/Add Scenes Folder...` and select a folder containing scenes.
2. A dropdown listing those scenes appears in the Editor toolbar. Selecting an entry opens that scene.
3. Use `Tools/Scene Selector/Clear Scene Folders` to reset registered folders.

## Requirements

- Unity 2019.1 or later

## Troubleshooting

**Dropdown doesn't appear in the toolbar:** On some Unity versions the toolbar hides custom UI elements by default. Right-click in the toolbar near the Play/Pause/Step buttons and enable **Unsupported User Elements**.
