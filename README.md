# Unity Swarm Drones Simulator

**A drone created for Unity with realistic drone physics, to find important features**


### Code Explanation

Code explanation:

There are 3 important parts in this project:

1) Drone Physics
    https://github.com/UAVs-at-Berkeley/UnityDroneSim
    PID to control velocity and position in height
    The interactionHandler is here to access the correct part of the drone when applied a force for instance
2) Swarm Model (Assets\Scripts\SwarmModels\)
    There are 2 models created in this simulation both model has their own Obstacle avoidance and Target script that has predefined coefficient to match the model:
        1) Reynold nodel based on 3 forces: Cohesion, Separation and alignement (Thoses parameters can be modified by changing the weights (higher the weight, higher importance to the force))
        2) Olfti-Saber model, this model tries to keep the same distance between all the drones (Still figuring out the parameters reading the paper https://ieeexplore.ieee.org/abstract/document/1605401)
        
        r (float): distance between current drone and neighbour drone
        d_ref (float): desired distance between members of the swarm
        a (float): potential field parameter
        b (float): potential field parameter
        c (float): potential field parameter
        r0 (float): minimum distance to be part of the swarm
        delta (float): parameter for the neighbour weight

3) Graphs and HeatMap (Assets\Scripts\Graphs && Assets\Scripts\HeatMap)
    1) Graphs (Assets\Scripts\Graphs)
        Every drones keeps their past informations in the *variableManager* at a frequency defined in the config.cs file, thoses variables can be ploted using the *graphManager*. This script is using subscripts like Linechart and ULineRenderer that allows to draw on a canvas. 
    2) Heat Map
        The Heatmap is done by Spawning around the center of the swarm an array of cibes if differents heatmap color (red = 1 -- blue = 0 ) it is updated using the refreshRate (0.2 default) you can increase the resolution of the heatmap by reducing the spacing and the radius of the heatmap with the radius

### Unity Editor
    Every Main script are in the Gamemanger
    Every drones are responsible for the behavior and saving their own data (will be changed). (You can find the drone in the swarmHolder)

