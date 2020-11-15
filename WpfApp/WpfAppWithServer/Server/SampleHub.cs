using MagicOnion.Server.Hubs;
using Sample.Shared;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Server
{
    public class SampleHub : StreamingHubBase<ISampleHub, ISampleHubReceiver>, ISampleHub
    {
        IGroup room;
        string me;

        public async Task JoinAsync(string name)
        {
            const string roomName = "SampleRoom";

            Logger.Debug($"{nameof(JoinAsync)}:{name}");
            Debug.WriteLine($"{nameof(JoinAsync)}:{name}");

            me = name;
            this.room = await this.Group.AddAsync(roomName);
            this.Broadcast(room).OnJoin(me);
        }

        public async Task LeaveAsync()
        {
            Logger.Debug($"{nameof(LeaveAsync)}:{me}");
            Debug.WriteLine($"{nameof(LeaveAsync)}:{me}");
            await room.RemoveAsync(this.Context);
            this.Broadcast(room).OnLeave(me);
        }

        public async Task SendMessageAsync(string message)
        {
            Logger.Debug($"{nameof(SendMessageAsync)}:[{me}]{message}");
            Debug.WriteLine($"{nameof(SendMessageAsync)}:[{me}]{message}");
            this.Broadcast(room).OnSendMessage(me, message);
        }


        public async Task MovePosition(Vector3 position, Quaternion rotation)
        {
            this.Broadcast(room).OnMovePosition(position, rotation, me);
        }

        public async Task UpdateRotatableAxisAsync(RotatableAxis axis)
        {
            Logger.Debug($"{nameof(UpdateRotatableAxisAsync)}:[{me}]{axis}");
            Debug.WriteLine($"{nameof(UpdateRotatableAxisAsync)}:[{me}]{axis}");
            this.Broadcast(room).OnUpdateRotatableAxis(axis);
        }

        protected override ValueTask OnDisconnected()
        {
            return CompletedTask;
        }
    }
}
