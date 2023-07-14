import { FileInfo } from './../../../../shared/models/dto/request/file-info';
import { Component } from '@angular/core';
import { BsEighteenService } from '../../../../core/http/bs-eighteen.service'
import { DatePipe } from '@angular/common'
import Swal from 'sweetalert2';
import {SweetAlertService} from '../../../../core/service/sweet-alert.service'
import { UploadInfo, UploadType } from 'src/app/shared/components/new-upload/new-upload.component';
import { environment } from 'src/environments/environment';
import { FileTypeCode } from 'src/app/shared/models/file-type-code';


@Component({
  selector: 'app-bs-eighteen',
  templateUrl: './bs-eighteen.component.html',
  styleUrls: ['./bs-eighteen.component.scss']
})

export class BsEighteenComponent 
{  
  isLoading: boolean = false;
  loadingMsg: string = "";
  notificationS18: string = "";
  notificationB18: string = "";
  notificationPkResult: string = "";
  uploadApiURL: string = "";

  constructor(
    private _bsEighteenService: BsEighteenService,
    private _swlService: SweetAlertService,
    private datepipe: DatePipe
  ){  }

  ngOnInit()
  {
    this.uploadApiURL = `${environment.apiBaseUrl}/BsEighteen/Upload`;
  } 

  setUploadInfo(data: UploadInfo)
  {
    let fileInfo: FileInfo = {fileName: data.fileName, fileType: data.uploadFileType}
    let typeCN:string = '';
    let msgSuccess:string = '';
    let msgFail:string = '';

    switch(data.uploadFileType)
    {
      case FileTypeCode.S18 :
        typeCN = '小18報表';
        break;
      case FileTypeCode.B18 :
        typeCN = '大18報表';
        break;
      case FileTypeCode.Vendor :
        typeCN = 'pk報表';
        break;
    }

    msgSuccess = `${typeCN}匯入完成.`;
    msgFail = `${typeCN}匯入異常，請聯絡CAE Team.`;

    if(data.status == UploadType.Processing)
    {
      this.loadingMsg = '檔案上傳中...';
      this.isLoading = true;
    }
    else if (data.status == UploadType.Success)
    {    
      this.loadingMsg = '檔案資料處理中...';      
      if (data.uploadFileType == FileTypeCode.S18) 
      {
        this._bsEighteenService.insertS18(fileInfo).subscribe({
          next: (res) => 
          {
            if (res.success) 
            {
              this._swlService.showSwal('', msgSuccess, 'info');
              this.notificationS18 = res.content;
            }            
            else 
            {
              this._swlService.showSwal('', `${msgFail}<br\>${res.message}`, 'error');
            }
          },
          error: (err) =>{
            this._swlService.showSwal('', msgFail, 'error');
            this.isLoading = false;
          },
          complete: () =>{
            this.isLoading = false;
          }
        })
      }
      else if (data.uploadFileType == FileTypeCode.B18) 
      {
        this._bsEighteenService.insertB18(fileInfo).subscribe({
          next: (res) => 
          {
            if (res.success) 
            {
              this._swlService.showSwal('', msgSuccess, 'info');
              this.notificationB18 = res.content;
            }            
            else 
            {
              this._swlService.showSwal('', `${msgFail}<br\>${res.message}`, 'error');
            }
          },
          error: (err) =>
          {
            this._swlService.showSwal('', msgFail, 'error');
            this.isLoading = false;
          },
          complete: () =>
          {
            this.isLoading = false;
          }
        })
      }
      else if (data.uploadFileType == FileTypeCode.Vendor) 
      {
        this._bsEighteenService.exportPK(fileInfo).subscribe(res => {
          if (res.success)
          {
            this._swlService.showSwal('', 'pk報表產出成功.', 'info');
            this.notificationPkResult = `報表最後產出時間：${this.datepipe .transform(new Date(), "yyyy/MM/dd HH:mm:ss")}`;
          }
          else 
          {
            this._swlService.showSwal('', `pk報表產出異常，請聯絡CAE Team.<br\>${res.message}`, 'error');
          }

          this.isLoading = false;
        })
      }
    }
    else if(data.status == UploadType.Fail)
    {
      this._swlService.showSwal('', `${data.fileName}上傳失敗,請聯絡CAE Team。${data.message}`, 'error');      
      this.isLoading = false;
    }

  } 
}
