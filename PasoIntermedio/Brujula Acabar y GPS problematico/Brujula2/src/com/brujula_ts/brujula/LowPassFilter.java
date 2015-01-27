package com.brujula_ts.brujula;

public class LowPassFilter {

	
	float gradosN, minN, segN;
	float gradosW, minW, segW;
	
    static final float ALPHA = 0.15f;
    
	
    public  float calcula(float gr, float min, float seg)  {
           
    	float grado=0;
    	float aux;
    	
    	aux=seg/60;
    	grado=aux;
    	aux=(min+grado)/60;
    	grado=aux+gr;
    	
    	
    	return grado;
    }
	
    public float posicion (float gN, float gW){
    	
    	float gradoC=0;
    	float hipotenusa;
    	
    	hipotenusa=(float) Math.sqrt((Math.pow(gN, 2))+(Math.pow(gW, 2)));
    	
    	gradoC=(float) Math.asin((gN/hipotenusa));				//cuarto cuadrante
		gradoC=gradoC+270;
    	
    	return gradoC;
    	
    }
	
}
