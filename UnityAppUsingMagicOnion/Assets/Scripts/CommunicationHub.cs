using Grpc.Core;
using MagicOnion.Client;
using Sample.Shared;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CommunicationHub : MonoBehaviour, ISampleHubReceiver
{
    public GameObject sharedTarget;
    public TextMeshPro msg;

    private Channel channel;
    private ISampleHub sampleHub;
    private const string myName = "Unity";

    public Action<string, string> OnSendMessageCallback;
    public Action<RotatableAxis> OnUpdateRotatableAxisCallback;

    async void Start()
    {
        this.channel = new Channel("localhost:12345", ChannelCredentials.Insecure);
        this.sampleHub = StreamingHubClient.Connect<ISampleHub, ISampleHubReceiver>(channel, this);
        Application.wantsToQuit += Application_wantsToQuit;

        msg.text = "";
        await this.sampleHub.JoinAsync(myName);
        await this.sampleHub.SendMessageAsync($"Hello outside world.");
        await this.sampleHub.MovePosition(sharedTarget.transform.position, sharedTarget.transform.rotation);
    }

    int frameCount = 0; 
    async void Update()
    {
        if (cleanuping) { return; }
        if (frameCount == 0)
        {
            await this.sampleHub.MovePosition(sharedTarget.transform.position, sharedTarget.transform.rotation);
        }

        if (frameCount < 10)
            frameCount++;
        else
            frameCount = 0;
    }

    private bool Application_wantsToQuit()
    {
        Debug.Log("Application_wantsToQuit");
        CleanUp();
        return false;
    }

    async void OnDestroy()
    {
        Debug.Log("OnDestroy");
        CleanUp();
    }

    bool cleanuping;
    async void CleanUp()
    {
        if (!cleanuping)
        {
            try
            {
                cleanuping = true;
                Debug.Log("Disposing ...");
                await this.sampleHub.LeaveAsync();
                await this.sampleHub.DisposeAsync();
                await this.channel.ShutdownAsync();
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
            }
        }
        Application.wantsToQuit -= Application_wantsToQuit;
        Application.Quit();
    }

    #region ISampleHubReceiver

    public void OnJoin(string name)
    {
        Debug.Log($"{name} joined.");
    }

    public void OnLeave(string name)
    {
        Debug.Log($"{name} leaved.");
    }

    public void OnSendMessage(string name, string message)
    {
        Debug.Log($"{name} : {message}");
        msg.text += $"{name} : {message}\r\n";
    }

    public void OnMovePosition(Vector3 position, Quaternion rotation, string me)
    {
        Debug.Log($"{me} : Position{position} / Rotation:{rotation}");
    }

    public void OnUpdateRotatableAxis(RotatableAxis axis)
    {
        Debug.Log($"RotatableAxis updated : {axis}");
        OnUpdateRotatableAxisCallback?.Invoke(axis);
    }

    #endregion
}
