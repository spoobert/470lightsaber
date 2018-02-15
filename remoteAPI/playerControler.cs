using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WiimoteApi;
	
//kinetics calculus
public class playerControler : MonoBehaviour {

	private Rigidbody rb; 
	public float speed;
	public float saberspeed;
	private Wiimote wiimote;
	private bool activated = false;

	void Start()
	{ 
		rb = GetComponent< Rigidbody >();
		InitWiimotes ();

	}

	void OnDestroy ()
	{
		FinishedWithWiimotes ();
	}
		

	void FixedUpdate()
	{
		UpdateWiiMote ();

		if (!activated && wiimote.wmp_attached) {
			wiimote.ActivateWiiMotionPlus();
			activated = true;
			Debug.Log ("activating wmp");

		}

		if(/*activated &&*/ wiimote.current_ext == ExtensionController.MOTIONPLUS) {
			MotionPlusData data = wiimote.MotionPlus; // data!
			float dPitch = data.PitchSpeed;
			float dRoll = data.RollSpeed;

			Debug.Log (dRoll);
			//Vector3 mv = new Vector3 (0.0f, dPitch, 0.0f);
			transform.RotateAround(transform.position, Vector3.right, dPitch * Time.deltaTime );
			transform.RotateAround(transform.position, Vector3.forward, dRoll * Time.deltaTime );
				
		}

		float moveHorizontal = getHoizontal();
		float moveVertical = getVertical();
		Vector3 movement = new Vector3 (moveHorizontal, 0.0f, moveVertical);
		rb.AddForce (movement * speed);
	}

	float getHoizontal ()
	{
		
		if ( wiimote == null)
			return 0.0f;
		if (wiimote.Button.d_left) 
		{

			return -1.0f;
		}
		if (wiimote.Button.d_right) 
		{

			return 1.0f;
		}
		return 0.0f;

	}

	float getVertical ()
	{
		if (wiimote == null)
			return 0.0f;
		if (wiimote.Button.d_up) 
		{

			return 1.0f;
		}
		if (wiimote.Button.d_down) 
		{

			return -1.0f;
		}

		return 0.0f;
	}

	void InitWiimotes() 
	{
		WiimoteManager.FindWiimotes ();
		if ( WiimoteManager.HasWiimote()) // Poll native bluetooth drivers to find Wiimotes
		{	
			wiimote = WiimoteManager.Wiimotes[0];
			wiimote.SendPlayerLED (true, true, true, true);
			Debug.Log (wiimote.Status.battery_level);
			wiimote.SendDataReportMode (InputDataType.REPORT_BUTTONS_EXT19);
			wiimote.RequestIdentifyWiiMotionPlus ();
			wiimote.ActivateWiiMotionPlus();

		}
	}
	void FinishedWithWiimotes()
	{

		if (wiimote != null) 
		{
			WiimoteManager.Cleanup (wiimote);
		}
		
	}


	void UpdateWiiMote( ) { // called once per frame (for example)
		int ret;
		do
		{
			ret = wiimote.ReadWiimoteData();
		} while (ret > 0); // ReadWiimoteData() returns 0 when nothing is left to read.  So by doing this we continue to
		// update the Wiimote until it is "up to date."
	}


}



