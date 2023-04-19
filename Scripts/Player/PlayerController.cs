using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    private PlayerControls playerControls;

    private Vector2 moveVector = new Vector2(0.0f, 0.0f);

    public float scaleMoveDir;

    public int accelFrames;

    [Header("Walkable Layers")]
    [Tooltip("A collection of TileMaps without collision in order of vertical layer (bottom to top)")]
    public GameObject[] walkableLayers;

    [Header("Decor Layers")]
    [Tooltip("A collection of TileMaps to go above walkable tiles (bottom to top)")]
    public GameObject[] decorLayers;

    [Header("Collision Layers")]
    [Tooltip("A collection of TileMaps with collision in order of vertical layer (bottom to top)")]
    public GameObject[] colliderLayers;

    [Header("Bullet Collision Layers")]
    [Tooltip("A collection of TileMaps with collision in order of vertical layer (bottom to top)")]
    public GameObject[] bulletColliderLayers;

    public int startingLayer = 0;
    private int layer;

    private Facing facing = Facing.NORTH;
    private float viewRange = 0.95f;

    private bool isMovingRight, isMovingLeft, isMovingUp, isMovingDown;

    private int[] framesMovingDir = new int[4];

    private bool inputFrozen = false;

    public Collider2D playerCollider;
    public Rigidbody2D r2;

    [Header("Player Bullet")]
    public GameObject playerBullet;

    // True when mouse is held down.
    private bool isShooting;
    // Can shoot again when this is 0. Counts down from shootCD after a bullet is spawned.
    private int shootTimer = 0;
    [SerializeField]
    [Header("Base Cooldown between shots.")]
    // The time between which bullets can be shot.
    private int shootCD = 0;

    [SerializeField]
    CanvasGroup deathScreen;

    [SerializeField]
    CanvasGroup cardScreen;
    [SerializeField]
    CardScreen cardUI;

    [SerializeField]
    // The player stats object. Tries to retain stats from previous scenes -- defaults to default stats
    private PlayerStats stats;

    // Used for blackbox states.
    private bool fadeIn, fadeOut;

    // Tracks when the player has died.
    private bool isDead = false;

    // Motion sprites
    [SerializeField]
    private GameObject frontIdle, backIdle, leftIdle, rightIdle, frontWalk, backWalk, leftWalk, rightWalk;

    [SerializeField]
    private AudioSource throwSound;

    private void Awake()
    {
        playerControls = new PlayerControls();
    }

    private void OnEnable()
    {
        playerControls.Enable();
    }

    private void OnDisable()
    {
        playerControls.Disable();
    }

    // Start is called before the first frame update
    void Start()
    {
        // Movement
        playerControls.MouseKeyboard.up.started += _ => { setMoveData(Facing.NORTH); };
        playerControls.MouseKeyboard.down.started += _ => { setMoveData(Facing.SOUTH); };
        playerControls.MouseKeyboard.right.started += _ => { setMoveData(Facing.EAST); };
        playerControls.MouseKeyboard.left.started += _ => { setMoveData(Facing.WEST); };

        //playerControls.MouseKeyboard.reload.started += _ => { SceneTransferManager.Instance.loadScene(SceneManager.GetActiveScene().name); };

        playerControls.MouseKeyboard.up.canceled += _ => { isMovingUp = false; };
        playerControls.MouseKeyboard.down.canceled += _ => { isMovingDown = false; };
        playerControls.MouseKeyboard.right.canceled += _ => { isMovingRight = false; };
        playerControls.MouseKeyboard.left.canceled += _ => { isMovingLeft = false; };

        // Gun
        playerControls.MouseKeyboard.shoot.started += _ => { isShooting = true; };
        playerControls.MouseKeyboard.shoot.canceled += _ => { isShooting = false; };

        // Card Screen
        playerControls.MouseKeyboard.openCardScreen.started += _ => { openCardScreen(); };
        playerControls.MouseKeyboard.openCardScreen.canceled += _ => { closeCardScreen(); };

        // Interaction
        // playerControls.Player.Confirm.started += _ => { sendInteract(); };

        // Update layer and position data on player load from SceneTransferManager. (If applicable, which it will be in the final game.)
        SceneTransferManager stm = SceneTransferManager.Instance;
        if (stm.getIsSetup())
        {
            // Layer data
            this.layer = stm.getTargetLayer();
            updateLayerData(layer);

            // Position data.
            gameObject.transform.position = new Vector3(stm.getTargetCoords()[0], stm.getTargetCoords()[1]);

            // Player Stats
            stats.loadFrom(stm.getStats());
        }
        // This is probably only a debug case. Launching from anywhere should have set the above.
        else
        {
            layer = startingLayer;
            updateLayerData(startingLayer);
        }
    }

    private void setMoveData(Facing dir)
    {
        switch (dir)
        {
            case (Facing.NORTH):
                isMovingUp = true;
                if (!inputFrozen && !isMovingLeft && !isMovingRight && !isMovingDown) setFacing(Facing.NORTH);
                break;
            case (Facing.SOUTH):
                isMovingDown = true;
                if (!inputFrozen && !isMovingLeft && !isMovingRight && !isMovingUp) setFacing(Facing.SOUTH);
                break;
            case (Facing.EAST):
                isMovingRight = true;
                if (!inputFrozen && !isMovingLeft && !isMovingUp && !isMovingDown) setFacing(Facing.EAST);
                break;
            case (Facing.WEST):
                isMovingLeft = true;
                if (!inputFrozen && !isMovingUp && !isMovingRight && !isMovingDown) setFacing(Facing.WEST);
                break;
            default:
                Debug.LogError("Attempted to move in a nonexistent direction.");
                break;
        }
    }

    void FixedUpdate()
    {
        checkKill();
        if (!inputFrozen)
        {
            handleShoot();
            computeMove();
            makeMove();
            setCorrectSprite();
        }
    }

    private void setCorrectSprite()
    {
        switch (facing)
        {
            case (Facing.NORTH):
                if (getMoving(facing))
                {
                    if (!backWalk.activeInHierarchy)
                    {
                        deactivateAllSprites();
                        backWalk.SetActive(true);
                    }
                }
                else
                {
                    if (!backIdle.activeInHierarchy)
                    {
                        deactivateAllSprites();
                        backIdle.SetActive(true);
                    }
                }
                break;
            case (Facing.EAST):
                if (getMoving(facing))
                {
                    if (!rightWalk.activeInHierarchy)
                    {
                        deactivateAllSprites();
                        rightWalk.SetActive(true);
                    }
                }
                else
                {
                    if (!rightIdle.activeInHierarchy)
                    {
                        deactivateAllSprites();
                        rightIdle.SetActive(true);
                    }
                }
                break;
            case (Facing.WEST):
                if (getMoving(facing))
                {
                    if (!leftWalk.activeInHierarchy)
                    {
                        deactivateAllSprites();
                        leftWalk.SetActive(true);
                    }
                }
                else
                {
                    if (!leftIdle.activeInHierarchy)
                    {
                        deactivateAllSprites();
                        leftIdle.SetActive(true);
                    }
                }
                break;
            case (Facing.SOUTH):
                if (getMoving(facing))
                {
                    if (!frontWalk.activeInHierarchy)
                    {
                        deactivateAllSprites();
                        frontWalk.SetActive(true);
                    }
                }
                else
                {
                    if (!frontIdle.activeInHierarchy)
                    {
                        deactivateAllSprites();
                        frontIdle.SetActive(true);
                    }
                }
                break;
            default:

                break;
        }
    }

    private void deactivateAllSprites()
    {
        frontIdle.SetActive(false);
        backIdle.SetActive(false);
        leftIdle.SetActive(false);
        rightIdle.SetActive(false);
        frontWalk.SetActive(false);
        backWalk.SetActive(false);
        leftWalk.SetActive(false);
        rightWalk.SetActive(false);
    }

    private void checkKill()
    {
        if(!isDead && stats.getCurrHealth() <= 0)
        {
            isDead = true;
            r2.constraints = RigidbodyConstraints2D.FreezeAll;
            setFreezeInputs(true);
            StartCoroutine(loadDeathScreen());
        }
    }

    private IEnumerator loadDeathScreen()
    {
        for(int i = 0; i < 30; i++)
        {
            deathScreen.alpha = ((float)i + 1.0f) / 30.0f;
            yield return new WaitForEndOfFrame();
        }
        deathScreen.interactable = true;
        deathScreen.blocksRaycasts = true;
    }

    private void openCardScreen()
    {
        // Freeze time
        Time.timeScale = 0.0f;
        // Freeze input
        setFreezeInputs(true);
        // Populate menu
        cardUI.onUIOpen(stats.getCards());
        // Open menu.
        StartCoroutine(loadCardScreen());
    }

    private void closeCardScreen()
    {
        // Unfreeze time
        Time.timeScale = 1.0f;
        // Unfreeze input
        setFreezeInputs(false);
        // Apply effects of cards based on their present slots.
        cardUI.onUIClose();
        // Close menu.
        StartCoroutine(unloadCardScreen());
    }

    private IEnumerator loadCardScreen()
    {
        for (int i = 0; i < 30; i++)
        {
            cardScreen.alpha = ((float)i + 1.0f) / 30.0f;
            yield return new WaitForEndOfFrame();
        }
        cardScreen.interactable = true;
        cardScreen.blocksRaycasts = true;
    }

    private IEnumerator unloadCardScreen()
    {
        cardScreen.interactable = true;
        cardScreen.blocksRaycasts = true;
        for (int i = 0; i < 30; i++)
        {
            cardScreen.alpha = (29.0f - (float)i) / 30.0f;
            yield return new WaitForEndOfFrame();
        }
    }

    private void handleShoot()
    {
        if (isShooting)
        {
            attemptShoot();
        }
        if (shootTimer > 0) shootTimer--;
    }

    // The shooting controller
    private void attemptShoot()
    {
        if (shootTimer <= 0)
        {
            throwSound.Play();
            // Get overworld position via technical camera magic.
            Vector2 playerPos = gameObject.transform.position - new Vector3(0f, 0.5f, 0f);

            Vector3 mousePosition = playerControls.MouseKeyboard.mousePos.ReadValue<Vector2>();
            mousePosition.z = Camera.main.nearClipPlane;
            Vector3 worldPosition = Camera.main.ScreenToWorldPoint(mousePosition);

            Vector2 dirVec = new Vector2(worldPosition.x - playerPos.x, worldPosition.y - playerPos.y).normalized;

            Vector3 spawnLoc = new Vector3(transform.position.x + dirVec.x * 0.5f, transform.position.y + dirVec.y * 0.5f, 0.0f);
            // End getting overworld position
            spawnBullet(dirVec, spawnLoc);

            if (stats.getEffectPower(EffectType.SPREAD_SHOT) > 0)
            {
                dirVec = Rotate(dirVec, 15.0f);
                spawnBullet(dirVec, spawnLoc);

                dirVec = Rotate(dirVec, -30.0f);
                spawnBullet(dirVec, spawnLoc);
            }

            // After shooting, set the timer to the cooldown amount.
            float multiplier = (float)stats.getEffectPower(EffectType.ATTACK_RATE_UP);
            if (multiplier == 0) multiplier = 1;

            shootTimer = (int)((float)shootCD * multiplier);
        }
    }

    private Vector2 Rotate(Vector2 v, float degrees)
    {
        float sin = Mathf.Sin(degrees * Mathf.Deg2Rad);
        float cos = Mathf.Cos(degrees * Mathf.Deg2Rad);

        float tx = v.x;
        float ty = v.y;
        v.x = (cos * tx) - (sin * ty);
        v.y = (sin * tx) + (cos * ty);
        return v;
    }

