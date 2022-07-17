using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Projection : MonoBehaviour {
    [SerializeField] private LineRenderer _line;
    [SerializeField] private int maxPhysicsFrameIterations = 100;
    [SerializeField] private Transform[] obstaclesParents;

    private Scene _simulationScene;
    private PhysicsScene _physicsScene;
    private readonly Dictionary<Transform, Transform> _spawnedObjects = new Dictionary<Transform, Transform>();

    private void Start() {
        CreatePhysicsScene();
    }

    private void CreatePhysicsScene() {
        _simulationScene = SceneManager.CreateScene("Simulation", new CreateSceneParameters(LocalPhysicsMode.Physics3D));
        _physicsScene = _simulationScene.GetPhysicsScene();

        foreach (Transform parent in obstaclesParents)
        {
            makeGhostObject(parent);
            foreach (Transform obj in parent)
            {
                makeGhostObject(obj);
            }
        }
    }

    public void SimulateTrajectory(Spider spider, Vector3 pos, Quaternion rotation, Vector3 velocity) {
        var ghostObj = Instantiate(spider, pos, rotation);
        
        SceneManager.MoveGameObjectToScene(ghostObj.gameObject, _simulationScene);
        ghostObj.Jump(velocity);

        _line.positionCount = maxPhysicsFrameIterations;

        for (var i = 0; i < maxPhysicsFrameIterations; i++) {
            _physicsScene.Simulate(Time.fixedDeltaTime);
            _line.SetPosition(i, ghostObj.transform.position);
        }

        Destroy(ghostObj.gameObject);
    }

    public void EnableProjection(bool enabled)
    {
        _line.enabled = enabled;
    }

    private void makeGhostObject(Transform obj)
    {
        var ghostObj = Instantiate(obj.gameObject, obj.position, obj.rotation);
        Renderer renderer = ghostObj.GetComponent<Renderer>();
        if (renderer)
        {
            renderer.enabled = false;
        }

        SceneManager.MoveGameObjectToScene(ghostObj, _simulationScene);
        if (!ghostObj.isStatic) _spawnedObjects.Add(obj, ghostObj.transform);
    }
}