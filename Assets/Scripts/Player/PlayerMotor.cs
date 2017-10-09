using UnityEngine;

// PlayerScript requires the GameObject to have a Rigidbody component
[RequireComponent(typeof(Rigidbody))]//привязывает компонент ТТ к объекту
public class PlayerMotor : MonoBehaviour
{


    [SerializeField]
    private Camera cam;//ссылка на камеру


    private Vector3 velocity = Vector3.zero;//начальное значение скорости перемещения
    private Vector3 rotation = Vector3.zero;//начальное значение скорости вращения
    private float cameraRotationX = 0f;//начальное значение скорости вращения камеры относительно OX
    private float currentCameraRotationX = 0f;//текущее положение камеры относительно OX
    private Vector3 thrusterForce = Vector3.zero;//начальное значение силы тяги


    [SerializeField]
    private float cameraRotationLimit = 85f;//максимальный урол поворота камеры



    private Rigidbody rb;//ссылка на ТТ

    void Start()
    {
        //set up references
        rb = GetComponent<Rigidbody>();//получаем объект класса который указали через аттрибут
    }

    public void Move(Vector3 _velocity)
        //Gets a movement vector
    {
        velocity = _velocity;
    }

    public void Rotate(Vector3 _rotation)
    //Gets a rotational vector
    {
        rotation = _rotation;
    }

    public void RotateCamera(float _cameraRotationX)
    //Gets a camera rotation vector
    {
        cameraRotationX = _cameraRotationX;
    }

    public void ApplyThruster(Vector3 _thrusterForce)
        //Get a force vector for our thrusters
    {
        thrusterForce = _thrusterForce;
    }


    void FixedUpdate()
        //Run every physics iterations
    {
        PerformMovement();//выполнить движение perform = выполнить
        PerformRotation();//выполнить вражение perform = выполнить
        PerformCameraRotation();//выполнить вращение камеры perform = выполнить
        PerformThrusterForce();//применить силу тяги

    }

    //Perform movement based on velocity variable
    void PerformMovement()
    {
        if (velocity != Vector3.zero)
        {
            //перемещаем наше тело
            rb.MovePosition(rb.position + velocity * Time.deltaTime);
        }
    }

    //Perform rotational based on velocity variable
    void PerformRotation()
    {
        if (rotation != Vector3.zero)
        {
            //вращаем наше тело
            rb.MoveRotation(rb.rotation * Quaternion.Euler(rotation));
        }
    }

    //Perform camera rotational based on velocity variable
    void PerformCameraRotation()
    {
        //если камера подключена
        if (cam != null)
        {
            //cam.transform.Rotate(-cameraRotation);//вращаем камеру вверх-вниз
            //изменяем текущее положение камеры
            currentCameraRotationX -= cameraRotationX;
            //заключаем значение в промежуток
            currentCameraRotationX = Mathf.Clamp(currentCameraRotationX, -cameraRotationLimit, cameraRotationLimit);
            //устанавливаем значение камеры как Vector3
            cam.transform.localEulerAngles = new Vector3(currentCameraRotationX, 0f, 0f);
        }
    }

    void PerformThrusterForce()
    {
        if (thrusterForce != Vector3.zero)
        {
            //перемещаем наше тело
            rb.AddForce(thrusterForce * Time.fixedDeltaTime, ForceMode.Acceleration);
        }
    }



}
