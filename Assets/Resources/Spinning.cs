using UnityEngine;

public class Spinning : MonoBehaviour {
    [Tooltip("Velocidade de rotação em graus por segundo")]
    public float rotationSpeed = 90f;

    // Update is called once per frame
    void Update() {
        // Rotaciona o objeto no eixo Y
        transform.Rotate(0f, 0f, rotationSpeed * Time.deltaTime);
    }
}