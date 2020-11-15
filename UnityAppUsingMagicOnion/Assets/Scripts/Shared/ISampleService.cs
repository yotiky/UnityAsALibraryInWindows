using Grpc.Core;
using MagicOnion;
using MessagePack;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Sample.Shared
{

    public interface ISampleService : IService<ISampleService>
    {
        UnaryResult<string> EchoAsync(string name);
        UnaryResult<int> SumAsync(int x, int y);
    }
    public interface ISampleHub : IStreamingHub<ISampleHub, ISampleHubReceiver>
    {
        Task JoinAsync(string name);
        Task LeaveAsync();
        Task SendMessageAsync(string message);
        Task MovePosition(Vector3 position, Quaternion rotation);
        Task UpdateRotatableAxisAsync(RotatableAxis axis);
    }
    public interface ISampleHubReceiver
    {
        void OnJoin(string name);
        void OnLeave(string name);
        void OnSendMessage(string name, string message);
        void OnMovePosition(Vector3 position, Quaternion rotation, string name);
        void OnUpdateRotatableAxis(RotatableAxis axis);
    }
    public enum RotatableAxis
    {
        None = 0,
        X = 1,
        Y = 2,
        XY = 3,
    }
}