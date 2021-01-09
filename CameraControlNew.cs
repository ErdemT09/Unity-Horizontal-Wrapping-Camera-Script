using UnityEngine;

public class CameraControlNew : MonoBehaviour
{
    [SerializeField] private float CamSpeed = 12f; //The base speed the camera can be moved around with for the variable under this
    private float camSpeed ; // The speed that camera moves at, value of this can be changed
    Vector2 pos; //The position the camera is moved to
    public Vector2 LimitCoord; //Spacial limits of the camera's movement
    public Camera cam; //This camera, set to "Depth Only"
    public Camera SideCam; //Other camera, depth of this camera is lower than the main camera
    public Vector2 lockedPosition; 
    public bool lockedPos = false; //Sometimes locking the camera is needed, used for that
    public float MaxHorizPos; //Rightmost posiiton of the camera
    public float MinHorizPos; //Leftmost
    public float width;//Actually, half of the camera's view, it is more useful like this
    float widthShifted = -0.2f; 
    public float MapLength;
    private void Start()
    {
        width = cam.orthographicSize * cam.aspect; //Basically, by ortographic cameras, the half width of the camera is its size times the aspect ratio
        // For example, on a 16/9 screen, if size equals to 9, the half width equals to 9* (16/9)=16
        widthShifted += width; //The shifted width will be useful later on
        camSpeed = CamSpeed;
        MaxHorizPos = MapLength - 1f +0.5f - width ;
        //Find the rightmost visible thing in your map substract the camera width from it, since after that point, camera will see blank space
        MinHorizPos = -MapLength - 0.5f + width;
        //Same logic appkies to this as well
        pos = transform.position;
    }
    void Update()
    {

        if (lockedPos == false)
        {
            pos = transform.position;
            MouseInput();
            KeyboardInput();
            pos.x = Mathf.Clamp(pos.x, -LimitCoord.x, LimitCoord.x);
            pos.y = Mathf.Clamp(pos.y, -LimitCoord.y, LimitCoord.y);
            transform.position = new Vector3(pos.x, pos.y, transform.position.z);
        }
        else { transform.position = new Vector3(lockedPosition.x, lockedPosition.y, transform.position.z); }

        HorizontalWrap();
        //This is the horizontal wrap part, the rest of the update function is for camera movement
    }

    private void HorizontalWrap()
    {
        if (transform.position.x >= MaxHorizPos + widthShifted)
        {
            transform.position = new Vector3(MinHorizPos +0.8f + (transform.position.x - MaxHorizPos - width), transform.position.y, transform.position.z);
            

            cam.rect = new Rect(0, 0, 1, 1);
            camSpeed = CamSpeed;
            //if camera position is greater than rightmost object - 0.2f, then we teleport the camera to the other side of the screen
            // it is -0.2f because of the fact that camera can experience problems when its width is lower than zero
        }
        else if (transform.position.x > MaxHorizPos-0.5f)
        {
            camSpeed = CamSpeed * 0.5f;
            SideCam.gameObject.SetActive(true);
            cam.rect = new Rect(cam.rect.x, cam.rect.y, 1 - (transform.position.x - MaxHorizPos) / width, cam.rect.height);
            if (transform.position.x > MaxHorizPos)
            {
                SideCam.transform.position = new Vector3(MinHorizPos - (2 * width) + (transform.position.x - MaxHorizPos + 0.5f) * 2f, transform.position.y, SideCam.transform.position.z); 
            }
            else
            {
                SideCam.transform.position = new Vector3(MinHorizPos - (2 * width) + (transform.position.x - MaxHorizPos + 1f), transform.position.y, SideCam.transform.position.z);
            }
            //Camera speed is halvened, otherwise camera would in double its speed
            //The width of the camera rect is lowered, while side camera's position is changed to display the other side of the map and to cover the blank space
        }
        else if (transform.position.x <= MinHorizPos - widthShifted)
        {
            transform.position = new Vector3(MaxHorizPos -0.8f - (MinHorizPos - transform.position.x - width), transform.position.y, transform.position.z);
            Debug.Log(transform.position);

            cam.rect = new Rect(0, 0, 1, 1);
            camSpeed = CamSpeed;
        }
        else if (transform.position.x < MinHorizPos+1f)
        {
            camSpeed = CamSpeed * 0.5f;
            SideCam.gameObject.SetActive(true);
            
            if (transform.position.x < MinHorizPos)
            {
                cam.rect = new Rect((MinHorizPos - transform.position.x) / width, cam.rect.y, 1, cam.rect.height);
                SideCam.transform.position = new Vector3(MaxHorizPos + (2 * width) - (MinHorizPos - transform.position.x + 0.5f) * 2f, transform.position.y, SideCam.transform.position.z); 
            }
            else
            {
                SideCam.transform.position = new Vector3(MaxHorizPos + (2 * width) - (MinHorizPos - transform.position.x + 1f), transform.position.y, SideCam.transform.position.z);
            }
            //This time, we play around with the camera rect's x value, so it starts from a location suitably to the right of the screen
            //The rest is the same as going from right to the left side of the screen
        }
        else
        {
            cam.rect = new Rect(0, 0, 1, 1);
            camSpeed = CamSpeed;
            //if camera is in the middle and such, everything is set to normal
        }
    }

    private void MouseInput()
    {
        if (Input.GetMouseButton(0)) 
        {
            // Hold button and drag camera around
            pos -= new Vector2(Input.GetAxis("Mouse X") * camSpeed*3f* Time.deltaTime,
                               Input.GetAxis("Mouse Y") * camSpeed *3f * Time.deltaTime);
        }
    }
    private void KeyboardInput()
    {
        if(Input.GetKey("w"))
        {
            pos.y += camSpeed * Time.deltaTime;
        }
        if (Input.GetKey("s"))
        {
            pos.y -= camSpeed * Time.deltaTime;
        }
        if (Input.GetKey("a"))
        {
            pos.x -= camSpeed * Time.deltaTime;
        }
        if (Input.GetKey("d"))
        {
            pos.x += camSpeed * Time.deltaTime;
        }
    }
    public void SetLocationInit(float pX, float pY)
    {
        transform.position = new Vector3(pX, pY, transform.position.z);
    }
    public void lockPosition(Vector2 lp)
    {
        lockedPos = true;
        lockedPosition = lp;
    }
    public void stopLock()
    {
        lockedPos = false;
    }
}
