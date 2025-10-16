using UnityEngine;

public class EffectSc : MonoBehaviour
{
    [SerializeField] ParticleSystem OlumEfekti;
    [SerializeField] ParticleSystem KanEfekti;
    [SerializeField] ParticleSystem BuildEffect;


    private void Start()
    {
    }
    public void KanEfektiCalistir() {
        KanEfekti.Play();
        Debug.Log("cal");
    } 
    public void OlumEfektiCalistir() => OlumEfekti.Play();

}
