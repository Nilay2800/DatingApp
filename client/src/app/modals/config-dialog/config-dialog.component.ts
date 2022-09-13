import { Component, OnInit } from '@angular/core';
import { BsModalRef } from 'ngx-bootstrap/modal';

@Component({
  selector: 'app-config-dialog',
  templateUrl: './config-dialog.component.html',
  styleUrls: ['./config-dialog.component.css']
})
export class ConfigDialogComponent implements OnInit {
  title:string;
  message:string;
  btnOkText:string;
  btnCancleText:string;
  result:boolean;

  constructor(public bsModalRef:BsModalRef) { }

  ngOnInit(): void {
  }

  confirm(){
    this.result=true;
    this.bsModalRef.hide();                                                                          
  }

  decline(){
    this.result=false;
    this.bsModalRef.hide();
  }

}
