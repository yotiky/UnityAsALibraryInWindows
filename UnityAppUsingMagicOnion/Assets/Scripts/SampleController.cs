using Grpc.Core;
using MagicOnion.Client;
using Sample.Shared;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SampleController : MonoBehaviour
{
    public TextMeshPro text;

    private Channel channel;
    private ISampleService sampleService;
    
    void Start()
    {
        try
        {
            this.channel = new Channel("localhost:12345", ChannelCredentials.Insecure);
            this.sampleService = MagicOnionClient.Create<ISampleService>(channel);

            CallService();
        }
        catch (Exception e)
        {
            text.text = e.Message;
        }
    }

    public async void CallService()
    {
        try
        {
            var echoResult = await this.sampleService.EchoAsync("world");
            Debug.Log($"{nameof(echoResult)}: {echoResult}");

            var sumResult = await this.sampleService.SumAsync(1, 2);
            Debug.Log($"{nameof(sumResult)}: {sumResult}");
        }
        catch (Exception e)
        {
            text.text = e.Message;
        }
    }

    async void OnDestroy()
    {
        await this.channel.ShutdownAsync();
    }
}
