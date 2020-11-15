using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Linq;
using MessagePack;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UniRx;
using UnityEngine;

public class MessageReceiver : MonoBehaviour
{
    public ProcessPipelineClient pipelineClient;
    public ProcessPipelineServer pipelineServer;
    public ObjectRotator rotator;
    public TextMeshPro tmp;
    private Channel<IMessageBody> channel;

    async void Start()
    {
        tmp.text = "";

        channel = Channel.CreateSingleConsumerUnbounded<IMessageBody>();

        RunConsumerAsync().Forget();

        pipelineClient.OnReceived
            .Subscribe(x =>
            {
                var deserialized = MessagePackSerializer.Deserialize<IMessageBody>(x);
                channel.Writer.TryWrite(deserialized);
            })
            .AddTo(this);

        await pipelineServer.ConnectionAsync();
        var msg = new Letter
        {
            Name = "Unity",
            Message = "Hello outside world.",
        };
        var serialized = MessagePackSerializer.Serialize<IMessageBody>(msg);
        await pipelineServer.Write(serialized);
    }

    private async UniTaskVoid RunConsumerAsync()
    {
        await channel.Reader.ReadAllAsync(this.GetCancellationTokenOnDestroy())
            .ForEachAwaitAsync(async x =>
            {
                switch (x)
                {
                    case UpdateRotatableAxis body:
                        rotator.RotatableAxis = body.Axis;
                        break;
                    case Letter body:
                        tmp.text += $"{body.Name} : {body.Message}\r\n";
                        break;
                    default:
                        break;
                }
            });
    }
}
