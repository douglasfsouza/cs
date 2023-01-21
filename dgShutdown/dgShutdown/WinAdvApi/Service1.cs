using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WinAdvApi
{
    public partial class Service1 : ServiceBase
    {
        private Timer _timer;
        public Service1()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            _timer = new Timer(Desligar, null, 0, 120000);
        }

        protected override void OnStop()
        {
            Win w = new Win();
            w.Desligar();
        }

        protected override void OnPause()
        {
            Win w = new Win();             
            w.Desligar();            

        }

        private void Desligar(object state)
        {
            Win w = new Win();
            if (DateTime.Now.Hour > 22 || DateTime.Now.Hour < 6)
            {
                w.Desligar();
            }
        }

        

    }
}
