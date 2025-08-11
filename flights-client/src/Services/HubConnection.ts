import * as signalR from '@microsoft/signalr';

export const connection = new signalR.HubConnectionBuilder()
  .withUrl('https://localhost:7172/flightsHub') 
  .withAutomaticReconnect()
  .build();