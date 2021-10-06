using UnityEngine;

public class DestroyByContact : MonoBehaviour
{
    public GameObject explosion;
    public GameObject playerExplosion;
    public int scoreValue;

    private GameController gameController;

    void Start()
    {
        GameObject gameControllerObject = GameObject.FindWithTag("GameController");
        if (gameControllerObject != null)
        {
            gameController = gameControllerObject.GetComponent<GameController>();
        }
        if (gameController == null)
        {
            Debug.Log("Cannot find 'GameController' script");
        }
    }

    void OnTriggerEnter(Collider other)
    {
        GameObject first = gameObject;
        GameObject second = other.gameObject;
        if (first.CompareTag("Enemy") && second.CompareTag("Player"))
        {
            first = other.gameObject;
            second = gameObject;
        }

        if (first.CompareTag("Player") && second.CompareTag("Enemy"))
        {
            if (first.name == "Player") //Player spaceship (not bullet)
            {
                first.SetActive(false);
                Instantiate(playerExplosion, first.transform.position, first.transform.rotation);
                gameController.GameOver();
            }
            else
            {
                PlayerController playerController = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
                if(playerController) playerController.ReloadBullet(first.gameObject);
                gameController.AddScore(scoreValue);
            }
            if(explosion) Instantiate(explosion, transform.position, transform.rotation);
            Destroy(second);
        }
    }
}