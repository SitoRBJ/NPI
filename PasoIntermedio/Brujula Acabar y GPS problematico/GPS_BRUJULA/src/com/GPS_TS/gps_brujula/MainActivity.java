package com.GPS_TS.gps_brujula;







import java.util.ArrayList;

import android.app.Activity;
import android.content.Context;
import android.content.Intent;
import android.hardware.Sensor;
import android.hardware.SensorEvent;
import android.hardware.SensorEventListener;
import android.hardware.SensorManager;
import android.location.Location;
import android.location.LocationListener;
import android.location.LocationManager;
import android.os.Bundle;
import android.speech.RecognizerIntent;
import android.view.View;
import android.view.View.OnClickListener;
import android.view.animation.Animation;
import android.view.animation.RotateAnimation;
import android.widget.Button;
import android.widget.ImageView;
import android.widget.TextView;





public class MainActivity extends Activity implements SensorEventListener {
	
	TextView t1;
	TextView t2;
	
	Double longitud, latitud;
	Double long_objetivo, lat_objetivo, long_objetivo_neg, lat_objetivo_neg;
	
	private float currentDegree =0f;
	private ImageView image, flecha;
	
	private float antiguo = 0F;
	public Button boton_latitud;
	public Button boton_longitud;
	
	private SensorManager miSensorManager; 
	
	public boolean lat_negativa;
	public boolean long_negativa;
	public String lat_resultado;
	public String long_resultado;
	public boolean puede_lanzarA=false, puede_lanzarB=false;
	
	
	
