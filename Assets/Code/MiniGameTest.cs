using UnityEngine;

public class MiniGameTest : MonoBehaviour {
    public Animator anim;
    
    void Start(){
        GameManager.Instance.SetActiveScene(this.name);
    }
    
    public void FinishMiniGame(){
        StoryChapter s = FindAnyObjectByType<StoryChapter>();
        s.minigameRunning = false;
    }

    public void SetStarCount(int count){
        anim.SetInteger("starCount", count);
    }
    
}