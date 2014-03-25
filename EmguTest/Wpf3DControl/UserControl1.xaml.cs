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
using System.Windows.Media.Media3D;
namespace Wpf3DControl
{
    /// <summary>
    /// Логика взаимодействия для UserControl1.xaml
    /// </summary>
    public partial class UserControl1 : UserControl
    {
        AxisAngleRotation3D ax3d;

        public UserControl1()
        {
            InitializeComponent();

            ax3d = new AxisAngleRotation3D(new Vector3D(0, 2, 0), 1);
            RotateTransform3D myRotateTransform = new RotateTransform3D(ax3d);

            var ModelVisual3D = (ModelVisual3D)this.myViewport.Children.FirstOrDefault();
            ModelVisual3D.Transform = myRotateTransform;
        }

        public void RotateTest()
        {
            //ax3d = new AxisAngleRotation3D(new Vector3D(0, 2, 0), 1);
            //RotateTransform3D myRotateTransform = new RotateTransform3D(ax3d);

            //var ModelVisual3D = (ModelVisual3D)this.myViewport.Children.FirstOrDefault();
            //var transform = (Transform3DGroup)ModelVisual3D.Transform;
            //RotateTransform3D t = new RotateTransform3D();
            //var cam = (PerspectiveCamera)this.myViewport.Camera;
            //cam.Position = new Point3D(cam.Position.X + 0.001, cam.Position.Y, cam.Position.Z );
            ax3d.Angle += 1;

            //

            //MyModel.Transform = myRotateTransform;
            ////
            // // 


        }
    }
}
