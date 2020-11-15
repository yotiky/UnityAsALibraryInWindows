using Grpc.Core;
using MagicOnion.Client;
using Sample.Shared;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace WpfApp
{
    public class CommunicationHub : ISampleHubReceiver
    {
        private Channel channel;
        private ISampleHub sampleHub;

        public CommunicationHub()
        {
            this.channel = new Channel("localhost:12345", ChannelCredentials.Insecure);
            this.sampleHub = StreamingHubClient.Connect<ISampleHub, ISampleHubReceiver>(channel, this);
        }

        public Task JoinAsync(string name)
            => this.sampleHub.JoinAsync(name);

        public Task LeaveAsync()
            => this.sampleHub.LeaveAsync();

        public Task SendMessageAsync(string message)
            => this.sampleHub.SendMessageAsync(message);

        public Task UpdateRotatableAxis(RotatableAxis axis)
            => this.sampleHub.UpdateRotatableAxisAsync(axis);

        public async Task Dispose()
        {
            await this.sampleHub.LeaveAsync();
            await this.sampleHub.DisposeAsync();
            await this.channel.ShutdownAsync();
        }

        public Action<string> OnJoinCallback;
        public Action<string> OnLeaveCallback;
        public Action<string, string> OnSendMessageCallback;
        public Action<Vector3, Quaternion, string> OnMovePositionCallback;
        public Action<RotatableAxis> OnUpdateRotatableAxisCallback;

        public void OnJoin(string name)
        {
            Debug.WriteLine($"{name} joined.");
            OnJoinCallback?.Invoke(name);
        }

        public void OnLeave(string name)
        {
            Debug.WriteLine($"{name} leaved.");
            OnLeaveCallback?.Invoke(name);
        }

        public void OnSendMessage(string name, string message)
        {
            Debug.WriteLine($"{name} : {message}");
            OnSendMessageCallback?.Invoke(name, message);
        }

        public void OnMovePosition(Vector3 position, Quaternion rotation, string name)
        {
            Debug.WriteLine($"{name} : Position({position.x},{position.y},{position.z}) / Rotation({rotation.x},{rotation.y},{rotation.z},{rotation.w})");
            OnMovePositionCallback?.Invoke(position, rotation, name);
        }

        public void OnUpdateRotatableAxis(RotatableAxis axis)
        {
            Debug.WriteLine($"RotatableAxis updated : {axis}");
            OnUpdateRotatableAxisCallback?.Invoke(axis);
        }
    }
}