	private static final int VOICE_RECOGNITION_REQUEST_CODE_LAT = 1;
	private static final int VOICE_RECOGNITION_REQUEST_CODE_LONG = 2;
	
	
    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_main);
        
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_main);
        
        lat_negativa = false;
        long_negativa=false;
        
        
        
        t1=(TextView)findViewById(R.id.TextLat);
        t2=(TextView)findViewById(R.id.TextLong);
        boton_latitud=(Button)findViewById(R.id.intro_lat);
        boton_longitud=(Button)findViewById(R.id.intro_long);
        image=(ImageView) findViewById(R.id.imageViewCompass);
        flecha=(ImageView) findViewById(R.id.imageViewflecha);
        
        miSensorManager = (SensorManager)getSystemService(SENSOR_SERVICE);
        
        LocationManager lm = (LocationManager)getSystemService(Context.LOCATION_SERVICE);
        LocationListener ll = new mylocationListener();
        lm.requestLocationUpdates(LocationManager.GPS_PROVIDER, 0, 0, ll);
        
        
      
        boton_latitud.setOnClickListener(new View.OnClickListener(){
        	
        	public void onClick(View v){
        		
        		escuchar1();
        		
        	}
        });
        
        boton_longitud.setOnClickListener(new View.OnClickListener() {
			
			@Override
			public void onClick(View v) {
				// TODO Auto-generated method stub
				escuchar2();
			}
		});
    }
    
    
    // CODIGO DE GPS COMIENZO

    class mylocationListener implements LocationListener{

		@Override
		public void onLocationChanged(Location location) {
			// TODO Auto-generated method stub
			
			if(location !=null){
				double pLong=location.getLongitude();
				double pLat = location.getLatitude();
				
				t1.setText(Double.toString(pLat));
				t2.setText(Double.toString(pLong));
				
				latitud =pLat;
				longitud = pLong;							      
			}
			
		}
		
		@Override
		public void onProviderDisabled(String arg0) {
			// TODO Auto-generated method stub
			
		}

		@Override
		public void onProviderEnabled(String arg0) {
			// TODO Auto-generated method stub
			
		}

		@Override
		public void onStatusChanged(String arg0, int arg1, Bundle arg2) {
			// TODO Auto-generated method stub
			
		}
    	
    }
    
    // TERMINA GPS
    
    
    
    // OPCIONES DE IMAGEN
    
    public void llevame(SensorEvent event){
		
		//float degree =(float) calculaPunto(); 
    	
    	//float diferencia;
    	boolean llego=false;
    	float degree = (Math.round(event.values[0]));
		
    	float mi_punto = calculaPunto();
    	
    	//diferencia = mi_punto - degree;
    	
    	//degree=degree+diferencia; 	
				    	 
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
    
    public float calculaPunto(){
		
		boolean derecha=false;
		boolean encima=false;
		double base_longitud;
		double base_latitud;
		double hipotenusa;
		double resultado=0;
		
		if(longitud>long_objetivo){
			derecha=true;
			base_longitud=Math.abs(longitud - long_objetivo);
		}
		else{
			derecha=false;
			base_longitud=Math.abs(long_objetivo-longitud);
		}
		
		if(latitud>lat_objetivo){
			encima=true;
			base_latitud=Math.abs(latitud-lat_objetivo);
		}
		else{
			encima=false;
			base_latitud=Math.abs(lat_objetivo-latitud);
		}
		
		hipotenusa=Math.sqrt((Math.pow(base_longitud, 2))+(Math.pow(base_latitud, 2)));
		
		if(derecha==true && encima==true){
			
			resultado=Math.asin((base_longitud/hipotenusa));			//tercer cuadrante
			resultado=resultado+180;
		}
		if(derecha==true && encima==false){
			resultado=Math.asin((base_latitud/hipotenusa));				//cuarto cuadrante
			resultado=resultado+270;
		}
		if(derecha==false && encima==true){								//segundo cuadrante
			resultado=Math.asin((base_latitud/hipotenusa));
			resultado=resultado+90;
		}
		if(derecha==false && encima==false){
			resultado=Math.asin((base_longitud/hipotenusa));			//primer cuadrante
		}
		
		
		return (float) resultado;
		
	}

	@Override
	public void onAccuracyChanged(Sensor arg0, int arg1) {
		// TODO Auto-generated method stub
		
	}

	@Override
	public void onSensorChanged(SensorEvent event) {
		// TODO Auto-generated method stub
		if(puede_lanzarA==true && puede_lanzarB==true){
			llevame(event);		
		}		
	}
    
    @SuppressWarnings("deprecation")
	protected void onResume(){
    	super.onResume();
    	
    	miSensorManager.registerListener(this, miSensorManager.getDefaultSensor(Sensor.TYPE_ORIENTATION), SensorManager.SENSOR_DELAY_GAME);
    }
    protected void onPause(){
    	super.onPause();
    	
    	miSensorManager.unregisterListener(this);
    }
    
    
    // FIN OPCIONES IMAGEN
    
    
    // COMIENZA CAPTURA DE VOZ
    
    private void escuchar1(){
    	
    	Intent intent = new Intent(RecognizerIntent.ACTION_RECOGNIZE_SPEECH);
    	intent.putExtra(RecognizerIntent.EXTRA_LANGUAGE_MODEL, RecognizerIntent.LANGUAGE_MODEL_FREE_FORM);
    	intent.putExtra(RecognizerIntent.EXTRA_PROMPT, "pronuncie ','coma "); 	
    	startActivityForResult(intent, VOICE_RECOGNITION_REQUEST_CODE_LAT);
    	

    	
    }
    private void escuchar2(){
    	
    	Intent intent = new Intent(RecognizerIntent.ACTION_RECOGNIZE_SPEECH);
    	intent.putExtra(RecognizerIntent.EXTRA_LANGUAGE_MODEL, RecognizerIntent.LANGUAGE_MODEL_FREE_FORM);
    	intent.putExtra(RecognizerIntent.EXTRA_PROMPT, "pronuncie ','coma ");	    	
    	startActivityForResult(intent, VOICE_RECOGNITION_REQUEST_CODE_LONG);
    	
    }
    
    protected void onActivityResult(int requestCode, int resultCode, Intent data){
    	
    	if(requestCode==VOICE_RECOGNITION_REQUEST_CODE_LAT && resultCode == RESULT_OK){
    		
    		ArrayList<String> matches = data.getStringArrayListExtra(RecognizerIntent.EXTRA_RESULTS);
    		String[] palabras = matches.get(0).toString().split(" ");
    		String noEspacios = matches.get(0).toString().replace(" ", "");
    		
    		if(palabras[0].equals("menos")){
    			lat_negativa=true;
    			noEspacios=noEspacios.replace("menos", "");
    			noEspacios=noEspacios.replace(",",".");
    		}
    		else if(palabras[0].equals("-")){
    			lat_negativa=true;
    			noEspacios=noEspacios.replace("menos", "");
    			noEspacios=noEspacios.replace(",",".");
    		}
    		else{
    			lat_negativa=false;
    			noEspacios=noEspacios.replace(",",".");
    		}
    		
    		lat_objetivo=Double.valueOf(noEspacios).doubleValue();
    		if(lat_negativa==true){
    			lat_objetivo_neg=0-lat_objetivo;
    			lat_objetivo=lat_objetivo_neg;
    			lat_resultado= new Double(lat_objetivo).toString();
    		}
    		else{
    			lat_resultado=new Double(lat_objetivo).toString();
    		}
    		
    		boton_latitud.setText(lat_resultado);
    		
    		puede_lanzarA=true;
    		    		
    	}
    	
    	if(requestCode==VOICE_RECOGNITION_REQUEST_CODE_LONG && resultCode == RESULT_OK){
    		
    		ArrayList<String> matches = data.getStringArrayListExtra(RecognizerIntent.EXTRA_RESULTS);
    		String[] palabras = matches.get(0).toString().split(" ");
    		String noEspacios = matches.get(0).toString().replace(" ", "");
    		
    		if(palabras[0].equals("menos")){
    			long_negativa=true;
    			noEspacios=noEspacios.replace("menos", "");
    			noEspacios=noEspacios.replace(",",".");
    		}
    		else if(palabras[0].equals("-")){
    			long_negativa=true;
    			noEspacios=noEspacios.replace("menos", "");
    			noEspacios=noEspacios.replace(",",".");
    		}
    		else{
    			long_negativa=false;
    			noEspacios=noEspacios.replace(",",".");
    		}
    		
    		long_objetivo=Double.valueOf(noEspacios).doubleValue();
    		if(long_negativa==true){
    			long_objetivo_neg=0-long_objetivo;
    			long_objetivo=long_objetivo_neg;
    			long_resultado= new Double(long_objetivo).toString();
    		}
    		else{
    			long_resultado=new Double(long_objetivo).toString();
    		}
    		
    		boton_longitud.setText(long_resultado);
    		
    		puede_lanzarB=true;
    		
    	}
    	 	
    }
}
