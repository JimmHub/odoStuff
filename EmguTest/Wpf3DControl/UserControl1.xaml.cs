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

            var ModelVisual3D = this.GetModel();
            MatrixTransform3D mTransform = new MatrixTransform3D();
            ModelVisual3D.Transform = myRotateTransform;

            //model
            ////
        }

        public ModelVisual3D GetModel()
        {
            return (ModelVisual3D)this.myViewport.Children.FirstOrDefault();
        }

        public void SetTransformMatrix(Matrix3D matrix)
        {
            MatrixTransform3D t3d = new MatrixTransform3D(matrix);
            this.GetModel().Transform = t3d;
        }

        public void SetTransformMatrix(double[][] matrix)
        {
            this.SetTransformMatrix(this.ConvertToMatrix3D(matrix));
        }

        public Matrix3D GetTransformMatrix()
        {
            return this.GetModel().Transform.Value;
        }

        public Matrix3D ConvertToMatrix3D(double[][] matrix)
        {
            Matrix3D res = new Matrix3D(
                matrix[0][0], matrix[0][1], matrix[0][2], 0,
                matrix[1][0], matrix[1][1], matrix[1][2], 0,
                matrix[2][0], matrix[2][1], matrix[2][2], 0,
                0, 0, 0, 1);

            return res;
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
            ax3d.Angle += 10;

            //

            //MyModel.Transform = myRotateTransform;
            ////
            // // 


        }
    }
}
