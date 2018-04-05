using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class PlayerController : MonoBehaviour {

    LevelManager levelManager;
    Rigidbody2D rb;
    SpriteRenderer Srend;
    Animator anim;
    bool isOnGround;
    int health = 100;
    public static int score = 0;
    //change these variables if you wish to test different speeds and jump heights
    [SerializeField]
    float moveForce = .1f;
    [SerializeField]
    float jumpForce = 5f;
    [SerializeField]
    float maxVelocity = 10f;
    public Text heatlthLabel;
    public Text scoreLabel;

    // Use this for initialization
    void Start () {
        rb = GetComponent<Rigidbody2D>();
        Srend = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();

        //these lines are used to calculate screen wrapping

        heatlthLabel.text = ""+health;
        scoreLabel.text = "" + score;
    }
	
	// Update is called once per frame
	void FixedUpdate () {
        #region
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            if (rb.velocity.x > 3)
            {
                anim.SetBool("IsSkid", true);
            }
            else
            {
                anim.SetBool("IsSkid", false);
            }


            if (Mathf.Abs(rb.velocity.x) < maxVelocity)
            {
                rb.AddForce(Vector2.right * -1 * moveForce, ForceMode2D.Impulse);//moves the object
                anim.SetFloat("MoveX", Mathf.Abs(rb.velocity.x));

            }
            if (rb.velocity.x < 0)
            {
                Srend.flipX = true;//flips the sprite
            }
            anim.SetBool("Idle", false);
            //call animation
        }

        if (Input.GetKey(KeyCode.RightArrow))
        {
            if (rb.velocity.x < -3)
            {
                anim.SetBool("IsSkid", true);
            }
            else
            {
                anim.SetBool("IsSkid", false);
            }

            if (Mathf.Abs(rb.velocity.x) < maxVelocity)
            {
                rb.AddForce(Vector2.right * 1 * moveForce, ForceMode2D.Impulse);//moves the object
                anim.SetFloat("MoveX", Mathf.Abs(rb.velocity.x));

            }
            //call animation
            if (rb.velocity.x > 0)
            {
                Srend.flipX = false;//flips the sprite
            }
            anim.SetBool("Idle", false);

        }

        if (Input.GetKeyDown(KeyCode.UpArrow) && isOnGround)
        {


            rb.AddForce(Vector2.up * 1 * jumpForce, ForceMode2D.Impulse);//moves the sprite
            anim.SetTrigger("Jump");//call animation
            anim.SetBool("Idle", false);

        }

        anim.SetFloat("MoveX", Mathf.Abs(rb.velocity.x));
        if (isOnGround)
        {
            anim.SetBool("Idle", true);
        }
        else
        {
            anim.SetBool("Idle", false);
        }

        if (isOnGround && Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (anim.GetBool("Idle"))
            {
                anim.SetBool("IsDuck", true);
            }
            else
            {
                return;
            }
        }
        if (isOnGround && Input.GetKeyUp(KeyCode.DownArrow))
        {
            anim.SetBool("IsDuck", false);
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            anim.SetTrigger("Attack");
            anim.SetBool("Attacking", true);
        }
        if (Input.GetKeyUp(KeyCode.Space))
        {
            anim.SetBool("Attacking", false);
        }
        if (health <=0)
        {
            anim.SetBool("Die", true);
            anim.SetTrigger("Death");
            levelManager =  new LevelManager();
            levelManager.LoadLevel("Lose");
        }
        
        #endregion
    }
    public void OnCollisionEnter2D(Collision2D other)
    {
        if (other.collider.tag == "Ground")
        {
            isOnGround = true;
            anim.SetBool("IsOnGround", true);
        }
        if (other.collider.tag == "enemy")
        {
            
            health -= other.gameObject.GetComponent<Enemy>().damage;
            if (health < 0)
            {
                health = 0;
            }
            Vector2 tweak = new Vector2(Random.Range(-4f, 4f), Random.Range(2f, 4f));
           
                
                this.GetComponent<Rigidbody2D>().velocity += tweak;
            
            heatlthLabel.text = "" + health;
            anim.SetTrigger("Damaged");
        }

    }

    public void OnCollisionStay2D(Collision2D other)
    {
        if (other.collider.tag == "Ground")
        {
            isOnGround = true;
            anim.SetBool("IsOnGround", true);

        }
    }

    public void OnCollisionExit2D(Collision2D other)
    {
        if (other.collider.tag == "Ground")
        {
            isOnGround = false;
            anim.SetBool("IsOnGround", false);
            anim.SetBool("Idle", false);


        }
    }
  
    public void OnTriggerEnter2D(Collider2D collision)
    {
        
        if (collision.tag == "reward")
        {
            score += collision.gameObject.GetComponent<Reward>().points;
            scoreLabel.text = "" + score;
        }
        if (collision.tag == "healing")
        {
            health += collision.gameObject.GetComponent<Healing>().points;
            if (health > 100)
            {
                health = 100;
            }
            heatlthLabel.text = "" + health;
            
        }

        Destroy(collision.gameObject);

    }
}
