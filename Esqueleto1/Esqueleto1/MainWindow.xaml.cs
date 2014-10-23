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

namespace Esqueleto1
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private KinectSensor sensor;
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
            //throw new NotImplementedException();  ver si funciona

            string mensaje = "no hay datos de esqueleto";
            string mensajeCalidad = "";
            string mensaje2 = "";
            string mensaje3= "";
            string mensaje4= "";
            string mensaje5 = "";
            string mensaje6 = "";
            
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
                    /*
                    if (esqueleto.ClippedEdges == 0)
                    {
                        mensajeCalidad = "Colocado Perfectamente";
                    }
                    else
                    {

                    }
                     * */

                    Joint jointMI = esqueleto.Joints[JointType.WristLeft];

                    SkeletonPoint posicionMI = jointMI.Position;
                    mensaje = string.Format("MI : X:{0:0.0} Y:{1:0.0} Z:{2:0.0}", posicionMI.X, posicionMI.Y, posicionMI.Z);

                    Joint jointCI = esqueleto.Joints[JointType.ElbowLeft];

                    SkeletonPoint posicionCI = jointCI.Position;
                    mensaje2 = string.Format("CI : X:{0:0.0} Y:{1:0.0} Z:{2:0.0}", posicionCI.X, posicionCI.Y, posicionCI.Z);

                    Joint jointHI = esqueleto.Joints[JointType.ShoulderLeft];

                    SkeletonPoint posicionHI = jointHI.Position;
                    mensaje3 = string.Format("HI : X:{0:0.0} Y:{1:0.0} Z:{2:0.0}", posicionHI.X, posicionHI.Y, posicionHI.Z);


                    Joint jointMD = esqueleto.Joints[JointType.WristRight];

                    SkeletonPoint posicionMD = jointMD.Position;
                    mensaje4 = string.Format("MD : X:{0:0.0} Y:{1:0.0} Z:{2:0.0}", posicionMD.X, posicionMD.Y, posicionMD.Z);


                    Joint jointCD = esqueleto.Joints[JointType.ElbowRight];

                    SkeletonPoint posicionCD = jointCD.Position;
                    mensaje5 = string.Format("CD : X:{0:0.0} Y:{1:0.0} Z:{2:0.0}", posicionCD.X, posicionCD.Y, posicionCD.Z);

                    Joint jointHD = esqueleto.Joints[JointType.ShoulderRight];

                    SkeletonPoint posicionHD= jointHD.Position;
                    mensaje6 = string.Format("HD : X:{0:0.0} Y:{1:0.0} Z:{2:0.0}", posicionHD.X, posicionHD.Y, posicionHD.Z);

                    int a, b, c;
                    a = (int)(posicionMD.Y*10.0) ;
                    b = (int)(posicionCD.Y*10.0) ;
                    c = (int)(posicionHD.Y*10.0) ;
                    
                    if ((c == a) && (c == b))
                    {
                        mensaje3 = " BIEN";
                    }
                }
            }
            textBlockStatus.Text = mensaje;
            textBlockCaptura.Text = mensaje2;
            cajaHI.Text = mensaje3;
            cajaMD.Text = mensaje4;
            cajaBD.Text = mensaje5;
            cajaHD.Text = mensaje6;

           
        }
    }
}
