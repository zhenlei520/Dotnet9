﻿namespace Dotnet9.EventBus;

public interface IEventBus
{
    void Publish(string eventName, object? eventData);


    void Subscribe(string eventName, Type handlerType);

    void Unsubscribe(string eventName, Type handlerType);
}