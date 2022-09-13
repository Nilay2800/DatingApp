import { Injectable } from '@angular/core';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
import { observable, Observable } from 'rxjs';
import { ConfigDialogComponent } from '../modals/config-dialog/config-dialog.component';

@Injectable({
  providedIn: 'root'
})
export class ConfirmService {
  bsModelRef:BsModalRef;

  constructor(private modalService:BsModalService) { }

  confirm(title='Confirmation',message='Are you sure',
          btnOkText='Ok',btnCancleText='Cancle'):Observable<boolean> {
            const config={
              initialState:{
                title,
                message,
                btnOkText,btnCancleText
              }
            }
            this.bsModelRef=this.modalService.show(ConfigDialogComponent,config);
            return new Observable<boolean>(this.getResult());
            
          }

  private getResult(){
    return(observer)=>{
      const subscription=this.bsModelRef.onHidden.subscribe(()=> {
        observer.next(this.bsModelRef.content.result);
        observer.complete();
      });

      return{
        unsubscribe(){
          subscription.unsubscribe();
        }
      }
    }
  }
}
