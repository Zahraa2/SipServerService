﻿
using SIPServer;
using System.ServiceProcess;

namespace SipServer
{
    public partial class Service1 : ServiceBase
    {
        public Service1()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
        }

        protected override void OnStop()
        {
        }

        //public void onDebug()
        //{
        //    OnStart(null);
        //}
    }
}
