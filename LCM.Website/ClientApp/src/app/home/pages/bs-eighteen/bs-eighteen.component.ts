import { SelectComponent } from './../../../shared/components/select/select.component';
import { LoadingService } from './../../../shared/components/loading/loading.service';
import { LoadingInfo } from './../../../shared/components/loading/loading.component';
import { ShareService } from './../../../core/service/share.service';
import { FileInfo } from './../../../shared/models/dto/request/file-info';
import { Component, ViewChild } from '@angular/core';
import { BsEighteenService } from '../../../core/http/bs-eighteen.service'
import { DatePipe } from '@angular/common'
import Swal from 'sweetalert2';
import {SweetAlertService} from '../../../core/service/sweet-alert.service'
import { UploadInfo, UploadType } from 'src/app/shared/components/new-upload/new-upload.component';
import { environment } from 'src/environments/environment';
import { FileTypeCode } from 'src/app/shared/models/file-type-code';
import { DropdownOption } from 'src/app/shared/components/select/select.component';


@Component({
  selector: 'app-bs-eighteen',
  templateUrl: './bs-eighteen.component.html',
  styleUrls: ['./bs-eighteen.component.scss']
})

export class BsEighteenComponent 
{  
  @ViewChild("selectTemplateFileType") selectTemplateFileType?:SelectComponent;
  loadingInfo: LoadingInfo = { isLoading: false}
  notificationS18: string = "";
  notificationB18: string = "";
  notificationPkResult: string = "";
  uploadApiURL: string = "";
  templateFileType: string|undefined = "";
  templateFileInfo: string = "";
  optTemplateFileType: DropdownOption[] = [
    {value:"", text:"無"}, 
    {value:"S18", text:"小18"}, 
    {value:"B18", text:"大18"},
    {value:"Vendor", text:"廠商提供報表"},
  ]

  constructor(
    private _bsEighteenService: BsEighteenService,
    private _swlService: SweetAlertService,
    private _shareService: ShareService,
    private _loadingService: LoadingService,
    private datepipe: DatePipe
  ){  }

  ngOnInit()
  {
    this.uploadApiURL = `${environment.apiBaseUrl}/BsEighteen/Upload`;
  } 

  ngAfterViewInit()
  {
    setTimeout(() => {
      this._shareService.emitChange("大小18報表比對");
    }); 

    //訂閱#selectFilterFileType的selectedValue$
    this.selectTemplateFileType?.selectedValue$.subscribe(res => {
      this.templateFileType = res;
      //console.log(c);
    })
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
      this._loadingService.setLoading(true, '檔案上傳中...');
    }
    else if (data.status == UploadType.Success)
    {     
      this._loadingService.setLoading(true, '檔案資料處理中...');

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
            this._loadingService.setLoading(false);
          },
          complete: () =>{
            this._loadingService.setLoading(false);
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
            this._loadingService.setLoading(false);
          },
          complete: () =>
          {
            this._loadingService.setLoading(false);
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

          this._loadingService.setLoading(false);
        })
      }
    }
    else if(data.status == UploadType.Fail)
    {
      this._swlService.showSwal('', `${data.fileName}上傳失敗,請聯絡CAE Team。${data.message}`, 'error');      
      this._loadingService.setLoading(false);
    }

  } 

  templateDownload()
  {
    this._loadingService.setLoading(true, '範本檔下載中...');

    this._bsEighteenService.ExportTemplate(this.templateFileType).subscribe({
      next: () => 
      {
        let templateName = '';
        switch (this.templateFileType)
        {
          case "S18":
            templateName = '小18'
            break;
          case "B18":
            templateName = '大18'
            break;  
          case "Vendor":
            templateName = '廠商提供報表'
            break;  
        }

        this._swlService.showSwal('', `[${templateName}]範本檔下載完成.`, 'success');
        this._loadingService.setLoading(false);
      },
      error: () => {
        this._swlService.showSwal('', '系統異常,請聯絡CAE Team...', 'error');
        this._loadingService.setLoading(false);
      },
      complete: () => { 

      }
    });
  }

  public apiRes: string = "";
  qq()
  {
    this._bsEighteenService.qq().subscribe(c => {
      this.apiRes = JSON.stringify(c);
    })
  }
}
