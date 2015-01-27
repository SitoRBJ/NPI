package com.brujula_ts.brujula;

import java.text.SimpleDateFormat;
import java.util.Date;

import android.app.Activity;
import android.content.ContentValues;
import android.content.Intent;
import android.graphics.Bitmap;
import android.graphics.BitmapFactory;
import android.net.Uri;
import android.os.Bundle;
import android.provider.MediaStore;
import android.view.View;
import android.view.View.OnClickListener;
import android.widget.Button;
import android.widget.ImageView;
import android.widget.Toast;


public class Camara extends Activity {

	private static final int CAMERA_CAPTURE_IMAGE_REQUEST_CODE = 100;
	public static final int MEDIA_TYPE_IMAGE = 1;
	private static final String IMAGE_DIRECTORY_NAME = "Hello Camera";
	
	private Uri fileUri;
	
	private ImageView imgPreview;

	
	@Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_main);
 
        imgPreview = (ImageView) findViewById(R.id.imgPreview);
       
                captureImage();
         
        
    }
	
	  private void captureImage() {
	    	// Dar un nombre a la fotografía para almacenarla en la carpeta por defecto del movil
	        String timeStamp = new SimpleDateFormat("yyyyMMdd_HHmmss").format(new Date());
	    	
	    	ContentValues values = new ContentValues();
	        values.put(MediaStore.Images.Media.TITLE, "IMG_" + timeStamp + ".jpg");
	 
	        Intent intent = new Intent(MediaStore.ACTION_IMAGE_CAPTURE);
	     
	        //fileUri = getOutputMediaFileUri(MEDIA_TYPE_IMAGE);
	        fileUri = getContentResolver().insert(MediaStore.Images.Media.EXTERNAL_CONTENT_URI, values); // store content values
	        
	        intent.putExtra(MediaStore.EXTRA_OUTPUT, fileUri);
	     
	        // start el intent" de captura
	        startActivityForResult(intent, CAMERA_CAPTURE_IMAGE_REQUEST_CODE);
	    }
	  
	  @Override
	    protected void onActivityResult(int requestCode, int resultCode, Intent data) {
	        //Comprobar la solicitud del usuario de guardar la fotografía
	        if (requestCode == CAMERA_CAPTURE_IMAGE_REQUEST_CODE) {
	            if (resultCode == RESULT_OK) {
	                previewCapturedImage();
	            } else if (resultCode == RESULT_CANCELED) {
	                Toast.makeText(getApplicationContext(),
	                        "User cancelled image capture", Toast.LENGTH_SHORT)
	                        .show();
	            } else {
	                Toast.makeText(getApplicationContext(),
	                        "Sorry! Failed to capture image", Toast.LENGTH_SHORT)
	                        .show();
	            }
	        }
	    }
	  
	  private void previewCapturedImage() {
	        try {
	     
	 
	            imgPreview.setVisibility(View.VISIBLE);
	 
	            // bimatp factory
	            BitmapFactory.Options options = new BitmapFactory.Options();
	 
	            // downsizing image as it throws OutOfMemory Exception for larger
	            // images
	            options.inSampleSize = 8;
	 
	            final Bitmap bitmap = BitmapFactory.decodeFile(fileUri.getPath(),
	                    options);
	 
	            imgPreview.setImageBitmap(bitmap);
	        } catch (NullPointerException e) {
	            e.printStackTrace();
	        }
	    }
	
	
}
