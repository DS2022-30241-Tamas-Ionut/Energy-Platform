﻿namespace Assignment1.Business.Interfaces
{
    public interface IReceiveMessageService
    {
        void ReceiveMessage();
        Task<string> ConsumeMessage();
    }
}
