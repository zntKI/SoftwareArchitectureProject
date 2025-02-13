# SoftwareArchitectureProject
A Unity tower defense game in which I learn and apply design principles and patterns

<p align="center">
  <img src="Media/demo.gif"><br/>
  *<i>Low frame rate caused by gif limitations</i>*
</p>

## Overview

Throughout the course, I had the chance to familiarize myself with concepts like SOLID principles, design patterns and software design, and how to apply them to write clear and sustainable code.

## Software Architecture

Generally speaking, I have followed and applied the SOLID principles by:
- reducing responsibility and tasks of classes to the minimum
- relying more on 'has a' relationships than 'is a' since they offer more versatility and ease of modifications
- having abstract behaviours allowing for easy substitution among their subclass implementations<br><br>

Regarding design principles, I have applied the following:
- **Strategy**
- **Observer**
- **Model-View-Presenter(Controller) (MVP)**
- **Singleton**<br><br>

The following section gives concrete examples of where they have been applied:

## Gameplay

<br>Gamelay can be broken down into two main phases: Wave spawning and Tower building.

- **Waves:**
  1. Regarding Enemies, MVP pattern is applied so that specific logic can be separated into its own classes ('EnemyView' and 'EnemyModel') while being managed by a presenter ('EnemyController').
  2. Wave spawning, on the highest level, is managed by a Singleton 'WaveManager', responsible for storing general information about waves and handing out tasks accordingly.
  3. When a wave has started, 'WaveManager' utilizes the Observer pattern, by firing an event that informs 'SpawnController' to start spawning enemies.
  4. Spawning, and therefore 'SpawnController', relies heavily on the Strategy pattern, with which wave properties can be easily configurable through the inspector, allowing for many unique combination to be tested out without having to make changes to the code and wait for recompile time.
 
- **Building:**
  1. Analogically to Enemies, Towers also apply the MVP pattern to spread out tasks and responsibilities to avoid clutternes in code.
  2. Again, similar to Waves, Building is also supervised by a Singleton 'BuildManager' that handles tower buying, upgrading and selling.
  3. 'BuildManager' communicates (also through the Observer pattern) mostly with the 'GameUIManager', since all tower interactions require some form of mouse input, transferred from the UI elements to the logic behind that interprets it appropriately.
  4. Towers have two kinds of behaviours: Targeting and Attacking; both are defined utilizing abstraction as a tool to make their implementations interchangeable and design-friendly to modify through the inspector.

# Assets:
In-game assets: [Isometric Tower defense pack](https://assetstore.unity.com/packages/2d/environments/isometric-tower-defense-pack-183472)<br>
GUI: [Graphics created by Penzilla Design](https://penzilla.itch.io/basic-gui-bundle)
