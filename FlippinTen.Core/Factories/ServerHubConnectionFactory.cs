using FlippinTen.Core.Interfaces;
using FlippinTen.Core.Utilities;
using FlippinTen.Models.Constants;
using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace FlippinTen.Core.Factories
{
    public class ServerHubConnectionFactory
    {
        public static IServerHubConnection Create(ICardGameService cardGameService, bool online)
        {
            return online
                ? new ServerHubConnection(new HubConnectionBuilder(), UriConstants.BaseUri + UriConstants.GameHub)
                : (IServerHubConnection)new ServiceHubLocal(cardGameService);
        }
    }
}
