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



namespace Esqueleto2
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        KinectSensor sensor;                                                        // Creamos una instancia sensor del tipo KinectSensor, este sera nuestro dispositivo de kinect
        
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (KinectSensor.KinectSensors.Count == 0)                              // En primer lugar comprobamos si hay algun dispositivo kinect conectado, de no ser asi cerramos el programa
            {
                MessageBox.Show("Ningun kinect detectado");
                Application.Current.Shutdown();
                return;
            }

            sensor = KinectSensor.KinectSensors.FirstOrDefault();                   // Si detecta uno o mas dispositivos kinect almacenamos en la variable sensor el primer kinect detectado

            try
            {
                sensor.SkeletonStream.Enable();                                     // Activamos el Stream de Skeleton e iniciamos su captura
                sensor.Start();
            }
            catch
            {
                MessageBox.Show("Error en la iniciacion de Kinect");                // Si hay algun error en la iniciacion cerramos el programa
                Application.Current.Shutdown();
            }

            sensor.SkeletonFrameReady += sensor_SkeletonFrameReady;                 // Llamamos a la funcion de controlador de eventos o Event Handler, dado que hemos creado uno del tipo Load, es decir
        }                                                                           // su ejecucion comenzara al cargar la ventan principal

        void sensor_SkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e)        //Esta es la funcion de controlador de eventos
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

            foreach (Skeleton esqueleto in esqueletos)                                          // En caso contrario para cada esqueleto 
            {
                if (esqueleto.TrackingState == SkeletonTrackingState.Tracked)
                {
                    // LLamamos a la funcion agregar Linea para dibujar la Columna Vertebral, pasamos por parametros los puntos a unir y un entero que informara del color para pintarla
                    agregarLinea(esqueleto.Joints[JointType.Head], esqueleto.Joints[JointType.ShoulderCenter],2);
                    agregarLinea(esqueleto.Joints[JointType.ShoulderCenter], esqueleto.Joints[JointType.Spine],2);

                    // LLamamos a la funcion agregar Linea para dibujar la Pierna Izquierda, pasamos por parametros los puntos a unir y un entero que informara del color para pintarla
                    // En este caso pintamos hasta la rodilla, dado que nuestro movimiento consiste en detectar que levantamos la pierna izquierda hacia atras.
                    agregarLinea(esqueleto.Joints[JointType.Spine], esqueleto.Joints[JointType.HipCenter],2);
                    agregarLinea(esqueleto.Joints[JointType.HipCenter], esqueleto.Joints[JointType.HipLeft],2);
                    agregarLinea(esqueleto.Joints[JointType.HipLeft], esqueleto.Joints[JointType.KneeLeft],2);


                    // LLamamos a la funcion agregar Linea para dibujar la Pierna Derecha, pasamos por parametros los puntos a unir y un entero que informara del color para pintarla
                    agregarLinea(esqueleto.Joints[JointType.HipCenter], esqueleto.Joints[JointType.HipRight],2);
                    agregarLinea(esqueleto.Joints[JointType.HipRight], esqueleto.Joints[JointType.KneeRight],2);
                    agregarLinea(esqueleto.Joints[JointType.KneeRight], esqueleto.Joints[JointType.AnkleRight],2);
                    agregarLinea(esqueleto.Joints[JointType.AnkleRight], esqueleto.Joints[JointType.FootRight],2);
                                    
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

                    int  a, b, c, d;
                    double distanciaPD, distanciaPI, angulo, division, grados;
                    double g_entrada;
                    g_entrada = 0;

                    a = (int)(posicionRI.Z * 100.0);                                    //Almacenamos en variables enteras los puntos necesarios con una precision de dos digitos para calcular las distancias
                    b = (int)(posicionTI.Z * 100.0);
                    c = (int)(posicionRD.Y * 100.0);
                    d = (int)(posicionTD.Y * 100.0);

                    distanciaPI = Math.Abs (b - a);                                        // lado opuesto
                    distanciaPD = Math.Abs(c - d);                                        // lado adyacente

                    // Lo realizamos calculando la arcotangente, puesto que el calculo lo obtenemos usando la pierna derecha y la proyeccion del punto de la rodilla izquierda unido al punto
                    //del tobillo izquierdo sobre el plano Z
                    
                        division = (distanciaPI / distanciaPD);                         // Obtenemos la tangente
                        angulo = Math.Atan(division);                                   //Realizamos la arcotangente
                        grados = ((angulo * 180) / Math.PI);                            // Convertimos de radianes a grados

                        if (g_entrada == 0 || g_entrada >90)                            //Como los grados para comprobar que el ejercicio esta bien hecho los introduciremos en el futuro manualmente
                        {                                                               //comprobamos que dicha variable esta entre 0 y 90 grados si no es asi, ponemos 20 grados por defecto
                            g_entrada = 20;                             
                        }
                        else
                        {
                            g_entrada = g_entrada / 2;                                  //En caso contrario dividiremos entre dos los grados dado que el grado maximo que obtenemos es de 45, dado que usamos la 
                        }                                                               // arcotangente para calcular los grados
                    if (grados >= g_entrada)                                             // Si superamos los grados dibujaremos la pierna correctamente, con su color apropiado
                    {
                        agregarLinea(esqueleto.Joints[JointType.KneeLeft], esqueleto.Joints[JointType.AnkleLeft],2);
                        agregarLinea(esqueleto.Joints[JointType.AnkleLeft], esqueleto.Joints[JointType.FootLeft], 2);

                    }                                                                   // En caso contrario lo dibujamos con un color diferente para que veamos que no es correcto el movimiento
                    else
                    {
                        agregarLinea(esqueleto.Joints[JointType.KneeLeft], esqueleto.Joints[JointType.AnkleLeft], 1);
                        agregarLinea(esqueleto.Joints[JointType.AnkleLeft], esqueleto.Joints[JointType.FootLeft], 1);
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
                    manoD = (int)(posicionMD.Y * 10.0);
                    codoD = (int)(posicionCD.Y * 10.0);
                    hombroD = (int)(posicionHD.Y * 10.0);

                    if ((hombroD == manoD) && (hombroD == codoD))
                    {
                        // Brazo Derecho
                        agregarLinea(esqueleto.Joints[JointType.ShoulderCenter], esqueleto.Joints[JointType.ShoulderRight], 2);
                        agregarLinea(esqueleto.Joints[JointType.ShoulderRight], esqueleto.Joints[JointType.ElbowRight], 2);
                        agregarLinea(esqueleto.Joints[JointType.ElbowRight], esqueleto.Joints[JointType.WristRight], 2);
                        agregarLinea(esqueleto.Joints[JointType.WristRight], esqueleto.Joints[JointType.HandRight], 2);
                    }
                    else
                    {
                        // Brazo Derecho
                        agregarLinea(esqueleto.Joints[JointType.ShoulderCenter], esqueleto.Joints[JointType.ShoulderRight], 1);
                        agregarLinea(esqueleto.Joints[JointType.ShoulderRight], esqueleto.Joints[JointType.ElbowRight], 1);
                        agregarLinea(esqueleto.Joints[JointType.ElbowRight], esqueleto.Joints[JointType.WristRight], 1);
                        agregarLinea(esqueleto.Joints[JointType.WristRight], esqueleto.Joints[JointType.HandRight], 1);
                    }

                    // Dibujamos el brazo izquierdo siguiendo el mismo proceso que en el derecho
                    
                    Joint jointMI = esqueleto.Joints[JointType.WristLeft];
                    SkeletonPoint posicionMI = jointMI.Position;                 
                    Joint jointCI = esqueleto.Joints[JointType.ElbowLeft];
                    SkeletonPoint posicionCI = jointCI.Position;           
                    Joint jointHI = esqueleto.Joints[JointType.ShoulderLeft];
                    SkeletonPoint posicionHI = jointHI.Position;
                    
                    int manoI, codoI, hombroI;
                    manoI = (int)(posicionMI.Y * 10.0);
                    codoI = (int)(posicionCI.Y * 10.0);
                    hombroI = (int)(posicionHI.Y * 10.0);

                    if ((hombroI == manoI) && (hombroI == codoI))
                    {

                        // Brazo Izquierdo
                        agregarLinea(esqueleto.Joints[JointType.ShoulderCenter], esqueleto.Joints[JointType.ShoulderLeft], 2);
                        agregarLinea(esqueleto.Joints[JointType.ShoulderLeft], esqueleto.Joints[JointType.ElbowLeft], 2);
                        agregarLinea(esqueleto.Joints[JointType.ElbowLeft], esqueleto.Joints[JointType.WristLeft], 2);
                        agregarLinea(esqueleto.Joints[JointType.WristLeft], esqueleto.Joints[JointType.HandLeft], 2);
                    }
                    else
                    {
                        // Brazo Izquierdo
                        agregarLinea(esqueleto.Joints[JointType.ShoulderCenter], esqueleto.Joints[JointType.ShoulderLeft], 1);
                        agregarLinea(esqueleto.Joints[JointType.ShoulderLeft], esqueleto.Joints[JointType.ElbowLeft], 1);
                        agregarLinea(esqueleto.Joints[JointType.ElbowLeft], esqueleto.Joints[JointType.WristLeft], 1);
                        agregarLinea(esqueleto.Joints[JointType.WristLeft], esqueleto.Joints[JointType.HandLeft], 1);
                    }
                }
            }

        }
        // Esta funcion nos sirve para dibujar las lineas que unen los puntos, es decir, los huesos
        void agregarLinea(Joint j1, Joint j2, int colorea)
        {
            Line lineaHueso = new Line();
            if (colorea == 2) {                                                 //seleccionamos color
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
    }
}
