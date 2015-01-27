package com.brujula_ts.brujula;




import android.app.Activity;
import android.content.Intent;
import android.hardware.Sensor;
import android.hardware.SensorEvent;
import android.hardware.SensorEventListener;
import android.hardware.SensorManager;
import android.os.Bundle;
import android.view.Menu;
import android.view.MenuItem;
import android.view.animation.Animation;
import android.view.animation.RotateAnimation;
import android.widget.ImageView;
import android.widget.TextView;



public class MainActivity extends Activity implements SensorEventListener {
	
	private ImageView image;
	
	private float currentDegree =0f;
	
	private SensorManager mSensorManager;
	
	public float punto = 9340;
	
	public float prueba =180;
	
	
	TextView tvHeading;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_main);
        
        image=(ImageView) findViewById(R.id.imageViewCompass);
        
        tvHeading = (TextView) findViewById(R.id.tvHeading);
        
        mSensorManager = (SensorManager) getSystemService(SENSOR_SERVICE);
         
        
    }
        
    @SuppressWarnings("deprecation")
	protected void onResume() {
        super.onResume();
         
        // for the system's orientation sensor registered listeners
        mSensorManager.registerListener(this, mSensorManager.getDefaultSensor(Sensor.TYPE_ORIENTATION),
                SensorManager.SENSOR_DELAY_GAME);
    }
 
    @Override
    protected void onPause() {
        super.onPause();
         
        // to stop the listener and save battery
        mSensorManager.unregisterListener(this);
    }
  
    @Override
    public boolean onCreateOptionsMenu(Menu menu) {
        // Inflate the menu; this adds items to the action bar if it is present.
        getMenuInflater().inflate(R.menu.main, menu);
        return true;
    }

    @Override
    public boolean onOptionsItemSelected(MenuItem item) {
        // Handle action bar item clicks here. The action bar will
        // automatically handle clicks on the Home/Up button, so long
        // as you specify a parent activity in AndroidManifest.xml.
        int id = item.getItemId();
        if (id == R.id.action_settings) {
            return true;
        }
        return super.onOptionsItemSelected(item);
    }


	@Override
	public void onAccuracyChanged(Sensor arg0, int arg1) {
		// TODO Auto-generated method stub
		
	}


	@Override
	public void onSensorChanged(SensorEvent event) {
		// TODO Auto-generated method stub
		
		  // get the angle around the z-axis rotated
      //  float degree = Math.round(event.values[0]); 
		
		//int adas = (int) ((Math.round(event.values[0])+prueba)%360); 
		//float degree = (Math.round(event.values[0])); 
		
		float degree = (Math.round(event.values[0])); 
		
		
		
      //  float degree = LowPassFilter.filter3D( event.values[0], ((float)-currentDegree) );
 
		// float degree = Math.round(LowPassFilter.filter3D( event.values[0], ((float)-currentDegree)));
		
        tvHeading.setText("Heading: " + Float.toString(degree) + " degrees");
        
        if(degree==punto){
        	lanzaCamara();
        }
 
        // create a rotation animation (reverse turn degree degrees)
        RotateAnimation ra = new RotateAnimation(
                currentDegree, 
                -degree,
                Animation.RELATIVE_TO_SELF, 0.5f, 
                Animation.RELATIVE_TO_SELF,
                0.5f);
 
        // how long the animation will take place
        ra.setDuration(210);
 
        // set the animation after the end of the reservation status
        ra.setFillAfter(true);
 
        // Start the animation
        image.startAnimation(ra);
        currentDegree = -degree;
 
    }
	
	public void lanzaCamara(){
		Intent cam_int=new Intent(this, Camara.class);
		startActivity(cam_int);
	}
	
	
}