public void spawnBullet(Vector2 dirVec, Vector3 spawnLoc)
    {
        GameObject bullet = Instantiate(playerBullet, spawnLoc, Quaternion.identity);
        PlayerBullet pb = bullet.GetComponent<PlayerBullet>();
        pb.setDirection(dirVec);

        if (stats.getEffectPower(EffectType.GLASS_CANNON) > 0)
        {
            pb.setDamage(30);
        }

        // Ignore collisions with irrelevant layers and the player
        Collider2D bulletCollider = bullet.GetComponent<Collider2D>();

        disableEdgeCollisions(bulletCollider);

        Physics2D.IgnoreCollision(playerCollider, bullet.GetComponent<Collider2D>());
    }

    // Define the tryMoveVector according to the accelframes and scaleMoveDirs of the 4 cardinal vectors summed.
    private void computeMove()
    {
        // Accel up
        if (isMovingUp) { if (framesMovingDir[(int)Facing.NORTH] < accelFrames) framesMovingDir[(int)Facing.NORTH] += 1; }
        // Decel up
        else { if (framesMovingDir[(int)Facing.NORTH] > 0) framesMovingDir[(int)Facing.NORTH] -= 1; }

        // Accel down
        if (isMovingDown) { if (framesMovingDir[(int)Facing.SOUTH] < accelFrames) framesMovingDir[(int)Facing.SOUTH] += 1; }
        // Decel down
        else { if (framesMovingDir[(int)Facing.SOUTH] > 0) framesMovingDir[(int)Facing.SOUTH] -= 1; }

        // Accel right
        if (isMovingRight) { if (framesMovingDir[(int)Facing.EAST] < accelFrames) framesMovingDir[(int)Facing.EAST] += 1; }
        // Decel right
        else { if (framesMovingDir[(int)Facing.EAST] > 0) framesMovingDir[(int)Facing.EAST] -= 1; }

        // Accel left
        if (isMovingLeft) { if (framesMovingDir[(int)Facing.WEST] < accelFrames) framesMovingDir[(int)Facing.WEST] += 1; }
        // Decel left
        else { if (framesMovingDir[(int)Facing.WEST] > 0) framesMovingDir[(int)Facing.WEST] -= 1; }

        Vector2 tryMoveVector = new Vector2
            (
                // East
                ((float)framesMovingDir[(int)Facing.EAST] / (float)accelFrames)
                // West
                - ((float)framesMovingDir[(int)Facing.WEST] / (float)accelFrames),
                // North
                ((float)framesMovingDir[(int)Facing.NORTH] / (float)accelFrames)
                // South
                - ((float)framesMovingDir[(int)Facing.SOUTH] / (float)accelFrames)
            );
        moveVector = scaleMoveDir * tryMoveVector.normalized;
        applyForces();
    }

    private void applyForces()
    {

    }


    private void makeMove()
    {
        //Debug.Log("Vector: (" + moveVector.x + ", " + moveVector.y + ") :: Magnitude: " + moveVector.magnitude);
        Rigidbody2D r2 = gameObject.GetComponent<Rigidbody2D>();
        r2.MovePosition(new Vector2(transform.position.x + moveVector.x, transform.position.y + moveVector.y));
    }

    public void setFreezeInputs(bool val)
    {
        inputFrozen = val;
    }

    public bool getFreezeInputs()
    {
        return inputFrozen;
    }

    /**
     * This should *ALWAYS* be used, rather than facing = dir;
     */
    public void setFacing(Facing dir)
    {
        facing = dir;
        // Code that actually reorients the player sprite here.
    }

    public void setLayer(int newLayer)
    {
        layer = newLayer;
        updateLayerData(newLayer);
    }

    public int getLayer()
    {
        return layer;
    }

    public void disableEdgeCollisions(Collider2D collider)
    {
        if (collider != null)
        {
            foreach (GameObject map in colliderLayers)
            {
                CompositeCollider2D c = map.GetComponent<CompositeCollider2D>();
                if (collider != null)
                {
                    Physics2D.IgnoreCollision(c, collider, true);
                }
            }
        }
    }

    // Black Box Render State Util
    public bool getFadeIn() { return fadeIn; }
    public bool getFadeOut() { return fadeOut; }
    public void setFadeIn(bool val) { fadeIn = val; }
    public void setFadeOut(bool val) { fadeOut = val; }


    public bool getMoving(Facing dir)
    {
        switch (dir)
        {
            case (Facing.NORTH):
                return moveVector.y > 0;
            case (Facing.EAST):
                return moveVector.x > 0;
            case (Facing.SOUTH):
                return moveVector.y < 0;
            case (Facing.WEST):
                return moveVector.x < 0;
            default:
                return false;
        }
    }

    // Helper method to run all the functions needed to change layer number.
    private void updateLayerData(int newLayer)
    {
        // First, load the correct collider
        for(int i = 0; i < colliderLayers.Length; i++)
        {
            GameObject collider = colliderLayers[i];
            // If the collider is not the new active layer collider and is currently active
            if (i != newLayer)
            {
                // Deactivate it.
                if(collider.activeInHierarchy)
                {
                    collider.SetActive(false);
                }
            }
            else
            {
                // Otherwise, this is the collider we should turn on.
                collider.SetActive(true);
            }
        }

        // Now, bullet colliders
        for (int i = 0; i < bulletColliderLayers.Length; i++)
        {
            GameObject collider = bulletColliderLayers[i];
            // If the collider is not the new active layer collider and is currently active
            if (i != newLayer)
            {
                // Deactivate it.
                if (collider.activeInHierarchy)
                {
                    collider.SetActive(false);
                }
            }
            else
            {
                // Otherwise, this is the collider we should turn on.
                collider.SetActive(true);
                // Make sure the player doesn't collide with it.
                CompositeCollider2D compositeCollider = collider.GetComponent<CompositeCollider2D>();
                if(compositeCollider != null)
                {
                    Physics2D.IgnoreCollision(playerCollider, compositeCollider, true);
                }
            }
        }

        // Push decoration to the right level. Same as walkable pushes.
        for (int i = 0; i < decorLayers.Length; i++)
        {
            GameObject decor = decorLayers[i];
            // Get the renderer
            TilemapRenderer renderer = decor.GetComponent<TilemapRenderer>();
            // Crash check. Should never fail.
            if (renderer != null)
            {
                // If i is below newLayer or at newLayer.
                if (i <= newLayer)
                {
                    // Set it to the background tilemap
                    renderer.sortingLayerName = "BGDTilemap";
                }
                else
                {
                    // Otherwise, set it to a Foreground tilemap
                    renderer.sortingLayerName = "FGDTilemap";
                }
                // Set the renderer's order in layer according to it's tilemap ID. This works for both FG and BG tilemaps.
                renderer.sortingOrder = i;
            }
            // Something that was not a walkable tilemap was passed here, so an error prints out accordingly.
            else
            {
                Debug.LogError("Are you using a non-decor tilemap in the decor Tilemaps field?");
            }
        }

        // Now, make sure tilemap rendering is updated correctly so the player is masked by the right layers.
        for (int i = 0; i < walkableLayers.Length; i++)
        {
            GameObject walkable = walkableLayers[i];
            // Get the renderer
            TilemapRenderer renderer = walkable.GetComponent<TilemapRenderer>();
            // Crash check. Should never fail.
            if (renderer != null)
            {
                // If i is below newLayer or at newLayer.
                if (i <= newLayer)
                {
                    // Set it to the background tilemap
                    renderer.sortingLayerName = "BGTilemap";
                }
                else
                {
                    // Otherwise, set it to a Foreground tilemap
                    renderer.sortingLayerName = "FGTilemap";
                }
                // Set the renderer's order in layer according to it's tilemap ID. This works for both FG and BG tilemaps.
                renderer.sortingOrder = i;
            }
            // Something that was not a walkable tilemap was passed here, so an error prints out accordingly.
            else
            {
                Debug.LogError("Are you using a non-walkable tilemap in the walkable Tilemaps field?");
            }
        }
    }
}
