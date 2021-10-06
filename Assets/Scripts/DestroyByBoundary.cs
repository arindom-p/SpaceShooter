using UnityEngine;

public class DestroyByBoundary : MonoBehaviour
{
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController playerController = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
            if(playerController)
                playerController.ReloadBullet(other.gameObject);
        }
        else Destroy(other.gameObject);
    }
}
