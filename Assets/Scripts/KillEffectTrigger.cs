using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillEffectTrigger : MonoBehaviour {

    Figure attacker, killedFig;
    public GameObject killParticle;

    private bool hasInit = false, hasTriggered = false;

    public void init(Figure attacker, Figure killedFig) {
        this.attacker = attacker;
        this.killedFig = killedFig;
        hasInit = true;
    }

    public void Update() {
        if (hasInit && !hasTriggered) {
            Vector3 vec = killedFig.transform.position - attacker.transform.position;
            if (vec.magnitude < 10f) {


                GameObject obj = Instantiate(killParticle);
                obj.transform.position = killedFig.transform.parent.position;

                ParticleSystem particle = obj.gameObject.GetComponent<ParticleSystem>();
                Renderer r = particle.GetComponent<Renderer>();
                if (killedFig.color == Figure.GameColor.White) {
                    r.material = FindObjectOfType<GameManager>().whiteMaterial;
                } else {
                    r.material = FindObjectOfType<GameManager>().blackMaterial;
                }
                GameManager.figs.Remove(killedFig);

                Debug.Log("KILL!");

                killedFig.destroy();
                Destroy(obj, 1f);
                Destroy(this.gameObject, 1f);

                hasTriggered = true;
                Destroy(this);
            }
        }
    }

}
