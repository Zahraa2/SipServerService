using Microsoft.Extensions.Configuration;
using SIPServer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SIPServer.Call
{
    internal abstract  class KHService
    {
        protected SIPCall                   _call;
        protected CancellationTokenSource   _cancellationTokenSource;

        public KHService(SIPCall call)
        {
            _call = call;

            _cancellationTokenSource = new CancellationTokenSource();
        }

        public abstract void main();

        public virtual async Task Initialization() 
        { 
        }

        public virtual void Finalization()
        {
        }

        public async void Start()
        {
            await Initialization();

            Task.Run(() => main());
        }

        public void Stop()
        {
            _cancellationTokenSource.Cancel();

            Finalization();
        }

    }
}
