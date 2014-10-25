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

        KinectSensor sensor;
        
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (KinectSensor.KinectSensors.Count == 0)
            {
                MessageBox.Show("Ningun kinect detectado");
                Application.Current.Shutdown();
                return;
            }

            sensor = KinectSensor.KinectSensors.FirstOrDefault();

            try
            {
                sensor.SkeletonStream.Enable();
                sensor.Start();
            }
            catch
            {
                MessageBox.Show("Error en la iniciacion de Kinect");
                Application.Current.Shutdown();
            }

            sensor.SkeletonFrameReady += sensor_SkeletonFrameReady;
        }

        void sensor_SkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e)
        {

           canvasEsqueleto.Children.Clear();
           Skeleton[] esqueletos = null;

            using (SkeletonFrame frameEsqueleto = e.OpenSkeletonFrame())
            {
                if (frameEsqueleto != null)
                {
                    esqueletos = new Skeleton[frameEsqueleto.SkeletonArrayLength];
                    frameEsqueleto.CopySkeletonDataTo(esqueletos);

                }
            }

            if (esqueletos == null) return;

            foreach (Skeleton esqueleto in esqueletos)
            {
                if (esqueleto.TrackingState == SkeletonTrackingState.Tracked)
                {
                    // Columna Vertebral
                    agregarLinea(esqueleto.Joints[JointType.Head], esqueleto.Joints[JointType.ShoulderCenter],2);
                    agregarLinea(esqueleto.Joints[JointType.ShoulderCenter], esqueleto.Joints[JointType.Spine],2);

                    // Pierna Izquierda
                    agregarLinea(esqueleto.Joints[JointType.Spine], esqueleto.Joints[JointType.HipCenter],2);
                    agregarLinea(esqueleto.Joints[JointType.HipCenter], esqueleto.Joints[JointType.HipLeft],2);
                    agregarLinea(esqueleto.Joints[JointType.HipLeft], esqueleto.Joints[JointType.KneeLeft],2);
                   

                    // Pierna Derecha
                    agregarLinea(esqueleto.Joints[JointType.HipCenter], esqueleto.Joints[JointType.HipRight],2);
                    agregarLinea(esqueleto.Joints[JointType.HipRight], esqueleto.Joints[JointType.KneeRight],2);
                    agregarLinea(esqueleto.Joints[JointType.KneeRight], esqueleto.Joints[JointType.AnkleRight],2);
                    agregarLinea(esqueleto.Joints[JointType.AnkleRight], esqueleto.Joints[JointType.FootRight],2);
                                    
                    Joint jointRI = esqueleto.Joints[JointType.KneeLeft];
                    SkeletonPoint posicionRI = jointRI.Position;
                    Joint jointTI = esqueleto.Joints[JointType.AnkleLeft];
                    SkeletonPoint posicionTI = jointTI.Position;

                    Joint jointRD = esqueleto.Joints[JointType.KneeRight];
                    SkeletonPoint posicionRD = jointRD.Position;
                    Joint jointTD = esqueleto.Joints[JointType.AnkleRight];
                    SkeletonPoint posicionTD = jointTD.Position;


                    int  a, b, c, d;
                    double distanciaPD, distanciaPI, angulo, division, grados;
                    double g_entrada;
                    g_entrada = 0;

                    a = (int)(posicionRI.Z * 100.0);
                    b = (int)(posicionTI.Z * 100.0);
                    c = (int)(posicionRD.Y * 100.0);
                    d = (int)(posicionTD.Y * 100.0);

                    distanciaPI = Math.Abs (b - a);                                        // lado opuesto
                    distanciaPD = Math.Abs(c - d);                                        // lado adyacente

                    
                    
                        division = (distanciaPI / distanciaPD);
                        angulo = Math.Atan(division);
                        grados = ((angulo * 180) / Math.PI);

                        if (g_entrada == 0 || g_entrada >90)
                        {
                            g_entrada = 20;
                        }
                        else
                        {
                            g_entrada = g_entrada / 2; 
                        }
                    if (grados > 20)
                    {
                        agregarLinea(esqueleto.Joints[JointType.KneeLeft], esqueleto.Joints[JointType.AnkleLeft],2);
                        agregarLinea(esqueleto.Joints[JointType.AnkleLeft], esqueleto.Joints[JointType.FootLeft], 2);

                    }
                    else
                    {
                        agregarLinea(esqueleto.Joints[JointType.KneeLeft], esqueleto.Joints[JointType.AnkleLeft], 1);
                        agregarLinea(esqueleto.Joints[JointType.AnkleLeft], esqueleto.Joints[JointType.FootLeft], 1);
                    }

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
        void agregarLinea(Joint j1, Joint j2, int colorea)
        {
            Line lineaHueso = new Line();
            if (colorea == 2) {
                lineaHueso.Stroke = new SolidColorBrush(Colors.GreenYellow);
            }
            else
            {
                lineaHueso.Stroke = new SolidColorBrush(Colors.Blue);
            }
            lineaHueso.StrokeThickness = 5;

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
