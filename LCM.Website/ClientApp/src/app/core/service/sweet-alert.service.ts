import { Injectable } from '@angular/core';
import Swal, {SweetAlertOptions, SweetAlertIcon, SweetAlertResult} from 'sweetalert2/dist/sweetalert2.js';


@Injectable({
  providedIn: 'root'
})
export class SweetAlertService {

  constructor() { }

  Swal = Swal;

  showSwal(
    title?: string,
    message?: string,
    icon?: SweetAlertIcon,
    options?: SweetAlertOptions
  ): Promise<SweetAlertResult<any>> {
    options = Object.assign(
      {
        title: title,
        html: message,
        icon: icon       
      },
      options
    );

    return Swal.fire(options);
  }

  showSwalConfirm(
    title: string,
    text: string,
    icon: SweetAlertIcon,
    confirmCallBack?: () => any,
    cancelCallBack?: () => any,
    confirmButtonText?: string,
    cancelButtonText?: string) {

    if (confirmCallBack == undefined) {
      confirmCallBack = () => { };
    }
    if (cancelCallBack == undefined) {
      cancelCallBack = () => { };
    }
    if (!confirmButtonText) {
      confirmButtonText = "是"
    }
    if (!cancelButtonText) {
      cancelButtonText = "否"
    }

    Swal.fire({
      title: title,
      html: text,
      icon: icon,
      showCancelButton: true,
      confirmButtonText: confirmButtonText,
      cancelButtonText: cancelButtonText,
      allowOutsideClick: false,//點擊空白處關閉confirm
      allowEscapeKey: false,//ESC關閉confirm
    }).then((result) => {
      if (result.value) {
        confirmCallBack;
      }
      else if (result.dismiss === Swal.DismissReason.cancel) {
        cancelCallBack;
      }
    })
  }


  showSwalNoButtonConfirm(
    title: string,
    text: string,
    icon: SweetAlertIcon)
  {
    Swal.fire({
      title: title,
      html: text,
      icon: icon,
      showConfirmButton:false,
      showCancelButton: false,
      allowOutsideClick: false,//點擊空白處關閉confirm
      allowEscapeKey: false,//ESC關閉confirm
    })
  } 
}
