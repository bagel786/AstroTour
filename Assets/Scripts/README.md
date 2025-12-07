# AstroTour

An educational adventure game that takes players on a journey through different career worlds, where they complete challenges to earn badges and explore various professional fields.

## Inspiration

Inspired by Pokémon games, where the goal is to collect badges as proof of completing tasks (defeating gym leaders). AstroTour follows a similar concept, where to collect a badge from a career 'world', you must first complete a series of tasks to prove competence and knowledge in that field.

## What it does

AstroTour is an adventure game with RPG elements that takes players on an educational journey through different career worlds. Players explore various professional fields, completing challenges to earn badges that prove their competence and knowledge.

### Core Features

- **Puzzle-based Challenges** - Solve logic gate puzzles, DNA sequencing challenges, fabric mixing tasks, and marketing strategy problems that simulate real-world career scenarios

- **Multiple Choice + Critical Thinking Questions** - Interactive dialogue system with NPCs who present career-related questions and scenarios that test understanding and decision-making skills

- **Inventory System** - Collect and manage items throughout your journey, with a hotbar for quick access and persistent save functionality

- **Quest System** - Track your progress through structured quests in each career world, with objectives that guide you toward earning badges

- **Interactive Mini-Games** - Engage with specialized terminals and workstations including:
  - Logic Gate Factory (computational thinking)
  - DNA Sequencing Lab (biotechnology)
  - Fabric Mixer (materials science)
  - Marketing Strategy Console (business)
  - Compound Synthesis Station (chemistry)

- **Exploration & Discovery** - Navigate through different maps and career worlds, interact with NPCs, unlock new areas through map transitions, and discover items that aid your journey

- **Progression System** - Earn badges by completing career-specific challenges, with cloud save support to track your achievements across sessions

## How we built it

Built with Unity and C#, AstroTour features:
- Custom drag-and-drop systems for interactive puzzles
- Modular terminal interfaces for different career challenges
- Persistent save system with both local and cloud storage options
- Dynamic quest and dialogue management
- Responsive UI with inventory, map, and settings management

## Game Systems

### Mini-Games & Challenges
- **Logic Gate Factory** - Build circuits using logic gates (AND, OR, NOT, etc.)
- **DNA Sequencing Lab** - Match and sequence DNA fragments
- **Fabric Mixer** - Combine materials to create new fabrics
- **Marketing Game** - Develop marketing strategies with card-based mechanics
- **Compound Synthesis** - Mix chemical compounds

### Player Systems
- 2D character movement and camera follow
- Interaction detection with NPCs and objects
- Item collection and inventory management
- Quest tracking and completion
- Badge collection and rewards

### Technical Features
- Save/Load system with cloud integration
- Scene transitions with loading screens
- Waypoint teleportation system
- Sound effects and audio management
- Settings and pause menu

## Setup & Installation

1. Clone this repository
2. Open the project in Unity (version [specify your Unity version])
3. Open the main scene to start
4. Refer to the setup guides in the project for specific mini-game configurations:
   - `SimpleConsoleSetup.md`
   - `LogicGateFactorySetup.md`
   - `Simple_DNA_Sequencing_Setup_Guide.md`
   - `MarketingGameSetup.md`

## Documentation

Additional documentation can be found in the project:
- `SaveSystemDocumentation.md` - Save system implementation details
- `DNA_Game_Quick_Reference.md` - DNA sequencing game reference
- `GymBadgeQuestExample.md` - Quest system examples

## Contributing

Contributions are welcome! Please feel free to submit pull requests or open issues for bugs and feature requests.

## License

[Add your license here]

## Acknowledgments

[Add acknowledgments, team members, or resources used]
