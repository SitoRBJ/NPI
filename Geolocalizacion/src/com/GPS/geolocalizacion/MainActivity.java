package com.GPS.geolocalizacion;



import com.google.android.gms.maps.CameraUpdate;
import com.google.android.gms.maps.CameraUpdateFactory;
import com.google.android.gms.maps.GoogleMap;

import android.content.Context;
import android.location.Location;
import android.location.LocationListener;
import android.location.LocationManager;				//Para poder comunicarse con el sistema operativo android
import android.os.Bundle;
import android.support.v4.app.FragmentActivity;
import android.view.Menu;
import android.view.MenuItem;
import android.widget.TextView; 						//Para poder usar las variables que indican latitud y longitud en el layout
import com.google.android.gms.maps.SupportMapFragment;
import com.google.android.gms.maps.model.CameraPosition;
import com.google.android.gms.maps.model.LatLng;


public class MainActivity extends FragmentActivity {
	
	TextView t1;	//en esta variable se guarda el valor de latitud
	TextView t2;	//en esta variable se guarda el valor de longitud
	
	Double longitud, latitud;
	GoogleMap mapa;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_main);
        
        t1=(TextView)findViewById(R.id.TextLat);	//Se asigna t1 al texto de latitud creado en layout
        t2=(TextView)findViewById(R.id.TextLong);   //Se asigna t2 al texto de longitud creado en layout
        
        LocationManager lm = (LocationManager)getSystemService(Context.LOCATION_SERVICE); //Creamos un LocationManager para poder comunicarnos con el sistema operativo android y obtener la localización
        LocationListener ll = new mylocationListener(); //Creamos un LocationListener que "escuche" los cambios en localización y los actualice.
        lm.requestLocationUpdates(LocationManager.GPS_PROVIDER, 0, 0, ll); //obtenemos la posición por GPS con un tiempo mínimo de 0 y una distancia mínima de 0. Usamos el LocationListener para detectar cambios en la posición
        
        mapa=((SupportMapFragment)getSupportFragmentManager().findFragmentById(R.id.map)).getMap();
        
       
        
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
    
    //Especificación de la clase que se ha creado anteriormente
    class mylocationListener implements LocationListener{
    	
    	//Esta función se activará cuando la localización cambie
		@Override
		public void onLocationChanged(Location location) {
			if(location !=null){						//se comprueba que exista una posición válida
				double pLong=location.getLongitude();	//obtenemos la longitud y se guarda
				double pLat = location.getLatitude();	//obtenemos la latitud y se guarda
				
				t1.setText(Double.toString(pLat));		//asignamos la latitud obtenida
				t2.setText(Double.toString(pLong));		//asignamos la longitud obtenida
				
				latitud =pLat;
				longitud = pLong;
				
				 LatLng actual = new LatLng(latitud, longitud);
					CameraPosition camPos = new CameraPosition.Builder()
						    .target(actual)   //Centramos el mapa
						    .zoom(10)         //Establecemos el zoom en 19
						    .bearing(45)      //Establecemos la orientación con el noreste arriba
						    .tilt(10)         //Bajamos el punto de vista de la cámara 70 grados
						    .build();
					
					CameraUpdate cam = CameraUpdateFactory.newCameraPosition(camPos);
			        
			        mapa.animateCamera(cam);
			}
		}

		@Override
		public void onProviderDisabled(String provider) {
			// TODO Auto-generated method stub
			
		}

		@Override
		public void onProviderEnabled(String provider) {
			// TODO Auto-generated method stub
			
		}

		@Override
		public void onStatusChanged(String provider, int status, Bundle extras) {
			// TODO Auto-generated method stub
			
		}
    	
    }
}
