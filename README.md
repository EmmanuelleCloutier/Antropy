# 🐜 Ant Colony Simulator – Godot C#

### A self-organizing ant colony simulation built with **Godot Engine (C#)**  
Observe, tweak, and study emergent AI behavior — no direct control, only nature evolving.

---
 
##  1. Project Goal  
 
Create an **autonomous simulation** of an ant colony in a natural environment.  
The player **observes** the colony’s growth and interactions:
- Ants explore, collect food, defend the nest, and reproduce.
- Everything works automatically — the user can simply **observe** or **adjust parameters** like speed or density.

---

## 2. Core Gameplay Loop

The simulation runs continuously, and the player can view stats such as:
- Total population  
- Food stock  
- Temperature or environmental conditions  

---

##  3. Main Systems

###  A. Ant AI System (FSM)
Each ant follows a **Finite State Machine**:

| State | Description | Transition |
|--------|--------------|-------------|
| Idle | Wait near the nest | Random → Explore |
| Explore | Wander randomly | Finds food → Collect |
| Collect | Pick up food | Food gathered → ReturnToNest |
| ReturnToNest | Bring food home | At nest → Deposit |
| Deposit | Drop food | Done → Explore | 
| Attack | Engage enemy | Enemy gone → Explore |
| Dead | Lifespan ended | Removed from world |

**Ant Types:**
- **Worker:** gathers and carries food  
- **Soldier:** defends against enemies  
- **Queen:** spawns new ants when food is sufficient  

---

###  B. Resource System
- **Food:** spawns randomly on the map  
- **Stock:** stored in the nest and consumed to create new ants  
- **Population:** grows with available resources  
- **Energy & Aging:** each ant has a limited lifespan  

---

###  C. Nest System
- Central hub for the colony  
- Storage, spawning, and information display  

**UI Elements:**
- Total ants  
- Stored food  
- Recent births  
 
---

###  D. Movement & Pheromone System
- Ants wander semi-randomly  
- Upon finding food, they leave a **pheromone trail** leading back to the nest  
- Other ants follow the strongest trails  
- Trails **evaporate over time** (exponential decay)

**Visual Representation:**
A semi-transparent 2D overlay showing pheromone intensity.

---

###  E. Enemy System
- Periodic enemy spawns (spiders, beetles, etc.)  
- Enemies target the nest or nearby ants  
- Soldiers automatically defend  
- Player observes the outcome  

---

###  F. Dynamic Environment
- **Day/Night Cycle:**  
  - Day = more food, more activity  
  - Night = less visibility, more danger  
- **Weather (optional):** rain slows ants and erases pheromones  

---

###  G. Save System *(Future Feature)*
- Save and load simulation state (food stock, pheromones, population, etc.)  

---

##  4. Visual Style
- **Top-down 2D view**  
- **Ants:** small animated sprites or dots  
- **Food:** green/brown chunks  
- **Nest:** brown circle at the center  
- **Pheromones:** blue semi-transparent overlay  

**UI:**
- Population counter  
- Food counter  
- Buttons: *Pause*, *Speed x2*, *Reset*  

---


##  5. Development Roadmap (Core Simulation)

| Week | Focus | Description |
|------|--------|-------------|
| 1 |  World Base | Create world scene, nest, and food spawner |
| 2 |  Simple AI | Ant FSM: Idle, Explore, ReturnToNest |
| 3 |  Resources | Add food collection and UI counters |
| 4 |  Pheromones | Implement pheromone map and visualization |
| 5 |  Reproduction | Queen lays eggs, manage lifespan |
| 6 |  Enemies | Add basic enemy AI and soldier behavior |
| 7 |  Environment | Day/Night cycle, UI polish, optimizations |
| 8 |  Save System | Implement save/load and final polish |

---

##  6. Possible Future Features
- Rival ant colonies (territory wars)  
- Seasonal changes and hibernation  
- Procedural terrain generation  
- Weather effects (rain, wind)  
- Genetic evolution (trait inheritance)  
- “God Mode” – tweak parameters live  

---

##  7. Key Learning Outcomes
- Modular **C# architecture** in Godot  
- Building and managing **Finite State Machines**  
- Handling **multiple AI agents efficiently**  
- Designing **emergent systems** (pheromones, environment, interactions)  
- Creating a clean and optimized **simulation viewer**  

---



