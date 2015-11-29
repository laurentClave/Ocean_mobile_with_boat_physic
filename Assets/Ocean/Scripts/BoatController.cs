using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BoatController : Boyancy{

    [SerializeField] private List<GameObject> m_motors;

	[SerializeField] private bool m_enableAudio = true;
	[SerializeField] private AudioSource m_boatAudioSource;
	[SerializeField] private float m_boatAudioMinPitch = 0.4F;
	[SerializeField] private float m_boatAudioMaxPitch = 1.2F;

	[SerializeField] private float m_accelerationFactor = 2.0F;
	[SerializeField] private float m_turningFactor = 2.0F;
    [SerializeField] private float m_accelerationTorqueFactor = 35F;
	[SerializeField] private float m_turningTorqueFactor = 35F;

  
    private Rigidbody m_rigidbody;

    protected override void Start()
    {
        base.Start();

        m_rigidbody = GetComponent<Rigidbody>();
        m_rigidbody.drag = 1;
        m_rigidbody.angularDrag = 1;
    }

    protected override void FixedUpdate () {

        base.FixedUpdate();

        float verticalInput = Input.GetAxisRaw("Vertical");
        float horizontalInput = Input.GetAxisRaw("Horizontal");

        m_rigidbody.AddRelativeForce(Vector3.forward * verticalInput * m_accelerationFactor);
        m_rigidbody.AddRelativeTorque(
            verticalInput * -m_accelerationTorqueFactor,
            horizontalInput * m_turningFactor,
            horizontalInput * -m_turningTorqueFactor
        );

        if(m_motors.Count > 0)
        {
            float motorRotationAngle = 0F;
			float motorMaxRotationAngle = 70;

			motorRotationAngle = - horizontalInput * motorMaxRotationAngle;


            foreach (GameObject motor in m_motors)
            {
				float currentAngleY = motor.transform.localEulerAngles.y;
				if (currentAngleY > 180.0f)
					currentAngleY -= 360.0f;

				float localEulerAngleY = Mathf.Lerp(currentAngleY, motorRotationAngle, Time.deltaTime*10);
				motor.transform.localEulerAngles = new Vector3(
					motor.transform.localEulerAngles.x,
					localEulerAngleY,
					motor.transform.localEulerAngles.z
				);
            }
        }

		if (m_enableAudio && m_boatAudioSource != null) 
		{
			float pitchLevel = verticalInput * m_boatAudioMaxPitch;
			if (pitchLevel < m_boatAudioMinPitch)
				pitchLevel = m_boatAudioMinPitch;
			float smoothPitchLevel = Mathf.Lerp(m_boatAudioSource.pitch, pitchLevel, Time.deltaTime);

			m_boatAudioSource.pitch = smoothPitchLevel;
		}
    }
}
