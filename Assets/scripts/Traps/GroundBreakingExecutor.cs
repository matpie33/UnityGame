using System.Collections;
using UnityEngine;

public class GroundBreakingExecutor : MonoBehaviour
{
    private GameObject fracturesParent;
    private void Start() {
        fracturesParent = transform.Find("Fractures").gameObject;
    }
    public void Execute() {
        fracturesParent.SetActive(true);
        StartCoroutine(AddRigidBodies());
    }

    private IEnumerator AddRigidBodies() {
        
        for (int i = 0; i < fracturesParent.transform.childCount; i++) {
            GameObject gameObject = fracturesParent.transform.GetChild(i).gameObject;
            Destroy(gameObject.GetComponent<Collider>());
            gameObject.AddComponent<Rigidbody>();
            yield return new WaitForSeconds(0.05f);
        }
    }

}
