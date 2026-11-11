using UnityEngine;

public class NewMonoBehaviourScript_test : MonoBehaviour
{
     public AudioSource musicThing;
     public SpriteRenderer spriteRenderer;
    

    public void OnblueMushromPauseEventishThing()
    {
        Debug.Log("funcWithLongName run, musicThing is null:" + musicThing is null);
        if (musicThing.isPlaying)
        {
            musicThing.Pause();
            spriteRenderer.color = Color.blue;
        }
        else
        {
            musicThing.Play();
            spriteRenderer.color = Color.yellow;
        }
    }
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}