using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WinformsEngine.Engine.Systems.Scene;
using WinformsEngine.Utils;

namespace WinformsEngine.Samples.Snake
{
    public partial class SnakeForm : Form
    {
        private Scene _scene = new Scene();

        public SnakeForm()
        {
            InitializeComponent();
        }

        private void SnakeForm_Load(object sender, EventArgs e)
        {
            this.ConfigureForm(_scene);
        }
    }
}
