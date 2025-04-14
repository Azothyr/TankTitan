using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GameSpecific.Tank
{
    public class SceneBehavior : MonoBehaviour
    {
        [Header("Scene Transition Settings")]
        [SerializeField] private Animator transitionAnimator; // Animator for transition animations
        [SerializeField] private float transitionDuration = 1f; // Duration of the transition animation
        [SerializeField] private string transitionTrigger = "StartTransition"; // Trigger for the animation

        [Header("Re-Instantiation Settings")]
        [SerializeField] private GameObject[] objectsToSpawn; // Objects to spawn when re-instantiating the scene
        [SerializeField] private Transform[] spawnPoints; // Spawn points for the objects

        /// <summary>
        /// Transitions to a different scene with an animation and timer.
        /// </summary>
        /// <param name="sceneName">The name of the scene to load.</param>
        public void TransitionToScene(string sceneName)
        {
            StartCoroutine(TransitionSceneCoroutine(sceneName));
        }

        /// <summary>
        /// Re-instantiates the current scene with new objects.
        /// </summary>
        public void ReInstantiateScene()
        {
            StartCoroutine(ReInstantiateSceneCoroutine());
        }

        private IEnumerator TransitionSceneCoroutine(string sceneName)
        {
            if (transitionAnimator != null)
            {
                transitionAnimator.SetTrigger(transitionTrigger); // Play transition animation
            }

            yield return new WaitForSeconds(transitionDuration); // Wait for the animation to finish

            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
            while (!asyncLoad.isDone)
            {
                yield return null; // Wait until the scene is fully loaded
            }
        }

        private IEnumerator ReInstantiateSceneCoroutine()
        {
            if (transitionAnimator != null)
            {
                transitionAnimator.SetTrigger(transitionTrigger); // Play transition animation
            }

            yield return new WaitForSeconds(transitionDuration); // Wait for the animation to finish

            // Reload the current scene
            Scene currentScene = SceneManager.GetActiveScene();
            SceneManager.LoadScene(currentScene.name);

            // Wait for the scene to reload
            yield return null;

            // Spawn new objects
            for (int i = 0; i < objectsToSpawn.Length; i++)
            {
                if (i < spawnPoints.Length)
                {
                    Instantiate(objectsToSpawn[i], spawnPoints[i].position, spawnPoints[i].rotation);
                }
            }
        }
    }
}