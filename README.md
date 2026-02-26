Poker Game Prototype — Unity
Overview
This project is a poker game prototype built in Unity with a focus on clean architecture and scalable system design.
The goal of the implementation is to demonstrate proper separation of gameplay logic, state management, and UI rather than production-level visuals.
The game includes:
Full poker round flow (PreFlop → Flop → Turn → River → Showdown)
Turn-based gameplay with timer
Chip deduction and pot distribution
Basic AI opponent
Match restart without reloading the scene
Event-driven UI updates

Architecture
The project follows a layered architecture.
Presentation
Handles UI rendering and player input.
Examples:
SimpleGameUI / Table UI
Card display
Action buttons
UI listens to events and does not contain game logic.




Application Layer
Coordinates gameplay flow.
Main components:
PokerGameManager — overall match orchestration
GameStateController — round state machine
TurnManager — turn sequencing
TurnTimerService — turn timer
AIDecisionService — AI actions

Core Domain
Contains all poker logic (independent from Unity).
Models
GameSnapshot (single source of truth)
Player
Card
PlayerAction
Services
DeckService
DealService
BetService
PotService
HandEvaluator
Enums
PokerRound
ActionType
HandRank




Infrastructure
Provides decoupled communication.
EventManager (event bus)
GameEvents
GenericSingleton

Game Flow
Match starts → snapshot created
Cards dealt in PreFlop
Players take turns
Actions update pot
Betting round completes → state advances
Community cards dealt (Flop, Turn, River)
Showdown → winner determined
Pot distributed → match restarts
Both player and AI use the same action pipeline.

AI
AI decisions are based on hand strength from the evaluator.
The AI generates PlayerAction events just like a real player, keeping gameplay consistent and deterministic.

Design Principles
Clear separation between UI, orchestration, and domain logic
GameSnapshot acts as the authoritative state
Event-driven communication reduces coupling
State machine controls round flow
Services encapsulate poker logic
Architecture prepared for future multiplayer integration

How to Run
Open the project in Unity
Load the main scene
Press Play
Use buttons to play against the AI

Possible Improvements
Full poker hand evaluation
Hide AI cards until showdown
Networking integration
Advanced AI strategy
UI polish

This prototype focuses on demonstrating scalable architecture and clean gameplay flow suitable for future multiplayer expansion.


Multiplayer architecture implemented using Netcode for GameObjects with server-authoritative design. Core gameplay is fully reusable; networking layer is partially implemented due to scope constraints.

