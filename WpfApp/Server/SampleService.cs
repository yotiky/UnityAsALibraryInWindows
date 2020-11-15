using MagicOnion;
using MagicOnion.Server;
using MagicOnion.Server.Hubs;
using Sample.Shared;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Server
{
    class SampleService : ServiceBase<ISampleService>, ISampleService
    {
        public async UnaryResult<string> EchoAsync(string name)
        {
            Logger.Debug($"{nameof(EchoAsync)} received:{name}");
            return $"Hello {name}.";
        }

        public async UnaryResult<int> SumAsync(int x, int y)
        {
            Logger.Debug($"{nameof(SumAsync)} received:{x}, {y}");
            return x + y;
        }
    }
}
