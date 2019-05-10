using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Proyecto_microSQL
{
    static class Program
    {
        // Solo manda a llamar
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            (new Form1()).Show();
            Application.Run(); 

        }
    }
}
