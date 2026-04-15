# AstroTour - Educational Quest-Based Game

> **A Unity FBLA Game Project** — Travel through various worlds to learn about different careers & complete challenges!

[![Play on Unity](https://img.shields.io/badge/Play%20on-Unity%20Play-blue?logo=unity)](https://play.unity.com/en/games/4840d816-e430-4887-87ae-8159a44fba21/astrotour)
[![GitHub](https://img.shields.io/badge/GitHub-bagel786%2FAstroTour-black?logo=github)](https://github.com/bagel786/AstroTour)

🎮 **[Play AstroTour on Unity Play](https://play.unity.com/en/games/4840d816-e430-4887-87ae-8159a44fba21/astrotour)**

A 2D Unity game featuring an astronaut character exploring different themed environments while completing educational quests and mini-games. Play it directly in your browser — no download required!

## Overview

AstroTour is an educational adventure game where players navigate through various themed areas including cybersecurity labs, medical facilities, entrepreneurship centers, and quantum computing spaces. Players complete quests by interacting with NPCs, solving puzzles, and collecting items.

Built as an **FBLA (Future Business Leaders of America)** game project, AstroTour aims to teach students about different STEM and business careers through interactive gameplay.

## Features

### Quest System
- Dynamic quest tracking with multiple objective types
- Support for collection, interaction, and completion objectives
- Retroactive quest progress (completed objectives before accepting quest count)
- Quest rewards system (items, gold, experience)
- Persistent quest state across game sessions

### Mini-Games & Terminals
- **DNA Sequencing**: Match DNA base pairs in the correct sequence
- **Marketing Game**: Strategic card placement and matching
- **Logic Gate Factory**: Build and test logic circuits
- **Password Terminals**: Simple console-based challenges

### Game Environments
- **Cyber Lab**: Cybersecurity-themed area with tech decorations
- **Medical Lab**: Hospital environment with DNA sequencing challenges
- **Entrepreneurship Center**: Business-themed area with marketing games
- **Quantum Computing Lab**: Advanced computing environment
- **Space Station**: Sci-fi themed hub area

### Core Systems
- **Inventory Management**: Hotbar and full inventory with drag-and-drop
- **Save System**: Local file and cloud save support
- **Dialogue System**: NPC interactions with quest integration
- **Map Transitions**: Seamless area transitions with loading screens
- **Sound Effects**: UI feedback and ambient audio

## Technical Details

### Built With
- Unity 2022+ (Universal Render Pipeline)
- C# scripting
- Unity Input System
- TextMesh Pro for UI

### Key Components
- Quest Controller: Manages active quests and objective tracking
- Inventory Controller: Handles item collection and storage
- Dialogue Controller: NPC conversation system
- Save System Manager: Persistent data management
- Terminal Controllers: Mini-game logic for various challenges

## Project Structure

```
Assets/
├── Animations/ # Character animations
├── Audio/      # Sound effects and music
├── Decorations/ # Environment props by theme
├── Prefabs/    # Reusable game objects
├── Quests/     # Quest ScriptableObjects
├── Scenes/     # Game levels
├── Scripts/    # C# game logic
├── Sprites/    # 2D artwork and UI
└── Tiles/      # Tilemap assets
```

## Getting Started

### Prerequisites
- Unity 2022.3 LTS or newer
- TextMesh Pro package
- Universal Render Pipeline package

### Installation
1. Clone this repository
   ```bash
   git clone https://github.com/bagel786/AstroTour.git
   ```
2. Open the project in Unity
3. Open `Assets/Scenes/MainMenu.unity` to start
4. Press Play to run the game

### Controls
- **WASD / Arrow Keys**: Move character
- **E**: Interact with NPCs and terminals
- **TAB**: Open inventory
- **SPACE**: To skip specific dialogue lines

## Quest Types

The game features multiple quest types:
- **CyberMania**: Cybersecurity challenges
- **DNAQuest**: DNA sequencing puzzles
- **EntreFrenzy**: Entrepreneurship tasks
- **GateGalore**: Logic gate construction
- **MarketingQuest**: Marketing strategy game
- **MedlabComplexities**: Medical lab challenges
- **QuantQuest**: Quantum computing puzzles
- **TerminalQuest**: Console-based challenges

## Development

### Adding New Quests
1. Create a new Quest ScriptableObject: `Assets > Create > Quests > Quest`
2. Configure quest name, description, and objectives
3. Set objective types (CollectItem, TalkNPC, CompleteTerminal, etc.)
4. Define rewards
5. Assign to NPC dialogue

### Creating Mini-Games
See documentation in `Assets/Scripts/`:
- `Simple_DNA_Sequencing_Setup_Guide.md`
- `LogicGateFactorySetup.md`
- `MarketingGameSetup.md`
- `ConsoleSetupGuide.md`

## Save System

The game supports two save methods:
- **Local File Save**: Saves to persistent data path
- **Cloud Save**: Integration ready for cloud storage

Save data includes:
- Quest progress and completion
- Inventory contents
- Player position
- Game settings

## Links
- 🎮 **Play the game**: [AstroTour on Unity Play](https://play.unity.com/en/games/4840d816-e430-4887-87ae-8159a44fba21/astrotour)
- 📦 **Source code**: [bagel786/AstroTour on GitHub](https://github.com/bagel786/AstroTour)

## Contributing

This is an educational project. Contributions welcome for:
- New mini-games
- Additional quest content
- Bug fixes
- Performance improvements

## License

Educational project - check with repository owner for usage rights.

## Acknowledgments
- Sprite assets from various pixel art sources
- Unity community for tutorials and support
- Educational content inspired by STEM learning objectives
