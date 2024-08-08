# Unity Swarm Drones Simulator

**A drone simulator created for Unity with realistic drone physics, designed to explore and identify important features.**

## Overview

This project consists of three main components:

1. **Drone Physics**
2. **Swarm Models**
3. **Graphs and Heat Maps**

Each section below provides a detailed explanation of these components.

---

## 1. Drone Physics

This section handles the core physics of the drones, ensuring realistic movement and control.

- **Repository**: [UnityDroneSim](https://github.com/UAVs-at-Berkeley/UnityDroneSim)
- **Control System**: Uses a PID controller to manage velocity and altitude.
- **Interaction Handler**: Facilitates access to different parts of the drone when forces are applied.

---

## 2. Swarm Models (Assets\Scripts\SwarmModels)

Two swarm models have been implemented in this simulation, each with its own obstacle avoidance and target acquisition scripts. These scripts come with predefined coefficients that match each model's specific needs.

### 2.1. Reynold's Model

Based on three main forces:
- **Cohesion**: Attraction between members of the swarm.
- **Separation**: Avoidance of crowding neighbors.
- **Alignment**: Steering towards the average heading of neighbors.

These parameters can be adjusted by modifying their respective weights. Increasing the weight emphasizes the importance of that force.

### 2.2. Olfati-Saber Model

This model attempts to maintain a uniform distance between all drones. The parameters are still being fine-tuned according to the paper [Olfati-Saber](https://ieeexplore.ieee.org/abstract/document/1605401).

Key Parameters:
- `r (float)`: Distance between the current drone and its neighbor.
- `d_ref (float)`: Desired distance between members of the swarm.
- `a, b, c (float)`: Potential field parameters.
- `r0 (float)`: Minimum distance to be part of the swarm.
- `delta (float)`: Parameter for the neighbor weight.

---

## 3. Graphs and Heat Maps

### 3.1. Graphs (Assets\Scripts\Graphs)

- **Data Storage**: Each drone logs its past states into the `variableManager` at a configurable frequency (set in `config.cs`).
- **Plotting**: Data can be visualized using the `graphManager`, which utilizes sub-scripts like `Linechart` and `ULineRenderer` to draw on a canvas.

### 3.2. Heat Maps (Assets\Scripts\HeatMap)

- **Heatmap Generation**: A grid of cubes is spawned around the swarm center, colored based on heat (red = 1, blue = 0).
- **Configuration**: The heatmap is updated at a configurable `refreshRate` (default: 0.2). Resolution can be enhanced by adjusting the spacing and radius.

---

## Unity Editor Setup

1. **Main Scripts**: Located in the `GameManager`.
2. **Drone Management**:
    - Each drone is autonomous, responsible for its behavior and data logging.
    - Drones are nested under `swarmHolder`. The "Drone Parent" is the core drone object where forces are applied. It contains a `RigidBody` and tracks the drone's true position.
3. **Canvas**:
    - Graphs are plotted here, with a dropdown menu available to select variables for plotting.
    - Managed by the `GraphManager` script.
