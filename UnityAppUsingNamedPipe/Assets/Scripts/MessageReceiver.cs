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
    public ProcessPipelineClient pipeline;
    public ObjectRotator rotator;
    public TextMeshPro tmp;
    private Channel<IMessageBody> channel;

    void Start()
    {
        tmp.text = "";

        channel = Channel.CreateSingleConsumerUnbounded<IMessageBody>();

        RunConsumerAsync().Forget();

        pipeline.OnReceived
            .Subscribe(x =>
            {
                var deserialized = MessagePackSerializer.Deserialize<IMessageBody>(x);
                channel.Writer.TryWrite(deserialized);
            })
            .AddTo(this);
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
