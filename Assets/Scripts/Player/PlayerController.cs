using UnityEngine;

// PlayerScript requires the GameObject to have a PlayerMotor component
[RequireComponent(typeof(Animation))]
[RequireComponent(typeof(PlayerMotor))]//привязывает скрипт к объекту
[RequireComponent(typeof(ConfigurableJoint))]
public class PlayerController : MonoBehaviour
{

    //показывает приватное поле в эдиторе
    [SerializeField]
    private float speed = 5f;//скорость движения игрока

    [SerializeField]
    private float lookSensitivity = 5f;//чувствительность мышки


    [SerializeField]
    private float thrusterForce = 1000f;// толкательная сила - сила тяги(сила прыжка)


    [SerializeField]
    private float thrusterFuelBurnSpeed = 1f;//скорость сжигания топлива силы тяги
    [SerializeField]
    private float thrusterFuelRegenSpeed = 0.3f;//скорость востоновления топлива силы тяги

    private float thrusterFuelAmount = 1f;//всего топлива


    public float GetThrusterFuelAmount()
    {
        return thrusterFuelAmount;
    }


    [SerializeField]
    private LayerMask envirinmentMask;//маска слоя среды


    //выносим параметры из Configurable Joint в скрипт (полёт игрока)
    [Header("Spring settings:")]
    //[SerializeField]
    //private JointDriveMode jointMode = JointDriveMode.Position;
    [SerializeField]
    private float jointSpring = 20f;//сила пружины прыжка
    [SerializeField]
    private float jointMaxForce = 40f;//максимальная сила пружины


    //Components caching
    private Animator animator; //ссылка на анимацию движения
    private PlayerMotor motor; //ссылка на объект класса PlayerMotor
    private ConfigurableJoint joint; //ссылка на Configurable Joint


    void Start()
    {
        //set up references
        motor = GetComponent<PlayerMotor>();//получаем объект класса который указали через аттрибут
        joint = GetComponent<ConfigurableJoint>();
        animator = GetComponent<Animator>();

        SetJointSettings(jointSpring);
    }

    void Update()
    {


        //if pause is ON
        if (PauseMenu.isOn)
            return;

        //Setting target positiong for spring
        //This make physics act right when it comes to applying gravity when flying over objects;
        RaycastHit _hit;
        if (Physics.Raycast(transform.position, Vector3.down, out _hit, 100f, envirinmentMask))
            joint.targetPosition = new Vector3(0f, -_hit.point.y, 0f);
        else
            joint.targetPosition = new Vector3(0f, 0f, 0f);
        


        #region MOVEMENT
        //рассчёт скорости движения как 3D vector
        float _xMovement = Input.GetAxis("Horizontal");//получение направления движения по горизонтали от -1 0 1
        float _zMovement = Input.GetAxis("Vertical");//получение направления движения по вертикали от -1 0 1

        //составляем вектор направления движения например (-1 0 0) - влево\ (0 0 1) - вперёд
        Vector3 _movementHorizontal = transform.right * _xMovement;//красная ось редактора
        Vector3 _movementVertical = transform.forward * _zMovement;//синяя ось редактора

        //Final movement vector
        //нормализуем вектор - его длина равна единице * скорость = скорость изменения координат в простаранстве
        Vector3 _velocity = (_movementHorizontal + _movementVertical).normalized * speed;


        //Animate movement
        animator.SetFloat("ForwardVelocity", _zMovement);







        //Apply movement
        motor.Move(_velocity);//передаём рассчитаный вектор в класс PlayerMotor метод Move
        #endregion
        #region ROTATION_X
        //Calculate rotation as a 3D vector(поворот вокруг)
        float _yRotation = Input.GetAxisRaw("Mouse X");//вращаем вокруг оси Y, саму мышь двигаем по X

        Vector3 _rotation = new Vector3(0f, _yRotation, 0f) * lookSensitivity;

        //Apply rotation
        motor.Rotate(_rotation);
        #endregion
        #region ROTATION_CAMERA_Y
        //Calculate camera rotation as a 3D vector(поворот камеры вверх-вниз)
        //если мы будем поворачивать игрока вверх-вниз, то он будет двигаться соответственно,
        //а нам надо чтобы поворачивалась только камера
        float _xRotation = Input.GetAxisRaw("Mouse Y");//вращаем вокруг оси Y, саму мышь двигаем по X

        float _cameraRrotationX = _xRotation * lookSensitivity;

        //Apply camera rotation
        motor.RotateCamera(_cameraRrotationX);
        #endregion
        #region THRUSTER_FORCE
        //Calculate the thruster force based on player input
        Vector3 _thrusterForce = Vector3.zero;//пустой вектор силы тяги

        if (Input.GetButton("Jump") && thrusterFuelAmount > 0f) //если нажата кнопка прыжок и количество горючего больше нуля
        {
            //уменьшаем количество горючего
            thrusterFuelAmount -= thrusterFuelBurnSpeed*Time.deltaTime;

            //если топлива больше нуля, чтобы избавится от бага зависания в воздухе
            if (thrusterFuelAmount >= 0.01f)
            {
                //увеличиваем координату по Y на значение переменной прыжка(сила тяги)
                //координаты после применения силы тяги
                _thrusterForce = Vector3.up * thrusterForce;
                SetJointSettings(0f);//сила тяги равна нулю
            }
        }
        else
        {
            //регеним количество горючего
            thrusterFuelAmount += thrusterFuelRegenSpeed * Time.deltaTime;

            SetJointSettings(jointSpring);//сила тяги равна чему-то
        }

        //значения к-ва топлива от 0 - до 1
        thrusterFuelAmount = Mathf.Clamp(thrusterFuelAmount, 0f, 1f);




        //Apply the thruster force
        motor.ApplyThruster(_thrusterForce);//передаём координату прыжка в мотор
        #endregion
    }

    private void SetJointSettings(float _jointSpring)
    {
        //изменяем параметры скриптом
        joint.yDrive = new JointDrive {positionSpring = _jointSpring, maximumForce = jointMaxForce};
    }


}
