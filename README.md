# 🏗️ Unity Grid-Based Building Placement System
### A Technical Demonstration of Modular City-Builder Architecture

---

## 📍 Game Overview

This project is a **tech demonstrator** inspired by grid-based city-building games such as **SimCity** and **Clash of Clans**.  
It is not a full commercial game — instead, it is a **learning-oriented prototype** designed to master:

- Clean game code architecture
- Unity grid system fundamentals
- Data-driven placement logic
- Modular & scalable gameplay systems
- Professional development practices

The objective is to **understand how real strategy/base-building games structure their internal systems**, not to build a full product.

---

## 🎯 Core Learning Outcomes

| System | Skills Learned |
|--------|---------------|
| Grid management | Grid creation, indexing, world↔grid conversion |
| Placement logic | Coordinate validation, rule strategy pattern |
| Tile visualization | Dynamic green/red tile feedback |
| Ghost preview | Real-time snapping and preview control |
| Object pooling | Efficient quad rendering & performance |
| UI interaction | Building selection flow |
| NPC systems (Phase 3) | Navigation, roaming, object interaction |

---

# 🧱 Development Phases

---

## **PHASE 1 — Core Grid System (Foundation Layer)**

### Objective:
Build a **robust grid infrastructure** reusable by all systems.

### Deliverables:
- 2D logical grid array (`grid[x,y]`)
- World ↔ Grid coordinate conversion
- Tile occupancy tracking (`bool[,] occupied`)
- Visual debug grid using `Debug.DrawLine()`
- Configurable grid size
- GridSystem Manager
- Object pool base (for tile quads later)
- Mouse raycast to tile coordinate conversion

### Preview
Place image here after this section:

![Grid Debug View](README_Images/grid_debug_numbers.png)
![Grid Top View](README_Images/grid_full.png)
![Grid Rotated Closeup](README_Images/grid_close.png)

---

## **PHASE 2 — Building Placement System (Core Gameplay Layer)**

### Objective:
Add **Clash-of-Clans-style base construction** with visual feedback.

### Deliverables:
- Building selection UI
- Ghost preview object
- Real-time placement validation (valid/invalid)
- Tile highlight using quads (green/red)
- Object placement snap to grid center
- Object pool reuse system
- Strategy pattern placement rules:
  - SingleTileRule
  - MultiTileRule (2x2, 3x3)
  - RoadRule
  - DecorationRule

### Example Result
![Placement System Result](README_Images/building_placement_preview.png)

---

## **PHASE 3 — NPC Movement & Interaction (Future Work)**

### Deliverables:
- NPC navigation system (NavMesh or grid pathfinding)
- Random roaming
- Interactions with buildings (inspect / collect / repair)
- Animation triggers & UI popups

---

# 🧠 Full System Architecture Overview

![System Architecture UML](README_Images/system_uml.png)

---

# 🏗️ Game Coding Architecture — FINAL

### **Modular & Expandable Architecture**
- Singleton Managers
- Object Pooling System
- Strategy Pattern (rules)
- Event-Driven system
- Data-Driven SO architecture

---

## **Core System Layer (Phase 1)**
**Classes**
- Grid
- GridVisualizer
- QuadObjectPooler
- InputHandler
- GameManager

**Purpose:** Foundation for all higher systems.

---

## **Placement System Layer (Phase 2)**
**Classes**
- PlacementManager
- GhostPreviewController
- PlacementValidator
- IPlacementRule + rule variants
- BuildingTypeSO

**Purpose:** Controls object placement workflow.

---

## **Visual System Layer**
**Classes**
- Tile quads
- Grid highlight controller
- Preview visuals

---

## **Gameplay System Layer**
(Phase 3 NPC Systems)

---

## **UI Layer**
- BuildingSelectionUI
- BuildMenuController
- HUD

---

# 📦 Installation & Usage

### Requires:
- Unity 2022.3 LTS or newer
- URP recommended

### Steps:
1. Import project or scripts into new Unity scene
2. Add `GroundPlane`
3. Add `GridSystem` object and attach:
   - GridMaker
   - GridVisualizer
   - PlacementManager
   - QuadObjectPooler
4. Assign quad prefab & materials
5. Run with `Play` and use **Left-Click** to place objects

---

# 📜 Credits / Reference
This project uses key concepts inspired by **Code Monkey Grid System tutorials**, extended into a professional architecture system.

---

