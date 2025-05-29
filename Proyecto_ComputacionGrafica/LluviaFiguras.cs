using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Proyecto_ComputacionGrafica
{
    public partial class LluviaFiguras : Form
    {

        public LluviaFiguras()
        {
            InitializeComponent();
            ReproductorMedia mReproductor = new ReproductorMedia(pictureBox1);
        }
    }
}
