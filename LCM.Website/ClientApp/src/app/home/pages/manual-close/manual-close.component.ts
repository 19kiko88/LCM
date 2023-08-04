import { LoadingService } from './../../../shared/components/loading/loading.service';
import { ShareService } from './../../../core/service/share.service';
import { DatePipe } from '@angular/common';
import { AfterViewInit, Component, OnInit } from '@angular/core';
import { BsEighteenService } from 'src/app/core/http/bs-eighteen.service';
import { SweetAlertService } from 'src/app/core/service/sweet-alert.service';
import { UploadInfo, UploadType } from 'src/app/shared/components/new-upload/new-upload.component';
import { environment } from 'src/environments/environment';

@Component({
  selector: 'app-manual-close',
  templateUrl: './manual-close.component.html',
  styleUrls: ['./manual-close.component.scss']
})
export class ManualCloseComponent implements OnInit, AfterViewInit {

  uploadApiURL: string = "";
  updateResult: string = "";

  constructor(
    private _bsEighteenService: BsEighteenService,
    private _swlService: SweetAlertService,
    private _shareService: ShareService,
    private _loadingService: LoadingService
  ){  }

  ngOnInit()
  {
    this.uploadApiURL = `${environment.apiBaseUrl}/BsEighteen/Upload`;
  }

  ngAfterViewInit()
  {
    setTimeout(() => {
      this._shareService.emitChange("手動結案作業");
    });   
  }

   setUploadInfo(data: UploadInfo)
   {     
     if(data.status == UploadType.Processing)
     {
       this._loadingService.setLoading(true, '資料上傳中...');
     }
     else if (data.status == UploadType.Success)
     {    
      this._loadingService.setLoading(true, '手動更新結案狀態處理中...');

      this._bsEighteenService.manualClose(data).subscribe(res => {
        if (res.success)
        {
          this._swlService.showSwal('', '手動更新結案狀態成功.', 'info');
          this.updateResult = `更新筆數：${res.content}`;
        }
        else 
        {
          this._swlService.showSwal('', `手動更新結案狀態異常，請聯絡CAE Team.<br\>${res.message}`, 'error');
        }

        this._loadingService.setLoading(false);
      })
     }
     else if(data.status == UploadType.Fail)
     {
       this._swlService.showSwal('', `${data.fileName}上傳失敗,請聯絡CAE Team。${data.message}`, 'error');
       this._loadingService.setLoading(false);
     }
   }   

}
