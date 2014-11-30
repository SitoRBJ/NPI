using Microsoft.Kinect;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Kinect;

namespace PruebasFitness
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        KinectSensor sensor;
        bool ejer1_realizado = false;
        bool ejer2_realizado = false;                                                       // Variables para controlar el orden de ejecucion de los ejercicios y los errores al realizarlos
        bool ejer3_realizado = false;
        bool ejer4_realizado = false;
        bool ejer5_realizado = false;
        bool condicion_aux = false;
        bool dif_elegida = false;
        bool primer_tiempo = false;
        double dificultad_de_ejercicio;
        TimeSpan[] tiempos;
        
        DateTime inicio;
        DateTime ejer1, ejer2,ejer3, ejer4, ejer5;
        TimeSpan tiempo1, tiempo2, tiempo3, tiempo4, tiempo5;
                    
        public MainWindow()
        {
            InitializeComponent();
        }


        private void Window_Loaded_1(object sender, RoutedEventArgs e)
        {
            if (KinectSensor.KinectSensors.Count == 0)                              // En primer lugar comprobamos si hay algun dispositivo kinect conectado, de no ser asi cerramos el programa
            {
                MessageBox.Show("Ningun kinect detectado");
                Application.Current.Shutdown();
                return;
            }

            sensor = KinectSensor.KinectSensors.FirstOrDefault();                   // Si detecta uno o mas dispositivos kinect almacenamos en la variable sensor el primer kinect detectado
            tiempos = new TimeSpan[5];

            try
            {
                sensor.SkeletonStream.Enable();                                     // Activamos el Stream de Skeleton e iniciamos su captura
                sensor.Start();
                sensor.ColorStream.Enable();                                        //NUEVO
            }
            catch
            {
                MessageBox.Show("Error en la iniciacion de Kinect");                // Si hay algun error en la iniciacion cerramos el programa
                Application.Current.Shutdown();
            }

            sensor.SkeletonFrameReady += sensor_SkeletonFrameReady;                 // Llamamos a la funcion de controlador de eventos o Event Handler, dado que hemos creado uno del tipo Load, es decir
            sensor.ColorFrameReady += sensor_ColorFrameReady;                       //NUEVO
        }

        void sensor_ColorFrameReady(object sender, Microsoft.Kinect.ColorImageFrameReadyEventArgs e)
        {
            using (ColorImageFrame frameImagen = e.OpenColorImageFrame())
            {
                {
                    if (frameImagen == null) return;
                }
                byte[] datosColor = new byte[frameImagen.PixelDataLength];
  

                frameImagen.CopyPixelDataTo(datosColor);
          

                mostrarVideo.Source= BitmapSource.Create(
                    frameImagen.Width, frameImagen.Height,
                    96,
                    96,
                    PixelFormats.Bgr32,
                    null,
                    datosColor,
                    frameImagen.Width * frameImagen.BytesPerPixel
                    );
                }
        }
        





        //FUNCION DE CONTROLADOR DE EVENTOS



        void sensor_SkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e)        
        {

            canvasEsqueleto.Children.Clear();                                                   //En el MainWindow.xaml creamos un elemento canvas o lienzo, para poder dibujar el esqueleto sobre el,
                                                                                                // limpiamos cualquier esqueleto anterior para poder redibujar
            Skeleton[] esqueletos = null;                                                        // creamos una instancia del tipo Skeleton
           
            using (SkeletonFrame frameEsqueleto = e.OpenSkeletonFrame())                        // Utilizamos using para eliminar el frame al leer el cierre de llave y poder recibir mas flujo de datos,
                                                                                                // asi podremos redibujarlo una y otra vez
            {
                if (frameEsqueleto != null)                                                     //si recibe flujo de datos los dibuja
                {
                    esqueletos = new Skeleton[frameEsqueleto.SkeletonArrayLength];              // Almacenamos el tamaño del buffer de datos
                    frameEsqueleto.CopySkeletonDataTo(esqueletos);                              // Almacenamos el esqueleto en nuestra instancia esqueletos del tipo Skeleton
                }
            }
            if (esqueletos == null) return;                                                     //Si no hay esqueleto salimos        

            foreach (Skeleton usuario in esqueletos)                                          // En caso contrario para cada esqueleto 
            {
                if (usuario.TrackingState == SkeletonTrackingState.Tracked)
                {                  
                    if (dif_elegida == false)                                                   // Hasta que no se elija la dificultad del ejercicio no comenzaremos
                    {
                       dificultad_de_ejercicio= elegir_dificultad(usuario);                     // Elegimos dificultad                                       
                       
                    }
                    else{
                            if (ejer1_realizado == false) { posicion1(usuario, dificultad_de_ejercicio); }          //Hasta que no se realice el ejercicio 1 no pasa al 2
                            else
                            {
                                if (ejer2_realizado == false) { posicion2(usuario, dificultad_de_ejercicio); }      //Hasta que no se realice el ejercicio 2 no pasa al 3
                                else
                                {
                                    if (ejer3_realizado == false) { posicion3(usuario, dificultad_de_ejercicio); }  //Hasta que no se realice el ejercicio 3 no pasa al 4
                                    else
                                    {
                                        if (ejer4_realizado == false) { posicion4(usuario, dificultad_de_ejercicio); }  //Hasta que no se realice el ejercicio 4 no pasa al 5
                                        else
                                        {
                                            if (ejer5_realizado == false) { posicion5(usuario, dificultad_de_ejercicio); }  //Realizamos el ejercicio 5
                                            else
                                            {
                                                continuar(usuario);                                                         //Mostramos los resultados y preguntamos si se quiere repetir o terminar
                                            }
                                        }
                                    }
                                }
                            }
                    }
                }
            }
        }


        // FUNCION PARA DIBUJAR EL ESQUELETO


        void agregarLinea(Joint j1, Joint j2, int colorea)
        {
            Line lineaHueso = new Line();
            if (colorea == 2)
            {                                                                   //seleccionamos color
                lineaHueso.Stroke = new SolidColorBrush(Colors.GreenYellow);
            }
            else
            {
                lineaHueso.Stroke = new SolidColorBrush(Colors.Blue);
            }
            lineaHueso.StrokeThickness = 5;                                     // Seleccionamos el grosor

            // Unimos los puntos

            ColorImagePoint j1P = sensor.CoordinateMapper.MapSkeletonPointToColorPoint(j1.Position, ColorImageFormat.RgbResolution640x480Fps30);
            lineaHueso.X1 = j1P.X;
            lineaHueso.Y1 = j1P.Y;

            ColorImagePoint j2P = sensor.CoordinateMapper.MapSkeletonPointToColorPoint(j2.Position, ColorImageFormat.RgbResolution640x480Fps30);
            lineaHueso.X2 = j2P.X;
            lineaHueso.Y2 = j2P.Y;

            canvasEsqueleto.Children.Add(lineaHueso);
        }



        // EJERCICIO 1 DE LA RUTINA 


        void posicion1(Skeleton esqueleto, double g_entrada)
        {
            double dificultad=20;                           //Dificultad de los ejercicios, por defecto 20
            int contador_de_solucion = 0;                   //Variable que nos valdra para controlar la correcta realizacion del ejercicio

            if (primer_tiempo == false)
            {
                inicio = DateTime.Now;
                primer_tiempo = true;
            }

            String ejercicio = "Ejercicio 1";                                                                                   //Mostramos por pantalla el ejercicio que es y como se realiza
            String instrucciones = "Con los brazos en cruz alzar hacia atras la pierna izquierda a partir de la rodilla";

            caja1.Text = ejercicio;
            caja2.Text = instrucciones;
          
            // LLamamos a la funcion agregar Linea para dibujar la Columna Vertebral, pasamos por parametros los puntos a unir y un entero que informara del color para pintarla
            agregarLinea(esqueleto.Joints[JointType.Head], esqueleto.Joints[JointType.ShoulderCenter], 2);
            agregarLinea(esqueleto.Joints[JointType.ShoulderCenter], esqueleto.Joints[JointType.Spine], 2);

            // LLamamos a la funcion agregar Linea para dibujar la Pierna Izquierda, pasamos por parametros los puntos a unir y un entero que informara del color para pintarla
            // En este caso pintamos hasta la rodilla, dado que nuestro movimiento consiste en detectar que levantamos la pierna izquierda hacia atras.
            agregarLinea(esqueleto.Joints[JointType.Spine], esqueleto.Joints[JointType.HipCenter], 2);
            agregarLinea(esqueleto.Joints[JointType.HipCenter], esqueleto.Joints[JointType.HipLeft], 2);
            agregarLinea(esqueleto.Joints[JointType.HipLeft], esqueleto.Joints[JointType.KneeLeft], 2);


            // LLamamos a la funcion agregar Linea para dibujar la Pierna Derecha, pasamos por parametros los puntos a unir y un entero que informara del color para pintarla
            agregarLinea(esqueleto.Joints[JointType.HipCenter], esqueleto.Joints[JointType.HipRight], 2);
            agregarLinea(esqueleto.Joints[JointType.HipRight], esqueleto.Joints[JointType.KneeRight], 2);
            agregarLinea(esqueleto.Joints[JointType.KneeRight], esqueleto.Joints[JointType.AnkleRight], 2);
            agregarLinea(esqueleto.Joints[JointType.AnkleRight], esqueleto.Joints[JointType.FootRight], 2);

            //Aqui comenzamos la captura de puntos del tipo Skeleton de las rodillas y de los tobillos

            Joint jointRI = esqueleto.Joints[JointType.KneeLeft];
            SkeletonPoint posicionRI = jointRI.Position;
            Joint jointTI = esqueleto.Joints[JointType.AnkleLeft];
            SkeletonPoint posicionTI = jointTI.Position;

            Joint jointRD = esqueleto.Joints[JointType.KneeRight];
            SkeletonPoint posicionRD = jointRD.Position;
            Joint jointTD = esqueleto.Joints[JointType.AnkleRight];
            SkeletonPoint posicionTD = jointTD.Position;

            // declaramos las variables para calcular el angulo de apertura de la pierna izquierda

            int a, b, c, d;
            double distanciaPD, distanciaPI, angulo, division, grados;
            

            a = (int)(posicionRI.Z * 100.0);                                    //Almacenamos en variables enteras los puntos necesarios con una precision de dos digitos para calcular las distancias
            b = (int)(posicionTI.Z * 100.0);
            c = (int)(posicionRD.Y * 100.0);
            d = (int)(posicionTD.Y * 100.0);

            distanciaPI = Math.Abs(b - a);                                        // lado opuesto
            distanciaPD = Math.Abs(c - d);                                        // lado adyacente

            // Lo realizamos calculando la arcotangente, puesto que el calculo lo obtenemos usando la pierna derecha y la proyeccion del punto de la rodilla izquierda unido al punto
            //del tobillo izquierdo sobre el plano Z

            division = (distanciaPI / distanciaPD);                         // Obtenemos la tangente
            angulo = Math.Atan(division);                                   //Realizamos la arcotangente
            grados = ((angulo * 180) / Math.PI);                            // Convertimos de radianes a grados

            if (g_entrada == 15 )                            //Dependiendo de la dificultad que se haya elegido estableceremos unos grados a superar a la hora de flexionar la rodilla
            {                                                               
               dificultad = 20;
            }
            if (g_entrada == 10)
            {
                dificultad = 27;
            }
            if (g_entrada == 5)
            {
                dificultad = 35;
            }

            if (grados >= dificultad)                                             // Si superamos los grados dibujaremos la pierna correctamente, con su color apropiado
            {
                agregarLinea(esqueleto.Joints[JointType.KneeLeft], esqueleto.Joints[JointType.AnkleLeft], 2);
                agregarLinea(esqueleto.Joints[JointType.AnkleLeft], esqueleto.Joints[JointType.FootLeft], 2);
                contador_de_solucion++;                                                                         //Si se realiza correctamente aumentamos el valor del contador
            }                                                                                           // En caso contrario lo dibujamos con un color diferente para que veamos que no es correcto el movimiento
            else
            {
                agregarLinea(esqueleto.Joints[JointType.KneeLeft], esqueleto.Joints[JointType.AnkleLeft], 1);
                agregarLinea(esqueleto.Joints[JointType.AnkleLeft], esqueleto.Joints[JointType.FootLeft], 1);
                contador_de_solucion = 0;                                                                       //Reiniciamos el contador de solucion porque el ejrcicio no esta bien
               
            }

            // A partir de aqui dibujamos los brazos, que estaran correctamente situados si estan en cruz.
            //El proceso es semejante, almacenamos los puntos con precision de un digito en un enero y realizamos la comprobacion de que estan en el mismo punto del plano
            //Los dibujaremos verdes cuando esten en una posicion correcta y azul en caso contrario

            Joint jointMD = esqueleto.Joints[JointType.WristRight];
            SkeletonPoint posicionMD = jointMD.Position;
            Joint jointCD = esqueleto.Joints[JointType.ElbowRight];
            SkeletonPoint posicionCD = jointCD.Position;
            Joint jointHD = esqueleto.Joints[JointType.ShoulderRight];
            SkeletonPoint posicionHD = jointHD.Position;

            int manoD, codoD, hombroD;
            manoD = (int)(posicionMD.Y * 100.0);
            codoD = (int)(posicionCD.Y * 100.0);
            hombroD = (int)(posicionHD.Y * 100.0);

          

            if ((manoD >= hombroD - g_entrada) && (manoD <= hombroD + g_entrada) && (codoD >= hombroD - g_entrada) && (codoD <= hombroD + g_entrada))
            {
                // Brazo Derecho
                agregarLinea(esqueleto.Joints[JointType.ShoulderCenter], esqueleto.Joints[JointType.ShoulderRight], 2);
                agregarLinea(esqueleto.Joints[JointType.ShoulderRight], esqueleto.Joints[JointType.ElbowRight], 2);
                agregarLinea(esqueleto.Joints[JointType.ElbowRight], esqueleto.Joints[JointType.WristRight], 2);
                agregarLinea(esqueleto.Joints[JointType.WristRight], esqueleto.Joints[JointType.HandRight], 2);
                contador_de_solucion++;                                                                                 //Si se realiza correctamente aumentamos el valor del contador
            }
            else
            {
                // Brazo Derecho
                agregarLinea(esqueleto.Joints[JointType.ShoulderCenter], esqueleto.Joints[JointType.ShoulderRight], 1);
                agregarLinea(esqueleto.Joints[JointType.ShoulderRight], esqueleto.Joints[JointType.ElbowRight], 1);
                agregarLinea(esqueleto.Joints[JointType.ElbowRight], esqueleto.Joints[JointType.WristRight], 1);
                agregarLinea(esqueleto.Joints[JointType.WristRight], esqueleto.Joints[JointType.HandRight], 1);
                contador_de_solucion = 0;                                                                               //Reiniciamos el contador de solucion porque el ejrcicio no esta bien
                
            }

            // Dibujamos el brazo izquierdo siguiendo el mismo proceso que en el derecho

            Joint jointMI = esqueleto.Joints[JointType.WristLeft];
            SkeletonPoint posicionMI = jointMI.Position;
            Joint jointCI = esqueleto.Joints[JointType.ElbowLeft];
            SkeletonPoint posicionCI = jointCI.Position;
            Joint jointHI = esqueleto.Joints[JointType.ShoulderLeft];
            SkeletonPoint posicionHI = jointHI.Position;

            int manoI, codoI, hombroI;
            manoI = (int)(posicionMI.Y * 100.0);
            codoI = (int)(posicionCI.Y * 100.0);
            hombroI = (int)(posicionHI.Y * 100.0);

            if ((manoI >= hombroI - g_entrada) && (manoI <= hombroI + g_entrada) && (codoI >= hombroI - g_entrada) && (codoI <= hombroI + g_entrada))
            {

                // Brazo Izquierdo
                agregarLinea(esqueleto.Joints[JointType.ShoulderCenter], esqueleto.Joints[JointType.ShoulderLeft], 2);
                agregarLinea(esqueleto.Joints[JointType.ShoulderLeft], esqueleto.Joints[JointType.ElbowLeft], 2);
                agregarLinea(esqueleto.Joints[JointType.ElbowLeft], esqueleto.Joints[JointType.WristLeft], 2);
                agregarLinea(esqueleto.Joints[JointType.WristLeft], esqueleto.Joints[JointType.HandLeft], 2);
                contador_de_solucion++;                                                                                     //Si se realiza correctamente aumentamos el valor del contador
            }
            else
            {
                // Brazo Izquierdo
                agregarLinea(esqueleto.Joints[JointType.ShoulderCenter], esqueleto.Joints[JointType.ShoulderLeft], 1);
                agregarLinea(esqueleto.Joints[JointType.ShoulderLeft], esqueleto.Joints[JointType.ElbowLeft], 1);
                agregarLinea(esqueleto.Joints[JointType.ElbowLeft], esqueleto.Joints[JointType.WristLeft], 1);
                agregarLinea(esqueleto.Joints[JointType.WristLeft], esqueleto.Joints[JointType.HandLeft], 1);
                contador_de_solucion = 0;                                                                                   //Reiniciamos el contador de solucion porque el ejrcicio no esta bien
                                                                                                                   //aumentamos el contador de error
            }

            if (contador_de_solucion == 3)                                  //Si el contador de solucion es correcto damos la señal para ejecutar el siguiente ejercicio
            {              
                ejer1_realizado = true;
                ejer1 = DateTime.Now;
                tiempo1=ejer1-inicio;             // almacenamos los errores en un vector

                tiempos[0] = tiempo1;

                String frase1 = "Ejercicio 1: \r\n REALIZADO";
                Realizado1.Text = frase1;
               
            }
        }



        //EJERCICIO 2 DE LA RUTINA


        void posicion2(Skeleton esqueleto, double g_entrada)
        {

            double dificultad = 20;                                                         //dificultad del ejercicio, por defecto 20
            int contador_de_solucion = 0;                                                   //variable para controlar la resolucion del ejercicio

            String ejercicio = "Ejercicio 2";
            String instrucciones = "Con los brazos en cruz alzar hacia atras la pierna derecha a partir de la rodilla";

            caja1.Text = ejercicio;
            caja2.Text = instrucciones;

            // LLamamos a la funcion agregar Linea para dibujar la Columna Vertebral, pasamos por parametros los puntos a unir y un entero que informara del color para pintarla
            agregarLinea(esqueleto.Joints[JointType.Head], esqueleto.Joints[JointType.ShoulderCenter], 2);
            agregarLinea(esqueleto.Joints[JointType.ShoulderCenter], esqueleto.Joints[JointType.Spine], 2);

            // LLamamos a la funcion agregar Linea para dibujar la Pierna Izquierda, pasamos por parametros los puntos a unir y un entero que informara del color para pintarla
            // En este caso pintamos hasta la rodilla, dado que nuestro movimiento consiste en detectar que levantamos la pierna izquierda hacia atras.
            agregarLinea(esqueleto.Joints[JointType.Spine], esqueleto.Joints[JointType.HipCenter], 2);
            agregarLinea(esqueleto.Joints[JointType.HipCenter], esqueleto.Joints[JointType.HipLeft], 2);
            agregarLinea(esqueleto.Joints[JointType.HipLeft], esqueleto.Joints[JointType.KneeLeft], 2);
            agregarLinea(esqueleto.Joints[JointType.KneeLeft], esqueleto.Joints[JointType.AnkleLeft], 2);
            agregarLinea(esqueleto.Joints[JointType.AnkleLeft], esqueleto.Joints[JointType.FootLeft], 2);


            // LLamamos a la funcion agregar Linea para dibujar la Pierna Derecha, pasamos por parametros los puntos a unir y un entero que informara del color para pintarla
            agregarLinea(esqueleto.Joints[JointType.HipCenter], esqueleto.Joints[JointType.HipRight], 2);
            agregarLinea(esqueleto.Joints[JointType.HipRight], esqueleto.Joints[JointType.KneeRight], 2);
            

            //Aqui comenzamos la captura de puntos del tipo Skeleton de las rodillas y de los tobillos

            Joint jointRI = esqueleto.Joints[JointType.KneeLeft];
            SkeletonPoint posicionRI = jointRI.Position;
            Joint jointTI = esqueleto.Joints[JointType.AnkleLeft];
            SkeletonPoint posicionTI = jointTI.Position;

            Joint jointRD = esqueleto.Joints[JointType.KneeRight];
            SkeletonPoint posicionRD = jointRD.Position;
            Joint jointTD = esqueleto.Joints[JointType.AnkleRight];
            SkeletonPoint posicionTD = jointTD.Position;

            // declaramos las variables para calcular el angulo de apertura de la pierna derecha

            int a, b, c, d;
            double distanciaPD, distanciaPI, angulo, division, grados;


            a = (int)(posicionRD.Z * 100.0);                                    //Almacenamos en variables enteras los puntos necesarios con una precision de dos digitos para calcular las distancias
            b = (int)(posicionTD.Z * 100.0);
            c = (int)(posicionRI.Y * 100.0);
            d = (int)(posicionTI.Y * 100.0);

            distanciaPD = Math.Abs(b - a);                                        // lado opuesto
            distanciaPI = Math.Abs(c - d);                                        // lado adyacente

            // Lo realizamos calculando la arcotangente, puesto que el calculo lo obtenemos usando la pierna derecha y la proyeccion del punto de la rodilla izquierda unido al punto
            //del tobillo izquierdo sobre el plano Z

            division = (distanciaPD / distanciaPI);                         // Obtenemos la tangente
            angulo = Math.Atan(division);                                   //Realizamos la arcotangente
            grados = ((angulo * 180) / Math.PI);                            // Convertimos de radianes a grados

            if (g_entrada == 15)                            //Asignamos un grado de dificultad respecto a la dificultad elegida
            {                                                             
                dificultad = 20;
            }
            if (g_entrada == 10)
            {
                dificultad = 27;
            }
            if (g_entrada == 5)
            {
                dificultad = 35;
            }
            if (grados >= dificultad)                                             // Si superamos los grados dibujaremos la pierna correctamente, con su color apropiado
            {
                agregarLinea(esqueleto.Joints[JointType.KneeRight], esqueleto.Joints[JointType.AnkleRight], 2);
                agregarLinea(esqueleto.Joints[JointType.AnkleRight], esqueleto.Joints[JointType.FootRight], 2);
                contador_de_solucion++;                                         // Aumentamos el contador de solucion, dado que el ejercico se esta realizando bien
            }                                                                   // En caso contrario lo dibujamos con un color diferente para que veamos que no es correcto el movimiento
            else
            {
                agregarLinea(esqueleto.Joints[JointType.KneeRight], esqueleto.Joints[JointType.AnkleRight], 1);
                agregarLinea(esqueleto.Joints[JointType.AnkleRight], esqueleto.Joints[JointType.FootRight], 1);
                contador_de_solucion = 0;                                                                           //Reiniciamos el contador de solucion dado que no es correcta la realizacion
              
            }

            // A partir de aqui dibujamos los brazos, que estaran correctamente situados si estan en cruz. Esta parte no es necesaria.
            //El proceso es semejante, almacenamos los puntos con precision de un digito en un enero y realizamos la comprobacion de que estan en el mismo punto del plano
            //Los dibujaremos verdes cuando esten en una posicion correcta y azul en caso contrario

            Joint jointMD = esqueleto.Joints[JointType.WristRight];
            SkeletonPoint posicionMD = jointMD.Position;
            Joint jointCD = esqueleto.Joints[JointType.ElbowRight];
            SkeletonPoint posicionCD = jointCD.Position;
            Joint jointHD = esqueleto.Joints[JointType.ShoulderRight];
            SkeletonPoint posicionHD = jointHD.Position;

            int manoD, codoD, hombroD;
            manoD = (int)(posicionMD.Y * 100.0);
            codoD = (int)(posicionCD.Y * 100.0);
            hombroD = (int)(posicionHD.Y * 100.0);

            if ((manoD >= hombroD - g_entrada) && (manoD <= hombroD + g_entrada) && (codoD >= hombroD - g_entrada) && (codoD <= hombroD + g_entrada))
            {
                // Brazo Derecho
                agregarLinea(esqueleto.Joints[JointType.ShoulderCenter], esqueleto.Joints[JointType.ShoulderRight], 2);
                agregarLinea(esqueleto.Joints[JointType.ShoulderRight], esqueleto.Joints[JointType.ElbowRight], 2);
                agregarLinea(esqueleto.Joints[JointType.ElbowRight], esqueleto.Joints[JointType.WristRight], 2);
                agregarLinea(esqueleto.Joints[JointType.WristRight], esqueleto.Joints[JointType.HandRight], 2);
                contador_de_solucion++;                                                                                     //aumentamos el contador de solucion
            }
            else
            {
                // Brazo Derecho
                agregarLinea(esqueleto.Joints[JointType.ShoulderCenter], esqueleto.Joints[JointType.ShoulderRight], 1);
                agregarLinea(esqueleto.Joints[JointType.ShoulderRight], esqueleto.Joints[JointType.ElbowRight], 1);
                agregarLinea(esqueleto.Joints[JointType.ElbowRight], esqueleto.Joints[JointType.WristRight], 1);
                agregarLinea(esqueleto.Joints[JointType.WristRight], esqueleto.Joints[JointType.HandRight], 1);
                contador_de_solucion = 0;                                                                                   //reiniciamos el contador de solucion
               
            }

            // Dibujamos el brazo izquierdo siguiendo el mismo proceso que en el derecho

            Joint jointMI = esqueleto.Joints[JointType.WristLeft];
            SkeletonPoint posicionMI = jointMI.Position;
            Joint jointCI = esqueleto.Joints[JointType.ElbowLeft];
            SkeletonPoint posicionCI = jointCI.Position;
            Joint jointHI = esqueleto.Joints[JointType.ShoulderLeft];
            SkeletonPoint posicionHI = jointHI.Position;

            int manoI, codoI, hombroI;
            manoI = (int)(posicionMI.Y * 100.0);
            codoI = (int)(posicionCI.Y * 100.0);
            hombroI = (int)(posicionHI.Y * 100.0);

            if ((manoI >= hombroI - g_entrada) && (manoI <= hombroI + g_entrada) && (codoI >= hombroI - g_entrada) && (codoI <= hombroI + g_entrada))
            {

                // Brazo Izquierdo
                agregarLinea(esqueleto.Joints[JointType.ShoulderCenter], esqueleto.Joints[JointType.ShoulderLeft], 2);
                agregarLinea(esqueleto.Joints[JointType.ShoulderLeft], esqueleto.Joints[JointType.ElbowLeft], 2);
                agregarLinea(esqueleto.Joints[JointType.ElbowLeft], esqueleto.Joints[JointType.WristLeft], 2);
                agregarLinea(esqueleto.Joints[JointType.WristLeft], esqueleto.Joints[JointType.HandLeft], 2);
                contador_de_solucion++;                                                                             //aumentamos el contador de solucion
            }
            else
            {
                // Brazo Izquierdo
                agregarLinea(esqueleto.Joints[JointType.ShoulderCenter], esqueleto.Joints[JointType.ShoulderLeft], 1);
                agregarLinea(esqueleto.Joints[JointType.ShoulderLeft], esqueleto.Joints[JointType.ElbowLeft], 1);
                agregarLinea(esqueleto.Joints[JointType.ElbowLeft], esqueleto.Joints[JointType.WristLeft], 1);
                agregarLinea(esqueleto.Joints[JointType.WristLeft], esqueleto.Joints[JointType.HandLeft], 1);
                contador_de_solucion = 0;                                                                       //reiniciamos el contador de solucion
               
            }

            if (contador_de_solucion == 3)                                                                      //Si el ejercicio se ha llevado a cabo correctamente avisamos al siguiente
            {
                ejer2_realizado = true;
                ejer2 = DateTime.Now;
                tiempo2 = ejer2 - ejer1;
                tiempos[1] = tiempo2;
                String frase1 = "Ejercicio 2: \r\n REALIZADO";
                Realizado2.Text = frase1;
            }
        }


        // EJERCICIO 3 DE LA RUTINA

        void posicion3(Skeleton esqueleto, double g_entrada){

            int contador_de_solucion = 0;                                                   //Controlador de solucion

            String ejercicio = "Ejercicio 3";
            String instrucciones = "Alzar los brazos por encima de la cabeza";

            caja1.Text = ejercicio;
            caja2.Text = instrucciones;

            // LLamamos a la funcion agregar Linea para dibujar la Columna Vertebral, pasamos por parametros los puntos a unir y un entero que informara del color para pintarla
            agregarLinea(esqueleto.Joints[JointType.Head], esqueleto.Joints[JointType.ShoulderCenter], 2);
            agregarLinea(esqueleto.Joints[JointType.ShoulderCenter], esqueleto.Joints[JointType.Spine], 2);

            // LLamamos a la funcion agregar Linea para dibujar la Pierna Izquierda, pasamos por parametros los puntos a unir y un entero que informara del color para pintarla
            // En este caso pintamos hasta la rodilla, dado que nuestro movimiento consiste en detectar que levantamos la pierna izquierda hacia atras.
            agregarLinea(esqueleto.Joints[JointType.Spine], esqueleto.Joints[JointType.HipCenter], 2);
            agregarLinea(esqueleto.Joints[JointType.HipCenter], esqueleto.Joints[JointType.HipLeft], 2);
            agregarLinea(esqueleto.Joints[JointType.HipLeft], esqueleto.Joints[JointType.KneeLeft], 2);
            agregarLinea(esqueleto.Joints[JointType.KneeLeft], esqueleto.Joints[JointType.AnkleLeft], 2);
            agregarLinea(esqueleto.Joints[JointType.AnkleLeft], esqueleto.Joints[JointType.FootLeft], 2);


            // LLamamos a la funcion agregar Linea para dibujar la Pierna Derecha, pasamos por parametros los puntos a unir y un entero que informara del color para pintarla
            agregarLinea(esqueleto.Joints[JointType.HipCenter], esqueleto.Joints[JointType.HipRight], 2);
            agregarLinea(esqueleto.Joints[JointType.HipRight], esqueleto.Joints[JointType.KneeRight], 2);
            agregarLinea(esqueleto.Joints[JointType.KneeRight], esqueleto.Joints[JointType.AnkleRight], 2);
            agregarLinea(esqueleto.Joints[JointType.AnkleRight], esqueleto.Joints[JointType.FootRight], 2);

            //Capturamos los puntos para poder trabajar con ellos

            Joint jointMD = esqueleto.Joints[JointType.HandRight];
            SkeletonPoint posicionMD = jointMD.Position;
            Joint jointCD = esqueleto.Joints[JointType.ElbowRight];
            SkeletonPoint posicionCD = jointCD.Position;
            Joint jointHD = esqueleto.Joints[JointType.ShoulderRight];
            SkeletonPoint posicionHD = jointHD.Position;

            int manoD, codoD, hombroD;
            manoD = (int)(posicionMD.Y * 100.0);
            codoD = (int)(posicionCD.Y * 100.0);
            hombroD = (int)(posicionHD.Y * 100.0);

            Joint jointMI = esqueleto.Joints[JointType.HandLeft];
            SkeletonPoint posicionMI = jointMI.Position;
            Joint jointCI = esqueleto.Joints[JointType.ElbowLeft];
            SkeletonPoint posicionCI = jointCI.Position;
            Joint jointHI = esqueleto.Joints[JointType.ShoulderLeft];
            SkeletonPoint posicionHI = jointHI.Position;

            int manoI, codoI, hombroI;
            manoI = (int)(posicionMI.Y * 100.0);
            codoI = (int)(posicionCI.Y * 100.0);
            hombroI = (int)(posicionHI.Y * 100.0);            

            if((manoI>codoI)&&(codoI>hombroI)&&(manoD>codoD)&&(codoD>hombroD)){                                             

                agregarLinea(esqueleto.Joints[JointType.ShoulderCenter], esqueleto.Joints[JointType.ShoulderLeft], 2);
                agregarLinea(esqueleto.Joints[JointType.ShoulderLeft], esqueleto.Joints[JointType.ElbowLeft], 2);
                agregarLinea(esqueleto.Joints[JointType.ElbowLeft], esqueleto.Joints[JointType.WristLeft], 2);
                agregarLinea(esqueleto.Joints[JointType.WristLeft], esqueleto.Joints[JointType.HandLeft], 2);

                agregarLinea(esqueleto.Joints[JointType.ShoulderCenter], esqueleto.Joints[JointType.ShoulderRight], 2);
                agregarLinea(esqueleto.Joints[JointType.ShoulderRight], esqueleto.Joints[JointType.ElbowRight], 2);
                agregarLinea(esqueleto.Joints[JointType.ElbowRight], esqueleto.Joints[JointType.WristRight], 2);
                agregarLinea(esqueleto.Joints[JointType.WristRight], esqueleto.Joints[JointType.HandRight], 2);

                contador_de_solucion++;                                                         //aumentamos el contador de solucion
            }
            else
            {
                agregarLinea(esqueleto.Joints[JointType.ShoulderCenter], esqueleto.Joints[JointType.ShoulderLeft], 1);
                agregarLinea(esqueleto.Joints[JointType.ShoulderLeft], esqueleto.Joints[JointType.ElbowLeft], 1);
                agregarLinea(esqueleto.Joints[JointType.ElbowLeft], esqueleto.Joints[JointType.WristLeft], 1);
                agregarLinea(esqueleto.Joints[JointType.WristLeft], esqueleto.Joints[JointType.HandLeft], 1);

                agregarLinea(esqueleto.Joints[JointType.ShoulderCenter], esqueleto.Joints[JointType.ShoulderRight], 1);
                agregarLinea(esqueleto.Joints[JointType.ShoulderRight], esqueleto.Joints[JointType.ElbowRight], 1);
                agregarLinea(esqueleto.Joints[JointType.ElbowRight], esqueleto.Joints[JointType.WristRight], 1);
                agregarLinea(esqueleto.Joints[JointType.WristRight], esqueleto.Joints[JointType.HandRight], 1);

                contador_de_solucion = 0;                                                       //reiniciamos el contador de solucion
                                                                               // aumentamos el contador de error
            }

            if (contador_de_solucion == 1)                                                      //Si esta bien realizado el ejercicio avisamos al siguiente
            {
                ejer3_realizado = true;
                ejer3 = DateTime.Now;                                                    //almacenamos los errores
                tiempo3 = ejer3 - ejer2;
                tiempos[2] = tiempo3;
                String frase1 = "Ejercicio 3: \r\n REALIZADO";
                Realizado3.Text = frase1;
            }
        }


        // EJERCICIO 4 DE LA RUTINA

        void posicion4(Skeleton esqueleto, double g_entrada)
        {
            double dificultad = 20;                                             // Dificultad del ejercicio, por defecto 20
            int contador_de_solucion = 0;                                       // Contador de solucion

            String ejercicio = "Ejercicio 4";
            String instrucciones = "Con los brazos hacia arriba, inclinar el tronco hacia la derecha";

            caja1.Text = ejercicio;
            caja2.Text = instrucciones;

            // LLamamos a la funcion agregar Linea para dibujar la Pierna Izquierda, pasamos por parametros los puntos a unir y un entero que informara del color para pintarla
            // En este caso pintamos hasta la rodilla, dado que nuestro movimiento consiste en detectar que levantamos la pierna izquierda hacia atras.
            agregarLinea(esqueleto.Joints[JointType.Spine], esqueleto.Joints[JointType.HipCenter], 2);
            agregarLinea(esqueleto.Joints[JointType.HipCenter], esqueleto.Joints[JointType.HipLeft], 2);
            agregarLinea(esqueleto.Joints[JointType.HipLeft], esqueleto.Joints[JointType.KneeLeft], 2);
            agregarLinea(esqueleto.Joints[JointType.KneeLeft], esqueleto.Joints[JointType.AnkleLeft], 2);
            agregarLinea(esqueleto.Joints[JointType.AnkleLeft], esqueleto.Joints[JointType.FootLeft], 2);


            // LLamamos a la funcion agregar Linea para dibujar la Pierna Derecha, pasamos por parametros los puntos a unir y un entero que informara del color para pintarla
            agregarLinea(esqueleto.Joints[JointType.HipCenter], esqueleto.Joints[JointType.HipRight], 2);
            agregarLinea(esqueleto.Joints[JointType.HipRight], esqueleto.Joints[JointType.KneeRight], 2);
            agregarLinea(esqueleto.Joints[JointType.KneeRight], esqueleto.Joints[JointType.AnkleRight], 2);
            agregarLinea(esqueleto.Joints[JointType.AnkleRight], esqueleto.Joints[JointType.FootRight], 2);

             Joint jointMD = esqueleto.Joints[JointType.HandRight];
            SkeletonPoint posicionMD = jointMD.Position;
            Joint jointCD = esqueleto.Joints[JointType.ElbowRight];
            SkeletonPoint posicionCD = jointCD.Position;
            Joint jointHD = esqueleto.Joints[JointType.ShoulderRight];
            SkeletonPoint posicionHD = jointHD.Position;

            int manoD, codoD, hombroD;
            manoD = (int)(posicionMD.Y * 100.0);
            codoD = (int)(posicionCD.Y * 100.0);
            hombroD = (int)(posicionHD.Y * 100.0);
          

            Joint jointMI = esqueleto.Joints[JointType.HandLeft];
            SkeletonPoint posicionMI = jointMI.Position;
            Joint jointCI = esqueleto.Joints[JointType.ElbowLeft];
            SkeletonPoint posicionCI = jointCI.Position;
            Joint jointHI = esqueleto.Joints[JointType.ShoulderLeft];
            SkeletonPoint posicionHI = jointHI.Position;

            int manoI, codoI, hombroI;
            manoI = (int)(posicionMI.Y * 100.0);
            codoI = (int)(posicionCI.Y * 100.0);
            hombroI = (int)(posicionHI.Y * 100.0);
          
            if ( (manoI>codoI)&&(codoI>hombroI)&&(manoD>codoD)&&(codoD>hombroD))
            {

                agregarLinea(esqueleto.Joints[JointType.ShoulderCenter], esqueleto.Joints[JointType.ShoulderLeft], 2);
                agregarLinea(esqueleto.Joints[JointType.ShoulderLeft], esqueleto.Joints[JointType.ElbowLeft], 2);
                agregarLinea(esqueleto.Joints[JointType.ElbowLeft], esqueleto.Joints[JointType.WristLeft], 2);
                agregarLinea(esqueleto.Joints[JointType.WristLeft], esqueleto.Joints[JointType.HandLeft], 2);

                agregarLinea(esqueleto.Joints[JointType.ShoulderCenter], esqueleto.Joints[JointType.ShoulderRight], 2);
                agregarLinea(esqueleto.Joints[JointType.ShoulderRight], esqueleto.Joints[JointType.ElbowRight], 2);
                agregarLinea(esqueleto.Joints[JointType.ElbowRight], esqueleto.Joints[JointType.WristRight], 2);
                agregarLinea(esqueleto.Joints[JointType.WristRight], esqueleto.Joints[JointType.HandRight], 2);

                contador_de_solucion++;                                                                 //aumentamos el contador de solucion
            }
            else
            {
                agregarLinea(esqueleto.Joints[JointType.ShoulderCenter], esqueleto.Joints[JointType.ShoulderLeft], 1);
                agregarLinea(esqueleto.Joints[JointType.ShoulderLeft], esqueleto.Joints[JointType.ElbowLeft], 1);
                agregarLinea(esqueleto.Joints[JointType.ElbowLeft], esqueleto.Joints[JointType.WristLeft], 1);
                agregarLinea(esqueleto.Joints[JointType.WristLeft], esqueleto.Joints[JointType.HandLeft], 1);

                agregarLinea(esqueleto.Joints[JointType.ShoulderCenter], esqueleto.Joints[JointType.ShoulderRight], 1);
                agregarLinea(esqueleto.Joints[JointType.ShoulderRight], esqueleto.Joints[JointType.ElbowRight], 1);
                agregarLinea(esqueleto.Joints[JointType.ElbowRight], esqueleto.Joints[JointType.WristRight], 1);
                agregarLinea(esqueleto.Joints[JointType.WristRight], esqueleto.Joints[JointType.HandRight], 1);

                contador_de_solucion = 0;                                                               //reiniciamos el contador de solucion
                
            }

            Joint jointSC = esqueleto.Joints[JointType.ShoulderCenter];
            SkeletonPoint posicionSC = jointSC.Position;
            Joint jointHC = esqueleto.Joints[JointType.HipCenter];
            SkeletonPoint posicionHC = jointHC.Position;


            int hombroC, cadera;
            hombroC = (int)(posicionSC.X * 100.0);
            cadera = (int)(posicionHC.X * 100.0);

            if (g_entrada == 5)                                         //Dependiendo del grado de dificultad ajustamos la dificultad del ejercicio
            {
                dificultad = 15;
            }
            if (g_entrada == 10)
            {
                dificultad = 10;
            }
            if (g_entrada == 15)
            {
                dificultad = 5;
            }
            if (hombroC>cadera+dificultad)
            {
                // LLamamos a la funcion agregar Linea para dibujar la Columna Vertebral, pasamos por parametros los puntos a unir y un entero que informara del color para pintarla
                agregarLinea(esqueleto.Joints[JointType.Head], esqueleto.Joints[JointType.ShoulderCenter], 2);
                agregarLinea(esqueleto.Joints[JointType.ShoulderCenter], esqueleto.Joints[JointType.Spine], 2);
               
                contador_de_solucion++;                                //Aumentamos el contador de solucion
            }
            else
            {
                agregarLinea(esqueleto.Joints[JointType.Head], esqueleto.Joints[JointType.ShoulderCenter], 1);
                agregarLinea(esqueleto.Joints[JointType.ShoulderCenter], esqueleto.Joints[JointType.Spine], 1);
                contador_de_solucion = 0;                                                                           //reiniciamos el contador de solucion
                
            }
            if (contador_de_solucion == 2)                                          //Si se realiza el ejercicio avisamos al siguiente
            {
                ejer4_realizado = true;
                ejer4 = DateTime.Now;
                tiempo4 = ejer4 - ejer3;
                tiempos[3] = tiempo4;
                String frase1 = "Ejercicio 4: \r\n REALIZADO";
                Realizado4.Text = frase1;
               
            }
        }


        // EJERCICIO 5 DE LA RUTINA

        void posicion5(Skeleton esqueleto, double g_entrada)
        {
            double dificultad = 20;                                                     // Dificultad del ejercicio, por defecto 20
            int contador_de_solucion = 0;                                               // Contador de solucion

            String ejercicio = "Ejercicio 5";
            String instrucciones = "Con los brazos hacia arriba, inclinar el tronco hacia la izquierda";

            caja1.Text = ejercicio;
            caja2.Text = instrucciones;

            // LLamamos a la funcion agregar Linea para dibujar la Pierna Izquierda, pasamos por parametros los puntos a unir y un entero que informara del color para pintarla
            // En este caso pintamos hasta la rodilla, dado que nuestro movimiento consiste en detectar que levantamos la pierna izquierda hacia atras.
            agregarLinea(esqueleto.Joints[JointType.Spine], esqueleto.Joints[JointType.HipCenter], 2);
            agregarLinea(esqueleto.Joints[JointType.HipCenter], esqueleto.Joints[JointType.HipLeft], 2);
            agregarLinea(esqueleto.Joints[JointType.HipLeft], esqueleto.Joints[JointType.KneeLeft], 2);
            agregarLinea(esqueleto.Joints[JointType.KneeLeft], esqueleto.Joints[JointType.AnkleLeft], 2);
            agregarLinea(esqueleto.Joints[JointType.AnkleLeft], esqueleto.Joints[JointType.FootLeft], 2);


            // LLamamos a la funcion agregar Linea para dibujar la Pierna Derecha, pasamos por parametros los puntos a unir y un entero que informara del color para pintarla
            agregarLinea(esqueleto.Joints[JointType.HipCenter], esqueleto.Joints[JointType.HipRight], 2);
            agregarLinea(esqueleto.Joints[JointType.HipRight], esqueleto.Joints[JointType.KneeRight], 2);
            agregarLinea(esqueleto.Joints[JointType.KneeRight], esqueleto.Joints[JointType.AnkleRight], 2);
            agregarLinea(esqueleto.Joints[JointType.AnkleRight], esqueleto.Joints[JointType.FootRight], 2);

            Joint jointMD = esqueleto.Joints[JointType.HandRight];
            SkeletonPoint posicionMD = jointMD.Position;
            Joint jointCD = esqueleto.Joints[JointType.ElbowRight];
            SkeletonPoint posicionCD = jointCD.Position;
            Joint jointHD = esqueleto.Joints[JointType.ShoulderRight];
            SkeletonPoint posicionHD = jointHD.Position;

            int manoD, codoD, hombroD;
            manoD = (int)(posicionMD.Y * 100.0);
            codoD = (int)(posicionCD.Y * 100.0);
            hombroD = (int)(posicionHD.Y * 100.0);


            Joint jointMI = esqueleto.Joints[JointType.HandLeft];
            SkeletonPoint posicionMI = jointMI.Position;
            Joint jointCI = esqueleto.Joints[JointType.ElbowLeft];
            SkeletonPoint posicionCI = jointCI.Position;
            Joint jointHI = esqueleto.Joints[JointType.ShoulderLeft];
            SkeletonPoint posicionHI = jointHI.Position;

            int manoI, codoI, hombroI;
            manoI = (int)(posicionMI.Y * 100.0);
            codoI = (int)(posicionCI.Y * 100.0);
            hombroI = (int)(posicionHI.Y * 100.0);

            if ((manoI > codoI) && (codoI > hombroI) && (manoD > codoD) && (codoD > hombroD))
            {

                agregarLinea(esqueleto.Joints[JointType.ShoulderCenter], esqueleto.Joints[JointType.ShoulderLeft], 2);
                agregarLinea(esqueleto.Joints[JointType.ShoulderLeft], esqueleto.Joints[JointType.ElbowLeft], 2);
                agregarLinea(esqueleto.Joints[JointType.ElbowLeft], esqueleto.Joints[JointType.WristLeft], 2);
                agregarLinea(esqueleto.Joints[JointType.WristLeft], esqueleto.Joints[JointType.HandLeft], 2);

                agregarLinea(esqueleto.Joints[JointType.ShoulderCenter], esqueleto.Joints[JointType.ShoulderRight], 2);
                agregarLinea(esqueleto.Joints[JointType.ShoulderRight], esqueleto.Joints[JointType.ElbowRight], 2);
                agregarLinea(esqueleto.Joints[JointType.ElbowRight], esqueleto.Joints[JointType.WristRight], 2);
                agregarLinea(esqueleto.Joints[JointType.WristRight], esqueleto.Joints[JointType.HandRight], 2);

                contador_de_solucion++;                                                                                     //aumentamos el contador de solucion
            }
            else
            {
                agregarLinea(esqueleto.Joints[JointType.ShoulderCenter], esqueleto.Joints[JointType.ShoulderLeft], 1);
                agregarLinea(esqueleto.Joints[JointType.ShoulderLeft], esqueleto.Joints[JointType.ElbowLeft], 1);
                agregarLinea(esqueleto.Joints[JointType.ElbowLeft], esqueleto.Joints[JointType.WristLeft], 1);
                agregarLinea(esqueleto.Joints[JointType.WristLeft], esqueleto.Joints[JointType.HandLeft], 1);

                agregarLinea(esqueleto.Joints[JointType.ShoulderCenter], esqueleto.Joints[JointType.ShoulderRight], 1);
                agregarLinea(esqueleto.Joints[JointType.ShoulderRight], esqueleto.Joints[JointType.ElbowRight], 1);
                agregarLinea(esqueleto.Joints[JointType.ElbowRight], esqueleto.Joints[JointType.WristRight], 1);
                agregarLinea(esqueleto.Joints[JointType.WristRight], esqueleto.Joints[JointType.HandRight], 1);

                contador_de_solucion = 0;                                                                                    //reiniciamos el contador de solucion
              
            }

            Joint jointSC = esqueleto.Joints[JointType.ShoulderCenter];
            SkeletonPoint posicionSC = jointSC.Position;
            Joint jointHC = esqueleto.Joints[JointType.HipCenter];
            SkeletonPoint posicionHC = jointHC.Position;


            int hombroC, cadera;
            hombroC = (int)(posicionSC.X * 100.0);
            cadera = (int)(posicionHC.X * 100.0);

            if (g_entrada == 5)
            {
                dificultad = 15;
            }
            if (g_entrada == 10)
            {
                dificultad = 10;
            }
            if (g_entrada == 15)
            {
                dificultad = 5;
            }
            if (hombroC < cadera - dificultad)
            {

                // LLamamos a la funcion agregar Linea para dibujar la Columna Vertebral, pasamos por parametros los puntos a unir y un entero que informara del color para pintarla
                agregarLinea(esqueleto.Joints[JointType.Head], esqueleto.Joints[JointType.ShoulderCenter], 2);
                agregarLinea(esqueleto.Joints[JointType.ShoulderCenter], esqueleto.Joints[JointType.Spine], 2);

                contador_de_solucion++;                                                             //Aumentamos el contador de solucion
            }
            else
            {
                agregarLinea(esqueleto.Joints[JointType.Head], esqueleto.Joints[JointType.ShoulderCenter], 1);
                agregarLinea(esqueleto.Joints[JointType.ShoulderCenter], esqueleto.Joints[JointType.Spine], 1);
                contador_de_solucion = 0;                                                                           //reiniciamos el contador de solucion
            }   
            if (contador_de_solucion == 2)                                                                      //avisamos de que el ultimo ejercicio a finalizado
            {
                ejer5_realizado = true;
                ejer5 = DateTime.Now;
                tiempo5 = ejer5 - ejer4;
                tiempos[4] = tiempo5;
                String frase1 = "Ejercicio 5: \r\n REALIZADO";
                Realizado5.Text = frase1;
            }
        }

        //ELEGIR DIFICULTAD DE EJERCICIOS

        double elegir_dificultad(Skeleton esqueleto)
        { 
            String mensaje_elige = "Elige la Dificultad";
            String mensaje_dificultad = "Mano izquierda arriba = Principiante, mano derecha arriba = Intermedio, las dos manos arriba = Avanzado";

            caja1.Text = mensaje_elige;
            caja2.Text = mensaje_dificultad;

            //OPCIONES PARA ELEGIR DIFICULTAD. La elegimos mediante gestos o acciones
            Joint jointMI = esqueleto.Joints[JointType.HandLeft];
            SkeletonPoint posicionMI = jointMI.Position;
            Joint jointMD = esqueleto.Joints[JointType.HandRight];
            SkeletonPoint posicionMD = jointMD.Position;
            Joint jointH = esqueleto.Joints[JointType.ShoulderCenter];
            SkeletonPoint posicionH = jointH.Position;

            int manoI, manoD, hombro;
            manoI = (int)(posicionMI.Y * 100.0);
            manoD = (int)(posicionMD.Y * 100.0);
            hombro = (int)(posicionH.Y * 100.0);

            if (manoI > hombro && manoD < hombro)                                               //Si levantamos la mano izquierda optamos por una dificultad sencilla
            {
                
                String principiante = " Dificultad elegida = Principiante";
                dif_elegida = true;                                                             //avisamos de que hemos elegido la dificultad
                caja1.Text = principiante;
                caja2.Text = "";
               
                return 15;
            }
            if (manoI < hombro && manoD > hombro)                                               //Si levantamos la mano derecha optamos por una dificultad intermedia
            {
                String intermedio= " Dificultad elegida = Intermedia";
                caja1.Text = intermedio;
                caja2.Text = "";
                dif_elegida = true;                                                             //avisamos de que hemos elegido la dificultad
                
                return 10;
            } 
            if (manoI > hombro && manoD > hombro)                                               //Si levantamos las dos manos la dificultad sera avanzada, dentro de lo que cabe, no es muy dificil
            {               
                String avanzada = " Dificultad elegida = Avanzada";
                caja1.Text = avanzada;
                caja2.Text = "";
                dif_elegida = true;                                                             //avisamos de que hemos elegido la dificultad
               
                return 5;
            }
            return 15;                                           //devolvemos por defecto un valor para validar el metodo, pero se llamara de nuevo hasta que no se elija una dificultad gracias a dif_elegida
        }


        // FUNCION CONTINUAR. MOSTRAMOS LOS RESULTADOS. PERMITE REINICAR LAS RUTINAS O SALIR DEL PROGRAMA

        void continuar(Skeleton esqueleto)
        {
                                                                                                                                                //Mostramos los resultados
            String mensaje_elige = " Para iniciar la rutina levanta la mano izquierda, para salir del programa levanta la mano derecha ";
            String resuelto = " ¡ Bien Hecho !";

            

            caja1.Text = resuelto;
            caja2.Text = string.Format("Tiempos de Ejercicio 1: {0:ss}s  Ejercicio 2:  {1:ss}s Ejercicio 3:  {2:ss}s Ejercicio 4: {3:ss}s Ejercicio 5: {4:ss}s", tiempos[0], tiempos[1], tiempos[2], tiempos[3], tiempos[4]);
            caja3.Text = mensaje_elige;

            //OPCIONES PARA ELEGIR COMO CONTINUAR
            Joint jointMI = esqueleto.Joints[JointType.HandLeft];
            SkeletonPoint posicionMI = jointMI.Position;
            Joint jointMD = esqueleto.Joints[JointType.HandRight];
            SkeletonPoint posicionMD = jointMD.Position;
            Joint jointH = esqueleto.Joints[JointType.ShoulderCenter];
            SkeletonPoint posicionH = jointH.Position;

            int manoI, manoD, hombro;
            manoI = (int)(posicionMI.Y * 100.0);
            manoD = (int)(posicionMD.Y * 100.0);
            hombro = (int)(posicionH.Y * 100.0);

            if (manoI < hombro && manoD < hombro)           //damos tiempo para poder elegir al finalizar el ejercicio, unan condicion necesaria.
            {
                condicion_aux = true;
            }

            if (condicion_aux == true)                      //ahora elegimos si reiniciar la rutina levantando la mano izquierda, reiniciando todas las variables
            {
                if (manoI > hombro)
                {
                    caja3.Text = "";

                    ejer1_realizado = false;
                    ejer2_realizado = false;
                    ejer3_realizado = false;
                    ejer4_realizado = false;
                    ejer5_realizado = false;
                    primer_tiempo = false;
                    
                    condicion_aux = false;
                    Realizado1.Text = "";
                    Realizado2.Text = "";
                    Realizado3.Text = "";
                    Realizado4.Text = "";
                    Realizado5.Text = "";
                  
                }
                if (manoD > hombro)                         // salimos del porgrama al levantar la mano derecha
                {
                    Application.Current.Shutdown();
                }
            }
        }


    }
}