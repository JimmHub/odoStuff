using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.Integration;
namespace EmguTest
{
    public partial class MEMSForm : Form
    {
        public MEMSForm()
        {
            InitializeComponent();
        }

        public Wpf3DControl.UserControl1 Wpf3DControl;

        private void elementHost1_ChildChanged(object sender, System.Windows.Forms.Integration.ChildChangedEventArgs e)
        {

        }

        private void MEMSForm_Load(object sender, EventArgs e)
        {
            this.LoadWpf3dControl();
        }

        private void LoadWpf3dControl()
        {
            // Create the ElementHost control for hosting the
            // WPF UserControl.
            ElementHost host = this.elementHost1;
            //host.Dock = DockStyle.Fill;

            // Create the WPF UserControl.
            this.Wpf3DControl =
                new Wpf3DControl.UserControl1();

            // Assign the WPF UserControl to the ElementHost control's
            // Child property.
            host.Child = this.Wpf3DControl;

            // Add the ElementHost control to the form's
            // collection of child controls.
            //this.Controls.Add(host);
        }

        public void SetMEMSTransformationMatrix(double[][] matrix)
        {
            this.Wpf3DControl.SetTransformMatrix(matrix);
        }
    }
}
