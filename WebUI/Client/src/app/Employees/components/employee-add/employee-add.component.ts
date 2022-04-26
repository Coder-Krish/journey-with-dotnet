import { ChangeDetectorRef, Component, OnInit } from '@angular/core';
import * as signalR from '@microsoft/signalr';  
import { environment } from 'src/environments/environment';

@Component({
  selector: 'app-employee-add',
  templateUrl: './employee-add.component.html',
  styleUrls: ['./employee-add.component.css']
})
export class EmployeeAddComponent implements OnInit {

  public message:string = "Message Displaying area..";

  constructor(private changeDetector:ChangeDetectorRef) { }

  ngOnInit(): void {
    const connection = new signalR.HubConnectionBuilder()  
    .configureLogging(signalR.LogLevel.Information)
    .withUrl(environment.baseUrl + 'notify', {skipNegotiation:true, transport: signalR.HttpTransportType.WebSockets})  
    .build();  

  connection.start().then(function () {  
    console.log('SignalR Connected!');  
  }).catch(function (err) {  
    return console.error(err.toString());  
  });  

  connection.on("Notification",(message)=>{
    this.message = message; 
    this.changeDetector.detectChanges();
  });
  }

}
